# ONE OS — project state

Updated: 2026-11-19

## Milestone and stack

Initial functional Windows desktop prototype. C# / .NET 10 LTS / WPF (`net10.0-windows`). Windows manages the kernel, drivers, security and native apps; this remains an ordinary app, not an independent OS or shell replacement. WPF UI is in `src/OneOS`; platform-neutral app catalog and preferences live in `src/OneOS.Core`.

## Implemented

Desktop canvas, floating dock with open-app indicators, searchable launcher, `Ctrl+Space`, four internal apps (Settings, Notes, Calculator, Files), and in-app windows with move, resize, focus, minimize, maximize/restore and close. Light/dark theme uses saved local preferences. Notes save explicitly. Files listing is read-only; open delegates to the Windows registered app. Safe exit is available from the launcher.

## Verification

Microsoft .NET SDK 10.0.404 official build verified by SHA-512. WPF Release cross-target build on Ubuntu: **BUILD VERIFIED**, zero warnings/errors (2026-11-19). Platform-independent core test harness: **TESTED**, 6/6 passed for app catalog search and preferences persistence/error handling.

Windows execution, rendered UI, interactions, shell integration and shutdown: **NOT TESTED**. Ubuntu sandbox has no Windows runtime or Wine. See `TEST_RESULTS.md` and `KNOWN_ISSUES.md`.

## Next

Run on a Windows 10/11 PC and verify startup/shutdown, launcher/search, all app windows, resize/move/focus, theme persistence and file navigation. Fix any observed issues before merging into `main` or calling this a stable release. Public remote: `https://github.com/tlqkf2633-gif/ONE-OS`; development branch: `dev/prototype`.
