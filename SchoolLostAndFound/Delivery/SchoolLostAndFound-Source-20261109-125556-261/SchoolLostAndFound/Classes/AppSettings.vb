Option Strict On
Imports System.IO
Imports System.Xml.Linq
Public Class AppSettings
    Public Property SchoolName As String = "School Lost & Found"
    Public Property MatchThreshold As Integer = 40
    Public Shared Property FilePath As String = Path.Combine(Application.StartupPath,"settings.xml")
    Public Shared Function Load() As AppSettings
        Dim settings As New AppSettings()
        If File.Exists(FilePath) Then
            Dim xml=XDocument.Load(FilePath)
            settings.SchoolName=CStr(xml.Root.Element("SchoolName"))
            settings.MatchThreshold=CInt(xml.Root.Element("MatchThreshold"))
            settings.Validate()
        End If
        Return settings
    End Function
    Private Sub Validate()
        If String.IsNullOrWhiteSpace(SchoolName) OrElse SchoolName.Length>100 Then Throw New ArgumentException("School name is required (maximum 100 characters).")
        If MatchThreshold<0 OrElse MatchThreshold>100 Then Throw New ArgumentException("Match threshold must be 0-100.")
    End Sub
    Public Sub Save()
        Security.RequireAccess(True) : Validate()
        Dim temporary=FilePath & ".tmp"
        Dim document As New XDocument(New XElement("Settings",New XElement("SchoolName",SchoolName.Trim()),New XElement("MatchThreshold",MatchThreshold)))
        document.Save(temporary)
        If File.Exists(FilePath) Then
            File.Replace(temporary,FilePath,FilePath & ".previous")
        Else
            File.Move(temporary,FilePath)
        End If
        DatabaseHelper.LogActivity("Settings saved","Updated school name and matching threshold")
    End Sub
End Class
