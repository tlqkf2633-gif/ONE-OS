using System.ComponentModel;
using System.Diagnostics;

namespace OneOS.Services;

/// <summary>Launches only documented built-in Windows destinations; accepts no arbitrary command input.</summary>
public static class WindowsShellIntegration
{
    public static void OpenFileExplorer()
    {
        Start(new ProcessStartInfo("explorer.exe") { UseShellExecute = true });
    }

    public static void OpenWindowsSettings()
    {
        Start(new ProcessStartInfo("ms-settings:") { UseShellExecute = true });
    }

    private static void Start(ProcessStartInfo startInfo)
    {
        if (!OperatingSystem.IsWindows())
            throw new PlatformNotSupportedException("This action requires Windows.");
        _ = Process.Start(startInfo) ?? throw new InvalidOperationException("Windows did not start the requested application.");
    }
}
