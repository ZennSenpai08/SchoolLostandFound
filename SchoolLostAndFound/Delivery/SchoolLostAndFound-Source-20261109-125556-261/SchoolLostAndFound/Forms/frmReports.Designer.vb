Option Strict On
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmReports
    Inherits System.Windows.Forms.Form
    Friend WithEvents bar As FlowLayoutPanel
    Friend WithEvents cmbReport As ComboBox
    Friend WithEvents txtSearch As TextBox
    Friend WithEvents chkDates As CheckBox
    Friend WithEvents fromDate As DateTimePicker
    Friend WithEvents toDate As DateTimePicker
    Friend WithEvents btnLoad As Button
    Friend WithEvents btnExport As Button
    Friend WithEvents btnPrint As Button
    Friend WithEvents grid As DataGridView
    Friend WithEvents lblCount As Label
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.SuspendLayout()
        Me.bar = New FlowLayoutPanel()
        Me.cmbReport = New ComboBox()
        Me.txtSearch = New TextBox()
        Me.chkDates = New CheckBox()
        Me.fromDate = New DateTimePicker()
        Me.toDate = New DateTimePicker()
        Me.btnLoad = New Button()
        Me.btnExport = New Button()
        Me.btnPrint = New Button()
        Me.grid = New DataGridView()
        Me.lblCount = New Label()
        Me.Text="Reports and activity logs"
        Me.bar.Dock=DockStyle.Top
        Me.bar.Height=85
        Me.bar.Padding=New Padding(6)
        Me.cmbReport.Width=170
        Me.cmbReport.DropDownStyle=ComboBoxStyle.DropDownList
        Me.txtSearch.Width=180
        Me.chkDates.Text="Filter dates"
        Me.fromDate.Format=DateTimePickerFormat.Short
        Me.fromDate.Width=110
        Me.toDate.Format=DateTimePickerFormat.Short
        Me.toDate.Width=110
        Me.btnLoad.Text="Load"
        Me.btnExport.Text="Export CSV"
        Me.btnExport.Width=100
        Me.btnPrint.Text="Print preview"
        Me.btnPrint.Width=120
        Me.bar.Controls.AddRange(New Control() {Me.cmbReport,Me.txtSearch,Me.chkDates,Me.fromDate,Me.toDate,Me.btnLoad,Me.btnExport,Me.btnPrint})
        Me.grid.Dock=DockStyle.Fill
        Me.lblCount.Dock=DockStyle.Bottom
        Me.lblCount.Height=30
        Me.Controls.Add(Me.grid)
        Me.Controls.Add(Me.lblCount)
        Me.Controls.Add(Me.bar)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.Font = New Font("Segoe UI", 9.0F)
        Me.ClientSize = New Size(1100, 720)
        Me.MinimumSize = New Size(900, 600)
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Name = "frmReports"
        Me.ResumeLayout(False)
    End Sub
End Class
