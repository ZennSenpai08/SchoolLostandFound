Option Strict On
Imports System.Linq
Public Class frmRecords
    Private entity As String = "LostItems"
    Private currentId As Integer
    Private ReadOnly inputs As New Dictionary(Of String, Control)
    Public Sub Configure(recordType As String)
        RecordService.Fields(recordType)
        entity = recordType
        Text = If(entity = "Users", "User management", If(entity = "LostItems", "Lost items", "Found items"))
    End Sub
    Private Sub OpenForm(sender As Object, e As EventArgs) Handles MyBase.Load
        Ui.Attempt(Sub()
            Security.RequireAccess(entity = "Users")
            Ui.SetupGrid(grid)
            btnDelete.Visible = entity <> "Users"
            BuildEditor()
            Reload()
        End Sub)
    End Sub
    Private Sub BuildEditor()
        editor.Controls.Clear() : inputs.Clear()
        For Each f In RecordService.Fields(entity)
            Dim label As New Label With {.Text = f.Label & If(f.Required, " *", ""), .AutoSize = True, .Margin = New Padding(3, 8, 3, 3)}
            editor.Controls.Add(label)
            Dim control As Control
            Select Case f.Kind
                Case "date"
                    control = New DateTimePicker With {.Format = DateTimePickerFormat.Short, .MaxDate = Date.Today, .MinDate = New Date(2000, 1, 1)}
                Case "boolean"
                    control = New CheckBox With {.Text = "Account enabled", .Checked = True}
                Case "choice", "student", "category"
                    Dim combo As New ComboBox With {.DropDownStyle = ComboBoxStyle.DropDownList}
                    If f.Kind = "choice" Then
                        combo.Items.AddRange(f.Choices) : combo.SelectedIndex = 0
                    ElseIf f.Kind = "student" Then
                        Ui.Lookup(combo, DatabaseHelper.ExecuteQuery("SELECT StudentID, StudentNumber & ' - ' & FullName AS Caption FROM Students ORDER BY FullName"), "StudentID", "Caption")
                    Else
                        Ui.Lookup(combo, DatabaseHelper.ExecuteQuery("SELECT CategoryID,CategoryName FROM Categories ORDER BY CategoryName"), "CategoryID", "CategoryName")
                    End If
                    control = combo
                Case Else
                    control = New TextBox With {.MaxLength = f.Limit, .UseSystemPasswordChar = f.Kind = "password", .Multiline = f.Kind = "memo", .Height = If(f.Kind = "memo", 60, 25), .ScrollBars = If(f.Kind = "memo", ScrollBars.Vertical, ScrollBars.None)}
            End Select
            control.Name = f.Name : control.Width = 370
            inputs.Add(f.Name, control) : editor.Controls.Add(control)
        Next
    End Sub
    Private Sub Reload()
        grid.DataSource = RecordService.List(entity, txtSearch.Text)
        If entity <> "Users" Then
            grid.Columns("ItemName").DisplayIndex=0
            grid.Columns("Status").DisplayIndex=1
        End If
        ClearEditor()
    End Sub
    Private Sub ClearEditor()
        currentId = 0
        For Each f In RecordService.Fields(entity)
            Dim c = inputs(f.Name)
            If TypeOf c Is TextBox Then c.Text = ""
            If TypeOf c Is CheckBox Then DirectCast(c, CheckBox).Checked = True
            If TypeOf c Is DateTimePicker Then DirectCast(c, DateTimePicker).Value = Date.Today
            If TypeOf c Is ComboBox Then DirectCast(c, ComboBox).SelectedIndex = If(f.Kind = "choice", 0, -1)
        Next
    End Sub
    Private Sub SelectRow(sender As Object, e As DataGridViewCellEventArgs) Handles grid.CellClick
        If e.RowIndex < 0 Then Return
        Ui.Attempt(Sub()
            Dim row = DirectCast(grid.Rows(e.RowIndex).DataBoundItem, DataRowView).Row
            currentId = CInt(row(RecordService.Key(entity)))
            For Each f In RecordService.Fields(entity)
                Dim c = inputs(f.Name)
                If f.Kind = "password" Then
                    c.Text = "" : Continue For
                End If
                If TypeOf c Is TextBox Then c.Text = Convert.ToString(row(f.Name))
                If TypeOf c Is CheckBox Then DirectCast(c, CheckBox).Checked = Not row.IsNull(f.Name) AndAlso CBool(row(f.Name))
                If TypeOf c Is DateTimePicker Then DirectCast(c, DateTimePicker).Value = If(row.IsNull(f.Name), Date.Today, CDate(row(f.Name)))
                If TypeOf c Is ComboBox Then
                    If f.Kind = "choice" Then
                        DirectCast(c, ComboBox).SelectedItem = Convert.ToString(row(f.Name))
                    Else
                        DirectCast(c, ComboBox).SelectedValue = If(row.IsNull(f.Name), 0, row(f.Name))
                    End If
                End If
            Next
        End Sub)
    End Sub
    Private Sub SearchClick(sender As Object, e As EventArgs) Handles btnSearch.Click
        Ui.Attempt(AddressOf Reload)
    End Sub
    Private Sub NewClick(sender As Object, e As EventArgs) Handles btnNew.Click
        ClearEditor()
    End Sub
    Private Sub SaveClick(sender As Object, e As EventArgs) Handles btnSave.Click
        Ui.Attempt(Sub()
            Dim values As New Dictionary(Of String, Object)
            For Each f In RecordService.Fields(entity)
                Dim c = inputs(f.Name), value As Object = c.Text
                If TypeOf c Is DateTimePicker Then value = DirectCast(c, DateTimePicker).Value.Date
                If TypeOf c Is CheckBox Then value = DirectCast(c, CheckBox).Checked
                If TypeOf c Is ComboBox AndAlso f.Kind <> "choice" Then value = Ui.SelectedId(DirectCast(c, ComboBox))
                values(f.Name) = value
            Next
            RecordService.Save(entity, currentId, values)
            Reload()
            AppUtilities.ShowSuccess("Record saved.")
        End Sub)
    End Sub
    Private Sub DeleteClick(sender As Object, e As EventArgs) Handles btnDelete.Click
        If currentId = 0 OrElse Not AppUtilities.ConfirmAction("Delete the selected record?") Then Return
        Ui.Attempt(Sub()
            RecordService.Delete(entity, currentId)
            Reload()
        End Sub)
    End Sub
End Class
