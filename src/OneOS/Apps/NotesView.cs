using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace OneOS.Apps;

public static class NotesView
{
    public static FrameworkElement Create()
    {
        var page = ViewFactoryStyles.Page("Notes", "Notes are stored as plain text on this device. Saving is explicit.");
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OneOS", "notes.txt");
        var editor = new TextBox
        {
            AcceptsReturn = true, AcceptsTab = true, TextWrapping = TextWrapping.Wrap,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto, MinHeight = 240,
            FontSize = 15, Padding = new Thickness(10),
            Background = ViewFactoryStyles.CardBrush, Foreground = ViewFactoryStyles.ForegroundBrush
        };
        editor.SetResourceReference(Control.BackgroundProperty, "InputBrush");
        editor.SetResourceReference(Control.ForegroundProperty, "TextBrush");
        var status = ViewFactoryStyles.Status();
        try
        {
            if (File.Exists(path)) editor.Text = File.ReadAllText(path);
        }
        catch (Exception ex) { status.Text = $"Could not load notes: {ex.Message}"; }
        page.Children.Add(editor);
        page.Children.Add(ViewFactoryStyles.Button("Save notes", (_, _) =>
        {
            try
            {
                var directory = Path.GetDirectoryName(path)!;
                Directory.CreateDirectory(directory);
                File.WriteAllText(path, editor.Text);
                status.Text = $"Saved to {path}";
            }
            catch (Exception ex) { status.Text = $"Could not save notes: {ex.Message}"; }
        }));
        page.Children.Add(status);
        return page;
    }
}
