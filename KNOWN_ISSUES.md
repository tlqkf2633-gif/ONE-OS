# Known issues and limitations

- The environment is Ubuntu, so the Windows application has not been launched. WPF UI, input and shell integration need a Windows 10/11 run and interaction test.
- In-app windows approximate move/resize/minimize/maximize and focus. They are not independent native OS windows; sophisticated snap layouts and multi-monitor handling are not implemented.
- The dock runs in the application window, not as an always-on-top system dock. System tray and hardware status are not integrated.
- File Manager lists drives and directories and opens a chosen file via its Windows registered application. Copy/move/rename/delete and file properties are not implemented.
- Notes are stored as a single local plain-text file. There is no autosave or multi-note model.
- The starter executable test harness tests core logic only. No automated WPF UI tests exist yet.
- This is a prototype; do not deploy it as a replacement for Windows Explorer or as an independent operating system.
