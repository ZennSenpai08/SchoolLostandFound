# User manual

## Sign in and dashboard

Enter an active Administrator or Staff username/password. The dashboard displays current lost items, found items, pending claims, students and categories. Counts refresh when returning from a module. **Logout** returns to login; **Exit** closes the application.

Staff can manage operational records, claims and ordinary reports and change their own password. Administrators also manage users, activity logs, school settings and database backups/restoration.

## Students and categories

Open **Students**, enter a unique student number and full name, and optionally add course/grade, section, contact and email. Save creates a record. Select a row to edit, then save again. Clear starts a new entry. Search filters the list. Categories work the same way with a unique name and optional description.

Referenced students/categories cannot be deleted. Correct them by editing the existing record so historical references remain intact.

## Record lost or found property

Open **Lost Items** or **Found Items** and use **New / Clear**. Fields with `*` are required. Lost reports require a student, category, item name, date and location. Found records require category, item name, date/location, finder and storage location. Add descriptions, colors, brands and distinctive marks to improve matching. Scroll the editor for fields below the window.

Use Search to filter by item name, description or status. Click a row to load its fields. Save updates it; Delete removes an unreferenced, editable record. Active or released claims protect related items from changes. Any historical claim prevents deletion. Reservations, releases and claimed status are controlled through **Claims**, rather than directly editing the item status.

## Find possible matches

Open **Item Matching**, select an unresolved lost report, set a minimum score and choose **Find matches**. Candidates are available found items; items found before the loss and reserved/released property are excluded. The result shows a score and its component explanation.

| Component | Maximum points |
| --- | ---: |
| Same category | 25 |
| Item name token overlap | 25 |
| Same color | 15 |
| Same brand | 10 |
| Location token overlap | 10 |
| Distinctive-mark token overlap | 10 |
| Date proximity | 5 |

Text overlap is shared distinct tokens divided by all distinct tokens. Color and brand compare trimmed text without case differences. Missing fields earn no points. Date points are 5 within 7 days, 2 within 30 days, and 0 later. The score is a deterministic suggestion, not proof of ownership. Select a result and choose **Create claim** to preselect the pair in Claims.

## Verify a claim and release an item

1. In **Claims**, select the lost report and found item. The claimant is the student who owns that lost report. Enter ownership evidence and create a pending claim.
2. Select the claim row. Enter verification notes, choose **Under Verification**, then apply the status.
3. Inspect student identification and details that substantiate ownership. Enter the decision notes and choose **Approved** or **Rejected**. The application records the signed-in verifier and decision time.
4. Approval reserves the found item and marks the lost report matched. Competing approvals for either item are refused. A pending or verification claim may also be cancelled; cancellation of an approved claim frees its reservation.
5. For an approved claim, hand over the item to the verified owner, enter handover/receipt remarks, select **Released**, and confirm. The application atomically updates the claim, release time, found item and lost report and adds an audit entry.

Released, rejected and cancelled claims cannot reopen. Duplicate active claims for the same pair are refused. If a stale reservation prevents a change, refresh and investigate; failed operations roll back rather than partially releasing property. Search / Refresh reloads the claims table and selectors.

## Users and passwords

Administrators open **User Management**. Use New / Clear to create an account with a unique username, full name, recognized role and password. Select a row to update it. A blank password keeps the existing password; entering a new one resets it. Uncheck **Account enabled** to deactivate an account. Users are retained for audit history. The current administrator cannot deactivate/demote themselves, and at least one active administrator must remain.

Every user can change their own password in **Settings / Password**, providing their current password and a replacement of 10–128 characters. Password fields are masked and do not appear in user grids or reports.

## Reports and activity logs

Select a report type, enter an optional keyword, and click **Load**. Enable **Filter dates** to filter lost/found event dates, claim creation dates or activity timestamps, inclusive of both selected calendar days. Students and category totals include all dates. The table shows the loaded snapshot; click Load again after changing filters.

**Export CSV** exports that snapshot in UTF-8 with quotes and formula protection. **Print preview** uses a paginated record layout so full long-field values can continue across pages. Use its print control to send it to a configured printer. Administrators can open searchable **Activity Logs** from the dashboard or report selector. Reports and exports contain personal information: store and share them only with authorized personnel.

## Settings and maintenance

Administrators can change the school name and default minimum score. These persist in `settings.xml`. Backup/restore operates on the database only; see `INSTALLATION_GUIDE.md` for safety-copy behavior and recovery steps. Restoring forces a new login using the credentials in the backup.
