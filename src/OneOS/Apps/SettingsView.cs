using System;
using System.Windows;
using System.Windows.Controls;

namespace OneOS.Apps;

public static class SettingsView
{
    public static FrameworkElement Create(Action<bool> setDark, Func<bool> isDark)
    {
        ArgumentNullException.ThrowIfNull(setDark);
        ArgumentNullException.ThrowIfNull(isDark);
        var page = ViewFactoryStyles.Page("Appearance", "Choose the application color theme.");
        var card = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 4, 0, 0) };
        card.Children.Add(ViewFactoryStyles.Button("Light theme", (_, _) => setDark(false)));
        card.Children.Add(ViewFactoryStyles.Button("Dark theme", (_, _) => setDark(true)));
        page.Children.Add(card);
        page.Children.Add(new TextBlock
        {
            Text = $"Current theme: {(isDark() ? "Dark" : "Light")}",
            Foreground = ViewFactoryStyles.MutedBrush, Margin = new Thickness(0, 8, 0, 0)
        });
        return page;
    }
}
