# Changelog

## 2026-11-19 — Initial WPF prototype

Added .NET 10 WPF Windows application and platform-neutral core; desktop surface, dock, searchable launcher, app catalog, four internal apps, movable/resizable/minimize/maximize/close app windows, and saved light/dark theme. Added basic search/preference tests and developer/build/architecture/verification/limitations documentation. Cross-target build succeeded on Ubuntu; Windows execution remains unverified.

## 2026-11-20 — Desktop interaction redesign

Added explicit main-window client controls, Thumb-based internal window movement, eight-direction resizing with tested geometry constraints, and a persistent in-workspace dock overlay. Routed launcher File Explorer and Settings handoff to fixed Windows destinations; refreshed project and test documentation. Latest Ubuntu cross-target build passes with zero warnings/errors and core tests pass 11/11. Windows runtime/UI verification is pending.
