Option Strict On
Imports System.IO
Imports System.Data.OleDb
Imports System.Linq
Public Class BackupService
    Private Shared Function ConnectionString(path As String) As String
        Dim builder As New OleDbConnectionStringBuilder()
        builder.Provider="Microsoft.Jet.OLEDB.4.0" : builder.DataSource=path : builder("OLE DB Services")=-4
        Return builder.ConnectionString
    End Function
    Public Shared Sub Validate(path As String)
        If Not File.Exists(path) OrElse Not String.Equals(IO.Path.GetExtension(path),".mdb",StringComparison.OrdinalIgnoreCase) Then Throw New ArgumentException("Select an existing .mdb file.")
        Using conn As New OleDbConnection(ConnectionString(path))
            conn.Open()
            Dim required As New Dictionary(Of String,String) From {
                {"Users","UserID,Username,Password,FullName,Role,IsActive"},
                {"Students","StudentID,StudentNumber,FullName,CourseOrGrade,Section,ContactNumber,Email"},
                {"Categories","CategoryID,CategoryName,Description"},
                {"LostItems","LostItemID,StudentID,CategoryID,ItemName,Description,Color,Brand,DateLost,LocationLost,DistinctiveMarks,Status,DateReported"},
                {"FoundItems","FoundItemID,CategoryID,ItemName,Description,Color,Brand,DateFound,LocationFound,FoundBy,StorageLocation,DistinctiveMarks,Status,DateRecorded"},
                {"Claims","ClaimID,LostItemID,FoundItemID,StudentID,ClaimDate,ClaimDescription,VerificationDetails,Status,VerifiedBy,VerificationDate,ReleaseDate,Remarks"},
                {"ActivityLogs","LogID,UserID,Action,Description,ActionDate"}}
            For Each entry In required
                Using cmd As New OleDbCommand("SELECT " & String.Join(",",entry.Value.Split(","c).Select(Function(n) "[" & n & "]")) & " FROM [" & entry.Key & "] WHERE 1=0",conn), reader=cmd.ExecuteReader(CommandBehavior.KeyInfo)
                    For Each column As DataRow In reader.GetSchemaTable().Rows
                        Dim field=CStr(column("ColumnName")), actual=DirectCast(column("DataType"),Type)
                        Dim expected=If(field.EndsWith("ID") OrElse field="VerifiedBy",GetType(Integer),If(field="IsActive",GetType(Boolean),If(field.StartsWith("Date") OrElse field.EndsWith("Date"),GetType(Date),GetType(String))))
                        If actual IsNot expected Then Throw New InvalidDataException("Incorrect field type: " & entry.Key & "." & field)
                        If field=entry.Value.Split(","c)(0) AndAlso (Not CBool(column("IsKey")) OrElse Not CBool(column("IsAutoIncrement"))) Then Throw New InvalidDataException("Missing AutoNumber primary key: " & entry.Key)
                    Next
                End Using
            Next
            Dim indexes=conn.GetOleDbSchemaTable(OleDbSchemaGuid.Indexes,Nothing)
            For Each pair In New String() {"Users|Username","Students|StudentNumber","Categories|CategoryName"}
                Dim parts=pair.Split("|"c)
                Dim valid=indexes.Rows.Cast(Of DataRow)().Any(Function(r) CStr(r("TABLE_NAME"))=parts(0) AndAlso CStr(r("COLUMN_NAME"))=parts(1) AndAlso CBool(r("UNIQUE")) AndAlso indexes.Rows.Cast(Of DataRow)().Count(Function(other) CStr(other("TABLE_NAME"))=parts(0) AndAlso CStr(other("INDEX_NAME"))=CStr(r("INDEX_NAME")))=1)
                If Not valid Then Throw New InvalidDataException("Missing unique index: " & pair)
            Next
            Dim relationships As String()={"LostItems|StudentID|Students|StudentID","LostItems|CategoryID|Categories|CategoryID","FoundItems|CategoryID|Categories|CategoryID","Claims|LostItemID|LostItems|LostItemID","Claims|FoundItemID|FoundItems|FoundItemID","Claims|StudentID|Students|StudentID","Claims|VerifiedBy|Users|UserID","ActivityLogs|UserID|Users|UserID"}
            Dim foreignKeys=conn.GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys,Nothing)
            For Each relationship In relationships
                Dim parts=relationship.Split("|"c)
                If Not foreignKeys.Rows.Cast(Of DataRow)().Any(Function(r) CStr(r("FK_TABLE_NAME"))=parts(0) AndAlso CStr(r("FK_COLUMN_NAME"))=parts(1) AndAlso CStr(r("PK_TABLE_NAME"))=parts(2) AndAlso CStr(r("PK_COLUMN_NAME"))=parts(3)) Then Throw New InvalidDataException("Database is missing relationship " & relationship)
                Using cmd As New OleDbCommand("SELECT COUNT(*) FROM [" & parts(0) & "] AS child LEFT JOIN [" & parts(2) & "] AS parent ON child.[" & parts(1) & "]=parent.[" & parts(3) & "] WHERE child.[" & parts(1) & "] IS NOT NULL AND parent.[" & parts(3) & "] IS NULL",conn)
                    If CInt(cmd.ExecuteScalar())<>0 Then Throw New InvalidDataException("Database contains orphaned records.")
                End Using
            Next
            Using cmd As New OleDbCommand("SELECT COUNT(*) FROM Users WHERE Role='Administrator' AND IsActive=True",conn)
                If CInt(cmd.ExecuteScalar())=0 Then Throw New InvalidDataException("Database has no active administrator.")
            End Using
            For Each field In New String() {"FoundItemID","LostItemID"}
                Using cmd As New OleDbCommand("SELECT [" & field & "] FROM Claims WHERE Status IN ('Approved','Released') GROUP BY [" & field & "] HAVING COUNT(*)>1",conn), reader=cmd.ExecuteReader()
                    If reader.Read() Then Throw New InvalidDataException("Database contains conflicting approved or released claims.")
                End Using
            Next
        End Using
    End Sub
    Public Shared Sub Backup(destination As String)
        Security.RequireAccess(True)
        If String.Equals(Path.GetFullPath(destination),Path.GetFullPath(DatabaseHelper.DatabasePath),StringComparison.OrdinalIgnoreCase) Then Throw New ArgumentException("Backup cannot replace the active database.")
        If File.Exists(destination) Then Throw New IOException("Choose a new backup filename; existing backups are preserved.")
        If Not String.Equals(Path.GetExtension(destination),".mdb",StringComparison.OrdinalIgnoreCase) Then Throw New ArgumentException("Backup filename must end in .mdb.")
        OleDbConnection.ReleaseObjectPool()
        Using source As New FileStream(DatabaseHelper.DatabasePath,FileMode.Open,FileAccess.Read,FileShare.None), target As New FileStream(destination,FileMode.CreateNew,FileAccess.Write,FileShare.None)
            source.CopyTo(target) : target.Flush(True)
        End Using
        Validate(destination)
        DatabaseHelper.LogActivity("Database backup","Verified backup created")
    End Sub
    Public Shared Function Restore(source As String) As String
        Security.RequireAccess(True)
        Dim target=DatabaseHelper.DatabasePath
        If String.Equals(Path.GetFullPath(source),Path.GetFullPath(target),StringComparison.OrdinalIgnoreCase) Then Throw New ArgumentException("Cannot restore the active database onto itself.")
        Validate(source)
        Dim staged=target & ".restore-" & Guid.NewGuid().ToString("N") & ".mdb"
        Dim safety=target & ".before-restore-" & DateTime.Now.ToString("yyyyMMdd-HHmmss-fff") & ".mdb"
        Try
            Using input As New FileStream(source,FileMode.Open,FileAccess.Read,FileShare.Read), output As New FileStream(staged,FileMode.CreateNew,FileAccess.Write,FileShare.None)
                input.CopyTo(output) : output.Flush(True)
            End Using
            Validate(staged)
            OleDbConnection.ReleaseObjectPool()
            ' Deny readers/writers throughout the atomic replacement; permit the rename only.
            Using exclusive As New FileStream(target,FileMode.Open,FileAccess.Read,FileShare.Delete)
                File.Replace(staged,target,safety)
            End Using
        Finally
            If File.Exists(staged) Then File.Delete(staged)
        End Try
        SessionManager.Clear()
        Return safety
    End Function
End Class
