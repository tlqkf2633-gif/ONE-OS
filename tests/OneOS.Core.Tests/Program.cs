using OneOS.Core;
using System.Text.Json;

var tests = new List<(string Name, Action Body)>
{
    ("catalog has unique IDs", () => Check(AppCatalog.All.Select(a => a.Id).Distinct().Count() == 4)),
    ("empty search returns all", () => Check(AppCatalog.Search("  ").Count == 4)),
    ("search ignores case and surrounding whitespace", () => Check(AppCatalog.Search("  FILES ").Single().Id == "files")),
    ("unmatched search is empty", () => Check(AppCatalog.Search("nothing matches").Count == 0)),
    ("preferences round-trip and overwrite", PreferencesRoundTrip),
    ("corrupt preferences surface error", CorruptPreferences)
};
var failed = 0;
foreach (var (name, body) in tests)
{
    try { body(); Console.WriteLine($"PASS {name}"); }
    catch (Exception ex) { failed++; Console.Error.WriteLine($"FAIL {name}: {ex.Message}"); }
}
Console.WriteLine($"{tests.Count - failed}/{tests.Count} passed");
return failed == 0 ? 0 : 1;

static void Check(bool condition)
{
    if (!condition) throw new InvalidOperationException("Assertion failed");
}
static void PreferencesRoundTrip()
{
    var dir = Path.Combine(Path.GetTempPath(), "oneos-test-" + Guid.NewGuid().ToString("N"));
    try
    {
        var store = new PreferencesStore(Path.Combine(dir, "settings.json"));
        Check(!store.Load().DarkMode);
        store.Save(new Preferences(true));
        Check(store.Load().DarkMode);
        store.Save(new Preferences(false));
        Check(!store.Load().DarkMode);
        Check(Directory.GetFiles(dir).Length == 1);
    }
    finally { if (Directory.Exists(dir)) Directory.Delete(dir, recursive: true); }
}
static void CorruptPreferences()
{
    var dir = Path.Combine(Path.GetTempPath(), "oneos-test-" + Guid.NewGuid().ToString("N"));
    Directory.CreateDirectory(dir);
    try
    {
        var path = Path.Combine(dir, "settings.json");
        File.WriteAllText(path, "not json");
        try { _ = new PreferencesStore(path).Load(); throw new InvalidOperationException("Expected JSON error"); }
        catch (JsonException) { }
    }
    finally { Directory.Delete(dir, recursive: true); }
}
