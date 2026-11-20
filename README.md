# ONE OS

ONE OS is an experimental Windows desktop application, not an independent operating system and not a replacement Windows shell. It runs as an ordinary WPF app and leaves Windows startup, login, security, drivers, Explorer shell, and taskbar in place.

## Current prototype

The prototype includes a desktop workspace, a translucent floating dock, searchable app launcher, internal movable/resizable app windows, light/dark appearance, Notes, and Calculator. The launcher opens Windows File Explorer for its file entry. Settings includes a link to Windows Settings. The dock is an overlay inside the ONE OS application window, including over maximized internal app content; it is not a system-wide always-on-top dock.

Main-window client minimize, maximize/restore, and close commands complement the native Windows caption buttons. Internal windows support title-bar movement, eight-direction resizing, minimum bounds, minimize, maximize/restore, close, and z-order focus. These interactions are implemented but still require execution testing on Windows.

## Requirements and build

Use Windows 10/11 and the current .NET 10 SDK. From the repository root:

```powershell
dotnet build .\src\OneOS\OneOS.csproj -c Release
dotnet run --project .\src\OneOS\OneOS.csproj -c Release
```

For framework-dependent distribution, the Windows Desktop Runtime is needed on the target system. The project was cross-compiled on Ubuntu with .NET SDK 10.0.404 and produced zero warnings/errors. The WPF application has **not** yet been executed on Windows; perform a Windows startup and interaction test before treating it as verified.

Core checks run with:

```powershell
dotnet run --project .\tests\OneOS.Core.Tests\OneOS.Core.Tests.csproj -c Release
```

The core test harness currently reports 11/11 passes. See [TEST_REPORT.md](TEST_REPORT.md) for the exact scope and limitations.

## Interaction

Open Apps from the dock, search by name, then click a card or press Enter. `Ctrl+Space` toggles the launcher; Escape closes it. Use the dock to launch or focus apps. Drag an internal window by its title bar; resize from any edge or corner. Settings offers light and dark appearance and opens the native Windows Settings app. The File Explorer launcher item opens Windows Explorer. Exit ONE OS from the launcher.

Settings changes light/dark appearance and stores preferences under `%LOCALAPPDATA%\OneOS\settings.json`. Notes are explicitly saved as plain text under `%LOCALAPPDATA%\OneOS\notes.txt`.

Read [ARCHITECTURE.md](ARCHITECTURE.md), [TECHNICAL_DECISIONS.md](TECHNICAL_DECISIONS.md), [PROJECT_STATE.md](PROJECT_STATE.md), [DEVELOPMENT_STATUS.md](DEVELOPMENT_STATUS.md), [TEST_REPORT.md](TEST_REPORT.md), [KNOWN_ISSUES.md](KNOWN_ISSUES.md), and [ROADMAP.md](ROADMAP.md) before continuing development. The working branch is `dev/redesign`; `main` remains unchanged.

The current sandbox is Linux. Cross-compilation and platform-neutral tests are not Windows execution evidence. Run the app on Windows 10/11 and perform the checklist in `TEST_REPORT.md` before treating pointer behavior or Windows integrations as runtime verified.
