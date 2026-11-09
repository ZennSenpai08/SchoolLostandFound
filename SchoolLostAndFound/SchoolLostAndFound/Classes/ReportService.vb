Option Strict On
Imports System.Linq
Imports System.IO
Imports System.Text
Imports System.Globalization
Public Class ReportService
    Public Shared ReadOnly Names As String() = {"Lost items","Found items","Claims","Students","Category totals","Activity logs"}
    Public Shared Function Load(name As String, keyword As String, Optional fromDate As Nullable(Of Date) = Nothing, Optional toDate As Nullable(Of Date) = Nothing) As DataTable
        If Not Names.Contains(name) Then Throw New ArgumentException("Unknown report.")
        Security.RequireAccess(name="Activity logs")
        If fromDate.HasValue AndAlso toDate.HasValue AndAlso fromDate.Value.Date>toDate.Value.Date Then Throw New ArgumentException("Start date must not be after end date.")
        Dim result As DataTable
        Using db As New Store()
            Dim sql As String, dateColumn As String=""
            Select Case name
                Case "Lost items" : sql="SELECT l.*,s.FullName AS StudentName,c.CategoryName FROM (LostItems l INNER JOIN Students s ON l.StudentID=s.StudentID) INNER JOIN Categories c ON l.CategoryID=c.CategoryID" : dateColumn="l.DateLost"
                Case "Found items" : sql="SELECT f.*,c.CategoryName FROM FoundItems f INNER JOIN Categories c ON f.CategoryID=c.CategoryID" : dateColumn="f.DateFound"
                Case "Claims" : sql="SELECT c.*,s.FullName AS Claimant FROM Claims c INNER JOIN Students s ON c.StudentID=s.StudentID" : dateColumn="c.ClaimDate"
                Case "Students" : sql="SELECT * FROM Students"
                Case "Activity logs" : sql="SELECT a.*,u.Username FROM ActivityLogs a INNER JOIN Users u ON a.UserID=u.UserID" : dateColumn="a.ActionDate"
                Case Else : sql="SELECT CategoryID,CategoryName FROM Categories"
            End Select
            Dim args As New List(Of Object), filters As New List(Of String)
            If dateColumn<>"" Then
                If fromDate.HasValue Then
                    filters.Add(dateColumn & ">=?") : args.Add(fromDate.Value.Date)
                End If
                If toDate.HasValue Then
                    filters.Add(dateColumn & "<?") : args.Add(toDate.Value.Date.AddDays(1))
                End If
            End If
            If filters.Count>0 Then sql &= " WHERE " & String.Join(" AND ",filters)
            result=db.Table(sql,args.ToArray())
            If name="Category totals" Then
                result.Columns.Add("LostCount",GetType(Integer)) : result.Columns.Add("FoundCount",GetType(Integer))
                For Each row As DataRow In result.Rows
                    row("LostCount")=CInt(db.Scalar("SELECT COUNT(*) FROM LostItems WHERE CategoryID=?",CInt(row("CategoryID"))))
                    row("FoundCount")=CInt(db.Scalar("SELECT COUNT(*) FROM FoundItems WHERE CategoryID=?",CInt(row("CategoryID"))))
                Next
            End If
        End Using
        If Not String.IsNullOrWhiteSpace(keyword) Then
            For Each row As DataRow In result.Select()
                If Not row.ItemArray.Any(Function(v) Convert.ToString(v).IndexOf(keyword.Trim(),StringComparison.OrdinalIgnoreCase)>=0) Then result.Rows.Remove(row)
            Next
        End If
        Return result
    End Function
    Public Shared Function CellText(value As Object) As String
        If value Is Nothing OrElse value Is DBNull.Value Then Return ""
        If TypeOf value Is Date Then Return CDate(value).ToString("yyyy-MM-dd HH:mm:ss",CultureInfo.InvariantCulture)
        Return Convert.ToString(value,CultureInfo.InvariantCulture)
    End Function
    Public Shared Function CsvCell(value As Object) As String
        Dim s=CellText(value), trimmed=s.TrimStart()
        If trimmed.Length>0 AndAlso "=+-@".Contains(trimmed(0)) Then s="'" & s
        If s.StartsWith(vbTab) OrElse s.StartsWith(vbCr) OrElse s.StartsWith(vbLf) Then s="'" & s
        Dim quote As String = ChrW(34)
        Return quote & s.Replace(quote,quote & quote) & quote
    End Function
    Public Shared Sub ExportCsv(table As DataTable,path As String)
        Using writer As New StreamWriter(path,False,New UTF8Encoding(True))
            writer.WriteLine(String.Join(",",table.Columns.Cast(Of DataColumn)().Select(Function(c) CsvCell(c.ColumnName))))
            For Each row As DataRow In table.Rows
                writer.WriteLine(String.Join(",",row.ItemArray.Select(Function(v) CsvCell(v))))
            Next
        End Using
    End Sub
End Class
