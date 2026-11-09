param([Parameter(Mandatory=$true)][string]$DatabasePath)
$ErrorActionPreference = "Stop"
if ([Environment]::Is64BitProcess) { throw "Run with 32-bit Windows PowerShell (SysWOW64)." }
$dbPath = [System.IO.Path]::GetFullPath($DatabasePath)
if (Test-Path -LiteralPath $dbPath) { throw "Database exists. Choose a new path; existing databases are never overwritten." }
$catalog = New-Object -ComObject ADOX.Catalog
$catalog.Create("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=$dbPath")

$conn = New-Object -ComObject ADODB.Connection
$conn.Open("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=$dbPath")

# Create Users
$conn.Execute("CREATE TABLE Users (
    UserID AUTOINCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    [Password] VARCHAR(50) NOT NULL,
    FullName VARCHAR(100) NOT NULL,
    Role VARCHAR(20) NOT NULL,
    IsActive BIT DEFAULT -1
)")

# Create Students
$conn.Execute("CREATE TABLE Students (
    StudentID AUTOINCREMENT PRIMARY KEY,
    StudentNumber VARCHAR(20) NOT NULL UNIQUE,
    FullName VARCHAR(100) NOT NULL,
    CourseOrGrade VARCHAR(50),
    [Section] VARCHAR(50),
    ContactNumber VARCHAR(20),
    Email VARCHAR(50)
)")

# Create Categories
$conn.Execute("CREATE TABLE Categories (
    CategoryID AUTOINCREMENT PRIMARY KEY,
    CategoryName VARCHAR(50) NOT NULL UNIQUE,
    Description MEMO
)")

# Create LostItems
$conn.Execute("CREATE TABLE LostItems (
    LostItemID AUTOINCREMENT PRIMARY KEY,
    StudentID INTEGER,
    CategoryID INTEGER,
    ItemName VARCHAR(100) NOT NULL,
    Description MEMO,
    Color VARCHAR(30),
    Brand VARCHAR(50),
    DateLost DATETIME,
    LocationLost VARCHAR(100),
    DistinctiveMarks MEMO,
    Status VARCHAR(30) NOT NULL,
    DateReported DATETIME DEFAULT NOW()
)")

# Create FoundItems
$conn.Execute("CREATE TABLE FoundItems (
    FoundItemID AUTOINCREMENT PRIMARY KEY,
    CategoryID INTEGER,
    ItemName VARCHAR(100) NOT NULL,
    Description MEMO,
    Color VARCHAR(30),
    Brand VARCHAR(50),
    DateFound DATETIME,
    LocationFound VARCHAR(100),
    FoundBy VARCHAR(100),
    StorageLocation VARCHAR(100),
    DistinctiveMarks MEMO,
    Status VARCHAR(30) NOT NULL,
    DateRecorded DATETIME DEFAULT NOW()
)")

# Create Claims
$conn.Execute("CREATE TABLE Claims (
    ClaimID AUTOINCREMENT PRIMARY KEY,
    LostItemID INTEGER,
    FoundItemID INTEGER,
    StudentID INTEGER,
    ClaimDate DATETIME DEFAULT NOW(),
    ClaimDescription MEMO,
    VerificationDetails MEMO,
    Status VARCHAR(30) NOT NULL,
    VerifiedBy INTEGER,
    VerificationDate DATETIME,
    ReleaseDate DATETIME,
    Remarks MEMO
)")

# Create ActivityLogs
$conn.Execute("CREATE TABLE ActivityLogs (
    LogID AUTOINCREMENT PRIMARY KEY,
    UserID INTEGER,
    [Action] VARCHAR(50) NOT NULL,
    Description MEMO,
    ActionDate DATETIME DEFAULT NOW()
)")

# Add Foreign Keys
$conn.Execute("ALTER TABLE LostItems ADD CONSTRAINT FK_LostItems_Students FOREIGN KEY (StudentID) REFERENCES Students(StudentID)")
$conn.Execute("ALTER TABLE LostItems ADD CONSTRAINT FK_LostItems_Categories FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)")

