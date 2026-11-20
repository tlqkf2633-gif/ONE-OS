# ONE OS architecture

## Product boundary

ONE OS is a regular Windows WPF application. Windows remains responsible for the kernel, drivers, memory, processes, security, native application compatibility, and the Windows shell. The prototype does not replace Explorer, modify boot/login behavior, disable security, or alter protected system configuration.

## Solution structure

`src/OneOS` contains the WPF application, desktop canvas, launcher, dock, theme, internal app views, and window manager. `src/OneOS/Services/DesktopController.cs` controls app-window lifecycle, z-order, movement and resizing. `WindowGeometry` lives in `src/OneOS.Core` so bounds calculations can be tested without WPF. `WindowsShellIntegration` is a small Windows-only adapter that launches the fixed destinations Windows File Explorer and Windows Settings; it takes no arbitrary command strings.

`src/OneOS.Core` provides app metadata/search and preference storage. `tests/OneOS.Core.Tests` is a dependency-free executable test harness for core behavior; it is not a WPF UI automation suite.

## Interaction model

The main desktop and dock share one WPF application window. The dock overlays the work area and remains above internal workspace content, including when an internal app is maximized. Internal apps are visual elements on a WPF canvas, not separate native top-level windows. They use a `Thumb` for title-bar movement and eight `Thumb` regions for resizing. Minimum dimensions and workspace constraints are applied by shared core geometry functions. The main application window retains native Windows caption controls and adds client-area controls.

## Persistence and integrations

Theme preferences are stored as JSON under `%LOCALAPPDATA%/OneOS/settings.json`; Notes save explicitly as plain text under `%LOCALAPPDATA%/OneOS/notes.txt`. The launcher delegates File Explorer and Windows Settings to Windows rather than claiming these native tools are reimplemented by ONE OS. No Samsung-specific behavior is currently implemented or verified.

## Extension guidance

Keep Windows-specific calls in `src/OneOS/Services`. Keep platform-neutral logic in `OneOS.Core`. Add app metadata to `AppCatalog`, then add an explicit safe launch or internal view mapping in `DesktopController`. Do not treat user-entered search text as commands. Destructive install/removal, shell replacement, startup registration, and elevated operations are outside this prototype and require separate safety review and explicit owner approval.

## Microsoft references

The Settings handoff uses the documented `ms-settings:` URI scheme described in [Launch Windows Settings](https://learn.microsoft.com/en-us/windows/apps/develop/launch/launch-settings). Fixed Windows destinations are started through .NET `ProcessStartInfo.UseShellExecute`, which uses the operating system graphical shell; see the [Microsoft API reference](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.processstartinfo.useshellexecute). Runtime behavior still requires Windows testing.
