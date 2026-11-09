Option Strict On
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmSettings
    Inherits System.Windows.Forms.Form
    Friend WithEvents settingsPanel As FlowLayoutPanel
    Friend WithEvents lblSchool As Label
    Friend WithEvents txtSchool As TextBox
    Friend WithEvents lblThreshold As Label
    Friend WithEvents threshold As NumericUpDown
    Friend WithEvents btnSave As Button
    Friend WithEvents btnBackup As Button
    Friend WithEvents btnRestore As Button
    Friend WithEvents lblOld As Label
    Friend WithEvents txtOld As TextBox
    Friend WithEvents lblNew As Label
    Friend WithEvents txtNew As TextBox
    Friend WithEvents btnPassword As Button
    Friend WithEvents lblPath As Label
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.SuspendLayout()
        Me.settingsPanel = New FlowLayoutPanel()
        Me.lblSchool = New Label()
        Me.txtSchool = New TextBox()
        Me.lblThreshold = New Label()
        Me.threshold = New NumericUpDown()
        Me.btnSave = New Button()
        Me.btnBackup = New Button()
        Me.btnRestore = New Button()
        Me.lblOld = New Label()
        Me.txtOld = New TextBox()
        Me.lblNew = New Label()
        Me.txtNew = New TextBox()
        Me.btnPassword = New Button()
        Me.lblPath = New Label()
        Me.Text="Settings, password and database maintenance"
        Me.settingsPanel.Dock=DockStyle.Fill
        Me.settingsPanel.FlowDirection=FlowDirection.TopDown
        Me.settingsPanel.WrapContents=False
        Me.settingsPanel.AutoScroll=True
        Me.settingsPanel.Padding=New Padding(24)
        Me.lblSchool.Text="School name (administrator)"
        Me.lblSchool.AutoSize=True
        Me.txtSchool.Width=500
        Me.txtSchool.MaxLength=100
        Me.lblThreshold.Text="Default minimum match score (0-100)"
        Me.lblThreshold.AutoSize=True
        Me.threshold.Maximum=100
        Me.btnSave.Text="Save settings"
        Me.btnSave.Width=230
        Me.btnBackup.Text="Create verified database backup"
        Me.btnBackup.Width=300
        Me.btnRestore.Text="Restore database backup"
        Me.btnRestore.Width=300
        Me.lblOld.Text="Current password"
        Me.lblOld.AutoSize=True
        Me.txtOld.Width=350
        Me.txtOld.UseSystemPasswordChar=True
        Me.txtOld.MaxLength=128
        Me.lblNew.Text="New password (10-128 characters)"
        Me.lblNew.AutoSize=True
        Me.txtNew.Width=350
        Me.txtNew.UseSystemPasswordChar=True
        Me.txtNew.MaxLength=128
        Me.btnPassword.Text="Change my password"
        Me.btnPassword.Width=230
        Me.lblPath.AutoSize=True
        Me.lblPath.MaximumSize=New Size(950,0)
        Me.settingsPanel.Controls.AddRange(New Control() {Me.lblSchool,Me.txtSchool,Me.lblThreshold,Me.threshold,Me.btnSave,Me.btnBackup,Me.btnRestore,Me.lblOld,Me.txtOld,Me.lblNew,Me.txtNew,Me.btnPassword,Me.lblPath})
        Me.Controls.Add(Me.settingsPanel)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.Font = New Font("Segoe UI", 9.0F)
        Me.ClientSize = New Size(1100, 720)
        Me.MinimumSize = New Size(900, 600)
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Name = "frmSettings"
        Me.ResumeLayout(False)
    End Sub
End Class