$conn.Execute("ALTER TABLE FoundItems ADD CONSTRAINT FK_FoundItems_Categories FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)")

$conn.Execute("ALTER TABLE Claims ADD CONSTRAINT FK_Claims_LostItems FOREIGN KEY (LostItemID) REFERENCES LostItems(LostItemID)")
$conn.Execute("ALTER TABLE Claims ADD CONSTRAINT FK_Claims_FoundItems FOREIGN KEY (FoundItemID) REFERENCES FoundItems(FoundItemID)")
$conn.Execute("ALTER TABLE Claims ADD CONSTRAINT FK_Claims_Students FOREIGN KEY (StudentID) REFERENCES Students(StudentID)")
$conn.Execute("ALTER TABLE Claims ADD CONSTRAINT FK_Claims_Users FOREIGN KEY (VerifiedBy) REFERENCES Users(UserID)")

$conn.Execute("ALTER TABLE ActivityLogs ADD CONSTRAINT FK_ActivityLogs_Users FOREIGN KEY (UserID) REFERENCES Users(UserID)")

# Insert Users
$conn.Execute("INSERT INTO Users (Username, [Password], FullName, Role, IsActive) VALUES ('admin', 'admin123', 'System Administrator', 'Administrator', -1)")
$conn.Execute("INSERT INTO Users (Username, [Password], FullName, Role, IsActive) VALUES ('staff', 'staff123', 'Front Desk Staff', 'Staff', -1)")

# Insert Categories
$conn.Execute("INSERT INTO Categories (CategoryName, Description) VALUES ('Electronics', 'Phones, laptops, tablets, chargers')")
$conn.Execute("INSERT INTO Categories (CategoryName, Description) VALUES ('Wallet', 'Wallets, purses, coin pouches')")
$conn.Execute("INSERT INTO Categories (CategoryName, Description) VALUES ('ID Card', 'School IDs, driver''s licenses')")
$conn.Execute("INSERT INTO Categories (CategoryName, Description) VALUES ('Keys', 'Car keys, house keys, locker keys')")
$conn.Execute("INSERT INTO Categories (CategoryName, Description) VALUES ('Clothing', 'Jackets, sweaters, hats, uniforms')")
$conn.Execute("INSERT INTO Categories (CategoryName, Description) VALUES ('School Supplies', 'Notebooks, pens, calculators')")
$conn.Execute("INSERT INTO Categories (CategoryName, Description) VALUES ('Bags', 'Backpacks, tote bags')")
$conn.Execute("INSERT INTO Categories (CategoryName, Description) VALUES ('Other', 'Items that do not fit into other categories')")

