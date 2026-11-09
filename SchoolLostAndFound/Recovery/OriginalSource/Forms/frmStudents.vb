Option Strict On
Option Explicit On
Option Infer On

Imports System.Data.OleDb

Public Class frmStudents

    Private currentStudentId As Integer = 0

    Private Sub frmStudents_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AppUtilities.FormatDataGridView(dgvStudents)
        LoadData()
    End Sub

    Private Sub LoadData(Optional searchKeyword As String = "")
        Try
            Dim sql As String = "SELECT StudentID, StudentNumber, FullName, CourseOrGrade, [Section], ContactNumber, Email FROM Students"
            Dim dt As DataTable

            If String.IsNullOrWhiteSpace(searchKeyword) Then
                sql &= " ORDER BY FullName ASC"
                dt = DatabaseHelper.ExecuteQuery(sql)
            Else
                sql &= " WHERE StudentNumber LIKE ? OR FullName LIKE ? OR CourseOrGrade LIKE ? OR [Section] LIKE ? ORDER BY FullName ASC"
                Dim keyword As String = "%" & searchKeyword.Trim() & "%"
                Dim params() As OleDbParameter = {
                    DatabaseHelper.CreateParameter("@p1", keyword),
                    DatabaseHelper.CreateParameter("@p2", keyword),
                    DatabaseHelper.CreateParameter("@p3", keyword),
                    DatabaseHelper.CreateParameter("@p4", keyword)
                }
                dt = DatabaseHelper.ExecuteQuery(sql, params)
            End If

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
        Dim studentNo As String = txtStudentNumber.Text.Trim()
        Dim fullName As String = txtFullName.Text.Trim()
        Dim course As String = txtCourse.Text.Trim()
        Dim section As String = txtSection.Text.Trim()
        Dim contact As String = txtContact.Text.Trim()
        Dim email As String = txtEmail.Text.Trim()

        ' Validation: Required fields
        Dim errMsg As String = ""
        If Not ValidationHelper.IsRequired(studentNo, "Student Number", errMsg) Then
            AppUtilities.ShowError(errMsg, "Validation Error")
            txtStudentNumber.Focus()
            Return
        End If

        If Not ValidationHelper.IsRequired(fullName, "Full Name", errMsg) Then
            AppUtilities.ShowError(errMsg, "Validation Error")
            txtFullName.Focus()
            Return
        End If

        ' Validation: Email format (if provided)
        If Not ValidationHelper.IsValidEmail(email, errMsg) Then
            AppUtilities.ShowError(errMsg, "Validation Error")
            txtEmail.Focus()
            Return
        End If

        ' Validation: Unique Student Number
        If Not ValidationHelper.IsStudentNumberUnique(studentNo, currentStudentId, errMsg) Then
            AppUtilities.ShowError(errMsg, "Duplicate Record")
            txtStudentNumber.Focus()
            Return
        End If

        Try
            If currentStudentId = 0 Then
                ' INSERT
                Dim sql As String = "INSERT INTO Students (StudentNumber, FullName, CourseOrGrade, [Section], ContactNumber, Email) VALUES (?, ?, ?, ?, ?, ?)"
                Dim params() As OleDbParameter = {
                    DatabaseHelper.CreateParameter("@StudentNumber", studentNo),
                    DatabaseHelper.CreateParameter("@FullName", fullName),
                    DatabaseHelper.CreateParameter("@CourseOrGrade", course),
                    DatabaseHelper.CreateParameter("@Section", section),
                    DatabaseHelper.CreateParameter("@ContactNumber", contact),
                    DatabaseHelper.CreateParameter("@Email", email)
                }

                DatabaseHelper.ExecuteNonQuery(sql, params)
                DatabaseHelper.LogActivity("Student Added", "Added student: " & fullName & " (" & studentNo & ")")
                AppUtilities.ShowSuccess("Student added successfully.")
            Else
                ' UPDATE
                Dim sql As String = "UPDATE Students SET StudentNumber = ?, FullName = ?, CourseOrGrade = ?, [Section] = ?, ContactNumber = ?, Email = ? WHERE StudentID = ?"
                Dim params() As OleDbParameter = {
                    DatabaseHelper.CreateParameter("@StudentNumber", studentNo),
                    DatabaseHelper.CreateParameter("@FullName", fullName),
                    DatabaseHelper.CreateParameter("@CourseOrGrade", course),
                    DatabaseHelper.CreateParameter("@Section", section),
                    DatabaseHelper.CreateParameter("@ContactNumber", contact),
                    DatabaseHelper.CreateParameter("@Email", email),
                    DatabaseHelper.CreateParameter("@StudentID", currentStudentId)
                }

                DatabaseHelper.ExecuteNonQuery(sql, params)
                DatabaseHelper.LogActivity("Student Updated", "Updated student: " & fullName & " (" & studentNo & ")")
                AppUtilities.ShowSuccess("Student updated successfully.")
            End If

            LoadData(txtSearch.Text)
        Catch ex As Exception
            AppUtilities.ShowError("An error occurred while saving: " & ex.Message)
        End Try
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If currentStudentId = 0 Then Return

        ' Check dependencies using ValidationHelper
        Dim errMsg As String = ""
        If Not ValidationHelper.CanDeleteStudent(currentStudentId, errMsg) Then
            AppUtilities.ShowError(errMsg, "Deletion Denied")
            Return
        End If

        If AppUtilities.ConfirmAction("Are you sure you want to delete this student?", "Confirm Deletion") Then
            Try
                Dim deletedName As String = txtFullName.Text
                Dim deletedNumber As String = txtStudentNumber.Text

                Dim sqlDelete As String = "DELETE FROM Students WHERE StudentID = ?"
                DatabaseHelper.ExecuteNonQuery(sqlDelete, {DatabaseHelper.CreateParameter("@StudentID", currentStudentId)})

                DatabaseHelper.LogActivity("Student Deleted", "Deleted student: " & deletedName & " (" & deletedNumber & ")")
                AppUtilities.ShowSuccess("Student deleted successfully.")

                LoadData(txtSearch.Text)
            Catch ex As Exception
                AppUtilities.ShowError("An error occurred during deletion: " & ex.Message)
            End Try
        End If
    End Sub

End Class
