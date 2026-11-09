Option Strict On
Option Explicit On
Option Infer On

Imports System.Data.OleDb

Public Class frmDashboard

    Private Sub frmDashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Verify user is logged in
        If Not SessionManager.IsLoggedIn() Then
            Application.Exit()
            Return
        End If

        ' Display user info
        lblUserInfo.Text = "Welcome, " & SessionManager.CurrentFullName & " (" & SessionManager.CurrentRole & ")"

        ' Apply Role-based Access Control
        ApplyRolePermissions()

        ' Load Statistics
        LoadStatistics()
    End Sub

    Private Sub ApplyRolePermissions()
        ' Only Administrators can access Activity Logs
        ' Staff can access all operational modules (Students, Categories, Lost/Found Items, Claims)
        If SessionManager.IsStaff() Then
            btnActivityLogs.Enabled = False
            btnActivityLogs.ForeColor = Color.Gray
        End If
    End Sub

    Private Sub frmDashboard_Activated(sender As Object, e As EventArgs) Handles MyBase.Activated
        ' Refresh statistics whenever the dashboard regains focus (e.g. after closing a child form)
        LoadStatistics()
    End Sub

    Private Sub LoadStatistics()
        Try
            ' Load Total Lost Items
            Dim sqlLost As String = "SELECT COUNT(*) FROM LostItems"
            Dim countLost As Object = DatabaseHelper.ExecuteScalar(sqlLost)
            lblTotalLostItems.Text = If(countLost IsNot Nothing, countLost.ToString(), "0")

            ' Load Total Found Items
            Dim sqlFound As String = "SELECT COUNT(*) FROM FoundItems"
            Dim countFound As Object = DatabaseHelper.ExecuteScalar(sqlFound)
            lblTotalFoundItems.Text = If(countFound IsNot Nothing, countFound.ToString(), "0")

            ' Load Pending Claims
            Dim sqlClaims As String = "SELECT COUNT(*) FROM Claims WHERE Status = ?"
            Dim paramClaims() As OleDbParameter = {DatabaseHelper.CreateParameter("@Status", AppUtilities.STATUS_PENDING)}
            Dim countClaims As Object = DatabaseHelper.ExecuteScalar(sqlClaims, paramClaims)
            lblPendingClaims.Text = If(countClaims IsNot Nothing, countClaims.ToString(), "0")

            ' Load Total Students
            Dim sqlStudents As String = "SELECT COUNT(*) FROM Students"
            Dim countStudents As Object = DatabaseHelper.ExecuteScalar(sqlStudents)
            lblTotalStudents.Text = If(countStudents IsNot Nothing, countStudents.ToString(), "0")

            ' Load Total Categories
            Dim sqlCategories As String = "SELECT COUNT(*) FROM Categories"
            Dim countCategories As Object = DatabaseHelper.ExecuteScalar(sqlCategories)
            lblTotalCategories.Text = If(countCategories IsNot Nothing, countCategories.ToString(), "0")

        Catch ex As Exception
            AppUtilities.ShowError("Error loading dashboard statistics: " & ex.Message)
        End Try
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As EventArgs) Handles btnLogout.Click
        If AppUtilities.ConfirmAction("Are you sure you want to logout?", "Logout") Then
            DatabaseHelper.LogActivity("Logout", SessionManager.CurrentFullName & " logged out.")
            SessionManager.Clear()
            
            Dim loginForm As New frmLogin()
            loginForm.Show()
            Me.Close()
        End If
    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Application.Exit()
    End Sub

    Private Sub frmDashboard_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        ' Ensure the application closes if the user closes the form directly (e.g. via the 'X' button),
        ' unless we are just logging out and switching back to frmLogin.
        If Not SessionManager.IsLoggedIn() Then
            ' It was a logout, do nothing to the application
        Else
            Application.Exit()
        End If
    End Sub

    ' Navigation handlers
    Private Sub btnStudents_Click(sender As Object, e As EventArgs) Handles btnStudents.Click
        Dim frm As New frmStudents()
        frm.ShowDialog()
    End Sub

    Private Sub btnCategories_Click(sender As Object, e As EventArgs) Handles btnCategories.Click
        Dim frm As New frmCategories()
        frm.ShowDialog()
    End Sub

    Private Sub btnLostItems_Click(sender As Object, e As EventArgs) Handles btnLostItems.Click
        AppUtilities.ShowSuccess("Lost Items module will be implemented in Phase 6.", "Module Pending")
    End Sub

    Private Sub btnFoundItems_Click(sender As Object, e As EventArgs) Handles btnFoundItems.Click
        AppUtilities.ShowSuccess("Found Items module will be implemented in Phase 6.", "Module Pending")
    End Sub

    Private Sub btnClaims_Click(sender As Object, e As EventArgs) Handles btnClaims.Click
        AppUtilities.ShowSuccess("Claims module will be implemented in Phase 8.", "Module Pending")
    End Sub

    Private Sub btnReports_Click(sender As Object, e As EventArgs) Handles btnReports.Click
        AppUtilities.ShowSuccess("Reports module will be implemented in Phase 10.", "Module Pending")
    End Sub

    Private Sub btnActivityLogs_Click(sender As Object, e As EventArgs) Handles btnActivityLogs.Click
        AppUtilities.ShowSuccess("Activity Logs module will be implemented in Phase 6.", "Module Pending")
    End Sub
End Class
