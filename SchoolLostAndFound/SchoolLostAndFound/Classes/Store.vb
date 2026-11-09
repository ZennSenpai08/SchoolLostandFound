Option Strict On
Imports System.Data.OleDb
Imports System.IO

' One connection/transaction for each business operation; positional typed parameters.
Public Class Store
    Implements IDisposable
    Private ReadOnly connection As OleDbConnection
    Private ReadOnly transaction As OleDbTransaction
    Public Sub New()
        connection = DatabaseHelper.GetConnection()
        transaction = connection.BeginTransaction()
    End Sub
    Private Function Command(sql As String, values As Object()) As OleDbCommand
        Dim cmd As New OleDbCommand(sql, connection, transaction)
        For Each value As Object In values
            Dim p As New OleDbParameter()
            If TypeOf value Is DateTime Then
                p.OleDbType = OleDbType.Date
            ElseIf TypeOf value Is Integer Then
                p.OleDbType = OleDbType.Integer
            ElseIf TypeOf value Is Boolean Then
                p.OleDbType = OleDbType.Boolean
            Else
                p.OleDbType = OleDbType.VarWChar
                p.Size = Math.Max(1, If(value Is Nothing, 1, value.ToString().Length))
            End If
            p.Value = If(value, DBNull.Value)
            cmd.Parameters.Add(p)
        Next
        Return cmd
    End Function
    Public Function Table(sql As String, ParamArray values As Object()) As DataTable
        Using cmd = Command(sql, values), da As New OleDbDataAdapter(cmd)
            Dim result As New DataTable()
            da.Fill(result)
            Return result
        End Using
    End Function
    Public Function Scalar(sql As String, ParamArray values As Object()) As Object
        Using cmd = Command(sql, values)
            Return cmd.ExecuteScalar()
        End Using
    End Function
    Public Function Execute(sql As String, ParamArray values As Object()) As Integer
        Using cmd = Command(sql, values)
            Return cmd.ExecuteNonQuery()
        End Using
    End Function
    Public Sub Audit(action As String, description As String)
        Execute("INSERT INTO ActivityLogs (UserID,[Action],Description,ActionDate) VALUES (?,?,?,?)", SessionManager.CurrentUserID, action, description, DateTime.Now)
    End Sub
    Public Sub Commit()
        transaction.Commit()
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        transaction.Dispose()
        connection.Dispose()
    End Sub
End Class
