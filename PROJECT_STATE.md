# ONE OS — project state

Updated: 2026-11-20

## Milestone and stack

Redesign and interaction-fix milestone on `dev/redesign`. C# / .NET 10 / WPF (`net10.0-windows`). Windows manages the kernel, drivers, security and native apps; this remains an ordinary app, not an independent OS or shell replacement. WPF UI and Windows-only launch adapter are in `src/OneOS`; platform-neutral app catalog, preferences, and window geometry live in `src/OneOS.Core`.

## Implemented

Desktop canvas, translucent floating dock overlay, searchable launcher, `Ctrl+Space`, internal Settings/Notes/Calculator views, and workspace windows with title-bar move, eight-direction resize, focus, minimize, maximize/restore, and close. Main window has explicit client controls alongside native Windows controls. Light/dark theme uses saved local preferences; Notes save explicitly. The launcher delegates File Explorer and the Settings view can open Windows Settings. The dock remains over maximized internal content but is not system-wide. Safe exit is available from the launcher.

## Verification

WPF Release cross-target build on Ubuntu with .NET SDK 10.0.404: **BUILD VERIFIED**, zero warnings/errors (2026-11-20). Platform-independent core test harness: **TESTED**, 11/11 passed for app catalog search, preferences, and window geometry.

Windows execution, rendered UI, interactions, Explorer/Settings launch, DPI behavior and shutdown: **NOT TESTED**. Ubuntu sandbox has no Windows runtime or Wine. See `TEST_REPORT.md` and `KNOWN_ISSUES.md`.

## Next

Run on Windows 10/11 and complete the checklist in `TEST_REPORT.md`. Fix observed issues before merging to `main` or calling this a stable release. Public remote: `https://github.com/tlqkf2633-gif/ONE-OS`; working branch: `dev/redesign`. Do not perform shell replacement, system configuration changes, or destructive app removal.
