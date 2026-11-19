# ONE OS

ONE OS is an experimental Windows desktop application, not an independent operating system and not a replacement Windows shell. Its first prototype provides a custom desktop, floating dock, searchable app launcher, internal app windows, and Settings, Notes, Calculator and Files applications.

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

The local test harness reports 6/6 passes for app search and preference storage.

## Interaction

Open Apps from the dock, search by name, then click a card or press Enter. `Ctrl+Space` toggles the launcher; Escape closes it. Launch an app from the dock to focus it, or close, minimize, maximize and restore its in-workspace window. App windows can be moved by dragging their title bar and resized from the bottom-right grip. Exit ONE OS from the launcher; Windows shell and taskbar are untouched.

Settings changes light/dark appearance and stores preferences under `%LOCALAPPDATA%/OneOS/settings.json`. Notes are explicitly saved as plain text under `%LOCALAPPDATA%/OneOS/notes.txt`. Files lists drives and folder contents and opens a selected file in its registered Windows app; it does not modify files.

Read `ONE_OS_ARCHITECTURE.md`, `TECHNICAL_DECISIONS.md`, `PROJECT_STATE.md`, `TEST_RESULTS.md`, and `KNOWN_ISSUES.md` before continuing development. The development branch is `dev/prototype`; `main` retains the original repository state until Windows execution testing is complete.
