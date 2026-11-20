# ONE OS design system — prototype baseline

This desktop UI reinterprets a soft One UI-inspired visual language for a wide Windows workspace. The sample gradient is replaceable; it is not an installed wallpaper system.

## Tokens

Current shared WPF resources in `src/OneOS/App.xaml` are `TextBrush`, `MutedBrush`, `CardBrush`, `DockBrush`, `InputBrush`, `AccentBrush`, `StrokeBrush` and `DesktopBrush`. `ThemeService` swaps the shared brushes for light/dark appearance. Controls should use `DynamicResource` for theme-dependent color rather than local literals.

Typography uses Segoe UI for interface text and Segoe MDL2 Assets for existing glyphs. The main header uses 15/11 px display/support text; secondary app window titles use 13 px. Current common spacings are roughly 4–12 px in compact controls and 20–28 px in major panels. Dock controls target a compact 36–42 px height; launcher cards use a 48 px icon tile. Surfaces use 14–24 px corner radii depending on size.

## Shared controls and interaction states

`SoftButton` provides consistent rounded buttons, hover/pressed opacity and a disabled opacity state. Use WPF `Button` semantics and provide `ToolTip` and AutomationProperties names to icon-only controls. Search and text input use the shared input/text resources. Internal application content uses scrollable space and shared resources.

Window title bars expose minimize, maximize/restore and close actions. The top-level WPF window retains native Windows chrome and also exposes explicit accessible client controls. The dock overlays the desktop workspace. The launcher and dock are layered above internal windows without a global topmost window setting.

## Accessibility and motion

The prototype uses keyboard search (`Enter`, `Escape`, `Ctrl+Space`), tooltips and automation names on key window controls. It currently has no custom animation system, wallpaper picker, high-contrast audit, reduced-motion setting, or full keyboard traversal audit. Test display scaling, contrast and accessibility on Windows before release.
