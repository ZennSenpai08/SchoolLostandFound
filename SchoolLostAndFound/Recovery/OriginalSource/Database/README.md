# School Lost & Found Management System Database

This directory contains the primary Microsoft Access database file for the application.

## Database Name
`SchoolLostAndFound.mdb`

## How the Database is Generated
Since this environment (Antigravity) is purely script-based and does not have the Microsoft Access UI, the `.mdb` file was generated programmatically using a PowerShell script (`CreateDb.ps1`). 
The script leverages COM objects (`ADOX.Catalog` and `ADODB.Connection`) via the 32-bit `Microsoft.Jet.OLEDB.4.0` provider to:
1. Create the binary `.mdb` file.
2. Execute SQL commands to construct the tables.
3. Establish foreign key relationships.
4. Insert realistic sample data.

## How the Database is Copied to the Output Directory
During development in Visual Studio, you must set this file to **Copy to Output Directory** (e.g., "Copy if newer").
This ensures that when the application compiles, `SchoolLostAndFound.mdb` is copied to the `bin\Debug\Database\` folder. The application's connection string uses `Application.StartupPath` to dynamically point to the database in that runtime location.

## Tables and Fields

### Users
- `UserID` (AutoNumber, PK)
- `Username` (Short Text, Unique)
- `Password` (Short Text)
- `FullName` (Short Text)
- `Role` (Short Text)
- `IsActive` (Yes/No)

### Students
- `StudentID` (AutoNumber, PK)
- `StudentNumber` (Short Text, Unique)
- `FullName` (Short Text)
- `CourseOrGrade` (Short Text)
- `Section` (Short Text)
- `ContactNumber` (Short Text)
- `Email` (Short Text)

### Categories
- `CategoryID` (AutoNumber, PK)
- `CategoryName` (Short Text, Unique)
- `Description` (Memo)

### LostItems
- `LostItemID` (AutoNumber, PK)
- `StudentID` (Number, FK)
- `CategoryID` (Number, FK)
- `ItemName` (Short Text)
- `Description` (Memo)
- `Color` (Short Text)
- `Brand` (Short Text)
- `DateLost` (Date/Time)
- `LocationLost` (Short Text)
- `DistinctiveMarks` (Memo)
- `Status` (Short Text)
- `DateReported` (Date/Time)

### FoundItems
- `FoundItemID` (AutoNumber, PK)
- `CategoryID` (Number, FK)
- `ItemName` (Short Text)
- `Description` (Memo)
- `Color` (Short Text)
- `Brand` (Short Text)
- `DateFound` (Date/Time)
- `LocationFound` (Short Text)
- `FoundBy` (Short Text)
- `StorageLocation` (Short Text)
- `DistinctiveMarks` (Memo)
- `Status` (Short Text)
- `DateRecorded` (Date/Time)

### Claims
- `ClaimID` (AutoNumber, PK)
- `LostItemID` (Number, FK)
- `FoundItemID` (Number, FK)
- `StudentID` (Number, FK)
- `ClaimDate` (Date/Time)
- `ClaimDescription` (Memo)
- `VerificationDetails` (Memo)
- `Status` (Short Text)
- `VerifiedBy` (Number, FK - Allow Null)
- `VerificationDate` (Date/Time)
- `ReleaseDate` (Date/Time)
- `Remarks` (Memo)

### ActivityLogs
- `LogID` (AutoNumber, PK)
- `UserID` (Number, FK)
- `Action` (Short Text)
- `Description` (Memo)
- `ActionDate` (Date/Time)

## Relationships (Referential Integrity Enforced)
- `Students.StudentID` -> `LostItems.StudentID`
- `Students.StudentID` -> `Claims.StudentID`
- `Categories.CategoryID` -> `LostItems.CategoryID`
- `Categories.CategoryID` -> `FoundItems.CategoryID`
- `LostItems.LostItemID` -> `Claims.LostItemID`
- `FoundItems.FoundItemID` -> `Claims.FoundItemID`
- `Users.UserID` -> `Claims.VerifiedBy`
- `Users.UserID` -> `ActivityLogs.UserID`

## Default Accounts
| Username | Password | Role |
| :--- | :--- | :--- |
| admin | admin123 | Administrator |
| staff | staff123 | Staff |
