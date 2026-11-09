Option Strict On
Option Explicit On
Option Infer On

Imports System.Data.OleDb

Public Class frmDashboard
    Public Property ReturnToLogin As Boolean

    Private Sub frmDashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Verify user is logged in
        If Not SessionManager.IsLoggedIn() Then
            Close()
            Return
        End If

        Ui.Attempt(Sub() Text = AppSettings.Load().SchoolName)
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
        btnActivityLogs.Enabled = SessionManager.IsAdministrator()
        btnUsers.Enabled = SessionManager.IsAdministrator()
    End Sub

    Private Sub frmDashboard_Activated(sender As Object, e As EventArgs) Handles MyBase.Activated
        ' Refresh statistics whenever the dashboard regains focus (e.g. after closing a child form)
        LoadStatistics()
    End Sub

    Private Sub LoadStatistics()
        If Not SessionManager.IsLoggedIn() Then Return
        Try
            Security.RequireAccess()
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
            Ui.Attempt(Sub()
                Security.Logout()
                ReturnToLogin = True
                Close()
            End Sub)
        End If
    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Application.Exit()
    End Sub



    ' Navigation handlers
    Private Sub btnStudents_Click(sender As Object, e As EventArgs) Handles btnStudents.Click
        OpenModule(New frmStudents())
    End Sub

    Private Sub btnCategories_Click(sender As Object, e As EventArgs) Handles btnCategories.Click
        OpenModule(New frmCategories())
    End Sub

    Private Sub btnLostItems_Click(sender As Object, e As EventArgs) Handles btnLostItems.Click
        OpenRecords("LostItems")
    End Sub

    Private Sub btnFoundItems_Click(sender As Object, e As EventArgs) Handles btnFoundItems.Click
        OpenRecords("FoundItems")
    End Sub

    Private Sub btnClaims_Click(sender As Object, e As EventArgs) Handles btnClaims.Click
        OpenModule(New frmClaims())
    End Sub

    Private Sub btnReports_Click(sender As Object, e As EventArgs) Handles btnReports.Click
        OpenModule(New frmReports())
    End Sub

    Private Sub btnActivityLogs_Click(sender As Object, e As EventArgs) Handles btnActivityLogs.Click
        OpenModule(New frmReports With {.InitialReport="Activity logs"}, True)
    End Sub
    Private Sub OpenModule(form As Form, Optional adminOnly As Boolean=False)
        Using form
            Ui.Attempt(Sub()
                Security.RequireAccess(adminOnly)
                form.ShowDialog(Me)
                LoadStatistics()
            End Sub)
        End Using
    End Sub
    Private Sub OpenRecords(entity As String)
        Dim form As New frmRecords()
        form.Configure(entity)
        OpenModule(form,entity="Users")
    End Sub
    Private Sub MatchingClick(sender As Object,e As EventArgs) Handles btnMatching.Click
        OpenModule(New frmMatching())
    End Sub
    Private Sub UsersClick(sender As Object,e As EventArgs) Handles btnUsers.Click
        OpenRecords("Users")
    End Sub
    Private Sub SettingsClick(sender As Object,e As EventArgs) Handles btnSettings.Click
        Using form As New frmSettings()
            Ui.Attempt(Sub()
                Security.RequireAccess()
                form.ShowDialog(Me)
                If form.Restored Then
                    ReturnToLogin=True
                    Close()
                Else
                    Text=AppSettings.Load().SchoolName
                End If
            End Sub)
        End Using
    End Sub
    Private Sub DashboardClick(sender As Object,e As EventArgs) Handles btnDashboard.Click
        LoadStatistics()
    End Sub
End Class
