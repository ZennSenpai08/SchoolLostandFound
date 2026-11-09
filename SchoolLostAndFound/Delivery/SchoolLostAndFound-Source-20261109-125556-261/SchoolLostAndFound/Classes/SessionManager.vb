Option Strict On
Option Explicit On
Option Infer On

''' <summary>
''' Stores information about the currently logged-in user for the duration of the application session.
''' This is a Shared (static) class — there is no need to create an instance.
''' 
''' After a successful login, the Login form sets these properties.
''' All other forms read them to determine the current user's identity and role.
''' On logout, call SessionManager.Clear() to reset all values.
''' </summary>
Public Class SessionManager

    ' ------------------------------------------------------------------
    ' CURRENT USER PROPERTIES
    ' ------------------------------------------------------------------

    ''' <summary>UserID (primary key) of the logged-in user.</summary>
    Public Shared Property CurrentUserID As Integer = 0

    ''' <summary>Username of the logged-in user.</summary>
    Public Shared Property CurrentUsername As String = ""

    ''' <summary>Full display name of the logged-in user.</summary>
    Public Shared Property CurrentFullName As String = ""

    ''' <summary>Role of the logged-in user ("Administrator" or "Staff").</summary>
    Public Shared Property CurrentRole As String = ""

    ' ------------------------------------------------------------------
    ' SESSION MANAGEMENT
    ' ------------------------------------------------------------------

    ''' <summary>
    ''' Sets all session properties after a successful login.
    ''' </summary>
    Public Shared Sub SetSession(userId As Integer, username As String, fullName As String, role As String)
        CurrentUserID = userId
        CurrentUsername = username
        CurrentFullName = fullName
        CurrentRole = role
    End Sub

    ''' <summary>
    ''' Clears all session data. Call this on logout.
    ''' </summary>
    Public Shared Sub Clear()
        CurrentUserID = 0
        CurrentUsername = ""
        CurrentFullName = ""
        CurrentRole = ""
    End Sub

    ' ------------------------------------------------------------------
    ' ROLE CHECKS
    ' ------------------------------------------------------------------

    ''' <summary>
    ''' Returns True if the current user has the Administrator role.
    ''' </summary>
    Public Shared Function IsAdministrator() As Boolean
        Return String.Equals(CurrentRole, "Administrator", StringComparison.OrdinalIgnoreCase)
    End Function

    ''' <summary>
    ''' Returns True if the current user has the Staff role.
    ''' </summary>
    Public Shared Function IsStaff() As Boolean
        Return String.Equals(CurrentRole, "Staff", StringComparison.OrdinalIgnoreCase)
    End Function

    ''' <summary>
    ''' Returns True if a user is currently logged in (UserID > 0).
    ''' </summary>
    Public Shared Function IsLoggedIn() As Boolean
        Return CurrentUserID > 0
    End Function

End Class
