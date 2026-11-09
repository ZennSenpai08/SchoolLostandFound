Option Strict On
Option Explicit On
Option Infer On

Imports System.Data.OleDb

Public Class frmLogin

    Private Sub frmLogin_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Clear textboxes on load
        txtUsername.Text = ""
        txtPassword.Text = ""
    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Application.Exit()
    End Sub

    Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        Dim username As String = txtUsername.Text.Trim()
        Dim password As String = txtPassword.Text

        ' Basic validation using ValidationHelper
        Dim errMsg As String = ""
        If Not ValidationHelper.IsRequired(username, "Username", errMsg) Then
            AppUtilities.ShowError(errMsg, "Validation Error")
            txtUsername.Focus()
            Return
        End If

        If Not ValidationHelper.IsRequired(password, "Password", errMsg) Then
            AppUtilities.ShowError(errMsg, "Validation Error")
            txtPassword.Focus()
            Return
        End If

        Try
            ' Authenticate against the database
            Dim sql As String = "SELECT UserID, Username, FullName, Role, IsActive FROM Users WHERE Username = ? AND [Password] = ?"
            Dim params() As OleDbParameter = {
                DatabaseHelper.CreateParameter("@Username", username),
                DatabaseHelper.CreateParameter("@Password", password)
            }

            Dim dt As DataTable = DatabaseHelper.ExecuteQuery(sql, params)

            If dt.Rows.Count = 1 Then
                ' User found, check if active
                Dim row As DataRow = dt.Rows(0)
                Dim isActive As Boolean = AppUtilities.SafeBool(row, "IsActive")

                If Not isActive Then
                    AppUtilities.ShowError("Your account is currently inactive. Please contact the administrator.", "Account Inactive")
                    Return
                End If

                ' Login successful - Set session
                Dim userId As Integer = AppUtilities.SafeInt(row, "UserID")
                Dim fullName As String = AppUtilities.SafeString(row, "FullName")
                Dim role As String = AppUtilities.SafeString(row, "Role")

                SessionManager.SetSession(userId, username, fullName, role)

                ' Log activity
                DatabaseHelper.LogActivity("Login", fullName & " logged into the system.")

                ' In Phase 5, this will open frmDashboard
                Dim dashboard As New frmDashboard()
                dashboard.Show()
                Me.Hide()

            Else
                ' Invalid credentials (we don't reveal if username or password was wrong)
                AppUtilities.ShowError("Invalid username or password.", "Authentication Failed")
                txtPassword.Text = ""
                txtPassword.Focus()
            End If

        Catch ex As Exception
            AppUtilities.ShowError("A database error occurred during login." & Environment.NewLine & ex.Message, "Database Error")
        End Try

    End Sub

End Class
