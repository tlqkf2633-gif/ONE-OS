using System.Windows;
using System.Windows.Media;
using OneOS.Core;

namespace OneOS.Services;

public sealed class ThemeService
{
    private readonly PreferencesStore store;
    public bool IsDark { get; private set; }
    public event Action? Changed;

    public ThemeService()
    {
        var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        store = new PreferencesStore(System.IO.Path.Combine(basePath, "OneOS", "settings.json"));
        try { IsDark = store.Load().DarkMode; }
        catch (Exception ex) when (ex is System.IO.IOException or UnauthorizedAccessException or System.Text.Json.JsonException)
        {
            IsDark = false;
            MessageBox.Show("Could not read ONE OS settings. Light mode will be used.\n\n" + ex.Message,
                "ONE OS", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        Apply();
    }

    public void SetDark(bool dark)
    {
        try { store.Save(new Preferences(dark)); }
        catch (Exception ex) when (ex is System.IO.IOException or UnauthorizedAccessException)
        {
            MessageBox.Show("Could not save appearance settings. Your selection will apply only for this session.\n\n" + ex.Message,
                "ONE OS", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        IsDark = dark;
        Apply();
        Changed?.Invoke();
    }

    private void Apply()
    {
        var r = Application.Current.Resources;
        void Set(string key, string color) => r[key] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
        Set("TextBrush", IsDark ? "#FFF0F2FB" : "#FF252840");
        Set("MutedBrush", IsDark ? "#FFB6BED0" : "#FF677188");
        Set("CardBrush", IsDark ? "#E52D3245" : "#DDF9FAFF");
        Set("DockBrush", IsDark ? "#F0353A4F" : "#EDF9FAFF");
        Set("InputBrush", IsDark ? "#FF444A60" : "#FFF0F2FA");
        Set("AccentBrush", IsDark ? "#FFA3B0FF" : "#FF6576D9");
        Set("StrokeBrush", IsDark ? "#664F5770" : "#88FFFFFF");
        var background = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
        background.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString(IsDark ? "#FF1E253A" : "#FFE9EEF9"), 0));
        background.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString(IsDark ? "#FF313951" : "#FFDBE6F5"), 0.52));
        background.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString(IsDark ? "#FF473746" : "#FFF1E4EB"), 1));
        r["DesktopBrush"] = background;
    }
}
