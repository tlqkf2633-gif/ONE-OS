using System.ComponentModel;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
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

    public DesktopController(Canvas canvas, ThemeService theme, IEnumerable<AppItem> apps)
    {
        this.canvas = canvas;
        this.theme = theme;
        items = apps.ToDictionary(x => x.Id);
        canvas.SizeChanged += (_, _) => KeepWindowsVisible();
    }

    public void Launch(string id)
    {
        if (!items.TryGetValue(id, out var item)) return;
        if (id == "files")
        {
            try { WindowsShellIntegration.OpenFileExplorer(); }
            catch (Exception ex) when (ex is Win32Exception or InvalidOperationException or PlatformNotSupportedException)
            {
                MessageBox.Show($"Windows File Explorer could not be opened.\n\n{ex.Message}",
                    "ONE OS", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return;
        }
        if (windows.TryGetValue(id, out var existing))
        {
            if (existing.Tag is ValueTuple<string, FrameworkElement> state)
                state.Item2.Visibility = Visibility.Visible;
            Focus(id);
            return;
        }

        FrameworkElement content;
        try
        {
            content = id switch
            {
                "settings" => OneOS.Apps.SettingsView.Create(theme.SetDark, () => theme.IsDark),
                "notes" => OneOS.Apps.NotesView.Create(),
                "calculator" => OneOS.Apps.CalculatorView.Create(),
                _ => new TextBlock { Text = "This application is not available.", Margin = new Thickness(24) }
            };
        }
        catch (Exception ex)
        {
            content = new TextBlock
            {
                Text = $"{item.Name} could not be opened.\n\n{ex.Message}",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(24)
            };
        }

        var holder = new Grid();
        var root = new DockPanel();
        var titleBar = CreateTitleBar(item.Name, id, holder);
        DockPanel.SetDock(titleBar, Dock.Top);
        root.Children.Add(titleBar);
        root.Children.Add(new ScrollViewer
        {
            Content = content,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
        });

        var border = new Border
        {
            Width = id == "calculator" ? 370 : 530,
            Height = id == "calculator" ? 450 : 490,
            MinWidth = 310,
            MinHeight = 280,
            Background = (Brush)Application.Current.Resources["CardBrush"],
            BorderBrush = (Brush)Application.Current.Resources["StrokeBrush"],
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(18),
            Child = root,
            Tag = (id, (FrameworkElement)holder),
            Effect = new System.Windows.Media.Effects.DropShadowEffect { BlurRadius = 20, ShadowDepth = 5, Opacity = 0.14 }
        };
        border.SetResourceReference(Control.BackgroundProperty, "CardBrush");
        border.SetResourceReference(Control.BorderBrushProperty, "StrokeBrush");
        border.PreviewMouseDown += (_, _) => Focus(id);
        holder.Children.Add(border);
        AddResizeHandles(holder, border, id);

        Canvas.SetLeft(holder, Math.Max(0, 46 + (windows.Count * 28) % 210));
        Canvas.SetTop(holder, Math.Max(0, 32 + (windows.Count * 24) % 120));
        Panel.SetZIndex(holder, ++zIndex);
        canvas.Children.Add(holder);
        windows[id] = border;
        item.IsOpen = true;
        Focus(id);
    }

    public void Toggle(string id)
    {
        if (windows.TryGetValue(id, out var window)
            && window.Tag is ValueTuple<string, FrameworkElement> state
            && state.Item2.Visibility == Visibility.Visible
            && activeId == id)
        {
            state.Item2.Visibility = Visibility.Collapsed;
            items[id].IsActive = false;
            activeId = null;
            FocusMostRecent();
            return;
        }
        Launch(id);
    }

    private void Focus(string id)
    {
        foreach (var (key, item) in items) item.IsActive = key == id;
        activeId = id;
        if (windows.TryGetValue(id, out var window)
            && window.Tag is ValueTuple<string, FrameworkElement> state)
        {
            state.Item2.Visibility = Visibility.Visible;
            Panel.SetZIndex(state.Item2, ++zIndex);
        }
    }

    private void FocusMostRecent()
    {
        var next = windows
            .Where(pair => pair.Value.Tag is ValueTuple<string, FrameworkElement> state
                && state.Item2.Visibility == Visibility.Visible)
            .OrderByDescending(pair => pair.Value.Tag is ValueTuple<string, FrameworkElement> state
                ? Panel.GetZIndex(state.Item2) : 0)
            .FirstOrDefault();
        if (next.Value is not null) Focus(next.Key);
        else
        {
            activeId = null;
            foreach (var item in items.Values) item.IsActive = false;
        }
    }

    private Grid CreateTitleBar(string title, string id, FrameworkElement holder)
    {
        var grid = new Grid { Height = 42, Margin = new Thickness(7, 6, 7, 0), Background = Brushes.Transparent };
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var drag = new Thumb
        {
            Background = Brushes.Transparent,
            Cursor = Cursors.SizeAll,
            Focusable = false,
            ToolTip = "Drag to move window"
        };
        drag.DragDelta += (_, e) =>
        {
            if (restoreBounds.ContainsKey(id)) return;
            if (!windows.TryGetValue(id, out var window)) return;
            var current = CurrentBounds(window, holder);
            var moved = WindowGeometry.Move(current, e.HorizontalChange, e.VerticalChange,
                canvas.ActualWidth, canvas.ActualHeight);
            Canvas.SetLeft(holder, moved.Left);
            Canvas.SetTop(holder, moved.Top);
        };
        drag.PreviewMouseLeftButtonDown += (_, e) =>
        {
            if (e.ClickCount != 2) return;
            ToggleMaximize(id);
            e.Handled = true;
        };
        grid.Children.Add(drag);

        var heading = new TextBlock
        {
            Text = title,
            FontWeight = FontWeights.SemiBold,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(12, 0, 0, 0),
            FontSize = 13,
            IsHitTestVisible = false
        };
        heading.SetResourceReference(TextBlock.ForegroundProperty, "TextBrush");
        grid.Children.Add(heading);

        var buttons = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
        buttons.Children.Add(WindowButton("—", "Minimize", (_, _) => Minimize(id)));
        buttons.Children.Add(WindowButton("□", "Maximize or restore", (_, _) => ToggleMaximize(id)));
        buttons.Children.Add(WindowButton("×", "Close", (_, _) => Close(id)));
        Grid.SetColumn(buttons, 1);
        Panel.SetZIndex(buttons, 1);
        grid.Children.Add(buttons);
        grid.MouseLeftButtonDown += (_, e) =>
        {
            if (e.ClickCount == 2 && e.OriginalSource is not Button)
            {
                ToggleMaximize(id);
                e.Handled = true;
            }
        };
        return grid;
    }

    private static Button WindowButton(string text, string name, RoutedEventHandler click)
    {
        var button = new Button
        {
            Content = text,
            Width = 30,
            Height = 28,
            Padding = new Thickness(2),
            Margin = new Thickness(1, 0, 1, 0),
            ToolTip = name,
            Style = (Style)Application.Current.Resources["SoftButton"]
        };
        AutomationProperties.SetName(button, name);
        button.Click += click;
        return button;
    }

    private void Minimize(string id)
    {
        if (!windows.TryGetValue(id, out var window)
            || window.Tag is not ValueTuple<string, FrameworkElement> state) return;
        state.Item2.Visibility = Visibility.Collapsed;
        items[id].IsActive = false;
        if (activeId == id) activeId = null;
        FocusMostRecent();
    }

    private void ToggleMaximize(string id)
    {
        if (!windows.TryGetValue(id, out var window)
            || window.Tag is not ValueTuple<string, FrameworkElement> state) return;
        var holder = state.Item2;
        if (restoreBounds.Remove(id, out var bounds))
        {
            window.Width = bounds.Width;
            window.Height = bounds.Height;
            Canvas.SetLeft(holder, bounds.Left);
            Canvas.SetTop(holder, bounds.Top);
        }
        else
        {
            restoreBounds[id] = (Canvas.GetLeft(holder), Canvas.GetTop(holder), window.Width, window.Height);
            Canvas.SetLeft(holder, 0);
            Canvas.SetTop(holder, 0);
            window.Width = Math.Max(window.MinWidth, canvas.ActualWidth);
            window.Height = Math.Max(window.MinHeight, canvas.ActualHeight);
        }
        Focus(id);
    }

    private void AddResizeHandles(Grid holder, Border window, string id)
    {
        AddHandle(HorizontalAlignment.Left, VerticalAlignment.Stretch, Cursors.SizeWE, "L");
        AddHandle(HorizontalAlignment.Right, VerticalAlignment.Stretch, Cursors.SizeWE, "R");
        AddHandle(HorizontalAlignment.Stretch, VerticalAlignment.Top, Cursors.SizeNS, "T");
        AddHandle(HorizontalAlignment.Stretch, VerticalAlignment.Bottom, Cursors.SizeNS, "B");
        AddHandle(HorizontalAlignment.Left, VerticalAlignment.Top, Cursors.SizeNWSE, "TL", 12);
        AddHandle(HorizontalAlignment.Right, VerticalAlignment.Top, Cursors.SizeNESW, "TR", 12);
        AddHandle(HorizontalAlignment.Left, VerticalAlignment.Bottom, Cursors.SizeNESW, "BL", 12);
        AddHandle(HorizontalAlignment.Right, VerticalAlignment.Bottom, Cursors.SizeNWSE, "BR", 12);

        void AddHandle(HorizontalAlignment horizontal, VerticalAlignment vertical, Cursor cursor, string direction, double size = 7)
        {
            var handle = new Thumb
            {
                Width = horizontal is HorizontalAlignment.Left or HorizontalAlignment.Right ? size : double.NaN,
                Height = vertical is VerticalAlignment.Top or VerticalAlignment.Bottom ? size : double.NaN,
                HorizontalAlignment = horizontal,
                VerticalAlignment = vertical,
                Background = Brushes.Transparent,
                Cursor = cursor,
                Focusable = false,
                ToolTip = "Drag to resize"
            };
            if (horizontal == HorizontalAlignment.Stretch) { handle.Margin = new Thickness(8, 0, 8, 0); }
            if (vertical == VerticalAlignment.Stretch) { handle.Margin = new Thickness(0, 8, 0, 8); }
            Panel.SetZIndex(handle, direction.Length == 2 ? 3 : 2);
            handle.DragDelta += (_, e) => Resize(window, holder, id, direction, e.HorizontalChange, e.VerticalChange);
            holder.Children.Add(handle);
        }
    }

    private void Resize(Border window, Grid holder, string id, string direction, double dx, double dy)
    {
        if (restoreBounds.ContainsKey(id)) return;
        var edges = ResizeEdges.None;
        if (direction.Contains('L')) edges |= ResizeEdges.Left;
        if (direction.Contains('R')) edges |= ResizeEdges.Right;
        if (direction.Contains('T')) edges |= ResizeEdges.Top;
        if (direction.Contains('B')) edges |= ResizeEdges.Bottom;
        var resized = WindowGeometry.Resize(CurrentBounds(window, holder), edges, dx, dy,
            canvas.ActualWidth, canvas.ActualHeight, window.MinWidth, window.MinHeight);
        Canvas.SetLeft(holder, resized.Left);
        Canvas.SetTop(holder, resized.Top);
        window.Width = resized.Width;
        window.Height = resized.Height;
    }

    private static WindowBounds CurrentBounds(Border window, FrameworkElement holder)
    {
        var left = Canvas.GetLeft(holder);
        var top = Canvas.GetTop(holder);
        return new WindowBounds(double.IsNaN(left) ? 0 : left, double.IsNaN(top) ? 0 : top,
            window.Width, window.Height);
    }

    private void Close(string id)
    {
        if (windows.Remove(id, out var window)
            && window.Tag is ValueTuple<string, FrameworkElement> state)
            canvas.Children.Remove(state.Item2);
        restoreBounds.Remove(id);
        items[id].IsOpen = false;
        items[id].IsActive = false;
        if (activeId == id) activeId = null;
        FocusMostRecent();
    }

    private void KeepWindowsVisible()
    {
        foreach (var window in windows.Values)
        {
            if (window.Tag is not ValueTuple<string, FrameworkElement> state) continue;
            var holder = state.Item2;
            if (restoreBounds.ContainsKey(state.Item1))
            {
                window.Width = Math.Max(window.MinWidth, canvas.ActualWidth);
                window.Height = Math.Max(window.MinHeight, canvas.ActualHeight);
                Canvas.SetLeft(holder, 0);
                Canvas.SetTop(holder, 0);
                continue;
            }
            var current = CurrentBounds(window, holder);
            var constrained = WindowGeometry.Constrain(current, canvas.ActualWidth, canvas.ActualHeight);
            Canvas.SetLeft(holder, constrained.Left);
            Canvas.SetTop(holder, constrained.Top);
        }
    }
}
