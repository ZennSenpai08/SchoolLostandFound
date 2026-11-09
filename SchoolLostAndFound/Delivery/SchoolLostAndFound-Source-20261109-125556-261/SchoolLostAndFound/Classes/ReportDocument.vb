Option Strict On
Imports System.Drawing.Printing
Imports System.Text
Public Class ReportDocument
    Inherits PrintDocument
    Private ReadOnly body As String
    Private ReadOnly heading As String
    Private offset As Integer
    Private page As Integer
    Public Sub New(title As String,table As DataTable)
        heading=title
        DocumentName=title
        Dim content As New StringBuilder()
        content.AppendLine("Records: " & table.Rows.Count)
        For i As Integer=0 To table.Rows.Count-1
            content.AppendLine().AppendLine("Record " & (i+1))
            For Each column As DataColumn In table.Columns
                content.AppendLine(column.ColumnName & ": " & ReportService.CellText(table.Rows(i)(column)))
            Next
        Next
        body=content.ToString()
    End Sub
    Public Sub ResetPages()
        offset=0 : page=0
    End Sub
    Protected Overrides Sub OnBeginPrint(e As PrintEventArgs)
        MyBase.OnBeginPrint(e) : ResetPages()
    End Sub
    Public Function RenderPage(canvas As Graphics,bounds As Rectangle) As Boolean
        page+=1
        Using titleFont As New Font("Segoe UI",12,FontStyle.Bold), bodyFont As New Font("Segoe UI",9), format As New StringFormat(StringFormatFlags.LineLimit)
            canvas.DrawString(heading,titleFont,Brushes.Black,bounds.Left,bounds.Top)
            canvas.DrawString("Page " & page,bodyFont,Brushes.Black,bounds.Left,bounds.Bottom-20)
            Dim area As New RectangleF(bounds.Left,bounds.Top+35,bounds.Width,bounds.Height-65)
            Dim remaining=body.Substring(offset), chars As Integer, lines As Integer
            canvas.MeasureString(remaining,bodyFont,area.Size,format,chars,lines)
            If chars=0 AndAlso remaining.Length>0 Then Throw New InvalidOperationException("Printable area is too small.")
            canvas.DrawString(remaining.Substring(0,chars),bodyFont,Brushes.Black,area,format)
            offset+=chars
        End Using
        Return offset<body.Length
    End Function
    Protected Overrides Sub OnPrintPage(e As PrintPageEventArgs)
        MyBase.OnPrintPage(e)
        e.HasMorePages=RenderPage(e.Graphics,e.MarginBounds)
    End Sub
End Class
