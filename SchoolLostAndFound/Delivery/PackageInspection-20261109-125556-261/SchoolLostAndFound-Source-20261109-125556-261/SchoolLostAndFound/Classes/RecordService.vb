Option Strict On
Imports System.Linq
Public Class FieldSpec
    Public Property Name As String
    Public Property Label As String
    Public Property Limit As Integer
    Public Property Required As Boolean
    Public Property Kind As String
    Public Property Choices As String()
    Public Sub New(n As String, title As String, length As Integer, mandatory As Boolean, Optional type As String = "text", Optional options As String() = Nothing)
        Name = n : Label = title : Limit = length : Required = mandatory : Kind = type : Choices = options
    End Sub
End Class
Public Class RecordService
    Public Shared Function Fields(entity As String) As List(Of FieldSpec)
        If entity = "Students" Then Return New List(Of FieldSpec) From {
            New FieldSpec("StudentNumber", "Student number", 20, True), New FieldSpec("FullName", "Full name", 100, True),
            New FieldSpec("CourseOrGrade", "Course / grade", 50, False), New FieldSpec("Section", "Section", 50, False),
            New FieldSpec("ContactNumber", "Contact number", 20, False), New FieldSpec("Email", "Email", 50, False)}
        If entity = "Categories" Then Return New List(Of FieldSpec) From {
            New FieldSpec("CategoryName", "Category name", 50, True), New FieldSpec("Description", "Description", 4000, False, "memo")}
        If entity = "Users" Then Return New List(Of FieldSpec) From {
            New FieldSpec("Username", "Username", 50, True), New FieldSpec("FullName", "Full name", 100, True),
            New FieldSpec("Password", "New password (blank keeps current)", 128, False, "password"),
            New FieldSpec("Role", "Role", 20, True, "choice", {"Staff", "Administrator"}), New FieldSpec("IsActive", "Active", 0, False, "boolean")}
        If entity <> "LostItems" AndAlso entity <> "FoundItems" Then Throw New ArgumentException("Unknown record type.")
        Dim lost = entity = "LostItems"
        Dim f As New List(Of FieldSpec)
        If lost Then f.Add(New FieldSpec("StudentID", "Student", 0, True, "student"))
        f.Add(New FieldSpec("CategoryID", "Category", 0, True, "category"))
        f.Add(New FieldSpec("ItemName", "Item name", 100, True))
        f.Add(New FieldSpec("Description", "Description", 4000, False, "memo"))
        f.Add(New FieldSpec("Color", "Color", 30, False))
        f.Add(New FieldSpec("Brand", "Brand", 50, False))
        f.Add(New FieldSpec(If(lost, "DateLost", "DateFound"), If(lost, "Date lost", "Date found"), 0, True, "date"))
        f.Add(New FieldSpec(If(lost, "LocationLost", "LocationFound"), If(lost, "Location lost", "Location found"), 100, True))
        If Not lost Then
            f.Add(New FieldSpec("FoundBy", "Found by", 100, True))
            f.Add(New FieldSpec("StorageLocation", "Storage location", 100, True))
        End If
        f.Add(New FieldSpec("DistinctiveMarks", "Distinctive marks", 4000, False, "memo"))
        f.Add(New FieldSpec("Status", "Status (claim workflow controls reservation/release)", 30, True, "choice", If(lost, New String() {"Lost", "Closed", "Cancelled"}, New String() {"Available for Claim", "Found", "Unclaimed", "Disposed"})))
        Return f
    End Function
    Public Shared Function Key(entity As String) As String
        Fields(entity)
        If entity = "Students" Then Return "StudentID"
        If entity = "Categories" Then Return "CategoryID"
        Return If(entity = "Users", "UserID", If(entity = "LostItems", "LostItemID", "FoundItemID"))
    End Function
    Public Shared Function List(entity As String, Optional keyword As String = "") As DataTable
        Fields(entity)
        Security.RequireAccess(entity = "Users")
        Dim columns = If(entity = "Users", "UserID,Username,FullName,Role,IsActive", "*")
        Dim search = If(entity = "Users", "Username & ' ' & FullName", "ItemName & ' ' & Status & ' ' & IIF(Description IS NULL,'',Description)")
        If entity = "Students" Then search = "StudentNumber & ' ' & FullName & ' ' & IIF(CourseOrGrade IS NULL,'',CourseOrGrade) & ' ' & IIF([Section] IS NULL,'',[Section])"
        If entity = "Categories" Then search = "CategoryName & ' ' & IIF(Description IS NULL,'',Description)"
        Using db As New Store()
            Return db.Table("SELECT " & columns & " FROM [" & entity & "] WHERE " & search & " LIKE ? ORDER BY [" & Key(entity) & "] DESC", "%" & keyword.Trim() & "%")
        End Using
    End Function
    Public Shared Function Save(entity As String, id As Integer, values As Dictionary(Of String, Object)) As Integer
        Dim definitions = Fields(entity)
        Security.RequireAccess(entity = "Users")
        Dim columns As New List(Of String), args As New List(Of Object)
        For Each f In definitions
            If Not values.ContainsKey(f.Name) Then Throw New ArgumentException("Missing " & f.Label)
            Dim v As Object = values(f.Name)
            If f.Kind = "date" Then
                Dim dateValue = Convert.ToDateTime(v)
                If dateValue.Date > Date.Today OrElse dateValue.Year < 2000 Then Throw New ArgumentException(f.Label & " must be between 2000 and today.")
                v = dateValue.Date
            ElseIf f.Kind = "student" OrElse f.Kind = "category" Then
                v = Convert.ToInt32(v)
                If CInt(v) <= 0 Then Throw New ArgumentException("Select " & f.Label)
            ElseIf f.Kind = "boolean" Then
                v = Convert.ToBoolean(v)
            Else
                v = If(v Is Nothing, "", v.ToString())
                If f.Kind <> "password" Then v = CStr(v).Trim()
                If f.Required AndAlso String.IsNullOrWhiteSpace(CStr(v)) Then Throw New ArgumentException(f.Label & " is required.")
                If CStr(v).Length > f.Limit Then Throw New ArgumentException(f.Label & " exceeds " & f.Limit & " characters.")
                If f.Kind = "choice" AndAlso Not f.Choices.Contains(CStr(v)) Then Throw New ArgumentException("Invalid " & f.Label & ". Use the claim workflow for reserved or released items.")
                If f.Kind = "password" Then
                    If CStr(v) = "" AndAlso id > 0 Then Continue For
                    Security.ValidatePassword(CStr(v))
                    v = Security.Hash(CStr(v))
                End If
            End If
            columns.Add("[" & f.Name & "]") : args.Add(v)
        Next
        Using db As New Store()
            If id > 0 AndAlso Convert.ToInt32(db.Scalar("SELECT COUNT(*) FROM [" & entity & "] WHERE [" & Key(entity) & "]=?", id)) = 0 Then Throw New ArgumentException("The selected record no longer exists.")
            If entity = "Users" Then
                Dim username = CStr(values("Username")).Trim()
                If Convert.ToInt32(db.Scalar("SELECT COUNT(*) FROM Users WHERE Username=? AND UserID<>?", username, id)) > 0 Then Throw New ArgumentException("Username already exists.")
                If id = SessionManager.CurrentUserID AndAlso (Not CBool(values("IsActive")) OrElse CStr(values("Role")) <> "Administrator") Then Throw New ArgumentException("You cannot deactivate or demote your current administrator account.")
                If id > 0 AndAlso (Not CBool(values("IsActive")) OrElse CStr(values("Role")) <> "Administrator") Then
                    If Convert.ToInt32(db.Scalar("SELECT COUNT(*) FROM Users WHERE Role='Administrator' AND IsActive=True AND UserID<>?", id)) = 0 Then Throw New ArgumentException("At least one active administrator is required.")
                End If
            ElseIf entity = "Students" OrElse entity = "Categories" Then
                Dim uniqueField = If(entity = "Students", "StudentNumber", "CategoryName")
                If CInt(db.Scalar("SELECT COUNT(*) FROM [" & entity & "] WHERE [" & uniqueField & "]=? AND [" & Key(entity) & "]<>?", CStr(values(uniqueField)).Trim(), id)) > 0 Then Throw New ArgumentException("This " & uniqueField & " already exists.")
                If entity = "Students" Then
                    Dim message As String = ""
                    If Not ValidationHelper.IsValidEmail(Convert.ToString(values("Email")).Trim(), message) Then Throw New ArgumentException(message)
                End If
            Else
                If Convert.ToInt32(db.Scalar("SELECT COUNT(*) FROM Categories WHERE CategoryID=?", values("CategoryID"))) <> 1 Then Throw New ArgumentException("Category no longer exists.")
                If entity = "LostItems" AndAlso Convert.ToInt32(db.Scalar("SELECT COUNT(*) FROM Students WHERE StudentID=?", values("StudentID"))) <> 1 Then Throw New ArgumentException("Student no longer exists.")
                If id > 0 Then EnsureEditable(db, entity, id)
            End If
            If id = 0 Then
                If entity = "LostItems" OrElse entity = "FoundItems" Then
                    columns.Add(If(entity = "LostItems", "DateReported", "DateRecorded")) : args.Add(DateTime.Now)
                End If
                db.Execute("INSERT INTO [" & entity & "] (" & String.Join(",", columns) & ") VALUES (" & String.Join(",", columns.Select(Function(c) "?")) & ")", args.ToArray())
                id = Convert.ToInt32(db.Scalar("SELECT @@IDENTITY"))
            Else
                args.Add(id)
                db.Execute("UPDATE [" & entity & "] SET " & String.Join(",", columns.Select(Function(c) c & "=?")) & " WHERE [" & Key(entity) & "]=?", args.ToArray())
            End If
            db.Audit("Saved " & entity, "Record #" & id.ToString())
            db.Commit()
        End Using
        Return id
    End Function
    Private Shared Sub EnsureEditable(db As Store, entity As String, id As Integer)
        Dim status = Convert.ToString(db.Scalar("SELECT Status FROM [" & entity & "] WHERE [" & Key(entity) & "]=?", id))
        If {"Claimed", "Released", "Reserved", "Matched"}.Contains(status) Then Throw New InvalidOperationException("This record is controlled by its claim. Use Claims to change the workflow.")
        Dim fk = Key(entity)
        If Convert.ToInt32(db.Scalar("SELECT COUNT(*) FROM Claims WHERE [" & fk & "]=? AND Status IN ('Pending','Under Verification','Approved','Released')", id)) > 0 Then Throw New InvalidOperationException("An active or released claim protects this item from changes.")
    End Sub
    Public Shared Sub Delete(entity As String, id As Integer)
        Fields(entity) : Security.RequireAccess()
        If entity = "Users" Then Throw New InvalidOperationException("Deactivate users instead of deleting audit history.")
        If id <= 0 Then Throw New ArgumentException("Select a record.")
        Using db As New Store()
            If entity = "Students" OrElse entity = "Categories" Then
                Dim references = If(entity = "Students", New String() {"LostItems", "Claims"}, New String() {"LostItems", "FoundItems"})
                For Each table In references
                    If CInt(db.Scalar("SELECT COUNT(*) FROM [" & table & "] WHERE [" & Key(entity) & "]=?", id)) > 0 Then Throw New InvalidOperationException("Referenced records cannot be deleted.")
                Next
            Else
                EnsureEditable(db, entity, id)
                If Convert.ToInt32(db.Scalar("SELECT COUNT(*) FROM Claims WHERE [" & Key(entity) & "]=?", id)) > 0 Then Throw New InvalidOperationException("Claim history prevents deletion.")
            End If
            If db.Execute("DELETE FROM [" & entity & "] WHERE [" & Key(entity) & "]=?", id) <> 1 Then Throw New InvalidOperationException("The record no longer exists.")
            db.Audit("Deleted " & entity, "Record #" & id.ToString()) : db.Commit()
        End Using
    End Sub
End Class
