Option Strict On
Public Module Ui
    Public Sub Attempt(action As Action)
        Try
            action()
        Catch ex As Exception
            AppUtilities.ShowError(ex.Message)
        End Try
    End Sub
    Public Sub SetupGrid(grid As DataGridView)
        AppUtilities.FormatDataGridView(grid)
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells
        grid.RowHeadersVisible = False
    End Sub
    Public Sub Lookup(combo As ComboBox, table As DataTable, key As String, display As String)
        combo.DropDownStyle = ComboBoxStyle.DropDownList
        combo.ValueMember = key : combo.DisplayMember = display : combo.DataSource = table
        combo.SelectedIndex = -1
    End Sub
    Public Function SelectedId(combo As ComboBox) As Integer
        If combo.SelectedIndex < 0 OrElse combo.SelectedValue Is Nothing OrElse TypeOf combo.SelectedValue Is DataRowView Then Throw New ArgumentException("Select a record from the list.")
        Return Convert.ToInt32(combo.SelectedValue)
    End Function
End Module
