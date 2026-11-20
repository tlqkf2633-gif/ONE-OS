# Test results

Date: 2026-11-20. Environment: Ubuntu 24.04, .NET SDK 10.0.404, Linux x64. This file is a concise pointer; see [TEST_REPORT.md](TEST_REPORT.md) for test scope and the Windows checklist.

## Build

`dotnet build src/OneOS/OneOS.csproj -c Release --disable-build-servers --verbosity:minimal` — **PASS**, WPF Windows target cross-compiled; 0 warnings, 0 errors. Compile verification only; the app was not executed.

## Automated core checks

`dotnet run --project tests/OneOS.Core.Tests/OneOS.Core.Tests.csproj -c Release --disable-build-servers` — **11/11 PASS**: app catalog checks, preference round-trip/error handling, and move/resize geometry checks.

## Not verified

Windows startup and exit, XAML rendering, pointer/keyboard interaction, File Explorer and Windows Settings launch, resize/move/minimize/maximize/focus, actual theme rendering, and persistence on Windows remain untested. The current sandbox runs Ubuntu and has no Windows runtime or Wine; do not treat cross-compilation as execution evidence.
