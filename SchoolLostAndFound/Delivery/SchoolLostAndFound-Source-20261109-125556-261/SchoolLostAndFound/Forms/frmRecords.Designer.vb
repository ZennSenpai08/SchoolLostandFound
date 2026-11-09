Option Strict On
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmRecords
    Inherits System.Windows.Forms.Form
    Friend WithEvents toolbar As FlowLayoutPanel
    Friend WithEvents txtSearch As TextBox
    Friend WithEvents btnSearch As Button
    Friend WithEvents btnNew As Button
    Friend WithEvents btnSave As Button
    Friend WithEvents btnDelete As Button
    Friend WithEvents split As SplitContainer
    Friend WithEvents grid As DataGridView
    Friend WithEvents editor As TableLayoutPanel
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.SuspendLayout()
        Me.toolbar = New FlowLayoutPanel()
        Me.txtSearch = New TextBox()
        Me.btnSearch = New Button()
        Me.btnNew = New Button()
        Me.btnSave = New Button()
        Me.btnDelete = New Button()
        Me.split = New SplitContainer()
        Me.grid = New DataGridView()
        Me.editor = New TableLayoutPanel()
        Me.Text = "Records"
        Me.toolbar.Dock = DockStyle.Top
        Me.toolbar.Height = 44
        Me.toolbar.Padding = New Padding(6)
        Me.txtSearch.Width = 230
        Me.btnSearch.Text = "Search"
        Me.btnNew.Text = "New / Clear"
        Me.btnNew.Width = 100
        Me.btnSave.Text = "Save"
        Me.btnDelete.Text = "Delete"
        Me.toolbar.Controls.AddRange(New Control() {Me.txtSearch, Me.btnSearch, Me.btnNew, Me.btnSave, Me.btnDelete})
        Me.split.Dock = DockStyle.Fill
        Me.split.Size = New Size(1100, 676)
        Me.split.SplitterDistance = 650
        Me.grid.Dock = DockStyle.Fill
        Me.editor.Dock = DockStyle.Fill
        Me.editor.AutoScroll = True
        Me.editor.ColumnCount = 1
        Me.editor.Padding = New Padding(10)
        Me.split.Panel1.Controls.Add(Me.grid)
        Me.split.Panel2.Controls.Add(Me.editor)
        Me.Controls.Add(Me.split)
        Me.Controls.Add(Me.toolbar)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.Font = New Font("Segoe UI", 9.0F)
        Me.ClientSize = New Size(1100, 720)
        Me.MinimumSize = New Size(900, 600)
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Name = "frmRecords"
        Me.ResumeLayout(False)
    End Sub
End Class