# Insert Students
$conn.Execute("INSERT INTO Students (StudentNumber, FullName, CourseOrGrade, [Section], ContactNumber, Email) VALUES ('2023-0001', 'Juan Dela Cruz', 'BS Computer Science', 'CS-1A', '0917-123-4567', 'juan@student.edu.ph')")
$conn.Execute("INSERT INTO Students (StudentNumber, FullName, CourseOrGrade, [Section], ContactNumber, Email) VALUES ('2023-0002', 'Maria Clara', 'BS Information Tech', 'IT-2B', '0918-234-5678', 'maria@student.edu.ph')")
$conn.Execute("INSERT INTO Students (StudentNumber, FullName, CourseOrGrade, [Section], ContactNumber, Email) VALUES ('2023-0003', 'Jose Rizal', 'BS Civil Engineering', 'CE-3A', '0919-345-6789', 'jose@student.edu.ph')")
$conn.Execute("INSERT INTO Students (StudentNumber, FullName, CourseOrGrade, [Section], ContactNumber, Email) VALUES ('2023-0004', 'Andres Bonifacio', 'BS Architecture', 'AR-1C', '0920-456-7890', 'andres@student.edu.ph')")
$conn.Execute("INSERT INTO Students (StudentNumber, FullName, CourseOrGrade, [Section], ContactNumber, Email) VALUES ('2023-0005', 'Gabriela Silang', 'BA Communication', 'CO-4A', '0921-567-8901', 'gabriela@student.edu.ph')")
$conn.Execute("INSERT INTO Students (StudentNumber, FullName, CourseOrGrade, [Section], ContactNumber, Email) VALUES ('2023-0006', 'Apolinario Mabini', 'BS Accountancy', 'AC-2A', '0922-678-9012', 'apolinario@student.edu.ph')")
$conn.Execute("INSERT INTO Students (StudentNumber, FullName, CourseOrGrade, [Section], ContactNumber, Email) VALUES ('2023-0007', 'Emilio Aguinaldo', 'BS Business Admin', 'BA-1B', '0923-789-0123', 'emilio@student.edu.ph')")
$conn.Execute("INSERT INTO Students (StudentNumber, FullName, CourseOrGrade, [Section], ContactNumber, Email) VALUES ('2023-0008', 'Melchora Aquino', 'BS Nursing', 'NU-3C', '0924-890-1234', 'melchora@student.edu.ph')")
$conn.Execute("INSERT INTO Students (StudentNumber, FullName, CourseOrGrade, [Section], ContactNumber, Email) VALUES ('2023-0009', 'Lapu-Lapu', 'BS Marine Biology', 'MB-1A', '0925-901-2345', 'lapulapu@student.edu.ph')")
$conn.Execute("INSERT INTO Students (StudentNumber, FullName, CourseOrGrade, [Section], ContactNumber, Email) VALUES ('2023-0010', 'Marcelo H. del Pilar', 'BA Political Science', 'PS-4B', '0926-012-3456', 'marcelo@student.edu.ph')")

# Insert LostItems
$conn.Execute("INSERT INTO LostItems (StudentID, CategoryID, ItemName, Description, Color, Brand, DateLost, LocationLost, DistinctiveMarks, Status) VALUES (1, 1, 'iPhone 13', 'Black case with a small crack on the screen protector', 'Black', 'Apple', '2023-10-15', 'Library', 'Crack on top right', 'Lost')")
$conn.Execute("INSERT INTO LostItems (StudentID, CategoryID, ItemName, Description, Color, Brand, DateLost, LocationLost, DistinctiveMarks, Status) VALUES (2, 2, 'Leather Wallet', 'Contains my ID and some cash', 'Brown', 'Seiko', '2023-10-16', 'Cafeteria', 'Small scratch on the back', 'Lost')")
$conn.Execute("INSERT INTO LostItems (StudentID, CategoryID, ItemName, Description, Color, Brand, DateLost, LocationLost, DistinctiveMarks, Status) VALUES (3, 6, 'Scientific Calculator', 'Casio FX-991EX', 'Black', 'Casio', '2023-10-14', 'Engineering Bldg Room 302', 'Name sticker at the back', 'Lost')")
$conn.Execute("INSERT INTO LostItems (StudentID, CategoryID, ItemName, Description, Color, Brand, DateLost, LocationLost, DistinctiveMarks, Status) VALUES (4, 4, 'Car Keys', 'Toyota car keys with a red lanyard', 'Black/Silver', 'Toyota', '2023-10-17', 'Parking Lot B', 'Red lanyard with school logo', 'Lost')")
$conn.Execute("INSERT INTO LostItems (StudentID, CategoryID, ItemName, Description, Color, Brand, DateLost, LocationLost, DistinctiveMarks, Status) VALUES (5, 7, 'Jansport Backpack', 'Navy blue backpack containing books and notes', 'Navy Blue', 'Jansport', '2023-10-18', 'Student Lounge', 'Slightly faded zipper', 'Claimed')")
$conn.Execute("INSERT INTO LostItems (StudentID, CategoryID, ItemName, Description, Color, Brand, DateLost, LocationLost, DistinctiveMarks, Status) VALUES (6, 3, 'Student ID', 'My school ID with lace', 'White', 'School', '2023-10-19', 'Main Gate', 'Lace is green', 'Lost')")
$conn.Execute("INSERT INTO LostItems (StudentID, CategoryID, ItemName, Description, Color, Brand, DateLost, LocationLost, DistinctiveMarks, Status) VALUES (7, 5, 'Varsity Jacket', 'School varsity jacket, size L', 'Red/White', 'Local', '2023-10-20', 'Gymnasium', 'Has my initials EA embroidered inside', 'Lost')")
$conn.Execute("INSERT INTO LostItems (StudentID, CategoryID, ItemName, Description, Color, Brand, DateLost, LocationLost, DistinctiveMarks, Status) VALUES (8, 1, 'Airpods Pro', 'White case, no cover', 'White', 'Apple', '2023-10-21', 'Science Lab', 'Small dent on the left side', 'Lost')")
$conn.Execute("INSERT INTO LostItems (StudentID, CategoryID, ItemName, Description, Color, Brand, DateLost, LocationLost, DistinctiveMarks, Status) VALUES (9, 6, 'Binder Notebook', 'Thick blue binder with my notes', 'Blue', 'Cattleya', '2023-10-22', 'Library 2nd Floor', 'Sticker of a fish on the cover', 'Lost')")
$conn.Execute("INSERT INTO LostItems (StudentID, CategoryID, ItemName, Description, Color, Brand, DateLost, LocationLost, DistinctiveMarks, Status) VALUES (10, 8, 'Water Tumbler', 'Aquaflask 32oz', 'Mint Green', 'Aquaflask', '2023-10-23', 'Open Field', 'Several band stickers', 'Lost')")

