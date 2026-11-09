Option Strict On
Imports System.Linq
Imports System.Text.RegularExpressions
Public Class MatchingService
    Private Shared Function Tokens(value As String) As HashSet(Of String)
        Return New HashSet(Of String)(Regex.Matches(value.ToLowerInvariant(), "[\p{L}\p{N}]+").Cast(Of Match)().Select(Function(m) m.Value))
    End Function
    Private Shared Function Similarity(a As String, b As String) As Double
        Dim left = Tokens(a), right = Tokens(b)
        If left.Count = 0 OrElse right.Count = 0 Then Return 0
        Return CDbl(left.Intersect(right).Count()) / left.Union(right).Count()
    End Function
    Private Shared Function EqualValue(a As String, b As String) As Boolean
        Return Not String.IsNullOrWhiteSpace(a) AndAlso String.Equals(a.Trim(), b.Trim(), StringComparison.OrdinalIgnoreCase)
    End Function
    Public Shared Function Score(lost As DataRow, found As DataRow, ByRef explanation As String) As Double
        Dim points(6) As Double
        points(0) = If(Not lost.IsNull("CategoryID") AndAlso Not found.IsNull("CategoryID") AndAlso CInt(lost("CategoryID")) = CInt(found("CategoryID")),25,0)
        points(1) = 25 * Similarity(Convert.ToString(lost("ItemName")),Convert.ToString(found("ItemName")))
        points(2) = If(EqualValue(Convert.ToString(lost("Color")),Convert.ToString(found("Color"))),15,0)
        points(3) = If(EqualValue(Convert.ToString(lost("Brand")),Convert.ToString(found("Brand"))),10,0)
        points(4) = 10 * Similarity(Convert.ToString(lost("LocationLost")),Convert.ToString(found("LocationFound")))
        points(5) = 10 * Similarity(Convert.ToString(lost("DistinctiveMarks")),Convert.ToString(found("DistinctiveMarks")))
        If Not lost.IsNull("DateLost") AndAlso Not found.IsNull("DateFound") Then
            Dim days = (CDate(found("DateFound")).Date-CDate(lost("DateLost")).Date).Days
            If days < 0 Then
                explanation="Found before loss: excluded" : Return 0
            End If
            points(6)=If(days<=7,5,If(days<=30,2,0))
        End If
        explanation=String.Join("; ", {"Category", "Name", "Color", "Brand", "Location", "Marks", "Date"}.Select(Function(label,i) label & " " & points(i).ToString("0.##")))
        Return Math.Round(points.Sum(),2)
    End Function
    Public Shared Function Candidates(lostId As Integer, threshold As Double) As DataTable
        Security.RequireAccess()
        If threshold < 0 OrElse threshold > 100 Then Throw New ArgumentException("Threshold must be 0-100.")
        Using db As New Store()
            Dim lostRows=db.Table("SELECT * FROM LostItems WHERE LostItemID=? AND Status IN ('Lost','Possible Match')",lostId)
            If lostRows.Rows.Count<>1 Then Throw New ArgumentException("Select an unresolved lost report.")
            Dim table=db.Table("SELECT * FROM FoundItems WHERE Status IN ('Found','Available for Claim','Unclaimed') AND FoundItemID NOT IN (SELECT FoundItemID FROM Claims WHERE Status IN ('Approved','Released'))")
            table.Columns.Add("MatchScore",GetType(Double)) : table.Columns.Add("ScoreDetails",GetType(String))
            For Each row As DataRow In table.Select()
                Dim detail As String=""
                Dim result=Score(lostRows.Rows(0),row,detail)
                If result<threshold OrElse detail.StartsWith("Found before") Then
                    table.Rows.Remove(row)
                Else
                    row("MatchScore")=result : row("ScoreDetails")=detail
                End If
            Next
            table.DefaultView.Sort="MatchScore DESC, FoundItemID ASC"
            Return table.DefaultView.ToTable()
        End Using
    End Function
End Class
