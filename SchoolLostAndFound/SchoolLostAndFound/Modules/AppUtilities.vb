Option Strict On
Option Explicit On
Option Infer On

Imports System.Data.OleDb

''' <summary>
''' Application-wide utility functions used across multiple forms and classes.
''' This is a VB.NET Module, meaning all members are Shared/static by default
''' and can be called without creating an instance.
''' </summary>
Public Module AppUtilities
    ' Tests can capture dialog errors without blocking unattended verification.
    Public Property ErrorPresenter As Action(Of String)

    ' ------------------------------------------------------------------
    ' STATUS CONSTANTS
    ' ------------------------------------------------------------------

    ' Lost Item Statuses
    Public Const STATUS_LOST As String = "Lost"
    Public Const STATUS_POSSIBLE_MATCH As String = "Possible Match"
    Public Const STATUS_MATCHED As String = "Matched"
    Public Const STATUS_CLAIMED As String = "Claimed"
    Public Const STATUS_LOST_CLOSED As String = "Closed"
    Public Const STATUS_LOST_CANCELLED As String = "Cancelled"

    ' Found Item Statuses
    Public Const STATUS_FOUND As String = "Found"
    Public Const STATUS_AVAILABLE As String = "Available for Claim"
    Public Const STATUS_RESERVED As String = "Reserved"
    Public Const STATUS_FOUND_CLAIMED As String = "Claimed"
    Public Const STATUS_RELEASED As String = "Released"
    Public Const STATUS_UNCLAIMED As String = "Unclaimed"
    Public Const STATUS_DISPOSED As String = "Disposed"

    ' Claim Statuses
    Public Const STATUS_PENDING As String = "Pending"
    Public Const STATUS_UNDER_VERIFICATION As String = "Under Verification"
    Public Const STATUS_APPROVED As String = "Approved"
    Public Const STATUS_REJECTED As String = "Rejected"
    Public Const STATUS_CLAIM_RELEASED As String = "Released"
    Public Const STATUS_CLAIM_CANCELLED As String = "Cancelled"

    ' User Roles
    Public Const ROLE_ADMINISTRATOR As String = "Administrator"
    Public Const ROLE_STAFF As String = "Staff"

    ' ------------------------------------------------------------------
    ' COMBOBOX POPULATION HELPERS
    ' ------------------------------------------------------------------

    ''' <summary>
    ''' Populates a ComboBox with items from a database query.
    ''' </summary>
    ''' <param name="combo">The ComboBox control to populate.</param>
    ''' <param name="sql">The SELECT query (must return at least the value and display columns).</param>
    ''' <param name="valueMember">The column name to use as the Value (typically the ID).</param>
    ''' <param name="displayMember">The column name to display in the ComboBox.</param>
    ''' <param name="addEmptyItem">If True, adds an empty "-- Select --" item at the top.</param>
    Public Sub PopulateComboBox(combo As ComboBox, sql As String, valueMember As String, displayMember As String,
                                Optional addEmptyItem As Boolean = True)
        Try
            Dim dt As DataTable = DatabaseHelper.ExecuteQuery(sql)

            If addEmptyItem Then
                Dim emptyRow As DataRow = dt.NewRow()
                emptyRow(valueMember) = 0
                emptyRow(displayMember) = "-- Select --"
                dt.Rows.InsertAt(emptyRow, 0)
            End If

            combo.DataSource = dt
            combo.ValueMember = valueMember
            combo.DisplayMember = displayMember
            combo.SelectedIndex = 0
        Catch ex As Exception
            MessageBox.Show("Unable to load " & displayMember & " list.",
                            "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
    End Sub

    ''' <summary>
    ''' Populates a ComboBox with a fixed list of string values.
    ''' </summary>
    Public Sub PopulateComboBoxWithList(combo As ComboBox, items() As String, Optional addEmptyItem As Boolean = True)
        combo.Items.Clear()
        If addEmptyItem Then
            combo.Items.Add("-- Select --")
        End If
        For Each item As String In items
            combo.Items.Add(item)
        Next
        combo.SelectedIndex = 0
    End Sub

    ''' <summary>
    ''' Populates a ComboBox with Lost Item statuses.
    ''' </summary>
    Public Sub PopulateLostItemStatuses(combo As ComboBox)
        PopulateComboBoxWithList(combo, {STATUS_LOST, STATUS_POSSIBLE_MATCH, STATUS_MATCHED, STATUS_CLAIMED, STATUS_LOST_CLOSED, STATUS_LOST_CANCELLED})
    End Sub

    ''' <summary>
    ''' Populates a ComboBox with Found Item statuses.
    ''' </summary>
    Public Sub PopulateFoundItemStatuses(combo As ComboBox)
        PopulateComboBoxWithList(combo, {STATUS_FOUND, STATUS_AVAILABLE, STATUS_RESERVED, STATUS_FOUND_CLAIMED, STATUS_RELEASED, STATUS_UNCLAIMED, STATUS_DISPOSED})
    End Sub

    ''' <summary>
    ''' Populates a ComboBox with Claim statuses.
    ''' </summary>
    Public Sub PopulateClaimStatuses(combo As ComboBox)
        PopulateComboBoxWithList(combo, {STATUS_PENDING, STATUS_UNDER_VERIFICATION, STATUS_APPROVED, STATUS_REJECTED, STATUS_CLAIM_RELEASED, STATUS_CLAIM_CANCELLED})
    End Sub

    ''' <summary>
    ''' Populates a ComboBox with User roles.
    ''' </summary>
    Public Sub PopulateRoles(combo As ComboBox)
        PopulateComboBoxWithList(combo, {ROLE_ADMINISTRATOR, ROLE_STAFF}, addEmptyItem:=False)
    End Sub

    ''' <summary>
    ''' Populates a ComboBox from the Categories table.
    ''' </summary>
    Public Sub PopulateCategories(combo As ComboBox)
        PopulateComboBox(combo, "SELECT CategoryID, CategoryName FROM Categories ORDER BY CategoryName",
                         "CategoryID", "CategoryName")
    End Sub

    ''' <summary>
    ''' Populates a ComboBox from the Students table, displaying StudentNumber - FullName.
    ''' </summary>
    Public Sub PopulateStudents(combo As ComboBox)
        PopulateComboBox(combo, "SELECT StudentID, StudentNumber & ' - ' & FullName AS StudentDisplay FROM Students ORDER BY FullName",
                         "StudentID", "StudentDisplay")
    End Sub

    ' ------------------------------------------------------------------
    ' DATAGRIDVIEW FORMATTING
    ' ------------------------------------------------------------------

    ''' <summary>
    ''' Applies consistent formatting to a DataGridView control.
    ''' Call this once in the Form_Load event for every DataGridView.
    ''' </summary>
    Public Sub FormatDataGridView(dgv As DataGridView)
        With dgv
            .ReadOnly = True
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .AllowUserToResizeRows = False
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .MultiSelect = False
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            .RowHeadersVisible = False
            .BackgroundColor = Color.White
            .BorderStyle = BorderStyle.Fixed3D
            .AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 245)
            .DefaultCellStyle.SelectionBackColor = Color.FromArgb(51, 122, 183)
            .DefaultCellStyle.SelectionForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94)
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI Semibold", 9.0F)
            .EnableHeadersVisualStyles = False
            .DefaultCellStyle.Font = New Font("Segoe UI", 9.0F)
            .RowTemplate.Height = 28
            .ColumnHeadersHeight = 32
        End With
    End Sub

    ' ------------------------------------------------------------------
    ' SAFE DATA READING HELPERS
    ' ------------------------------------------------------------------

    ''' <summary>
    ''' Safely reads a String value from a DataRow, returning "" if DBNull.
    ''' </summary>
    Public Function SafeString(row As DataRow, columnName As String) As String
        If row.IsNull(columnName) Then Return ""
        Return row(columnName).ToString()
    End Function

    ''' <summary>
    ''' Safely reads an Integer value from a DataRow, returning 0 if DBNull.
    ''' </summary>
    Public Function SafeInt(row As DataRow, columnName As String) As Integer
        If row.IsNull(columnName) Then Return 0
        Return Convert.ToInt32(row(columnName))
    End Function

    ''' <summary>
    ''' Safely reads a Date value from a DataRow, returning Date.MinValue if DBNull.
    ''' </summary>
    Public Function SafeDate(row As DataRow, columnName As String) As Date
        If row.IsNull(columnName) Then Return Date.MinValue
        Return Convert.ToDateTime(row(columnName))
    End Function

    ''' <summary>
    ''' Safely reads a Boolean value from a DataRow, returning False if DBNull.
    ''' </summary>
    Public Function SafeBool(row As DataRow, columnName As String) As Boolean
        If row.IsNull(columnName) Then Return False
        Return Convert.ToBoolean(row(columnName))
    End Function

    ' ------------------------------------------------------------------
    ' UI HELPERS
    ' ------------------------------------------------------------------

    ''' <summary>
    ''' Shows a confirmation dialog. Returns True if the user clicks Yes.
    ''' </summary>
    Public Function ConfirmAction(message As String, Optional title As String = "Confirm Action") As Boolean
        Return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes
    End Function

    ''' <summary>
    ''' Shows a success message.
    ''' </summary>
    Public Sub ShowSuccess(message As String, Optional title As String = "Success")
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    ''' <summary>
    ''' Shows an error message. Use this instead of showing raw exception details to the user.
    ''' </summary>
    Public Sub ShowError(message As String, Optional title As String = "Error")
        If ErrorPresenter IsNot Nothing Then
            ErrorPresenter.Invoke(message)
            Return
        End If
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

    ''' <summary>
    ''' Shows a warning message.
    ''' </summary>
    Public Sub ShowWarning(message As String, Optional title As String = "Warning")
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning)
    End Sub

    ''' <summary>
    ''' Formats a Date for display. Returns "" if the date is MinValue (null).
    ''' </summary>
    Public Function FormatDate(value As Date) As String
        If value = Date.MinValue Then Return ""
        Return value.ToString("MMMM dd, yyyy")
    End Function

    ''' <summary>
    ''' Formats a DateTime for display including time. Returns "" if MinValue.
    ''' </summary>
    Public Function FormatDateTime(value As Date) As String
        If value = Date.MinValue Then Return ""
        Return value.ToString("MMMM dd, yyyy hh:mm tt")
    End Function

End Module
