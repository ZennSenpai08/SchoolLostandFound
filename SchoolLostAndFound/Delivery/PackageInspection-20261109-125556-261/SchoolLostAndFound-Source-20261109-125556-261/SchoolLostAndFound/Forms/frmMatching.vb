Option Strict On
Imports System.Linq
Public Class frmMatching
    Private Sub OpenForm(sender As Object,e As EventArgs) Handles MyBase.Load
        Ui.Attempt(Sub()
            Security.RequireAccess()
            Ui.SetupGrid(grid)
            threshold.Value = AppSettings.Load().MatchThreshold
            ReloadLost()
        End Sub)
    End Sub
    Private Sub ReloadLost()
        Ui.Lookup(cmbLost,DatabaseHelper.ExecuteQuery("SELECT LostItemID,ItemName & ' — Student #' & StudentID AS Caption FROM LostItems WHERE Status IN ('Lost','Possible Match') ORDER BY LostItemID DESC"),"LostItemID","Caption")
    End Sub
    Private Sub FindClick(sender As Object,e As EventArgs) Handles btnMatch.Click
        Ui.Attempt(Sub()
            grid.DataSource=MatchingService.Candidates(Ui.SelectedId(cmbLost),CDbl(threshold.Value))
            Dim position As Integer=0
            For Each column In New String() {"ItemName","MatchScore","ScoreDetails","FoundItemID"}
                grid.Columns(column).DisplayIndex=position : position+=1
            Next
        End Sub)
    End Sub
    Private Sub ClaimClick(sender As Object,e As EventArgs) Handles btnClaim.Click
        Ui.Attempt(Sub()
            If grid.CurrentRow Is Nothing Then Throw New ArgumentException("Select a candidate.")
            Using form As New frmClaims()
                form.InitialLostId=Ui.SelectedId(cmbLost)
                form.InitialFoundId=CInt(grid.CurrentRow.Cells("FoundItemID").Value)
                form.ShowDialog(Me)
            End Using
            grid.DataSource=Nothing : ReloadLost()
        End Sub)
    End Sub
End Class
