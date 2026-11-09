# Verification report — 2026-11-09

## Results

| Check | Result |
| --- | --- |
| Debug x86 solution rebuild | Passed |
| Release x86 solution rebuild | Passed |
| Release test-runner rebuild | Passed |
| Original integration baseline | 33 checks passed |
| Expanded Debug integration suite | 131 checks passed |
| Final Release integration suite | **131 checks passed; process exit code 0** |
| Form smoke verification | 11 form variants constructed; 7 populated grid bindings checked |
| Report rendering | Long text paginated to completion and reset successfully |
| Source MDB preservation | SHA-256 equals recovery baseline |

Built using the installed Visual Studio 18 Insiders MSBuild, targeting .NET Framework 4.7.2/x86. No warnings/errors were emitted by the final build commands. The Release executable matches the assembly used by the final test runner.

Evidence is in `Delivery/Verification-20261109-125246-727`: `build-Debug.txt`, `build-Release.txt`, `build-tests.txt`, `test-results.txt`, `database-hash.txt`, CSV output and PNG captures. The underlying disposable run is `Tests/bin/Runs/20261109-125252-133`.

Source/demo MDB SHA-256: `CF9E3815FD323DE2780CE4583598130BA0B420D409C8D3EE33D0F88E24A39B29`.

Verified Release executable SHA-256: `F999EB4296589D42C34E7CE99E2472FFBC3AA3BB632CF2BBA9B6961E3E61D3FA`.

## Coverage

- Authentication: SQL-injection rejection, wrong/case-sensitive passwords, repeated legacy migration, PBKDF2 verification, inactive users, own-password change, administrator reset, logout and denied logged-out access.
- Authorization: staff denied users/activity logs/settings/backup/restore; persisted deactivation invalidates session access; current/last-administrator protection; unknown roles denied; no password fields in user grids or reports.
- CRUD: student/category/item/user creation and updates, apostrophe search, optional email, invalid email/required/length validation, unique names/numbers/usernames, real foreign keys, future dates, deletion dependencies, stale edits, entity allowlists and manual-release rejection.
- Matching: exact 100 score, empty-field zero score, weighted token overlap, threshold validation, reserved-item exclusion and found-before-loss exclusion at threshold zero.
- Claims: required evidence, duplicate pair rejection, approved-only release, protected item edits, competing approval rejection, cancellation/freeing reservations, terminal-state protection, verifier identity, timestamps and historical deletion protection.
- Transactions: uncommitted data and audit roll back; a release that updates the found item and then encounters a stale lost reservation rolls back the found item, claim and audit count.
- Reporting: six actual database reports, keyword/date filtering, inclusive dates, invalid date ranges, reconciled category totals, CSV UTF-8 BOM/Unicode, comma/quote/newline handling and formula escaping, long-field print pagination and restart.
- Settings: XML special characters, threshold round-trip, invalid-value rejection without replacing settings, previous-file preservation and staff denial.
- Maintenance: verified backup, active-path/existing-file guards, corrupt MDB rejection, missing columns, wrong field type, rejection of in-use target, staging cleanup, exact pre-restore safety copy, data recovery and forced sign-in.
- Forms: login, dashboard, students, categories, matching, claims, reports, settings, lost-item editor, found-item editor and user editor. Tests run forms at zero opacity, invoke their actual load/data-binding behavior and save `DrawToBitmap` captures. Reviewed captures show usable default layouts and scrollable item editors. Updated claim/item table ordering is visible in final captures.

## Limits and remaining deployment acceptance

These are deterministic integration and form smoke tests, not a claim of exhaustive UI or security coverage. Service workflows are exercised against real Jet database copies. The UI tests do not click through every dialog, verify keyboard/screen-reader behavior, test all display-scaling levels, or perform a prolonged manual session. The logout session operation is tested; the application's login-loop wiring was also reviewed in source.

Printed pages were rendered to bitmaps with the same report-rendering method used by printing. No physical printer, printer-driver dialog or paper output was tested. No installation was performed on a separate clean Windows computer. Filesystem power-loss recovery and multi-machine concurrent access were not tested; the latter is outside this release's single-instance/local-database scope.

Before operational deployment, an operator should install the Release ZIP on the intended PC, replace demo passwords, walk through one test claim through handover, verify the actual printer and backup location, and confirm folder access permissions. Use a practice installation for that walkthrough.

## Reproduction

Run `powershell -ExecutionPolicy Bypass -File .\BuildAndTest.ps1` in the source root. This rebuilds Debug/Release, runs the Release suite and saves timestamped evidence. Add `-Package` for delivery archives. `Tests\bin\Tests.exe <source-mdb> --forms` runs only the form smoke checks against a fresh copy. All regular test mutations occur under `Tests\bin\Runs`; the source/demo/runtime MDB is never selected as the test's active database.
