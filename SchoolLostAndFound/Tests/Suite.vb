Option Strict On
Module Suite
    Private passed As Integer
    Friend Sub Check(ok As Boolean, name As String)
        If Not ok Then Throw New Exception("FAIL: " & name)
        passed += 1 : Console.WriteLine("PASS: " & name)
    End Sub
    Friend Sub Reject(action As Action, name As String)
        Dim rejected As Boolean
        Try
            action()
        Catch ex As Exception
            rejected = True
        End Try
        Check(rejected, name)
    End Sub
    Public Function ItemValues(lost As Boolean, category As Integer, student As Integer) As Dictionary(Of String, Object)
        Dim v As New Dictionary(Of String, Object) From {{"CategoryID",category},{"ItemName","Test O'Brien notebook"},{"Description","=private"},{"Color","Blue"},{"Brand","Campus"},{"DistinctiveMarks","Star sticker"},{"Status",If(lost,"Lost","Available for Claim")}}
        v(If(lost,"DateLost","DateFound")) = Date.Today
        v(If(lost,"LocationLost","LocationFound")) = "Library"
        If lost Then
            v("StudentID") = student
        Else
            v("FoundBy") = "School staff" : v("StorageLocation") = "A1"
        End If
        Return v
    End Function
    <STAThread>
    Public Sub Main(args As String())
        Try
            If args.Length < 1 OrElse args.Length > 2 Then Throw New Exception("Pass source MDB path and optional --forms.")
            AppUtilities.ErrorPresenter=Sub(message) Throw New InvalidOperationException("UI error: " & message)
            Dim directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Runs",DateTime.Now.ToString("yyyyMMdd-HHmmss-fff"))
            IO.Directory.CreateDirectory(directory)
            DatabaseHelper.DatabasePath = Path.Combine(directory,"test.mdb")
            File.Copy(args(0),DatabaseHelper.DatabasePath)
            Security.InitializeDatabase()
            Security.InitializeDatabase()
            If args.Length=2 AndAlso args(1)="--forms" Then
                Security.Login("admin","admin123")
                AppSettings.FilePath=Path.Combine(directory,"settings.xml")
                ExtendedSuite.RenderForms(directory)
                Console.WriteLine("ARTIFACTS: " & directory)
                Console.WriteLine("RESULT: " & passed & " checks passed")
                Return
            End If
            Check(Not Security.Login("admin' OR 1=1 --","admin123"),"Parameterized login rejects injection")
            Check(Not Security.Login("admin","wrong"),"Wrong password denied")
            Check(Security.Login("admin","admin123"),"Migrated admin login")
            Dim encoded = CStr(DatabaseHelper.ExecuteScalar("SELECT [Password] FROM Users WHERE Username='admin'"))
            Check(encoded.StartsWith("PBKDF2$") AndAlso Security.Verify("admin123", encoded),"Salted password migration is repeatable")
            Check(Not Security.Verify("ADMIN123",encoded),"Passwords are case sensitive")
            Dim category = CInt(DatabaseHelper.ExecuteScalar("SELECT TOP 1 CategoryID FROM Categories"))
            Dim student = CInt(DatabaseHelper.ExecuteScalar("SELECT TOP 1 StudentID FROM Students"))
            Dim lost = RecordService.Save("LostItems",0,ItemValues(True,category,student))
            Dim found = RecordService.Save("FoundItems",0,ItemValues(False,category,student))
            Check(lost > 0 AndAlso found > 0,"Create lost/found items")
            Check(RecordService.List("LostItems","O'Brien").Rows.Count = 1,"Search apostrophe safely")
            Dim edit = ItemValues(True,category,student) : edit("Brand") = "Edited"
            RecordService.Save("LostItems",lost,edit)
            Check(CStr(DatabaseHelper.ExecuteScalar("SELECT Brand FROM LostItems WHERE LostItemID=" & lost))="Edited","Update lost item")
            edit("DateLost") = Date.Today.AddDays(1)
            Reject(Sub() RecordService.Save("LostItems",lost,edit),"Future date rejected")
            edit("DateLost") = Date.Today : edit("CategoryID") = -1
            Reject(Sub() RecordService.Save("LostItems",lost,edit),"Invalid category rejected")
            RecordService.Delete("LostItems",lost) : RecordService.Delete("FoundItems",found)
            Check(RecordService.List("LostItems","O'Brien").Rows.Count=0,"Delete unreferenced item")
            Check(Security.Login("staff","staff123"),"Staff login")
            Reject(Sub() RecordService.List("Users"),"Staff cannot view users")
            Check(Security.Login("admin","admin123"),"Administrator restored")
            Dim user As New Dictionary(Of String,Object) From {{"Username","test.user"},{"FullName","Test User"},{"Password","Testing12345!"},{"Role","Staff"},{"IsActive",True}}
            Dim userId=RecordService.Save("Users",0,user)
            Check(userId > 0,"Create user")
            user("Password")="" : user("IsActive")=False
            RecordService.Save("Users",userId,user)
            Check(Not Security.Login("test.user","Testing12345!"),"Deactivated user denied")
            Check(Security.Login("admin","admin123"),"Admin remains active")
            user("Username")="admin" : user("Role")="Staff"
            Reject(Sub() RecordService.Save("Users",SessionManager.CurrentUserID,user),"Current administrator cannot be disabled")
            lost=RecordService.Save("LostItems",0,ItemValues(True,category,student))
            found=RecordService.Save("FoundItems",0,ItemValues(False,category,student))
            Dim matches=MatchingService.Candidates(lost,100)
            Check(matches.Select("FoundItemID=" & found).Length=1,"Exact weighted match scores 100")
            Dim claim=ClaimService.Create(lost,found,"Owner describes star sticker and contents")
            Reject(Sub() ClaimService.Create(lost,found,"Duplicate evidence"),"Duplicate active claim rejected")
            Reject(Sub() ClaimService.Transition(claim,"Released","Premature release"),"Cannot release pending claim")
            Reject(Sub() RecordService.Save("FoundItems",found,ItemValues(False,category,student)),"Active claim protects item edits")
            ClaimService.Transition(claim,"Under Verification","Checked student ID")
            Reject(Sub() ClaimService.Transition(claim,"Approved",""),"Approval requires evidence")
            ClaimService.Transition(claim,"Approved","Student ID and hidden marks match")
            Check(CStr(DatabaseHelper.ExecuteScalar("SELECT Status FROM FoundItems WHERE FoundItemID=" & found))="Reserved","Approval reserves found item")
            Check(MatchingService.Candidates(RecordService.Save("LostItems",0,ItemValues(True,category,student)),0).Select("FoundItemID=" & found).Length=0,"Reserved item excluded from matching")
            ClaimService.Transition(claim,"Cancelled","Owner withdrew claim")
            Check(CStr(DatabaseHelper.ExecuteScalar("SELECT Status FROM LostItems WHERE LostItemID=" & lost))="Lost","Cancellation frees lost report")
            Check(CStr(DatabaseHelper.ExecuteScalar("SELECT Status FROM FoundItems WHERE FoundItemID=" & found))="Available for Claim","Cancellation frees found item")
            claim=ClaimService.Create(lost,found,"Ownership evidence rechecked")
            ClaimService.Transition(claim,"Under Verification","ID checked")
            ClaimService.Transition(claim,"Approved","ID and marks verified")
            ClaimService.Transition(claim,"Released","Signed receipt and handed item to owner")
            Check(CStr(DatabaseHelper.ExecuteScalar("SELECT Status FROM LostItems WHERE LostItemID=" & lost))="Claimed","Release marks lost report claimed")
            Check(CStr(DatabaseHelper.ExecuteScalar("SELECT Status FROM FoundItems WHERE FoundItemID=" & found))="Released","Release updates found item")
            Check(DatabaseHelper.ExecuteScalar("SELECT ReleaseDate FROM Claims WHERE ClaimID=" & claim) IsNot Nothing,"Release timestamp saved")
            Reject(Sub() ClaimService.Transition(claim,"Released","Again"),"Double release rejected")
            Check(ClaimService.List("Released").Rows.Count>=2,"Claim report joins actual students/items")
            Dim before=CInt(DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM ActivityLogs"))
            Using db As New Store()
                db.Execute("UPDATE FoundItems SET Brand=? WHERE FoundItemID=?","Should roll back",found)
                db.Audit("Test rollback","This must roll back")
            End Using
            Check(CStr(DatabaseHelper.ExecuteScalar("SELECT Brand FROM FoundItems WHERE FoundItemID=" & found))="Campus" AndAlso CInt(DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM ActivityLogs"))=before,"Uncommitted data and audit both roll back")
            ExtendedSuite.Run(directory)
            Console.WriteLine("RESULT: " & passed & " checks passed")
        Catch ex As Exception
            Console.Error.WriteLine(ex.ToString())
            Environment.ExitCode=1
        End Try
    End Sub
End Module
