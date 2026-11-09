# School Lost & Found

A local VB.NET Windows Forms application for school staff to record lost/found property, review suggested matches, verify ownership and track handover. Uses .NET Framework 4.7.2 and the 32-bit Jet Access provider; no server or NuGet packages are required.

## Start

Extract the Release ZIP into a writable folder and run `SchoolLostAndFound.exe`. Keep its `Database` folder beside it. The shipped MDB contains demonstration records.

| Demo username | Demo password | Role |
| --- | --- | --- |
| admin | admin123 | Administrator |
| staff | staff123 | Staff |

Replace both demonstration passwords before storing real information. Passwords are migrated to salted hashes on first launch. New/reset passwords must contain 10–128 characters.

Read [Installation guide](INSTALLATION_GUIDE.md), [User manual](USER_MANUAL.md), [Test report](TEST_REPORT.md), and [Completion report](FINAL_COMPLETION_REPORT.md).

## Develop and verify

Open `SchoolLostAndFound.sln` in Visual Studio with VB desktop/.NET Framework tooling. Select x86. Run `powershell -ExecutionPolicy Bypass -File .\BuildAndTest.ps1` from this folder to build Debug and Release, run the integration suite on an isolated database, and preserve logs. Add `-Package` to produce fresh Release and source ZIPs in `Delivery`.

Tests never use the source MDB as their active database. Each run copies it into `Tests\bin\Runs`. Do not replace a deployed database with this demonstration MDB during an update. `CreateDb.ps1` is for a new demonstration database only; it refuses existing paths.
