Option Strict On
Imports System.Linq
Public Class frmReports
    Public Property InitialReport As String="Lost items"
    Private loadedName As String
    Private data As DataTable
    Private Sub OpenForm(sender As Object,e As EventArgs) Handles MyBase.Load
        Ui.Attempt(Sub()
            Security.RequireAccess()
            Ui.SetupGrid(grid)
            For Each reportName As String In ReportService.Names
                If reportName<>"Activity logs" OrElse SessionManager.IsAdministrator() Then cmbReport.Items.Add(reportName)
            Next
            cmbReport.SelectedItem=InitialReport
            If cmbReport.SelectedIndex<0 Then cmbReport.SelectedIndex=0
            fromDate.Value=Date.Today.AddMonths(-1)
            LoadReport()
        End Sub)
    End Sub
    Private Sub LoadReport()
        Dim start As Nullable(Of Date)=Nothing, finish As Nullable(Of Date)=Nothing
        If chkDates.Checked Then start=fromDate.Value.Date : finish=toDate.Value.Date
        data=ReportService.Load(cmbReport.Text,txtSearch.Text,start,finish)
        loadedName=cmbReport.Text
        grid.DataSource=data
        lblCount.Text=data.Rows.Count & " records. Date filter applies to event dates; Students and Category totals include all dates."
    End Sub
    Private Sub LoadClick(sender As Object,e As EventArgs) Handles btnLoad.Click
        Ui.Attempt(AddressOf LoadReport)
    End Sub
    Private Sub ExportClick(sender As Object,e As EventArgs) Handles btnExport.Click
        Ui.Attempt(Sub()
            If data Is Nothing Then Throw New InvalidOperationException("Load a report first.")
            Security.RequireAccess(loadedName="Activity logs")
            Using dialog As New SaveFileDialog With {.Filter="CSV file|*.csv",.FileName=loadedName & ".csv"}
                If dialog.ShowDialog(Me)=DialogResult.OK Then
                    ReportService.ExportCsv(data,dialog.FileName)
                    DatabaseHelper.LogActivity("Report exported",loadedName)
                    AppUtilities.ShowSuccess("CSV exported.")
                End If
            End Using
        End Sub)
    End Sub
    Private Sub PrintClick(sender As Object,e As EventArgs) Handles btnPrint.Click
        Ui.Attempt(Sub()
            If data Is Nothing Then Throw New InvalidOperationException("Load a report first.")
            Security.RequireAccess(loadedName="Activity logs")
            Using document As New ReportDocument(AppSettings.Load().SchoolName & " — " & loadedName,data), preview As New PrintPreviewDialog With {.Document=document,.Width=1000,.Height=750}
                preview.ShowDialog(Me)
            End Using
        End Sub)
    End Sub
End Class
