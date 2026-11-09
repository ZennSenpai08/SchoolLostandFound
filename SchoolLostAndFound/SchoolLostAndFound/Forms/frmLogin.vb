Option Strict On
Public Class frmLogin
    Private failures As Integer
    Private nextAttempt As DateTime
    Private Sub frmLogin_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AcceptButton = btnLogin
        CancelButton = btnExit
        txtUsername.MaxLength = 50
        txtPassword.MaxLength = 128
        txtPassword.UseSystemPasswordChar = True
    End Sub
    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        DialogResult = DialogResult.Cancel
        Close()
    End Sub
    Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        Try
            If DateTime.Now < nextAttempt Then Throw New InvalidOperationException("Please wait 30 seconds before trying again.")
            If String.IsNullOrWhiteSpace(txtUsername.Text) OrElse txtPassword.Text.Length = 0 Then Throw New ArgumentException("Enter your username and password.")
            If Security.Login(txtUsername.Text, txtPassword.Text) Then
                txtPassword.Clear()
                DialogResult = DialogResult.OK
                Close()
            Else
                failures += 1
                If failures >= 5 Then
                    nextAttempt = DateTime.Now.AddSeconds(30)
                    failures = 0
                End If
                txtPassword.Clear()
                AppUtilities.ShowError("Invalid credentials or inactive account.")
            End If
        Catch ex As Exception
            AppUtilities.ShowError(ex.Message)
        End Try
    End Sub
End Class