# Insert FoundItems
$conn.Execute("INSERT INTO FoundItems (CategoryID, ItemName, Description, Color, Brand, DateFound, LocationFound, FoundBy, StorageLocation, DistinctiveMarks, Status) VALUES (1, 'iPhone 13', 'Found on a table', 'Black', 'Apple', '2023-10-15', 'Library', 'Librarian', 'Cabinet A1', 'Crack on top right', 'Available for Claim')")
$conn.Execute("INSERT INTO FoundItems (CategoryID, ItemName, Description, Color, Brand, DateFound, LocationFound, FoundBy, StorageLocation, DistinctiveMarks, Status) VALUES (2, 'Leather Wallet', 'Found near the entrance', 'Brown', 'Unknown', '2023-10-16', 'Cafeteria', 'Janitor', 'Safe Box', 'Contains a picture inside', 'Available for Claim')")
$conn.Execute("INSERT INTO FoundItems (CategoryID, ItemName, Description, Color, Brand, DateFound, LocationFound, FoundBy, StorageLocation, DistinctiveMarks, Status) VALUES (6, 'Scientific Calculator', 'Left on the desk', 'Black', 'Casio', '2023-10-15', 'Engineering Bldg Room 302', 'Prof. Santos', 'Cabinet A2', 'Name sticker faded', 'Available for Claim')")
$conn.Execute("INSERT INTO FoundItems (CategoryID, ItemName, Description, Color, Brand, DateFound, LocationFound, FoundBy, StorageLocation, DistinctiveMarks, Status) VALUES (7, 'Jansport Backpack', 'Navy blue backpack', 'Navy Blue', 'Jansport', '2023-10-18', 'Student Lounge', 'Security Guard', 'Storage Room', 'Faded zipper', 'Released')")
$conn.Execute("INSERT INTO FoundItems (CategoryID, ItemName, Description, Color, Brand, DateFound, LocationFound, FoundBy, StorageLocation, DistinctiveMarks, Status) VALUES (3, 'Student ID - Jose Rizal', 'ID card found on floor', 'White', 'School', '2023-10-19', 'Main Gate', 'Guard on Duty', 'Drawer 1', 'Green lace', 'Available for Claim')")
$conn.Execute("INSERT INTO FoundItems (CategoryID, ItemName, Description, Color, Brand, DateFound, LocationFound, FoundBy, StorageLocation, DistinctiveMarks, Status) VALUES (5, 'Varsity Jacket', 'Red and white jacket', 'Red/White', 'Local', '2023-10-21', 'Gymnasium', 'Coach', 'Storage Room', 'Initials EA', 'Available for Claim')")
$conn.Execute("INSERT INTO FoundItems (CategoryID, ItemName, Description, Color, Brand, DateFound, LocationFound, FoundBy, StorageLocation, DistinctiveMarks, Status) VALUES (8, 'Water Tumbler', 'Aquaflask', 'Mint Green', 'Aquaflask', '2023-10-23', 'Open Field', 'Student', 'Cabinet A3', 'Band stickers', 'Available for Claim')")
$conn.Execute("INSERT INTO FoundItems (CategoryID, ItemName, Description, Color, Brand, DateFound, LocationFound, FoundBy, StorageLocation, DistinctiveMarks, Status) VALUES (4, 'House Keys', 'Set of 3 keys', 'Silver', 'Unknown', '2023-10-10', 'Library', 'Librarian', 'Drawer 2', 'Blue keychain', 'Unclaimed')")
$conn.Execute("INSERT INTO FoundItems (CategoryID, ItemName, Description, Color, Brand, DateFound, LocationFound, FoundBy, StorageLocation, DistinctiveMarks, Status) VALUES (6, 'Parker Pen', 'Gold plated pen', 'Gold', 'Parker', '2023-10-12', 'Admin Office', 'Staff', 'Safe Box', 'None', 'Unclaimed')")
$conn.Execute("INSERT INTO FoundItems (CategoryID, ItemName, Description, Color, Brand, DateFound, LocationFound, FoundBy, StorageLocation, DistinctiveMarks, Status) VALUES (1, 'Samsung Galaxy S22', 'Found on a bench', 'White', 'Samsung', '2023-10-24', 'Park', 'Security', 'Cabinet A1', 'Pink case', 'Available for Claim')")

