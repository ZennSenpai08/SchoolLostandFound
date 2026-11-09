Option Strict On
Imports System.Security.Cryptography
Imports System.Data.OleDb

Public Class Security
    Public Shared Sub InitializeDatabase()
        Using conn = DatabaseHelper.GetConnection()
            Dim columns = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, New Object() {Nothing, Nothing, "Users", "Password"})
            If Convert.ToInt32(columns.Rows(0)("CHARACTER_MAXIMUM_LENGTH")) < 255 Then
                Using cmd As New OleDbCommand("ALTER TABLE Users ALTER COLUMN [Password] VARCHAR(255)", conn)
                    cmd.ExecuteNonQuery()
                End Using
            End If
        End Using
        Using db As New Store()
            For Each row As DataRow In db.Table("SELECT UserID,[Password] FROM Users").Rows
                Dim password = CStr(row("Password"))
                If Not password.StartsWith("PBKDF2$") Then db.Execute("UPDATE Users SET [Password]=? WHERE UserID=?", Hash(password), CInt(row("UserID")))
            Next
            db.Execute("UPDATE FoundItems SET Status='Reserved' WHERE Status IN ('Found','Available for Claim','Unclaimed') AND FoundItemID IN (SELECT FoundItemID FROM Claims WHERE Status='Approved')")
            db.Execute("UPDATE LostItems SET Status='Matched' WHERE Status IN ('Lost','Possible Match') AND LostItemID IN (SELECT LostItemID FROM Claims WHERE Status='Approved')")
            db.Commit()
        End Using
    End Sub
    Public Shared Function Hash(password As String) As String
        Dim salt(15) As Byte
        Using rng = RandomNumberGenerator.Create()
            rng.GetBytes(salt)
        End Using
        Using derive As New Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256)
            Return "PBKDF2$100000$" & Convert.ToBase64String(salt) & "$" & Convert.ToBase64String(derive.GetBytes(32))
        End Using
    End Function
    Public Shared Function Verify(password As String, encoded As String) As Boolean
        Try
            Dim parts = encoded.Split("$"c)
            If parts.Length <> 4 OrElse parts(0) <> "PBKDF2" OrElse parts(1) <> "100000" Then Return False
            Dim expected = Convert.FromBase64String(parts(3))
            Using derive As New Rfc2898DeriveBytes(password, Convert.FromBase64String(parts(2)), 100000, HashAlgorithmName.SHA256)
                Dim actual = derive.GetBytes(32)
                If expected.Length <> actual.Length Then Return False
                Dim difference As Integer = 0
                For i As Integer = 0 To actual.Length - 1
                    difference = difference Or (CInt(actual(i)) Xor CInt(expected(i)))
                Next
                Return difference = 0
            End Using
        Catch ex As FormatException
            Return False
        End Try
    End Function
    Public Shared Function Login(username As String, password As String) As Boolean
        SessionManager.Clear()
        Using db As New Store()
            Dim rows = db.Table("SELECT * FROM Users WHERE Username=?", username.Trim())
            If rows.Rows.Count <> 1 Then Return False
            Dim row = rows.Rows(0)
            If Not CBool(row("IsActive")) OrElse Not Verify(password, CStr(row("Password"))) Then Return False
            Dim role = CStr(row("Role"))
            If role <> "Administrator" AndAlso role <> "Staff" Then Return False
            SessionManager.SetSession(CInt(row("UserID")), CStr(row("Username")), CStr(row("FullName")), role)
            Try
                db.Audit("Login", "Signed in")
                db.Commit()
            Catch
                SessionManager.Clear()
                Throw
            End Try
            Return True
        End Using
    End Function
    Public Shared Sub RequireAccess(Optional adminOnly As Boolean = False)
        If Not SessionManager.IsLoggedIn() Then Throw New UnauthorizedAccessException("Please sign in.")
        Using db As New Store()
            Dim rows = db.Table("SELECT Role,IsActive FROM Users WHERE UserID=?", SessionManager.CurrentUserID)
            If rows.Rows.Count <> 1 OrElse Not CBool(rows.Rows(0)("IsActive")) Then Throw New UnauthorizedAccessException("Your account is inactive. Please sign in again.")
            Dim role = CStr(rows.Rows(0)("Role"))
            If role <> "Administrator" AndAlso role <> "Staff" Then Throw New UnauthorizedAccessException("Unrecognized role.")
            SessionManager.CurrentRole = role
            If adminOnly AndAlso role <> "Administrator" Then Throw New UnauthorizedAccessException("Administrator access is required.")
        End Using
    End Sub
    Public Shared Sub ValidatePassword(password As String)
        If String.IsNullOrWhiteSpace(password) OrElse password.Length < 10 OrElse password.Length > 128 Then Throw New ArgumentException("Use a password of 10 to 128 characters.")
    End Sub
    Public Shared Sub ChangePassword(oldPassword As String, newPassword As String)
        RequireAccess()
        ValidatePassword(newPassword)
        Using db As New Store()
            Dim encoded = CStr(db.Scalar("SELECT [Password] FROM Users WHERE UserID=?", SessionManager.CurrentUserID))
            If Not Verify(oldPassword, encoded) Then Throw New ArgumentException("Current password is incorrect.")
            db.Execute("UPDATE Users SET [Password]=? WHERE UserID=?", Hash(newPassword), SessionManager.CurrentUserID)
            db.Audit("Password changed", "Changed own password")
            db.Commit()
        End Using
    End Sub
    Public Shared Sub Logout()
        RequireAccess()
        Using db As New Store()
            db.Audit("Logout", "Signed out")
            db.Commit()
        End Using
        SessionManager.Clear()
    End Sub
End Class
