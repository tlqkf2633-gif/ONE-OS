# Known issues and limitations

- The environment is Ubuntu, so the Windows application has not been launched. WPF UI, pointer input, and Explorer/Settings launch integrations need a Windows 10/11 interaction test.
- In-app windows are workspace elements, not independent native OS windows. Sophisticated snap layouts and multi-monitor state restoration are not implemented.
- The dock is an overlay inside the ONE OS window, including above maximized internal content. It is not a system-wide always-on-top dock. System tray and hardware status are not integrated.
- The prototype delegates file browsing to Windows File Explorer; it does not implement its own file manager or file operations.
- Notes are stored as a single local plain-text file. There is no autosave or multi-note model.
- The starter executable test harness tests core logic only. No automated WPF UI tests exist yet.
- This is a prototype; do not deploy it as a replacement for Windows Explorer or as an independent operating system.
