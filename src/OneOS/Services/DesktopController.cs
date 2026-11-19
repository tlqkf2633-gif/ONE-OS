using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using OneOS.Core;

namespace OneOS.Services;

public sealed class DesktopController
{
    private readonly Canvas canvas;
    private readonly ThemeService theme;
    private readonly Dictionary<string, AppItem> items;
    private readonly Dictionary<string, Border> windows = new();
    private readonly Dictionary<string, (double Left, double Top, double Width, double Height)> restoreBounds = new();
    private int zIndex = 1;
    private string? activeId;
    public event Action? StateChanged;

    public DesktopController(Canvas canvas, ThemeService theme, IEnumerable<AppItem> apps)
    {
        this.canvas = canvas;
        this.theme = theme;
        items = apps.ToDictionary(x => x.Id);
    }

    public void Launch(string id)
    {
        if (!items.TryGetValue(id, out var item)) return;
        if (windows.TryGetValue(id, out var existing))
        {
            if (existing.Tag is ValueTuple<string, FrameworkElement> state)
                state.Item2.Visibility = Visibility.Visible;
            Focus(id);
            StateChanged?.Invoke();
            return;
        }
        var content = id switch
        {
            "settings" => OneOS.Apps.SettingsView.Create(theme.SetDark, () => theme.IsDark),
            "notes" => OneOS.Apps.NotesView.Create(),
            "calculator" => OneOS.Apps.CalculatorView.Create(),
            "files" => OneOS.Apps.FilesView.Create(),
            _ => new TextBlock { Text = "This application is not available.", Margin = new Thickness(24) }
        };
        var titleBar = CreateTitleBar(item.Name, id);
        var root = new DockPanel();
        DockPanel.SetDock(titleBar, Dock.Top);
        root.Children.Add(titleBar);
        var scroll = new ScrollViewer { Content = content, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
        root.Children.Add(scroll);
        var border = new Border
        {
            Width = id == "calculator" ? 370 : id == "files" ? 630 : 530,
            Height = id == "calculator" ? 450 : id == "files" ? 520 : 490,
            MinWidth = 310, MinHeight = 280,
            Background = (Brush)Application.Current.Resources["CardBrush"],
            BorderBrush = (Brush)Application.Current.Resources["StrokeBrush"],
            BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(20),
            Child = root, Tag = id, Effect = new System.Windows.Media.Effects.DropShadowEffect { BlurRadius = 24, ShadowDepth = 8, Opacity = 0.14 }
        };
        border.SetResourceReference(Control.BackgroundProperty, "CardBrush");
        border.SetResourceReference(Control.BorderBrushProperty, "StrokeBrush");
        var grip = new Thumb
        {
            Width = 20, Height = 20, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom,
            Cursor = Cursors.SizeNWSE, Background = Brushes.Transparent, Template = ResizeGripTemplate()
        };
        grip.DragDelta += (_, e) =>
        {
            border.Width = Math.Max(border.MinWidth, border.ActualWidth + e.HorizontalChange);
            border.Height = Math.Max(border.MinHeight, border.ActualHeight + e.VerticalChange);
        };
        var holder = new Grid();
        holder.Children.Add(border);
        holder.Children.Add(grip);
        Canvas.SetLeft(holder, 50 + (windows.Count * 28) % 240);
        Canvas.SetTop(holder, 35 + (windows.Count * 26) % 140);
        Panel.SetZIndex(holder, ++zIndex);
        holder.MouseDown += (_, _) => Focus(id);
        border.MouseDown += (_, _) => Focus(id);
        canvas.Children.Add(holder);
        border.Tag = (id, holder);
        windows[id] = border;
        item.IsOpen = true;
        Focus(id);
        StateChanged?.Invoke();
    }

    public void Toggle(string id)
    {
        if (windows.TryGetValue(id, out var window) && window.Visibility == Visibility.Visible && activeId == id)
        {
            window.Visibility = Visibility.Collapsed;
            items[id].IsActive = false;
            activeId = null;
            FocusMostRecent();
            StateChanged?.Invoke();
        }
        else Launch(id);
    }

    private void Focus(string id)
    {
        foreach (var (key, item) in items) item.IsActive = key == id;
        activeId = id;
        if (windows.TryGetValue(id, out var w))
        {
            if (w.Tag is ValueTuple<string, FrameworkElement> state)
            {
                state.Item2.Visibility = Visibility.Visible;
                Panel.SetZIndex(state.Item2, ++zIndex);
            }
        }
        StateChanged?.Invoke();
    }

    private void FocusMostRecent()
    {
        var next = windows.Where(x => x.Value.Visibility == Visibility.Visible)
            .Where(x => x.Value.Tag is ValueTuple<string, FrameworkElement> s && s.Item2.Visibility == Visibility.Visible)
            .OrderByDescending(x => x.Value.Tag is ValueTuple<string, FrameworkElement> s ? Panel.GetZIndex(s.Item2) : 0)
            .FirstOrDefault();
        if (next.Value is not null) Focus(next.Key);
    }

    private FrameworkElement CreateTitleBar(string title, string id)
    {
        var grid = new Grid { Height = 46, Margin = new Thickness(8, 7, 8, 0), Background = Brushes.Transparent };
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        var heading = new TextBlock { Text = title, FontWeight = FontWeights.SemiBold, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(12, 0, 0, 0), FontSize = 14 };
        heading.SetResourceReference(TextBlock.ForegroundProperty, "TextBrush");
        grid.Children.Add(heading);
        var buttons = new StackPanel { Orientation = Orientation.Horizontal };
        var minimize = new Button { Content = "—", Width = 32, Height = 30, Padding = new Thickness(2), Margin = new Thickness(0, 0, 5, 0), Style = (Style)Application.Current.Resources["SoftButton"] };
        minimize.Click += (_, _) =>
        {
            if (windows.TryGetValue(id, out var window) && window.Tag is ValueTuple<string, FrameworkElement> s)
                s.Item2.Visibility = Visibility.Collapsed;
            items[id].IsActive = false;
            activeId = null;
            FocusMostRecent();
            StateChanged?.Invoke();
        };
        var close = new Button { Content = "×", Width = 32, Height = 30, Padding = new Thickness(2), Margin = new Thickness(0), Style = (Style)Application.Current.Resources["SoftButton"] };
        close.Click += (_, _) => Close(id);
        buttons.Children.Add(minimize);
        var maximize = new Button { Content = "□", Width = 32, Height = 30, Padding = new Thickness(2), Margin = new Thickness(0, 0, 5, 0), Style = (Style)Application.Current.Resources["SoftButton"] };
        maximize.Click += (_, _) => ToggleMaximize(id);
        buttons.Children.Add(maximize);
        buttons.Children.Add(close);
        Grid.SetColumn(buttons, 1);
        grid.Children.Add(buttons);
        grid.MouseLeftButtonDown += (_, e) =>
        {
            if (e.ClickCount == 2) { ToggleMaximize(id); e.Handled = true; return; }
            if (e.ButtonState == MouseButtonState.Pressed && windows.TryGetValue(id, out var window) && window.Tag is ValueTuple<string, FrameworkElement> s)
            {
                try { s.Item2.MoveByDrag(e); } catch (InvalidOperationException) { }
            }
        };
        return grid;
    }

    private void ToggleMaximize(string id)
    {
        if (!windows.TryGetValue(id, out var window) || window.Tag is not ValueTuple<string, FrameworkElement> state) return;
        var holder = state.Item2;
        if (restoreBounds.Remove(id, out var bounds))
        {
            window.Width = bounds.Width; window.Height = bounds.Height;
            Canvas.SetLeft(holder, bounds.Left); Canvas.SetTop(holder, bounds.Top);
        }
        else
        {
            restoreBounds[id] = (Canvas.GetLeft(holder), Canvas.GetTop(holder), window.Width, window.Height);
            Canvas.SetLeft(holder, 0); Canvas.SetTop(holder, 0);
            window.Width = Math.Max(window.MinWidth, canvas.ActualWidth);
            window.Height = Math.Max(window.MinHeight, canvas.ActualHeight);
        }
        Focus(id);
    }

    private void Close(string id)
    {
        if (windows.Remove(id, out var window) && window.Tag is ValueTuple<string, FrameworkElement> s) canvas.Children.Remove(s.Item2);
        items[id].IsOpen = false;
        items[id].IsActive = false;
        if (activeId == id) activeId = null;
        FocusMostRecent();
        StateChanged?.Invoke();
    }

    private static ControlTemplate ResizeGripTemplate()
    {
        var template = new ControlTemplate(typeof(Thumb));
        var path = new FrameworkElementFactory(typeof(TextBlock));
        path.SetValue(TextBlock.TextProperty, "◢");
        path.SetValue(TextBlock.ForegroundProperty, Brushes.Gray);
        path.SetValue(TextBlock.FontSizeProperty, 12d);
        path.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Right);
        path.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Bottom);
        template.VisualTree = path;
        return template;
    }
}

internal static class DragExtensions
{
    internal static void MoveByDrag(this FrameworkElement target, MouseButtonEventArgs e)
    {
        var window = Window.GetWindow(target);
        if (window is null) return;
        var point = e.GetPosition(target);
        var initial = e.GetPosition(target.Parent as IInputElement);
        target.CaptureMouse();
        MouseEventHandler? move = null;
        MouseButtonEventHandler? up = null;
        move = (_, _) =>
        {
            if (!target.IsMouseCaptured) return;
            var now = Mouse.GetPosition(target.Parent as IInputElement);
            Canvas.SetLeft(target, Math.Max(0, Canvas.GetLeft(target) + now.X - initial.X));
            Canvas.SetTop(target, Math.Max(0, Canvas.GetTop(target) + now.Y - initial.Y));
            initial = now;
        };
        up = (_, _) =>
        {
            target.ReleaseMouseCapture();
            target.MouseMove -= move;
            target.MouseLeftButtonUp -= up;
        };
        target.MouseMove += move;
        target.MouseLeftButtonUp += up;
        e.Handled = true;
    }
}
