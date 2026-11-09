Option Strict On
Option Explicit On
Option Infer On

Imports System.Text.RegularExpressions

''' <summary>
''' Provides reusable input validation methods used across all forms.
''' Every method returns True if the input is valid, or False if invalid.
''' Methods that return False also set a ByRef errorMessage explaining the problem.
''' </summary>
Public Class ValidationHelper

    ' ------------------------------------------------------------------
    ' GENERAL VALIDATIONS
    ' ------------------------------------------------------------------

    ''' <summary>
    ''' Validates that a required text field is not empty or whitespace-only.
    ''' </summary>
    ''' <param name="value">The text to validate.</param>
    ''' <param name="fieldName">The human-readable name of the field (for the error message).</param>
    ''' <param name="errorMessage">Output: the error message if validation fails.</param>
    ''' <returns>True if the field has a value.</returns>
    Public Shared Function IsRequired(value As String, fieldName As String, ByRef errorMessage As String) As Boolean
        If String.IsNullOrWhiteSpace(value) Then
            errorMessage = fieldName & " is required."
            Return False
        End If
        errorMessage = ""
        Return True
    End Function

    ''' <summary>
    ''' Validates that a ComboBox has a selected item (SelectedIndex >= 0).
    ''' </summary>
    Public Shared Function IsComboBoxSelected(combo As ComboBox, fieldName As String, ByRef errorMessage As String) As Boolean
        If combo.SelectedIndex < 0 Then
            errorMessage = "Please select a " & fieldName & "."
            Return False
        End If
        errorMessage = ""
        Return True
    End Function

    ''' <summary>
    ''' Validates that a date is not in the future.
    ''' </summary>
    Public Shared Function IsDateNotFuture(value As Date, fieldName As String, ByRef errorMessage As String) As Boolean
        If value.Date > DateTime.Now.Date Then
            errorMessage = fieldName & " cannot be a future date."
            Return False
        End If
        errorMessage = ""
        Return True
    End Function

    ''' <summary>
    ''' Validates that a date falls within a reasonable range (not before year 2000).
    ''' </summary>
    Public Shared Function IsDateReasonable(value As Date, fieldName As String, ByRef errorMessage As String) As Boolean
        If value.Year < 2000 Then
            errorMessage = fieldName & " has an unreasonable date (before year 2000)."
            Return False
        End If
        errorMessage = ""
        Return True
    End Function

    ' ------------------------------------------------------------------
    ' EMAIL VALIDATION
    ' ------------------------------------------------------------------

    ''' <summary>
    ''' Validates a basic email format. This is a simple check, not RFC-compliant,
    ''' but sufficient for a school project.
    ''' Allows empty emails (email is optional for students).
    ''' </summary>
    Public Shared Function IsValidEmail(email As String, ByRef errorMessage As String) As Boolean
        If String.IsNullOrWhiteSpace(email) Then
            ' Email is optional
            errorMessage = ""
            Return True
        End If

        Dim pattern As String = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"
        If Not Regex.IsMatch(email, pattern) Then
            errorMessage = "Please enter a valid email address."
            Return False
        End If

        errorMessage = ""
        Return True
    End Function

    ' ------------------------------------------------------------------
    ' STUDENT NUMBER VALIDATION
    ' ------------------------------------------------------------------

    ''' <summary>
    ''' Validates that a student number is not empty and is not already used by another student.
    ''' </summary>
    ''' <param name="studentNumber">The student number to validate.</param>
    ''' <param name="currentStudentId">The StudentID of the record being edited (0 if adding new).</param>
    ''' <param name="errorMessage">Output: the error message if validation fails.</param>
    ''' <returns>True if valid and unique.</returns>
    Public Shared Function IsStudentNumberUnique(studentNumber As String, currentStudentId As Integer, ByRef errorMessage As String) As Boolean
        If String.IsNullOrWhiteSpace(studentNumber) Then
            errorMessage = "Student Number is required."
            Return False
        End If

        Dim sql As String
        Dim params() As System.Data.OleDb.OleDbParameter

        If currentStudentId = 0 Then
            ' Adding new student — check all records
            sql = "SELECT COUNT(*) FROM Students WHERE StudentNumber = ?"
            params = {New System.Data.OleDb.OleDbParameter("@StudentNumber", studentNumber)}
        Else
            ' Editing existing student — exclude current record
            sql = "SELECT COUNT(*) FROM Students WHERE StudentNumber = ? AND StudentID <> ?"
            params = {
                New System.Data.OleDb.OleDbParameter("@StudentNumber", studentNumber),
                New System.Data.OleDb.OleDbParameter("@StudentID", currentStudentId)
            }
        End If

        If DatabaseHelper.RecordExists(sql, params) Then
            errorMessage = "Student Number '" & studentNumber & "' is already in use."
            Return False
        End If

        errorMessage = ""
        Return True
    End Function

    ' ------------------------------------------------------------------
    ' USERNAME VALIDATION
    ' ------------------------------------------------------------------

    ''' <summary>
    ''' Validates that a username is not empty and is not already used by another user.
    ''' </summary>
    Public Shared Function IsUsernameUnique(username As String, currentUserId As Integer, ByRef errorMessage As String) As Boolean
        If String.IsNullOrWhiteSpace(username) Then
            errorMessage = "Username is required."
            Return False
        End If

        Dim sql As String
        Dim params() As System.Data.OleDb.OleDbParameter

        If currentUserId = 0 Then
            sql = "SELECT COUNT(*) FROM Users WHERE Username = ?"
            params = {New System.Data.OleDb.OleDbParameter("@Username", username)}
        Else
            sql = "SELECT COUNT(*) FROM Users WHERE Username = ? AND UserID <> ?"
            params = {
                New System.Data.OleDb.OleDbParameter("@Username", username),
                New System.Data.OleDb.OleDbParameter("@UserID", currentUserId)
            }
        End If

        If DatabaseHelper.RecordExists(sql, params) Then
            errorMessage = "Username '" & username & "' is already taken."
            Return False
        End If

        errorMessage = ""
        Return True
    End Function

    ' ------------------------------------------------------------------
    ' CATEGORY VALIDATION
    ' ------------------------------------------------------------------

    ''' <summary>
    ''' Validates that a category name is unique.
    ''' </summary>
    Public Shared Function IsCategoryNameUnique(categoryName As String, currentCategoryId As Integer, ByRef errorMessage As String) As Boolean
        If String.IsNullOrWhiteSpace(categoryName) Then
            errorMessage = "Category Name is required."
            Return False
        End If

        Dim sql As String
        Dim params() As System.Data.OleDb.OleDbParameter

        If currentCategoryId = 0 Then
            sql = "SELECT COUNT(*) FROM Categories WHERE CategoryName = ?"
            params = {New System.Data.OleDb.OleDbParameter("@CategoryName", categoryName)}
        Else
            sql = "SELECT COUNT(*) FROM Categories WHERE CategoryName = ? AND CategoryID <> ?"
            params = {
                New System.Data.OleDb.OleDbParameter("@CategoryName", categoryName),
                New System.Data.OleDb.OleDbParameter("@CategoryID", currentCategoryId)
            }
        End If

        If DatabaseHelper.RecordExists(sql, params) Then
            errorMessage = "Category '" & categoryName & "' already exists."
            Return False
        End If

        errorMessage = ""
        Return True
    End Function

    ' ------------------------------------------------------------------
    ' CLAIM WORKFLOW VALIDATION
    ' ------------------------------------------------------------------

    ''' <summary>
    ''' Validates that a claim status transition is allowed.
    ''' </summary>
    Public Shared Function IsValidClaimTransition(currentStatus As String, newStatus As String, ByRef errorMessage As String) As Boolean
        Select Case currentStatus.ToLower()
            Case "pending"
                If newStatus = "Under Verification" OrElse newStatus = "Rejected" OrElse newStatus = "Cancelled" Then
                    errorMessage = ""
                    Return True
                End If

            Case "under verification"
                If newStatus = "Approved" OrElse newStatus = "Rejected" OrElse newStatus = "Cancelled" Then
                    errorMessage = ""
                    Return True
                End If

            Case "approved"
                If newStatus = "Released" OrElse newStatus = "Cancelled" Then
                    errorMessage = ""
                    Return True
                End If

            Case "rejected", "released", "cancelled"
                errorMessage = "A claim with status '" & currentStatus & "' cannot be changed."
                Return False
        End Select

        errorMessage = "Invalid status transition from '" & currentStatus & "' to '" & newStatus & "'."
        Return False
    End Function

    ''' <summary>
    ''' Validates that a found item is still available for claiming.
    ''' Checks the FoundItems.Status column.
    ''' </summary>
    Public Shared Function IsFoundItemAvailable(foundItemId As Integer, ByRef errorMessage As String) As Boolean
        Dim sql As String = "SELECT Status FROM FoundItems WHERE FoundItemID = ?"
        Dim params() As System.Data.OleDb.OleDbParameter = {
            New System.Data.OleDb.OleDbParameter("@FoundItemID", foundItemId)
        }

        Dim dt As DataTable = DatabaseHelper.ExecuteQuery(sql, params)

        If dt.Rows.Count = 0 Then
            errorMessage = "Found item does not exist."
            Return False
        End If

        Dim status As String = dt.Rows(0)("Status").ToString()
        If status = "Available for Claim" OrElse status = "Found" OrElse status = "Reserved" Then
            errorMessage = ""
            Return True
        End If

        errorMessage = "This found item is no longer available (current status: " & status & ")."
        Return False
    End Function

    ' ------------------------------------------------------------------
    ' DELETION SAFETY CHECKS
    ' ------------------------------------------------------------------

    ''' <summary>
    ''' Checks whether a category is referenced by any LostItems or FoundItems.
    ''' Prevents unsafe deletion.
    ''' </summary>
    Public Shared Function CanDeleteCategory(categoryId As Integer, ByRef errorMessage As String) As Boolean
        Dim sql As String = "SELECT COUNT(*) FROM LostItems WHERE CategoryID = ?"
        Dim params() As System.Data.OleDb.OleDbParameter = {
            New System.Data.OleDb.OleDbParameter("@CategoryID", categoryId)
        }

        If DatabaseHelper.RecordExists(sql, params) Then
            errorMessage = "This category cannot be deleted because it is used by one or more lost items."
            Return False
        End If

        sql = "SELECT COUNT(*) FROM FoundItems WHERE CategoryID = ?"
        params = {New System.Data.OleDb.OleDbParameter("@CategoryID", categoryId)}

        If DatabaseHelper.RecordExists(sql, params) Then
            errorMessage = "This category cannot be deleted because it is used by one or more found items."
            Return False
        End If

        errorMessage = ""
        Return True
    End Function

    ''' <summary>
    ''' Checks whether a student is referenced by any LostItems or Claims.
    ''' Prevents unsafe deletion.
    ''' </summary>
    Public Shared Function CanDeleteStudent(studentId As Integer, ByRef errorMessage As String) As Boolean
        Dim sql As String = "SELECT COUNT(*) FROM LostItems WHERE StudentID = ?"
        Dim params() As System.Data.OleDb.OleDbParameter = {
            New System.Data.OleDb.OleDbParameter("@StudentID", studentId)
        }

        If DatabaseHelper.RecordExists(sql, params) Then
            errorMessage = "This student cannot be deleted because they have associated lost item records."
            Return False
        End If

        sql = "SELECT COUNT(*) FROM Claims WHERE StudentID = ?"
        params = {New System.Data.OleDb.OleDbParameter("@StudentID", studentId)}

        If DatabaseHelper.RecordExists(sql, params) Then
            errorMessage = "This student cannot be deleted because they have associated claim records."
            Return False
        End If

        errorMessage = ""
        Return True
    End Function

    ' ------------------------------------------------------------------
    ' PASSWORD VALIDATION
    ' ------------------------------------------------------------------

    ''' <summary>
    ''' Validates password minimum length.
    ''' </summary>
    Public Shared Function IsValidPassword(password As String, ByRef errorMessage As String) As Boolean
        If String.IsNullOrWhiteSpace(password) Then
            errorMessage = "Password is required."
            Return False
        End If

        If password.Length < 5 Then
            errorMessage = "Password must be at least 5 characters long."
            Return False
        End If

        errorMessage = ""
        Return True
    End Function

End Class
