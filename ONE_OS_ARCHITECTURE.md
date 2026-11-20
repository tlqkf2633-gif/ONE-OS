# ONE OS — architecture

## Boundary

The deliverable is a regular WPF desktop application for Windows. Windows continues to manage hardware, kernel, memory, processes, security, drivers, and native Windows applications. ONE OS does not replace Explorer or the Windows shell and does not alter startup, login, registry, or protected settings.

## Components

`src/OneOS` contains WPF composition, desktop surface, launcher, dock, theme integration, and in-app window controller. `src/OneOS/Apps` contains internal Settings, Notes, and Calculator views. `src/OneOS/Services` contains application item presentation, theme preferences, window lifecycle behavior, and fixed Windows shell launch destinations. `src/OneOS.Core` contains Windows-UI-independent app metadata/search, JSON preferences storage, and window geometry. App launch is an explicit registry-to-view mapping; search does not evaluate commands or input as code.

`tests/OneOS.Core.Tests` is a dependency-free executable test harness for core functionality. It is not a WPF UI test suite.

## App persistence and external integration

Theme configuration is JSON under `%LOCALAPPDATA%/OneOS/settings.json`; writes use a temporary file and replace. Notes are explicit plain-text saves under `%LOCALAPPDATA%/OneOS/notes.txt`. The File Explorer launcher and Windows Settings handoff use fixed destinations; these launches are implemented but still require Windows runtime verification. No destructive file actions are implemented.

## Window model

The WPF main window represents the ONE OS workspace. The floating dock overlays the workspace rather than consuming its height and remains above maximized internal content. Internal apps are movable/resizable content windows on a canvas, with z-order focus, minimize, maximize/restore, and close actions. They are not native top-level OS windows. The system taskbar and security controls remain available.

## Extension

Add app metadata to `OneOS.Core/AppCatalog.cs` and add its explicit view mapping to `DesktopController.Launch`. Keep OS-specific integrations in the WPF application layer; do not add Windows dependencies to `OneOS.Core`.
