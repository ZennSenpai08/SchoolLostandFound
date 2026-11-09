Option Strict On
Option Explicit On
Option Infer On

Imports System.Data.OleDb
Imports System.IO

''' <summary>
''' Centralized database helper for all OleDb operations against the SchoolLostAndFound.mdb database.
''' Every form and module in the application uses this class instead of creating its own connections.
''' 
''' IMPORTANT: This class uses Microsoft.Jet.OLEDB.4.0 for .mdb files.
''' The project must target x86 to use the 32-bit Jet provider.
''' 
''' NOTE FOR STUDENTS: In a production system, passwords should be hashed using a secure
''' algorithm (e.g., bcrypt). This project stores passwords in plain text for simplicity.
''' </summary>
Public Class DatabaseHelper

    ' ------------------------------------------------------------------
    ' CONNECTION STRING
    ' ------------------------------------------------------------------

    ''' <summary>
    ''' Builds the OleDb connection string pointing to the .mdb file
    ''' located in the Database subfolder relative to the running executable.
    ''' </summary>
    Public Shared Function GetConnectionString() As String
        Dim dbPath As String = Path.Combine(Application.StartupPath, "Database", "SchoolLostAndFound.mdb")

        If Not File.Exists(dbPath) Then
            Throw New FileNotFoundException(
                "The database file was not found. Expected location: " & dbPath &
                Environment.NewLine & Environment.NewLine &
                "Make sure SchoolLostAndFound.mdb is in the Database folder next to the executable.")
        End If

        Return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & dbPath & ";"
    End Function

    ''' <summary>
    ''' Creates and returns a new open OleDbConnection.
    ''' The caller is responsible for closing/disposing this connection.
    ''' </summary>
    Public Shared Function GetConnection() As OleDbConnection
        Dim conn As New OleDbConnection(GetConnectionString())
        conn.Open()
        Return conn
    End Function

    ' ------------------------------------------------------------------
    ' QUERY HELPERS — SELECT operations
    ' ------------------------------------------------------------------

    ''' <summary>
    ''' Executes a SELECT query and returns the results as a DataTable.
    ''' Use this for populating DataGridViews, ComboBoxes, and reports.
    ''' </summary>
    ''' <param name="sql">The SQL SELECT statement. Use ? for parameters.</param>
    ''' <param name="parameters">Optional OleDbParameter array for parameterized queries.</param>
    ''' <returns>A DataTable containing the query results.</returns>
    Public Shared Function ExecuteQuery(sql As String, Optional parameters() As OleDbParameter = Nothing) As DataTable
        Dim dt As New DataTable()

        Using conn As OleDbConnection = GetConnection()
            Using cmd As New OleDbCommand(sql, conn)
                If parameters IsNot Nothing Then
                    cmd.Parameters.AddRange(parameters)
                End If

                Using adapter As New OleDbDataAdapter(cmd)
                    adapter.Fill(dt)
                End Using
            End Using
        End Using

        Return dt
    End Function

    ''' <summary>
    ''' Executes a query that returns a single scalar value (e.g., COUNT, MAX, first column of first row).
    ''' Returns Nothing if no result is found.
    ''' </summary>
    ''' <param name="sql">The SQL statement.</param>
    ''' <param name="parameters">Optional OleDbParameter array.</param>
    ''' <returns>The scalar result as Object, or Nothing.</returns>
    Public Shared Function ExecuteScalar(sql As String, Optional parameters() As OleDbParameter = Nothing) As Object
        Using conn As OleDbConnection = GetConnection()
            Using cmd As New OleDbCommand(sql, conn)
                If parameters IsNot Nothing Then
                    cmd.Parameters.AddRange(parameters)
                End If

                Dim result As Object = cmd.ExecuteScalar()
                If result Is DBNull.Value Then
                    Return Nothing
                End If
                Return result
            End Using
        End Using
    End Function

    ' ------------------------------------------------------------------
    ' COMMAND HELPERS — INSERT, UPDATE, DELETE operations
    ' ------------------------------------------------------------------

    ''' <summary>
    ''' Executes a non-query SQL command (INSERT, UPDATE, DELETE).
    ''' Returns the number of rows affected.
    ''' </summary>
    ''' <param name="sql">The SQL statement. Use ? for parameters.</param>
    ''' <param name="parameters">Optional OleDbParameter array.</param>
    ''' <returns>Number of rows affected.</returns>
    Public Shared Function ExecuteNonQuery(sql As String, Optional parameters() As OleDbParameter = Nothing) As Integer
        Using conn As OleDbConnection = GetConnection()
            Using cmd As New OleDbCommand(sql, conn)
                If parameters IsNot Nothing Then
                    cmd.Parameters.AddRange(parameters)
                End If

                Return cmd.ExecuteNonQuery()
            End Using
        End Using
    End Function

    ''' <summary>
    ''' Executes an INSERT statement and returns the AutoNumber ID of the newly created record.
    ''' This uses @@IDENTITY immediately after the INSERT, which is safe for Jet/Access.
    ''' </summary>
    ''' <param name="sql">The INSERT SQL statement. Use ? for parameters.</param>
    ''' <param name="parameters">Optional OleDbParameter array.</param>
    ''' <returns>The auto-generated ID of the inserted record.</returns>
    Public Shared Function ExecuteInsertAndGetId(sql As String, Optional parameters() As OleDbParameter = Nothing) As Integer
        Using conn As OleDbConnection = GetConnection()
            Using cmd As New OleDbCommand(sql, conn)
                If parameters IsNot Nothing Then
                    cmd.Parameters.AddRange(parameters)
                End If

                cmd.ExecuteNonQuery()

                ' Retrieve the auto-generated ID
                cmd.CommandText = "SELECT @@IDENTITY"
                cmd.Parameters.Clear()

                Dim result As Object = cmd.ExecuteScalar()
                Return Convert.ToInt32(result)
            End Using
        End Using
    End Function

    ' ------------------------------------------------------------------
    ' PARAMETER CREATION HELPERS
    ' ------------------------------------------------------------------

    ''' <summary>
    ''' Creates an OleDbParameter with proper null handling.
    ''' If the value is Nothing or an empty string, DBNull.Value is used.
    ''' </summary>
    Public Shared Function CreateParameter(name As String, value As Object) As OleDbParameter
        Dim param As New OleDbParameter(name, If(value Is Nothing OrElse (TypeOf value Is String AndAlso String.IsNullOrEmpty(CStr(value))),
                                                  DBNull.Value,
                                                  value))
        Return param
    End Function

    ''' <summary>
    ''' Creates an OleDbParameter for integer values with null support.
    ''' Use this for nullable foreign keys (e.g., Claims.VerifiedBy).
    ''' </summary>
    Public Shared Function CreateIntParameter(name As String, value As Integer, Optional allowZeroAsNull As Boolean = True) As OleDbParameter
        If allowZeroAsNull AndAlso value = 0 Then
            Return New OleDbParameter(name, DBNull.Value)
        End If
        Return New OleDbParameter(name, value)
    End Function

    ''' <summary>
    ''' Creates an OleDbParameter for Date values with null support.
    ''' If the date equals Date.MinValue, DBNull.Value is stored.
    ''' </summary>
    Public Shared Function CreateDateParameter(name As String, value As Date) As OleDbParameter
        If value = Date.MinValue Then
            Return New OleDbParameter(name, DBNull.Value)
        End If
        Return New OleDbParameter(name, value)
    End Function

    ' ------------------------------------------------------------------
    ' ACTIVITY LOG HELPER
    ' ------------------------------------------------------------------

    ''' <summary>
    ''' Records an action in the ActivityLogs table.
    ''' Uses the currently logged-in user from SessionManager.
    ''' </summary>
    ''' <param name="action">Short action name (e.g., "Login", "Added lost item").</param>
    ''' <param name="description">Detailed description of what happened.</param>
    Public Shared Sub LogActivity(action As String, description As String)
        Try
            Dim sql As String = "INSERT INTO ActivityLogs (UserID, [Action], Description, ActionDate) VALUES (?, ?, ?, ?)"
            Dim params() As OleDbParameter = {
                New OleDbParameter("@UserID", SessionManager.CurrentUserID),
                New OleDbParameter("@Action", action),
                New OleDbParameter("@Description", description),
                New OleDbParameter("@ActionDate", DateTime.Now)
            }
            ExecuteNonQuery(sql, params)
        Catch ex As Exception
            ' Activity logging should never crash the application.
            ' In debug mode, write to the debug output.
            System.Diagnostics.Debug.WriteLine("Failed to log activity: " & ex.Message)
        End Try
    End Sub

    ' ------------------------------------------------------------------
    ' EXISTENCE CHECK HELPERS
    ' ------------------------------------------------------------------

    ''' <summary>
    ''' Checks if a record exists in a table matching the given condition.
    ''' </summary>
    ''' <param name="sql">A SELECT COUNT(*) query.</param>
    ''' <param name="parameters">Parameters for the query.</param>
    ''' <returns>True if COUNT > 0.</returns>
    Public Shared Function RecordExists(sql As String, Optional parameters() As OleDbParameter = Nothing) As Boolean
        Dim result As Object = ExecuteScalar(sql, parameters)
        If result Is Nothing Then Return False
        Return Convert.ToInt32(result) > 0
    End Function

End Class
