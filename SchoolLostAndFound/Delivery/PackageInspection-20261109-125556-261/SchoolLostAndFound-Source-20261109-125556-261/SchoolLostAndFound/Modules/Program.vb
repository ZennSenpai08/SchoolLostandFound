Option Strict On
Imports System.Threading
Public Module Program
    <STAThread>
    Public Sub Main()
        Dim owns As Boolean
        Using instance As New Mutex(True, "Local\SchoolLostAndFound.Desktop", owns)
            If Not owns Then
                MessageBox.Show("School Lost & Found is already running.")
                Return
            End If
            Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)
            Try
                Do
                    Security.InitializeDatabase()
                    SessionManager.Clear()
                    Using login As New frmLogin()
                        If login.ShowDialog() <> DialogResult.OK Then Exit Do
                    End Using
                    Using dashboard As New frmDashboard()
                        dashboard.ShowDialog()
                        If Not dashboard.ReturnToLogin Then Exit Do
                    End Using
                Loop
            Catch ex As Exception
                MessageBox.Show("The application could not start: " & ex.Message, "School Lost & Found", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                SessionManager.Clear()
                instance.ReleaseMutex()
            End Try
        End Using
    End Sub
End Module
