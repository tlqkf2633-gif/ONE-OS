# ONE OS

ONE OS is an experimental Windows desktop application, not an independent operating system or a Windows shell replacement. The foundation currently contains a minimal WPF window; follow `PROJECT_STATE.md` for verified milestones.

## Build and run on Windows

Install the current .NET 10 SDK, then run from this directory:

```powershell
dotnet build .\src\OneOS\OneOS.csproj -c Release
dotnet run --project .\src\OneOS\OneOS.csproj -c Release
```

The Windows Desktop Runtime is needed for framework-dependent distribution. The project may also be cross-compiled from Linux using the SDK and network access, but WPF cannot run on Linux. The initial build has been cross-compiled only; a Windows execution test remains required.

See `TECHNICAL_DECISIONS.md` for the stack choice. The app does not alter the Windows shell, startup, drivers, registry, or security settings.
