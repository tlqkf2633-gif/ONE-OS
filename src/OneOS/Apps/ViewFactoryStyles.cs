using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OneOS.Apps;

internal static class ViewFactoryStyles
{
    internal static readonly Brush ForegroundBrush = new SolidColorBrush(Color.FromRgb(34, 40, 49));
    internal static readonly Brush MutedBrush = new SolidColorBrush(Color.FromRgb(94, 105, 119));
    internal static readonly Brush CardBrush = new SolidColorBrush(Color.FromRgb(246, 248, 251));
    internal static readonly Brush AccentBrush = new SolidColorBrush(Color.FromRgb(38, 103, 177));
    internal static readonly Brush BorderBrush = new SolidColorBrush(Color.FromRgb(211, 218, 227));

    internal static StackPanel Page(string title, string? description = null)
    {
        var page = new StackPanel { Margin = new Thickness(24), MaxWidth = 900 };
        page.Children.Add(new TextBlock
        {
            Text = title, FontSize = 26, FontWeight = FontWeights.SemiBold,
            Foreground = ForegroundBrush, Margin = new Thickness(0, 0, 0, 6)
        });
        page.SetResourceReference(Control.ForegroundProperty, "TextBrush");
        if (!string.IsNullOrWhiteSpace(description))
        {
            var detail = new TextBlock
            {
                Text = description, FontSize = 14, Foreground = MutedBrush,
                TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 18)
            };
            detail.SetResourceReference(Control.ForegroundProperty, "MutedBrush");
            page.Children.Add(detail);
        }
        return page;
    }

    internal static Button Button(string text, RoutedEventHandler click)
    {
        var button = new Button
        {
            Content = text, Padding = new Thickness(14, 8, 14, 8), Margin = new Thickness(0, 0, 8, 8),
            MinWidth = 76, Foreground = ForegroundBrush, Background = CardBrush,
            BorderBrush = BorderBrush, Cursor = System.Windows.Input.Cursors.Hand
        };
        button.SetResourceReference(Control.ForegroundProperty, "TextBrush");
        button.SetResourceReference(Control.BackgroundProperty, "CardBrush");
        button.SetResourceReference(Control.BorderBrushProperty, "InputBrush");
        button.Click += click;
        return button;
    }

    internal static TextBlock Status(string text = "")
    {
        var status = new TextBlock
        {
            Text = text, Foreground = MutedBrush, TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 8, 0, 0)
        };
        status.SetResourceReference(Control.ForegroundProperty, "MutedBrush");
        return status;
    }
}