# Insert Claims
$conn.Execute("INSERT INTO Claims (LostItemID, FoundItemID, StudentID, ClaimDescription, Status, VerifiedBy, VerificationDate, ReleaseDate, Remarks) VALUES (5, 4, 5, 'This is my bag, I left it at the lounge.', 'Released', 1, '2023-10-19', '2023-10-19', 'Verified via student ID and contents of the bag.')")
$conn.Execute("INSERT INTO Claims (LostItemID, FoundItemID, StudentID, ClaimDescription, Status) VALUES (1, 1, 1, 'That is my phone, it has a cracked screen.', 'Pending')")
$conn.Execute("INSERT INTO Claims (LostItemID, FoundItemID, StudentID, ClaimDescription, Status) VALUES (3, 3, 3, 'I left my calculator in that exact room.', 'Under Verification')")
$conn.Execute("INSERT INTO Claims (LostItemID, FoundItemID, StudentID, ClaimDescription, Status) VALUES (2, 2, 2, 'My wallet contains my ID which matches my face.', 'Pending')")
$conn.Execute("INSERT INTO Claims (LostItemID, FoundItemID, StudentID, ClaimDescription, Status, VerifiedBy, VerificationDate, Remarks) VALUES (7, 6, 7, 'That is my varsity jacket with my initials.', 'Approved', 2, '2023-10-25', 'Initials matched perfectly.')")

# Insert ActivityLogs
$conn.Execute("INSERT INTO ActivityLogs (UserID, [Action], Description) VALUES (1, 'Login', 'Administrator logged into the system.')")
$conn.Execute("INSERT INTO ActivityLogs (UserID, [Action], Description) VALUES (1, 'Release Item', 'Released Found Item ID 4 to Student ID 5.')")

$conn.Close()
Write-Output "Database created successfully at $dbPath"
