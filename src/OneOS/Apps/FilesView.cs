using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OneOS.Apps;

public static class FilesView
{
    public static FrameworkElement Create()
    {
        var page = ViewFactoryStyles.Page("Files", "Browse folders and drives. Opening a file uses its registered Windows application.");
        var location = new TextBox { IsReadOnly = true, Padding = new Thickness(8), Margin = new Thickness(0, 0, 0, 8),
            Background = ViewFactoryStyles.CardBrush, Foreground = ViewFactoryStyles.ForegroundBrush };
        location.SetResourceReference(Control.BackgroundProperty, "InputBrush");
        location.SetResourceReference(Control.ForegroundProperty, "TextBrush");
        var buttons = new StackPanel { Orientation = Orientation.Horizontal };
        var list = new ListBox { MinHeight = 260, Background = ViewFactoryStyles.CardBrush, Foreground = ViewFactoryStyles.ForegroundBrush };
        list.SetResourceReference(Control.BackgroundProperty, "CardBrush");
        list.SetResourceReference(Control.ForegroundProperty, "TextBrush");
        var status = ViewFactoryStyles.Status();
        string? currentDirectory = null;
        var entries = Array.Empty<FileSystemInfo>();

        void ShowDrives()
        {
            currentDirectory = null; location.Text = "This PC (drives)";
            list.ItemsSource = DriveInfo.GetDrives().Select(d => $"{d.Name} {(d.IsReady ? $"({d.VolumeLabel})" : "(not ready)")}").ToArray();
        }
        void Navigate(string path)
        {
            try
            {
                var full = Path.GetFullPath(path);
                var info = new DirectoryInfo(full);
                var found = info.GetFileSystemInfos().OrderByDescending(x => (x.Attributes & FileAttributes.Directory) != 0)
                    .ThenBy(x => x.Name, StringComparer.CurrentCultureIgnoreCase).ToArray();
                currentDirectory = full; location.Text = full; entries = found;
                list.ItemsSource = found.Select(x => (x.Attributes & FileAttributes.Directory) != 0 ? $"[Folder] {x.Name}" : x.Name).ToArray();
                status.Text = "";
            }
            catch (Exception ex) { status.Text = $"Could not open folder: {ex.Message}"; }
        }
        buttons.Children.Add(ViewFactoryStyles.Button("Drives", (_, _) =>
        {
            try { ShowDrives(); status.Text = ""; } catch (Exception ex) { status.Text = $"Could not list drives: {ex.Message}"; }
        }));
        buttons.Children.Add(ViewFactoryStyles.Button("Up", (_, _) =>
        {
            if (currentDirectory is null) return;
            var parent = Directory.GetParent(currentDirectory);
            if (parent is null) { try { ShowDrives(); } catch (Exception ex) { status.Text = $"Could not list drives: {ex.Message}"; } }
            else Navigate(parent.FullName);
        }));
        buttons.Children.Add(ViewFactoryStyles.Button("Open selected", (_, _) =>
        {
            try
            {
                var index = list.SelectedIndex;
                if (currentDirectory is null)
                {
                    var drive = DriveInfo.GetDrives().ElementAt(index);
                    Navigate(drive.RootDirectory.FullName); return;
                }
                if (index < 0 || index >= entries.Length) { status.Text = "Select a file or folder first."; return; }
                var item = entries[index];
                if ((item.Attributes & FileAttributes.Directory) != 0) Navigate(item.FullName);
                else
                {
                    Process.Start(new ProcessStartInfo(item.FullName) { UseShellExecute = true });
                    status.Text = "Opened with the registered application.";
                }
            }
            catch (Exception ex) { status.Text = $"Could not open selection: {ex.Message}"; }
        }));
        page.Children.Add(buttons); page.Children.Add(location); page.Children.Add(list); page.Children.Add(status);
        try { ShowDrives(); } catch (Exception ex) { status.Text = $"Could not list drives: {ex.Message}"; }
        return page;
    }
}
