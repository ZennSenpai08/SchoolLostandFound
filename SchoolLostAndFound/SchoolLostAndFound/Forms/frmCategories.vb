Option Strict On
Option Explicit On
Option Infer On

Imports System.Data.OleDb

Public Class frmCategories

    Private currentCategoryId As Integer = 0

    Private Sub frmCategories_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not SessionManager.IsLoggedIn() Then
            Close() : Return
        End If
        txtCategoryName.MaxLength = 50
        txtDescription.MaxLength = 4000
        AppUtilities.FormatDataGridView(dgvCategories)
        LoadData()
    End Sub

    Private Sub LoadData(Optional searchKeyword As String = "")
        Try
            Dim dt = RecordService.List("Categories", searchKeyword)

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
        Ui.Attempt(Sub()
            Dim values As New Dictionary(Of String, Object) From {{"CategoryName", txtCategoryName.Text}, {"Description", txtDescription.Text}}
            RecordService.Save("Categories", currentCategoryId, values)
            LoadData(txtSearch.Text)
            AppUtilities.ShowSuccess("Category saved successfully.")
        End Sub)
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If currentCategoryId = 0 OrElse Not AppUtilities.ConfirmAction("Delete the selected category?") Then Return
        Ui.Attempt(Sub()
            RecordService.Delete("Categories", currentCategoryId)
            LoadData(txtSearch.Text)
            AppUtilities.ShowSuccess("Category deleted successfully.")
        End Sub)
    End Sub
End Class
