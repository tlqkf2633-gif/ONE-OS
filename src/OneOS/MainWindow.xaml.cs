using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using OneOS.Core;
using OneOS.Services;

namespace OneOS;

public partial class MainWindow : Window
{
    private readonly ObservableCollection<AppItem> apps = new(AppCatalog.All.Select(x => new AppItem(x)));
    private readonly ThemeService theme;
    private readonly DesktopController desktop;
    private readonly DispatcherTimer clockTimer;

    public MainWindow()
    {
        InitializeComponent();
        theme = new ThemeService();
        desktop = new DesktopController(DesktopCanvas, theme, apps);
        DockItems.ItemsSource = apps;
        LauncherItems.ItemsSource = AppCatalog.All.Select(x => new AppItem(x)).ToArray();
        UpdateClock();
        clockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(15) };
        clockTimer.Tick += (_, _) => UpdateClock();
        clockTimer.Start();
        Closed += (_, _) => clockTimer.Stop();
        StateChanged += (_, _) => UpdateMainWindowGlyph();
        SearchBox.PreviewKeyDown += SearchBoxKeyDown;
        PreviewKeyDown += MainWindowKeyDown;
        UpdateMainWindowGlyph();
    }

    private void UpdateClock()
    {
        var now = DateTime.Now;
        HeaderClock.Text = now.ToString("dddd, MMMM d");
        DockTime.Text = now.ToString("h:mm tt");
        DockDate.Text = now.ToString("ddd, MMM d");
    }

    private void ToggleLauncher(object sender, RoutedEventArgs e)
    {
        if (LauncherPanel.Visibility == Visibility.Visible) CloseLauncher(sender, e);
        else
        {
            LauncherPanel.Visibility = Visibility.Visible;
            SearchBox.Clear();
            SearchBox.Focus();
        }
    }

    private void CloseLauncher(object sender, RoutedEventArgs e) => LauncherPanel.Visibility = Visibility.Collapsed;

    private void SearchBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        if (LauncherItems is null || SearchBox is null) return;
        LauncherItems.ItemsSource = AppCatalog.Search(SearchBox.Text).Select(x => new AppItem(x)).ToArray();
    }

    private void SearchBoxKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && LauncherItems.Items.Count > 0)
        {
            if (LauncherItems.Items[0] is AppItem app) Launch(app.Id);
            e.Handled = true;
        }
        else if (e.Key == Key.Escape) CloseLauncher(sender, e);
    }

    private void MainWindowKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape && LauncherPanel.Visibility == Visibility.Visible)
        {
            CloseLauncher(sender, e);
            e.Handled = true;
        }
        if ((Keyboard.Modifiers & ModifierKeys.Control) != 0 && e.Key == Key.Space)
        {
            if (LauncherPanel.Visibility == Visibility.Visible) CloseLauncher(sender, e);
            else ToggleLauncher(sender, e);
            e.Handled = true;
        }
    }

    private void DockItemClick(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: string id }) desktop.Toggle(id);
    }

    private void LaunchItemClick(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: string id }) Launch(id);
    }

    private void Launch(string id)
    {
        LauncherPanel.Visibility = Visibility.Collapsed;
        desktop.Launch(id);
    }

    private void ExitClick(object sender, RoutedEventArgs e) => Close();

    private void MinimizeMainWindowClick(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    private void ToggleMainWindowStateClick(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        UpdateMainWindowGlyph();
    }

    private void CloseMainWindowClick(object sender, RoutedEventArgs e) => Close();

    private void UpdateMainWindowGlyph()
    {
        if (MaximizeGlyph is not null)
        {
            var maximized = WindowState == WindowState.Maximized;
            MaximizeGlyph.Text = maximized ? "\uE923" : "\uE922";
            MaximizeButton.ToolTip = maximized ? "Restore" : "Maximize";
        }
    }
}
