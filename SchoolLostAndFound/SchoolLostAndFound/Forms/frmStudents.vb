Option Strict On
Option Explicit On
Option Infer On

Imports System.Data.OleDb

Public Class frmStudents

    Private currentStudentId As Integer = 0

    Private Sub frmStudents_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not SessionManager.IsLoggedIn() Then
            Close() : Return
        End If
        txtStudentNumber.MaxLength = 20
        txtFullName.MaxLength = 100
        txtCourse.MaxLength = 50
        txtSection.MaxLength = 50
        txtContact.MaxLength = 20
        txtEmail.MaxLength = 50
        AppUtilities.FormatDataGridView(dgvStudents)
        LoadData()
    End Sub

    Private Sub LoadData(Optional searchKeyword As String = "")
        Try
            Dim dt = RecordService.List("Students", searchKeyword)

            dgvStudents.DataSource = dt

            ' Hide the ID column
            If dgvStudents.Columns.Contains("StudentID") Then
                dgvStudents.Columns("StudentID").Visible = False
            End If

            ' Adjust column headers
            If dgvStudents.Columns.Contains("StudentNumber") Then dgvStudents.Columns("StudentNumber").HeaderText = "Student No."
            If dgvStudents.Columns.Contains("FullName") Then
                dgvStudents.Columns("FullName").HeaderText = "Full Name"
                dgvStudents.Columns("FullName").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            End If
            If dgvStudents.Columns.Contains("CourseOrGrade") Then dgvStudents.Columns("CourseOrGrade").HeaderText = "Course / Grade"
            If dgvStudents.Columns.Contains("Section") Then dgvStudents.Columns("Section").HeaderText = "Section"
            If dgvStudents.Columns.Contains("ContactNumber") Then dgvStudents.Columns("ContactNumber").HeaderText = "Contact No."
            If dgvStudents.Columns.Contains("Email") Then dgvStudents.Columns("Email").HeaderText = "Email"

            ClearInputs()

        Catch ex As Exception
            AppUtilities.ShowError("Error loading students: " & ex.Message)
        End Try
    End Sub

    Private Sub ClearInputs()
        currentStudentId = 0
        txtStudentNumber.Text = ""
        txtFullName.Text = ""
        txtCourse.Text = ""
        txtSection.Text = ""
        txtContact.Text = ""
        txtEmail.Text = ""
        btnSave.Text = "Add Student"
        btnDelete.Enabled = False
        txtStudentNumber.Focus()
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        ClearInputs()
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        txtSearch.Text = ""
        LoadData()
    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        LoadData(txtSearch.Text)
    End Sub

    Private Sub dgvStudents_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvStudents.CellClick
        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = dgvStudents.Rows(e.RowIndex)
            currentStudentId = Convert.ToInt32(row.Cells("StudentID").Value)
            txtStudentNumber.Text = If(row.Cells("StudentNumber").Value IsNot DBNull.Value, row.Cells("StudentNumber").Value.ToString(), "")
            txtFullName.Text = If(row.Cells("FullName").Value IsNot DBNull.Value, row.Cells("FullName").Value.ToString(), "")
            txtCourse.Text = If(row.Cells("CourseOrGrade").Value IsNot DBNull.Value, row.Cells("CourseOrGrade").Value.ToString(), "")
            txtSection.Text = If(row.Cells("Section").Value IsNot DBNull.Value, row.Cells("Section").Value.ToString(), "")
            txtContact.Text = If(row.Cells("ContactNumber").Value IsNot DBNull.Value, row.Cells("ContactNumber").Value.ToString(), "")
            txtEmail.Text = If(row.Cells("Email").Value IsNot DBNull.Value, row.Cells("Email").Value.ToString(), "")

            btnSave.Text = "Update Student"
            btnDelete.Enabled = True
        End If
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Ui.Attempt(Sub()
            Dim values As New Dictionary(Of String, Object) From {{"StudentNumber", txtStudentNumber.Text}, {"FullName", txtFullName.Text}, {"CourseOrGrade", txtCourse.Text}, {"Section", txtSection.Text}, {"ContactNumber", txtContact.Text}, {"Email", txtEmail.Text}}
            RecordService.Save("Students", currentStudentId, values)
            LoadData(txtSearch.Text)
            AppUtilities.ShowSuccess("Student saved successfully.")
        End Sub)
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If currentStudentId = 0 OrElse Not AppUtilities.ConfirmAction("Delete the selected student?") Then Return
        Ui.Attempt(Sub()
            RecordService.Delete("Students", currentStudentId)
            LoadData(txtSearch.Text)
            AppUtilities.ShowSuccess("Student deleted successfully.")
        End Sub)
    End Sub
End Class
