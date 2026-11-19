# Test results

Date: 2026-11-19. Environment: Ubuntu 24.04, .NET SDK 10.0.404, Linux x64.

## Build

`dotnet build src/OneOS/OneOS.csproj -c Release --disable-build-servers --verbosity:minimal` — **PASS**, WPF Windows target cross-compiled; 0 warnings, 0 errors. This is compile verification only. The app was not executed.

## Automated core checks

`dotnet run --project tests/OneOS.Core.Tests/OneOS.Core.Tests.csproj -c Release --disable-build-servers` — **6/6 PASS**: unique app IDs, empty search, case-insensitive trimmed search, no-result search, preference default/save/overwrite/read, corrupt JSON surfaced as an error.

## Not verified

Windows startup and exit, XAML rendering at runtime, pointer and keyboard interaction, file browsing via the Windows shell, resize/move/minimize/maximize/focus, actual theme rendering, and persistence behavior on Windows require execution on Windows 10/11. The current sandbox runs Ubuntu and has no Windows runtime or Wine; do not treat cross-compilation as execution evidence.
