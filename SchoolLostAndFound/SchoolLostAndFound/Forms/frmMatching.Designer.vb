Option Strict On
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMatching
    Inherits System.Windows.Forms.Form
    Friend WithEvents bar As FlowLayoutPanel
    Friend WithEvents lblLost As Label
    Friend WithEvents cmbLost As ComboBox
    Friend WithEvents lblThreshold As Label
    Friend WithEvents threshold As NumericUpDown
    Friend WithEvents btnMatch As Button
    Friend WithEvents btnClaim As Button
    Friend WithEvents grid As DataGridView
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.SuspendLayout()
        Me.bar = New FlowLayoutPanel()
        Me.lblLost = New Label()
        Me.cmbLost = New ComboBox()
        Me.lblThreshold = New Label()
        Me.threshold = New NumericUpDown()
        Me.btnMatch = New Button()
        Me.btnClaim = New Button()
        Me.grid = New DataGridView()
        Me.Text = "Item matching — suggestions require ownership verification"
        Me.bar.Dock = DockStyle.Top
        Me.bar.Height = 80
        Me.bar.Padding = New Padding(8)
        Me.lblLost.Text = "Lost report"
        Me.lblLost.AutoSize = True
        Me.cmbLost.Width = 370
        Me.lblThreshold.Text = "Minimum score"
        Me.lblThreshold.AutoSize = True
        Me.threshold.Maximum = 100
        Me.threshold.Value = 40
        Me.threshold.Width = 65
        Me.btnMatch.Text = "Find matches"
        Me.btnMatch.Width = 110
        Me.btnClaim.Text = "Create claim"
        Me.btnClaim.Width = 110
        Me.bar.Controls.AddRange(New Control() {Me.lblLost,Me.cmbLost,Me.lblThreshold,Me.threshold,Me.btnMatch,Me.btnClaim})
        Me.grid.Dock = DockStyle.Fill
        Me.Controls.Add(Me.grid)
        Me.Controls.Add(Me.bar)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.Font = New Font("Segoe UI", 9.0F)
        Me.ClientSize = New Size(1100, 720)
        Me.MinimumSize = New Size(900, 600)
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Name = "frmMatching"
        Me.ResumeLayout(False)
    End Sub
End Class
