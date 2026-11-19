# Technical decisions

## 2026-11-19: Windows desktop prototype

- C# and .NET 10 LTS, WPF (`net10.0-windows`). Windows remains responsible for kernel, drivers, security, and native applications. This is not an independent OS.
- WPF provides XAML, graphics, styling and normal native windows; it is Windows-only. It permits an early cross-targeting build using the .NET SDK without the Windows App SDK toolchain. This choice does not guarantee runtime behavior on Windows.
- Microsoft's recommended framework for new desktop applications is WinUI 3, part of Windows App SDK. It has newer UI features but adds SDK/deployment tooling; evaluate it again on a real Windows development system. WPF is chosen for this budget-constrained first prototype, not because WinUI is obsolete.
- .NET 10 is LTS and Microsoft lists support through 2028-11-14; apply current patch releases. .NET 11 is also supported but is STS.
- `EnableWindowsTargeting` downloads targeting packs for non-Windows builds. A successful Linux build does not verify execution, UI, or platform behavior.

References: [WPF overview](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/); [WinUI 3](https://learn.microsoft.com/en-us/windows/apps/winui/winui3/); [.NET support policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core); [NETSDK1100](https://learn.microsoft.com/en-us/dotnet/core/tools/sdk-errors/netsdk1100).
