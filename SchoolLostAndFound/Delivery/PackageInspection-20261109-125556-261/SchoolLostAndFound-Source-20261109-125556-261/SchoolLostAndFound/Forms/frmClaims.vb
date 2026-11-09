Option Strict On
Imports System.Linq
Public Class frmClaims
    Public Property InitialLostId As Integer
    Public Property InitialFoundId As Integer
    Private selectedClaim As Integer
    Private Sub OpenForm(sender As Object,e As EventArgs) Handles MyBase.Load
        Ui.Attempt(Sub()
            Security.RequireAccess()
            Ui.SetupGrid(grid)
            Reload()
            If InitialLostId>0 Then cmbLost.SelectedValue=InitialLostId
            If InitialFoundId>0 Then cmbFound.SelectedValue=InitialFoundId
        End Sub)
    End Sub
    Private Sub Reload()
        grid.DataSource=ClaimService.List(txtSearch.Text)
        Dim position As Integer=0
        For Each column In New String() {"ClaimID","Claimant","FoundItem","Status","ClaimDate"}
            grid.Columns(column).DisplayIndex=position : position+=1
        Next
        selectedClaim=0 : cmbStatus.Items.Clear() : txtNotes.Clear()
        Ui.Lookup(cmbLost,DatabaseHelper.ExecuteQuery("SELECT LostItemID,ItemName & ' — Student #' & StudentID AS Caption FROM LostItems WHERE Status IN ('Lost','Possible Match') ORDER BY LostItemID DESC"),"LostItemID","Caption")
        Ui.Lookup(cmbFound,DatabaseHelper.ExecuteQuery("SELECT FoundItemID,ItemName & ' — ' & LocationFound AS Caption FROM FoundItems WHERE Status IN ('Found','Available for Claim','Unclaimed') ORDER BY FoundItemID DESC"),"FoundItemID","Caption")
    End Sub
    Private Sub RowClick(sender As Object,e As DataGridViewCellEventArgs) Handles grid.CellClick
        If e.RowIndex<0 Then Return
        Dim row=grid.Rows(e.RowIndex)
        selectedClaim=CInt(row.Cells("ClaimID").Value)
        txtNotes.Text=Convert.ToString(row.Cells("Remarks").Value)
        cmbStatus.Items.Clear()
        Select Case Convert.ToString(row.Cells("Status").Value)
            Case "Pending" : cmbStatus.Items.AddRange(New String() {"Under Verification","Cancelled"})
            Case "Under Verification" : cmbStatus.Items.AddRange(New String() {"Approved","Rejected","Cancelled"})
            Case "Approved" : cmbStatus.Items.AddRange(New String() {"Released","Cancelled"})
        End Select
        If cmbStatus.Items.Count>0 Then cmbStatus.SelectedIndex=0
    End Sub
    Private Sub RefreshClick(sender As Object,e As EventArgs) Handles btnRefresh.Click
        Ui.Attempt(AddressOf Reload)
    End Sub
    Private Sub CreateClick(sender As Object,e As EventArgs) Handles btnCreate.Click
        Ui.Attempt(Sub()
            Dim id=ClaimService.Create(Ui.SelectedId(cmbLost),Ui.SelectedId(cmbFound),txtEvidence.Text)
            txtEvidence.Clear() : Reload()
            AppUtilities.ShowSuccess("Created claim #" & id)
        End Sub)
    End Sub
    Private Sub TransitionClick(sender As Object,e As EventArgs) Handles btnTransition.Click
        Ui.Attempt(Sub()
            If selectedClaim=0 OrElse cmbStatus.SelectedIndex<0 Then Throw New ArgumentException("Select a claim and valid next status.")
            If Not AppUtilities.ConfirmAction("Set claim #" & selectedClaim & " to " & cmbStatus.Text & "?" & If(cmbStatus.Text="Released",Environment.NewLine & "Confirm the verified owner has received the item.","")) Then Return
            ClaimService.Transition(selectedClaim,cmbStatus.Text,txtNotes.Text)
            Reload()
            AppUtilities.ShowSuccess("Claim updated.")
        End Sub)
    End Sub
End Class
