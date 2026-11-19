namespace OneOS.Core;

public sealed record AppDefinition(string Id, string Name, string Description, string Icon, string Color);

public static class AppCatalog
{
    public static IReadOnlyList<AppDefinition> All { get; } = Array.AsReadOnly(new[]
    {
        new AppDefinition("settings", "Settings", "Appearance & preferences", "\uE713", "#6C79E7"),
        new AppDefinition("files", "Files", "Browse your folders", "\uE8B7", "#F4B65D"),
        new AppDefinition("calculator", "Calculator", "Everyday calculations", "\uE8EF", "#62B8A4"),
        new AppDefinition("notes", "Notes", "Capture your thoughts", "\uE70B", "#E98D9B")
    });

    public static IReadOnlyList<AppDefinition> Search(string? query)
    {
        var term = query?.Trim();
        return string.IsNullOrEmpty(term)
            ? All
            : All.Where(app => app.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                || app.Description.Contains(term, StringComparison.OrdinalIgnoreCase)
                || app.Id.Contains(term, StringComparison.OrdinalIgnoreCase)).ToArray();
    }
}
