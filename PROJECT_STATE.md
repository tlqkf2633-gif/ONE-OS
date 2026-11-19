# ONE OS — project state

Updated: 2026-11-19

- Milestone: 1, foundation; development branch `dev/prototype`.
- Environment: Ubuntu 24.04 sandbox, .NET SDK 10.0.404 installed in user directory; no Windows runtime or Wine. Windows UI execution cannot be verified here.
- Stack: C# / .NET 10 LTS / WPF. Normal Windows application; no shell replacement.
- Structure: `src/OneOS` WPF app. Technical rationale: `TECHNICAL_DECISIONS.md`.
- Implemented: minimal app entry, window and color resources.
- Foundation build: BUILD VERIFIED on Linux, `dotnet build src/OneOS/OneOS.csproj -c Release`, 2026-11-19. This is cross-compilation, not Windows execution.
- Runtime and interactions: NOT TESTED.
- Next: desktop, dock, launcher, windows, internal apps and theme; build often. Verify on Windows separately.
- Repository: existing `tlqkf2633-gif/ONE-OS` is public; user decision on switching it to private is pending. Do not push while public.
