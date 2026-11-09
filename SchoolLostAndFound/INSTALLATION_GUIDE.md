# Installation and maintenance

## Requirements

- Windows desktop with .NET Framework 4.7.2 or compatible 4.8 runtime.
- Microsoft.Jet.OLEDB.4.0 accessible to a **32-bit** application. The supplied executable targets x86, including on 64-bit Windows. Microsoft Access itself is not required.
- A local writable application folder. Allow the operator to create/update the MDB, Jet lock file, settings XML and restoration safety copies.
- A display large enough for the 1024 × 640 dashboard. Item forms provide scrolling; maximize for more space.

This is a single-instance, local-database release. Do not share one MDB among computers or run it from a network/cloud-synchronized folder.

## First installation

1. Extract the Release ZIP completely to a folder such as `Documents\SchoolLostAndFound`. Do not run inside the ZIP or a protected `Program Files` folder.
2. Verify that `SchoolLostAndFound.exe` and `Database\SchoolLostAndFound.mdb` are present. Keep the folder structure intact.
3. Run `SchoolLostAndFound.exe`. Startup widens legacy password storage and migrates demonstration passwords to salted PBKDF2 hashes. It also reconciles reservations for legacy approved claims without deleting records.
4. Sign in with `admin` / `admin123`. Open **Settings / Password** and change this password. Open **User Management**, select `staff`, set a new password, and save, or deactivate that account after creating your own staff accounts.
5. Configure the school name and default matching score. Confirm students, categories and inventory appear. The included records are demonstration data, including historical claims. Keep a separate demo installation for practice; do not mistake sample records for actual school inventory.
6. Create a verified backup before entering real records. Protect the application, backup and export folders with Windows permissions appropriate to school personnel.

## Updating an existing installation

Close the app and create/retain a verified backup first. Replace application executable files only. Preserve the deployed `Database` folder, `settings.xml`, and backup files. Never extract the bundled demo MDB over a working database. Building this source copies the demo MDB to build output when newer; use a separate deployed folder for actual records.

## Backups and restoration

Administrators use **Settings / Password → Create verified database backup** and choose a new `.mdb` filename. Existing backup files are not overwritten. Keep another copy on protected storage outside the application directory. School settings are separate: also preserve `settings.xml` if you need to recover them.

To restore, select **Restore database backup**, choose the MDB, and confirm replacement. The application validates its schema and relationships, requires exclusive access, atomically replaces the current database, preserves an exact `.before-restore-<timestamp>.mdb` beside it and returns to login. Use credentials from the restored database. A newer account or password may not exist in an older backup. Settings XML is not replaced.

If recovery is needed after a failed application start, close all processes using the MDB. Preserve the current files before copying a known-good safety MDB back to `Database\SchoolLostAndFound.mdb`. A schema-validation failure must be investigated; do not delete constraints to bypass it.

## Troubleshooting

| Symptom | Action |
| --- | --- |
| Jet provider is not registered | Confirm you are running the supplied x86 build. Have the Windows administrator verify the 32-bit Jet installation. |
| Database not found | Restore the `Database` subfolder beside the executable; the error shows the expected path. |
| Access denied / database in use | Check folder write permissions, close Access/database viewers and other processes, and retry. |
| Already running | Use the existing application instance. |
| Invalid/inactive login | Check username and password; an administrator can reset or activate accounts. Five failed UI attempts trigger a 30-second wait. |
| Invalid settings XML | Preserve the damaged `settings.xml`; close the app and rename it to retain a copy, then restart to use defaults and save settings again. |
| Printer/preview failure | Configure a Windows printer driver and retry. CSV export remains available. |

Application roles govern actions inside the program. Anyone who can directly alter the MDB or executable can bypass those roles; Windows file access and protected backups are part of deployment.

## Source build

Use Visual Studio/MSBuild with VB Windows Forms and .NET Framework 4.7.2 reference assemblies. Both solution configurations are x86. `BuildAndTest.ps1` discovers MSBuild, builds both configurations, builds the test runner against Release and executes it. `-Package` creates new timestamped delivery folders and ZIPs without overwriting an existing deployment. See `TEST_REPORT.md` for verification limits.
