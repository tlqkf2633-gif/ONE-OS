# Test report

Date: 2026-11-20. Environment: Ubuntu 24.04 x64, .NET SDK 10.0.404, `DISPLAY=:0`; `wine` unavailable. The WPF Windows target can be cross-compiled but not run in this environment. No Windows screenshots or UI interaction evidence were produced.

## Build

`dotnet build src/OneOS/OneOS.csproj -c Release --disable-build-servers --verbosity:minimal` — PASS on the latest source, Windows-target project compiled, zero warnings and zero errors. XAML/code compilation only; this is not UI execution.

## Automated core tests

`dotnet run --project tests/OneOS.Core.Tests/OneOS.Core.Tests.csproj -c Release --disable-build-servers` — 11/11 PASS on the latest source: app catalog uniqueness and search (4); preference round-trip/overwrite and corrupt JSON error path (2); window move visibility, left/right/top resize, and minimum size (5). This is an executable core test harness, not a WPF UI test suite.

## Changes addressing owner-reported issues

Before this redesign, the main window relied on standard Windows caption buttons; there were no explicit in-client minimize/maximize/close controls. Internal windows had manually wired button handlers, one bottom-right Thumb resize point, and custom mouse-capture movement. These source-code limitations are confirmed; the reported click failures themselves were not reproduced. This branch adds explicit main-window commands while keeping native Windows chrome, uses Thumb-based internal movement, implements eight resize edges/corners and shared core geometry rules, and keeps the dock in an overlay layer. The file launcher now starts Windows File Explorer, and Settings offers to start Windows Settings through a fixed Windows-only adapter. Those external launches are not runtime-tested here.

## Required Windows verification — pending

On Windows 10/11, verify main native and client minimize/maximize/restore/close controls; main-window resize from each edge/corner; internal title-bar drag and all eight resize directions; minimum sizes; internal min/max/restore/close; focus and overlapping windows; dock overlay above maximized internal content; launcher keyboard and mouse interaction; Explorer launch; Settings launch; theme persistence; and repeated close/reopen. Repeat at laptop and external-monitor scaling if available. No critical UI behavior is marked as tested until these steps have run.
