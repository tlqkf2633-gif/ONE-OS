# ONE OS development status

Updated: 2026-11-20. Active branch: `dev/redesign`.

## Implemented — Windows runtime verification pending

The WPF main window has native Windows caption chrome, explicit in-client minimize/maximize/restore/close controls, a minimum window size, and full-workspace layout. The dock now overlays the desktop canvas rather than consuming a dedicated height row. Internal application windows now use WPF Thumb input for title-bar movement and eight-direction resize, plus minimize/maximize/restore/close and front-to-back focus. Geometry calculations are isolated in `OneOS.Core` and covered by unit-style tests. This is implementation status, not proof of actual pointer operation.

The launcher, internal Settings/Notes/Calculator views, and theme persistence are implemented. File Explorer and Windows Settings are delegated to Windows through fixed launch destinations. The project remains a normal Windows application; it does not control Windows shell/system settings.

## Verified in this sandbox

The latest WPF Windows-target Release build succeeds on Ubuntu with .NET SDK 10.0.404, zero warnings/errors. Eleven platform-neutral tests pass for catalog search, preference persistence/error handling, and window move/resize geometry.

## Not validated

The Windows application has not run here. Main and internal window control behavior, pointer hit testing, dock layering, DPI scaling, monitor changes, Explorer/Settings launch, and perceived design quality require Windows interaction testing. The owner-reported unresponsive controls cannot be reproduced in this Linux sandbox.

## Planned or outside this milestone

ONE Clock, ONE Calendar, movable widget system, notifications, hardware-backed quick settings, wallpaper/layout persistence, app discovery and safe installer workflow, first-run experience, removable-app cleanup, Samsung app distribution/compatibility, Galaxy Book and Phone Link integration, update/rollback, and ONE App Store are not implemented. Cleanup or installers must not execute without explicit item-level user approval and safe documented Windows APIs. Consult `ROADMAP.md`.
