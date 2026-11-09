Option Strict On
Imports System.Linq
Public Class frmSettings
    Public Property Restored As Boolean
    Private Sub OpenForm(sender As Object,e As EventArgs) Handles MyBase.Load
        Ui.Attempt(Sub()
            Security.RequireAccess()
            Dim settings=AppSettings.Load()
            txtSchool.Text=settings.SchoolName : threshold.Value=settings.MatchThreshold
            Dim admin=SessionManager.IsAdministrator()
            txtSchool.Enabled=admin : threshold.Enabled=admin : btnSave.Enabled=admin : btnBackup.Enabled=admin : btnRestore.Enabled=admin
            lblPath.Text="Database: " & DatabaseHelper.DatabasePath & Environment.NewLine & "Restore preserves a safety copy and requires a new sign-in. Keep backups in a protected location."
        End Sub)
    End Sub
    Private Sub SaveClick(sender As Object,e As EventArgs) Handles btnSave.Click
        Ui.Attempt(Sub()
            Dim settings As New AppSettings With {.SchoolName=txtSchool.Text,.MatchThreshold=CInt(threshold.Value)}
            settings.Save() : AppUtilities.ShowSuccess("Settings saved.")
        End Sub)
    End Sub
    Private Sub PasswordClick(sender As Object,e As EventArgs) Handles btnPassword.Click
        Ui.Attempt(Sub()
            Security.ChangePassword(txtOld.Text,txtNew.Text)
            txtOld.Clear() : txtNew.Clear() : AppUtilities.ShowSuccess("Password changed.")
        End Sub)
    End Sub
    Private Sub BackupClick(sender As Object,e As EventArgs) Handles btnBackup.Click
        Ui.Attempt(Sub()
            Using dialog As New SaveFileDialog With {.Filter="Access database|*.mdb",.FileName="SchoolLostAndFound-" & DateTime.Now.ToString("yyyyMMdd-HHmmss") & ".mdb"}
                If dialog.ShowDialog(Me)=DialogResult.OK Then
                    BackupService.Backup(dialog.FileName) : AppUtilities.ShowSuccess("Backup created and verified.")
                End If
            End Using
        End Sub)
    End Sub
    Private Sub RestoreClick(sender As Object,e As EventArgs) Handles btnRestore.Click
        Ui.Attempt(Sub()
            Using dialog As New OpenFileDialog With {.Filter="Access database|*.mdb"}
                If dialog.ShowDialog(Me)<>DialogResult.OK Then Return
                If Not AppUtilities.ConfirmAction("Replace the current database with this backup? A safety copy will be retained and you must sign in again.") Then Return
                Dim safety=BackupService.Restore(dialog.FileName)
                Restored=True
                AppUtilities.ShowSuccess("Restored. Safety copy: " & safety)
                Close()
            End Using
        End Sub)
    End Sub
End Class
