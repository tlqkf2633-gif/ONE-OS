using System.Text.Json;

namespace OneOS.Core;

public sealed record Preferences(bool DarkMode = false);

public sealed class PreferencesStore
{
    private readonly string path;
    public PreferencesStore(string path) => this.path = path ?? throw new ArgumentNullException(nameof(path));

    public Preferences Load()
    {
        if (!File.Exists(path)) return new Preferences();
        return JsonSerializer.Deserialize<Preferences>(File.ReadAllText(path)) ?? new Preferences();
    }

    public void Save(Preferences preferences)
    {
        ArgumentNullException.ThrowIfNull(preferences);
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
        // Atomic replacement avoids truncating the last working settings file on a failed write.
        var temp = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            File.WriteAllText(temp, JsonSerializer.Serialize(preferences));
            File.Move(temp, path, overwrite: true);
        }
        finally
        {
            if (File.Exists(temp)) File.Delete(temp);
        }
    }
}
