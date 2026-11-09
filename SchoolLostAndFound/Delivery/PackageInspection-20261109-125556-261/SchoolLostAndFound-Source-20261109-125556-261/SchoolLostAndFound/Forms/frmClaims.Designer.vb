Option Strict On
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmClaims
    Inherits System.Windows.Forms.Form
    Friend WithEvents bar As FlowLayoutPanel
    Friend WithEvents txtSearch As TextBox
    Friend WithEvents btnRefresh As Button
    Friend WithEvents grid As DataGridView
    Friend WithEvents panel As FlowLayoutPanel
    Friend WithEvents lblLost As Label
    Friend WithEvents cmbLost As ComboBox
    Friend WithEvents lblFound As Label
    Friend WithEvents cmbFound As ComboBox
    Friend WithEvents lblEvidence As Label
    Friend WithEvents txtEvidence As TextBox
    Friend WithEvents btnCreate As Button
    Friend WithEvents lblNotes As Label
    Friend WithEvents txtNotes As TextBox
    Friend WithEvents lblStatus As Label
    Friend WithEvents cmbStatus As ComboBox
    Friend WithEvents btnTransition As Button
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.SuspendLayout()
        Me.bar = New FlowLayoutPanel()
        Me.txtSearch = New TextBox()
        Me.btnRefresh = New Button()
        Me.grid = New DataGridView()
        Me.panel = New FlowLayoutPanel()
        Me.lblLost = New Label()
        Me.cmbLost = New ComboBox()
        Me.lblFound = New Label()
        Me.cmbFound = New ComboBox()
        Me.lblEvidence = New Label()
        Me.txtEvidence = New TextBox()
        Me.btnCreate = New Button()
        Me.lblNotes = New Label()
        Me.txtNotes = New TextBox()
        Me.lblStatus = New Label()
        Me.cmbStatus = New ComboBox()
        Me.btnTransition = New Button()
        Me.Text = "Claims and item release"
        Me.bar.Dock=DockStyle.Top
        Me.bar.Height=42
        Me.txtSearch.Width=300
        Me.btnRefresh.Text="Search / Refresh"
        Me.btnRefresh.Width=140
        Me.bar.Controls.AddRange(New Control() {Me.txtSearch,Me.btnRefresh})
        Me.grid.Dock=DockStyle.Fill
        Me.panel.Dock=DockStyle.Right
        Me.panel.Width=390
        Me.panel.FlowDirection=FlowDirection.TopDown
        Me.panel.WrapContents=False
        Me.panel.AutoScroll=True
        Me.panel.Padding=New Padding(10)
        Me.lblLost.Text="Lost report (claimant is its student)"
        Me.lblLost.AutoSize=True
        Me.cmbLost.Width=350
        Me.lblFound.Text="Found item"
        Me.lblFound.AutoSize=True
        Me.cmbFound.Width=350
        Me.lblEvidence.Text="Ownership evidence for NEW claim *"
        Me.lblEvidence.AutoSize=True
        Me.txtEvidence.Multiline=True
        Me.txtEvidence.Size=New Size(350,90)
        Me.txtEvidence.MaxLength=4000
        Me.btnCreate.Text="Create pending claim"
        Me.btnCreate.Width=220
        Me.lblNotes.Text="Verification / handover notes for SELECTED claim *"
        Me.lblNotes.AutoSize=True
        Me.txtNotes.Multiline=True
        Me.txtNotes.Size=New Size(350,130)
        Me.txtNotes.MaxLength=4000
        Me.lblStatus.Text="Next status (select a claim in the table first)"
        Me.lblStatus.AutoSize=True
        Me.cmbStatus.Width=350
        Me.cmbStatus.DropDownStyle=ComboBoxStyle.DropDownList
        Me.btnTransition.Text="Apply status / Release item"
        Me.btnTransition.Width=240
        Me.panel.Controls.AddRange(New Control() {Me.lblLost,Me.cmbLost,Me.lblFound,Me.cmbFound,Me.lblEvidence,Me.txtEvidence,Me.btnCreate,Me.lblNotes,Me.txtNotes,Me.lblStatus,Me.cmbStatus,Me.btnTransition})
        Me.Controls.Add(Me.grid)
        Me.Controls.Add(Me.panel)
        Me.Controls.Add(Me.bar)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.Font = New Font("Segoe UI", 9.0F)
        Me.ClientSize = New Size(1100, 720)
        Me.MinimumSize = New Size(900, 600)
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Name = "frmClaims"
        Me.ResumeLayout(False)
    End Sub
End Class
