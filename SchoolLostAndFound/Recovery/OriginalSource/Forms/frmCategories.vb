Option Strict On
Option Explicit On
Option Infer On

Imports System.Data.OleDb

Public Class frmCategories

    Private currentCategoryId As Integer = 0

    Private Sub frmCategories_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AppUtilities.FormatDataGridView(dgvCategories)
        LoadData()
    End Sub

    Private Sub LoadData(Optional searchKeyword As String = "")
        Try
            Dim sql As String = "SELECT CategoryID, CategoryName, Description FROM Categories"
            Dim dt As DataTable

            If String.IsNullOrWhiteSpace(searchKeyword) Then
                sql &= " ORDER BY CategoryName ASC"
                dt = DatabaseHelper.ExecuteQuery(sql)
            Else
                sql &= " WHERE CategoryName LIKE ? OR Description LIKE ? ORDER BY CategoryName ASC"
                Dim keyword As String = "%" & searchKeyword.Trim() & "%"
                Dim params() As OleDbParameter = {
                    DatabaseHelper.CreateParameter("@p1", keyword),
                    DatabaseHelper.CreateParameter("@p2", keyword)
                }
                dt = DatabaseHelper.ExecuteQuery(sql, params)
            End If

            dgvCategories.DataSource = dt

            ' Hide the ID column
            If dgvCategories.Columns.Contains("CategoryID") Then
                dgvCategories.Columns("CategoryID").Visible = False
            End If

            ' Adjust column headers and widths
            If dgvCategories.Columns.Contains("CategoryName") Then
                dgvCategories.Columns("CategoryName").HeaderText = "Category Name"
                dgvCategories.Columns("CategoryName").Width = 180
            End If
            If dgvCategories.Columns.Contains("Description") Then
                dgvCategories.Columns("Description").HeaderText = "Description"
                dgvCategories.Columns("Description").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            End If

            ClearInputs()

        Catch ex As Exception
            AppUtilities.ShowError("Error loading categories: " & ex.Message)
        End Try
    End Sub

    Private Sub ClearInputs()
        currentCategoryId = 0
        txtCategoryName.Text = ""
        txtDescription.Text = ""
        btnSave.Text = "Add Category"
        btnDelete.Enabled = False
        txtCategoryName.Focus()
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

    Private Sub dgvCategories_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvCategories.CellClick
        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = dgvCategories.Rows(e.RowIndex)
            currentCategoryId = Convert.ToInt32(row.Cells("CategoryID").Value)
            txtCategoryName.Text = If(row.Cells("CategoryName").Value IsNot DBNull.Value, row.Cells("CategoryName").Value.ToString(), "")
            txtDescription.Text = If(row.Cells("Description").Value IsNot DBNull.Value, row.Cells("Description").Value.ToString(), "")

            btnSave.Text = "Update Category"
            btnDelete.Enabled = True
        End If
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim catName As String = txtCategoryName.Text.Trim()
        Dim desc As String = txtDescription.Text.Trim()

        ' Validation: Required fields
        Dim errMsg As String = ""
        If Not ValidationHelper.IsRequired(catName, "Category Name", errMsg) Then
            AppUtilities.ShowError(errMsg, "Validation Error")
            txtCategoryName.Focus()
            Return
        End If

        ' Validation: Unique Category Name
        If Not ValidationHelper.IsCategoryNameUnique(catName, currentCategoryId, errMsg) Then
            AppUtilities.ShowError(errMsg, "Duplicate Record")
            txtCategoryName.Focus()
            Return
        End If

        Try
            If currentCategoryId = 0 Then
                ' INSERT
                Dim sql As String = "INSERT INTO Categories (CategoryName, Description) VALUES (?, ?)"
                Dim params() As OleDbParameter = {
                    DatabaseHelper.CreateParameter("@CategoryName", catName),
                    DatabaseHelper.CreateParameter("@Description", desc)
                }

                DatabaseHelper.ExecuteNonQuery(sql, params)
                DatabaseHelper.LogActivity("Category Added", "Added category: " & catName)
                AppUtilities.ShowSuccess("Category added successfully.")
            Else
                ' UPDATE
                Dim sql As String = "UPDATE Categories SET CategoryName = ?, Description = ? WHERE CategoryID = ?"
                Dim params() As OleDbParameter = {
                    DatabaseHelper.CreateParameter("@CategoryName", catName),
                    DatabaseHelper.CreateParameter("@Description", desc),
                    DatabaseHelper.CreateParameter("@CategoryID", currentCategoryId)
                }

                DatabaseHelper.ExecuteNonQuery(sql, params)
                DatabaseHelper.LogActivity("Category Updated", "Updated category: " & catName)
                AppUtilities.ShowSuccess("Category updated successfully.")
            End If

            LoadData(txtSearch.Text)
        Catch ex As Exception
            AppUtilities.ShowError("An error occurred while saving: " & ex.Message)
        End Try
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If currentCategoryId = 0 Then Return

        ' Check dependencies using ValidationHelper
        Dim errMsg As String = ""
        If Not ValidationHelper.CanDeleteCategory(currentCategoryId, errMsg) Then
            AppUtilities.ShowError(errMsg, "Deletion Denied")
            Return
        End If

        If AppUtilities.ConfirmAction("Are you sure you want to delete this category?", "Confirm Deletion") Then
            Try
                Dim deletedName As String = txtCategoryName.Text

                Dim sqlDelete As String = "DELETE FROM Categories WHERE CategoryID = ?"
                DatabaseHelper.ExecuteNonQuery(sqlDelete, {DatabaseHelper.CreateParameter("@CategoryID", currentCategoryId)})

                DatabaseHelper.LogActivity("Category Deleted", "Deleted category: " & deletedName)
                AppUtilities.ShowSuccess("Category deleted successfully.")

                LoadData(txtSearch.Text)
            Catch ex As Exception
                AppUtilities.ShowError("An error occurred during deletion: " & ex.Message)
            End Try
        End If
    End Sub

End Class
