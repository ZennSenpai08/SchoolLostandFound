Option Strict On
Imports Microsoft.VisualBasic
Imports System.Drawing
Imports System.Data.OleDb
Imports System.Reflection
Imports System.Security.Cryptography
Imports System.Text

Module ExtendedSuite
    Private Function Fingerprint(path As String) As String
        Using sha=SHA256.Create(), stream=File.OpenRead(path)
            Return Convert.ToBase64String(sha.ComputeHash(stream))
        End Using
    End Function
    Private Function StudentValues() As Dictionary(Of String,Object)
        Return New Dictionary(Of String,Object) From {{"StudentNumber","TEST-001"},{"FullName","Ana O'Brien"},{"CourseOrGrade","Grade 12"},{"Section","A"},{"ContactNumber","09123456789"},{"Email","ana@example.org"}}
    End Function
    Public Sub Run(directory As String)
        AppSettings.FilePath=Path.Combine(directory,"settings.xml")
        Dim studentData=StudentValues()
        Dim student=RecordService.Save("Students",0,studentData)
        Suite.Check(RecordService.List("Students","O'Brien").Rows.Count=1,"Student create/search preserves apostrophes")
        Suite.Reject(Sub() RecordService.Save("Students",0,studentData),"Duplicate student number rejected")
        studentData("Email")="invalid email"
        Suite.Reject(Sub() RecordService.Save("Students",student,studentData),"Invalid email rejected")
        studentData("Email")="" : studentData("FullName")=""
        Suite.Reject(Sub() RecordService.Save("Students",student,studentData),"Student full name required")
        studentData("FullName")=New String("A"c,101)
        Suite.Reject(Sub() RecordService.Save("Students",student,studentData),"Student field lengths enforced by service")
        studentData("FullName")="Updated Student"
        RecordService.Save("Students",student,studentData)
        Suite.Check(RecordService.List("Students","Updated Student").Rows.Count=1,"Student edit and optional blank email")
        Dim categoryData As New Dictionary(Of String,Object) From {{"CategoryName","Test category"},{"Description","Test description"}}
        Dim category=RecordService.Save("Categories",0,categoryData)
        Suite.Reject(Sub() RecordService.Save("Categories",0,categoryData),"Duplicate category rejected")
        categoryData("CategoryName")="Updated category"
        RecordService.Save("Categories",category,categoryData)
        Suite.Check(RecordService.List("Categories","Updated category").Rows.Count=1,"Category create/edit/search")
        Dim lost=RecordService.Save("LostItems",0,Suite.ItemValues(True,category,student))
        Dim found=RecordService.Save("FoundItems",0,Suite.ItemValues(False,category,student))
        Suite.Reject(Sub() RecordService.Delete("Students",student),"Referenced student cannot be deleted")
        Suite.Reject(Sub() RecordService.Delete("Categories",category),"Referenced category cannot be deleted")
        Suite.Reject(Sub() RecordService.Save("LostItems",0,New Dictionary(Of String,Object)()),"Missing item fields rejected")
        Dim invalid=Suite.ItemValues(True,category,student) : invalid("StudentID")=-5
        Suite.Reject(Sub() RecordService.Save("LostItems",0,invalid),"Invalid student foreign key rejected")
        invalid=Suite.ItemValues(False,category,student) : invalid("Status")="Released"
        Suite.Reject(Sub() RecordService.Save("FoundItems",0,invalid),"Manual release status rejected")
        Dim updated=Suite.ItemValues(False,category,student) : updated("StorageLocation")="Shelf B"
        RecordService.Save("FoundItems",found,updated)
        Suite.Check(CStr(DatabaseHelper.ExecuteScalar("SELECT StorageLocation FROM FoundItems WHERE FoundItemID=" & found))="Shelf B","Found item edit persists")
        RecordService.Delete("LostItems",lost) : RecordService.Delete("FoundItems",found)
        RecordService.Delete("Students",student) : RecordService.Delete("Categories",category)
        Suite.Check(RecordService.List("Students","TEST-001").Rows.Count=0 AndAlso RecordService.List("Categories","Updated category").Rows.Count=0,"Unreferenced students and categories deleted")
        Suite.Reject(Sub() RecordService.Save("Students",student,StudentValues()),"Stale edit rejected")
        Suite.Reject(Sub() RecordService.List("Users; DROP TABLE Students"),"Dynamic entity allowlist enforced")

        category=CInt(DatabaseHelper.ExecuteScalar("SELECT TOP 1 CategoryID FROM Categories"))
        student=CInt(DatabaseHelper.ExecuteScalar("SELECT TOP 1 StudentID FROM Students"))
        lost=RecordService.Save("LostItems",0,Suite.ItemValues(True,category,student))
        found=RecordService.Save("FoundItems",0,Suite.ItemValues(False,category,student))
        Dim early=Suite.ItemValues(False,category,student) : early("DateFound")=Date.Today.AddDays(-1)
        Dim earlyFound=RecordService.Save("FoundItems",0,early)
        Suite.Check(MatchingService.Candidates(lost,0).Select("FoundItemID=" & earlyFound).Length=0,"Found-before-lost excluded even at zero threshold")
        Suite.Reject(Sub() ClaimService.Create(lost,earlyFound,"Ownership evidence"),"Found-before-lost claim rejected")
        Suite.Reject(Sub() ClaimService.Create(lost,found," "),"Claim evidence required")
        Suite.Reject(Sub() MatchingService.Candidates(lost,101),"Invalid match threshold rejected")
        Dim l=DatabaseHelper.ExecuteQuery("SELECT * FROM LostItems WHERE LostItemID=" & lost).Rows(0)
        Dim f=DatabaseHelper.ExecuteQuery("SELECT * FROM FoundItems WHERE FoundItemID=" & found).Rows(0)
        Dim detail As String=""
        For Each field In New String() {"CategoryID","ItemName","Color","Brand","LocationLost","DistinctiveMarks","DateLost"}
            l(field)=DBNull.Value
        Next
        Suite.Check(MatchingService.Score(l,f,detail)=0,"Missing match fields earn zero points")
        l("ItemName")="blue notebook" : f("ItemName")="blue pen"
        Suite.Check(MatchingService.Score(l,f,detail)=8.33,"Name overlap uses weighted token intersection/union")
        Dim claim=ClaimService.Create(lost,found,"Identifies hidden marks")
        Dim otherLost=RecordService.Save("LostItems",0,Suite.ItemValues(True,category,student))
        Dim competitor=ClaimService.Create(otherLost,found,"Competing evidence")
        ClaimService.Transition(claim,"Under Verification","Identity checked")
        ClaimService.Transition(competitor,"Under Verification","Identity checked")
        ClaimService.Transition(claim,"Approved","Ownership verified")
        Dim message As String=""
        Suite.Check(Not ValidationHelper.IsFoundItemAvailable(found,message),"Reserved item unavailable in validation helper")
        Suite.Reject(Sub() ClaimService.Transition(competitor,"Approved","Competing evidence"),"Competing approval rejected")
        Suite.Check(CStr(DatabaseHelper.ExecuteScalar("SELECT Status FROM Claims WHERE ClaimID=" & competitor))="Under Verification","Failed approval preserves competitor status")
        Using db As New Store()
            db.Execute("UPDATE LostItems SET Status='Closed' WHERE LostItemID=?",lost) : db.Commit()
        End Using
        Dim before=CInt(DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM ActivityLogs"))
        Suite.Reject(Sub() ClaimService.Transition(claim,"Released","Signed handover"),"Stale lost reservation aborts release")
        Suite.Check(CStr(DatabaseHelper.ExecuteScalar("SELECT Status FROM FoundItems WHERE FoundItemID=" & found))="Reserved" AndAlso CStr(DatabaseHelper.ExecuteScalar("SELECT Status FROM Claims WHERE ClaimID=" & claim))="Approved" AndAlso CInt(DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM ActivityLogs"))=before,"Failed release rolls back item, claim and audit")
        Using db As New Store()
            db.Execute("UPDATE LostItems SET Status='Matched' WHERE LostItemID=?",lost) : db.Commit()
        End Using
        ClaimService.Transition(claim,"Cancelled","Withdrawn")
        ClaimService.Transition(competitor,"Rejected","Ownership evidence did not match")
        Suite.Reject(Sub() ClaimService.Transition(competitor,"Under Verification","Reopen"),"Rejected claims cannot reopen")
        Suite.Reject(Sub() RecordService.Delete("FoundItems",found),"Historical claims protect deletion")
        Suite.Check(Not DatabaseHelper.ExecuteQuery("SELECT VerifiedBy,VerificationDate FROM Claims WHERE ClaimID=" & competitor).Rows(0).IsNull("VerifiedBy"),"Decision saves verifier identity")

        For Each report In ReportService.Names
            Dim data=ReportService.Load(report,"")
            Suite.Check(data.Rows.Count>0 AndAlso Not data.Columns.Contains("Password"),report & " report loads without password fields")
        Next
        Suite.Check(ReportService.Load("Lost items","O'Brien",Date.Today,Date.Today).Rows.Count>0,"Report keyword and inclusive date filters")
        Suite.Check(ReportService.Load("Found items","",Date.Today.AddDays(1),Date.Today.AddDays(2)).Rows.Count=0,"Report excludes dates outside range")
        Suite.Reject(Sub() ReportService.Load("Claims","",Date.Today,Date.Today.AddDays(-1)),"Reversed report dates rejected")
        Dim totals=ReportService.Load("Category totals","")
        Suite.Check(totals.Rows.Cast(Of DataRow)().Sum(Function(row) CInt(row("LostCount")))=CInt(DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM LostItems")),"Category report totals reconcile with inventory")
        Dim csv As New DataTable()
        csv.Columns.Add("Text") : csv.Columns.Add("Unicode")
        csv.Rows.Add("a," & ChrW(34) & "b" & vbCrLf & "c","niño")
        csv.Rows.Add(" =SUM(A1:A2)","@formula")
        Dim csvPath=Path.Combine(directory,"report.csv")
        ReportService.ExportCsv(csv,csvPath)
        Dim bytes=File.ReadAllBytes(csvPath), content=File.ReadAllText(csvPath)
        Suite.Check(bytes(0)=239 AndAlso bytes(1)=187 AndAlso bytes(2)=191 AndAlso content.Contains("niño"),"CSV UTF-8 BOM and Unicode preserved")
        Suite.Check(content.Contains("' =SUM(A1:A2)") AndAlso content.Contains("'@formula"),"CSV formula payloads escaped")
        Suite.Check(content.Contains(ChrW(34) & "a," & ChrW(34) & ChrW(34) & "b" & vbCrLf & "c" & ChrW(34)),"CSV commas, quotes and embedded newlines preserved")
        Dim printData As New DataTable() : printData.Columns.Add("Long description")
        printData.Rows.Add(String.Join(" ",Enumerable.Repeat("Complete field value with wrapping.",500)))
        Using document As New ReportDocument("Pagination verification",printData), bitmap As New Bitmap(800,1100), canvas=Graphics.FromImage(bitmap)
            Dim pages=0, more=True
            Do While more AndAlso pages<100
                canvas.Clear(Color.White)
                more=document.RenderPage(canvas,New Rectangle(50,50,700,1000))
                pages+=1
                If pages=1 Then bitmap.Save(Path.Combine(directory,"report-page-1.png"))
            Loop
            Suite.Check(pages>1 AndAlso Not more,"Long report fields paginate to completion")
            document.ResetPages()
            Suite.Check(document.RenderPage(canvas,New Rectangle(50,50,700,1000)),"Print preview can restart pagination")
        End Using
        Dim settings As New AppSettings With {.SchoolName="Test School & Academy",.MatchThreshold=75}
        settings.Save()
        Suite.Check(AppSettings.Load().SchoolName=settings.SchoolName AndAlso AppSettings.Load().MatchThreshold=75,"Settings round-trip XML characters and threshold")
        settings.MatchThreshold=101
        Suite.Reject(Sub() settings.Save(),"Invalid settings rejected")
        Suite.Check(AppSettings.Load().MatchThreshold=75,"Invalid settings preserve previous file")
        settings.MatchThreshold=40 : settings.Save()
        Suite.Check(File.Exists(AppSettings.FilePath & ".previous"),"Settings replacement preserves previous copy")

        Suite.Check(Security.Login("staff","staff123"),"Staff signs in for authorization checks")
        Suite.Reject(Sub() ReportService.Load("Activity logs",""),"Staff activity log access denied")
        Suite.Reject(Sub() settings.Save(),"Staff settings update denied")
        Suite.Reject(Sub() BackupService.Backup(Path.Combine(directory,"staff.mdb")),"Staff backup denied")
        Suite.Reject(Sub() BackupService.Restore(DatabaseHelper.DatabasePath),"Staff restore denied")
        Suite.Reject(Sub() Security.ChangePassword("wrong","Replacement123!"),"Wrong current password rejected")
        Security.ChangePassword("staff123","Replacement123!")
        Suite.Check(Not Security.Login("staff","staff123") AndAlso Security.Login("staff","Replacement123!"),"Own password change invalidates previous password")
        Dim staffId=SessionManager.CurrentUserID
        Using db As New Store()
            db.Execute("UPDATE Users SET IsActive=False WHERE UserID=?",staffId) : db.Commit()
        End Using
        Suite.Reject(Sub() RecordService.List("LostItems"),"Persisted deactivation invalidates existing session access")
        Suite.Check(Security.Login("admin","admin123"),"Admin signs in for maintenance")
        Suite.Check(Not RecordService.List("Users").Columns.Contains("Password"),"User grid excludes password hashes")
        Dim user As New Dictionary(Of String,Object) From {{"Username","reset.user"},{"FullName","Reset User"},{"Password","BeforeReset123!"},{"Role","Staff"},{"IsActive",True}}
        Dim userId=RecordService.Save("Users",0,user)
        Suite.Reject(Sub() RecordService.Save("Users",0,user),"Duplicate username rejected")
        user("Password")="AfterReset123!" : RecordService.Save("Users",userId,user)
        Suite.Check(Not Security.Login("reset.user","BeforeReset123!") AndAlso Security.Login("reset.user","AfterReset123!"),"Admin password reset changes credentials")
        Security.Login("admin","admin123")
        user("Role")="Unknown"
        Suite.Reject(Sub() RecordService.Save("Users",userId,user),"Unknown account role rejected")
        user("Username")="admin" : user("Role")="Staff" : user("IsActive")=True
        Suite.Reject(Sub() RecordService.Save("Users",SessionManager.CurrentUserID,user),"Last/current administrator demotion rejected")
        RenderForms(directory)
        TestMaintenance(directory)
        Security.Logout()
        Suite.Check(Not SessionManager.IsLoggedIn(),"Logout clears session")
        Suite.Reject(Sub() RecordService.List("Students"),"Logged-out access rejected")
        Console.WriteLine("ARTIFACTS: " & directory)
    End Sub

    Private Sub TestMaintenance(directory As String)
        Dim backup=Path.Combine(directory,"backup.mdb")
        BackupService.Backup(backup)
        BackupService.Validate(backup)
        Suite.Check(File.Exists(backup),"Backup created and schema validated")
        Suite.Reject(Sub() BackupService.Backup(backup),"Existing backup preserved")
        Suite.Reject(Sub() BackupService.Backup(DatabaseHelper.DatabasePath),"Cannot back up onto active database")
        Suite.Reject(Sub() BackupService.Restore(DatabaseHelper.DatabasePath),"Cannot restore database onto itself")
        Dim invalid=Path.Combine(directory,"invalid.mdb") : File.WriteAllText(invalid,"not an Access database")
        Dim original=Fingerprint(DatabaseHelper.DatabasePath)
        Suite.Reject(Sub() BackupService.Restore(invalid),"Invalid database restore rejected")
        Suite.Check(Fingerprint(DatabaseHelper.DatabasePath)=original,"Rejected restore leaves active database unchanged")
        Dim wrongSchema=Path.Combine(directory,"wrong-schema.mdb") : File.Copy(backup,wrongSchema)
        Dim active=DatabaseHelper.DatabasePath
        Try
            DatabaseHelper.DatabasePath=wrongSchema
            DatabaseHelper.ExecuteNonQuery("ALTER TABLE Students DROP COLUMN Email")
        Finally
            DatabaseHelper.DatabasePath=active
        End Try
        Suite.Reject(Sub() BackupService.Validate(wrongSchema),"Missing schema column rejected")
        Dim wrongType=Path.Combine(directory,"wrong-type.mdb") : File.Copy(backup,wrongType)
        Try
            DatabaseHelper.DatabasePath=wrongType
            DatabaseHelper.ExecuteNonQuery("UPDATE Students SET Email=NULL")
            DatabaseHelper.ExecuteNonQuery("ALTER TABLE Students ALTER COLUMN Email INTEGER")
        Finally
            DatabaseHelper.DatabasePath=active
        End Try
        Suite.Reject(Sub() BackupService.Validate(wrongType),"Incorrect schema field type rejected")
        OleDbConnection.ReleaseObjectPool()
        Using locked As New FileStream(active,FileMode.Open,FileAccess.Read,FileShare.Read)
            Suite.Reject(Sub() BackupService.Restore(backup),"Restore refuses in-use database")
        End Using
        Suite.Check(IO.Directory.GetFiles(directory,"*.restore-*.mdb").Length=0,"Failed restore cleans staging files")
        Dim count=CInt(DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM Categories"))
        RecordService.Save("Categories",0,New Dictionary(Of String,Object) From {{"CategoryName","After backup"},{"Description","Must be removed by restore"}})
        original=Fingerprint(active)
        Dim safety=BackupService.Restore(backup)
        Suite.Check(File.Exists(safety) AndAlso Fingerprint(safety)=original,"Restore keeps exact pre-restore safety copy")
        Suite.Check(Not SessionManager.IsLoggedIn(),"Restore forces fresh sign-in")
        Security.InitializeDatabase()
        Suite.Check(Security.Login("admin","admin123"),"Restored database supports login")
        Suite.Check(CInt(DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM Categories"))=count,"Restore recovers backup data")
    End Sub

    Public Sub RenderForms(directory As String)
        Application.EnableVisualStyles()
        Dim forms As New List(Of Form) From {New frmLogin(),New frmDashboard(),New frmStudents(),New frmCategories(),New frmMatching(),New frmClaims(),New frmReports(),New frmSettings()}
        For Each entity In New String() {"LostItems","FoundItems","Users"}
            Dim records As New frmRecords() : records.Configure(entity) : records.Tag=entity : forms.Add(records)
        Next
        For Each form In forms
            Using form
                form.Opacity=0
                form.ShowInTaskbar=False
                form.Show()
                form.PerformLayout()
                Dim grids=Descendants(form).OfType(Of DataGridView)().ToArray()
                Dim suffix=If(form.Tag Is Nothing,"",CStr(form.Tag))
                If grids.Length>0 AndAlso Not TypeOf form Is frmMatching Then Suite.Check(grids.All(Function(g) g.DataSource IsNot Nothing AndAlso g.Columns.Count>0),form.Name & suffix & " binds data")
                Using bitmap As New Bitmap(form.Width,form.Height)
                    form.DrawToBitmap(bitmap,New Rectangle(0,0,bitmap.Width,bitmap.Height))
                    bitmap.Save(Path.Combine(directory,form.Name & suffix & ".png"))
                End Using
                Suite.Check(form.Controls.Count>0,form.Name & suffix & " constructs and renders")
            End Using
        Next
    End Sub
    Private Iterator Function Descendants(parent As Control) As IEnumerable(Of Control)
        For Each child As Control In parent.Controls
            Yield child
            For Each nested In Descendants(child)
                Yield nested
            Next
        Next
    End Function
End Module
