Option Strict On
Imports System.Linq
Public Class ClaimService
    Private Shared Sub RequireText(value As String, label As String)
        If String.IsNullOrWhiteSpace(value) OrElse value.Length>4000 Then Throw New ArgumentException(label & " is required (maximum 4000 characters).")
    End Sub
    Public Shared Function List(Optional keyword As String="") As DataTable
        Security.RequireAccess()
        Using db As New Store()
            Return db.Table("SELECT c.*,s.FullName AS Claimant,f.ItemName AS FoundItem FROM (Claims AS c INNER JOIN Students AS s ON c.StudentID=s.StudentID) INNER JOIN FoundItems AS f ON c.FoundItemID=f.FoundItemID WHERE c.Status & ' ' & s.FullName & ' ' & f.ItemName LIKE ? ORDER BY c.ClaimID DESC", "%" & keyword.Trim() & "%")
        End Using
    End Function
    Public Shared Function Create(lostId As Integer, foundId As Integer, evidence As String) As Integer
        Security.RequireAccess() : RequireText(evidence,"Ownership evidence")
        Using db As New Store()
            Dim lost=db.Table("SELECT * FROM LostItems WHERE LostItemID=?",lostId)
            Dim found=db.Table("SELECT * FROM FoundItems WHERE FoundItemID=?",foundId)
            If lost.Rows.Count<>1 OrElse found.Rows.Count<>1 Then Throw New ArgumentException("Select existing lost and found records.")
            Dim l=lost.Rows(0), f=found.Rows(0)
            If Not {"Lost","Possible Match"}.Contains(CStr(l("Status"))) OrElse Not {"Found","Available for Claim","Unclaimed"}.Contains(CStr(f("Status"))) Then Throw New InvalidOperationException("The items are not available for a new claim.")
            If l.IsNull("DateLost") OrElse f.IsNull("DateFound") OrElse CDate(f("DateFound")).Date<CDate(l("DateLost")).Date Then Throw New ArgumentException("Found date must be on or after the lost date.")
            If l.IsNull("StudentID") Then Throw New ArgumentException("Lost report must have an owner.")
            If Convert.ToInt32(db.Scalar("SELECT COUNT(*) FROM Claims WHERE (LostItemID=? OR FoundItemID=?) AND Status IN ('Approved','Released')",lostId,foundId))>0 Then Throw New InvalidOperationException("An item is already approved or released.")
            If Convert.ToInt32(db.Scalar("SELECT COUNT(*) FROM Claims WHERE LostItemID=? AND FoundItemID=? AND Status IN ('Pending','Under Verification','Approved','Released')",lostId,foundId))>0 Then Throw New InvalidOperationException("An active claim already exists for this pair.")
            db.Execute("INSERT INTO Claims (LostItemID,FoundItemID,StudentID,ClaimDate,ClaimDescription,Status) VALUES (?,?,?,?,?,?)",lostId,foundId,CInt(l("StudentID")),DateTime.Now,evidence.Trim(),"Pending")
            Dim id=Convert.ToInt32(db.Scalar("SELECT @@IDENTITY"))
            db.Audit("Claim created","Claim #" & id) : db.Commit() : Return id
        End Using
    End Function
    Public Shared Sub Transition(id As Integer, nextStatus As String, notes As String)
        Security.RequireAccess() : RequireText(notes,"Verification / handover notes")
        Using db As New Store()
            Dim rows=db.Table("SELECT * FROM Claims WHERE ClaimID=?",id)
            If rows.Rows.Count<>1 Then Throw New ArgumentException("Select an existing claim.")
            Dim row=rows.Rows(0), old=CStr(row("Status"))
            Dim permitted=(old="Pending" AndAlso {"Under Verification","Cancelled"}.Contains(nextStatus)) OrElse
                (old="Under Verification" AndAlso {"Approved","Rejected","Cancelled"}.Contains(nextStatus)) OrElse
                (old="Approved" AndAlso {"Released","Cancelled"}.Contains(nextStatus))
            If Not permitted Then Throw New InvalidOperationException("Invalid transition: " & old & " to " & nextStatus)
            Dim lostId=CInt(row("LostItemID")), foundId=CInt(row("FoundItemID"))
            If nextStatus="Approved" Then
                Dim lost=db.Table("SELECT * FROM LostItems WHERE LostItemID=?",lostId).Rows(0)
                Dim found=db.Table("SELECT * FROM FoundItems WHERE FoundItemID=?",foundId).Rows(0)
                If CInt(lost("StudentID"))<>CInt(row("StudentID")) Then Throw New InvalidOperationException("Claimant does not own the lost report.")
                If CDate(found("DateFound")).Date<CDate(lost("DateLost")).Date Then Throw New InvalidOperationException("Found date precedes loss.")
                If Convert.ToInt32(db.Scalar("SELECT COUNT(*) FROM Claims WHERE (LostItemID=? OR FoundItemID=?) AND ClaimID<>? AND Status IN ('Approved','Released')",lostId,foundId,id))>0 Then Throw New InvalidOperationException("A competing claim is already approved or released.")
                If db.Execute("UPDATE FoundItems SET Status='Reserved' WHERE FoundItemID=? AND Status IN ('Found','Available for Claim','Unclaimed')",foundId)<>1 Then Throw New InvalidOperationException("Found item is no longer available.")
                If db.Execute("UPDATE LostItems SET Status='Matched' WHERE LostItemID=? AND Status IN ('Lost','Possible Match')",lostId)<>1 Then Throw New InvalidOperationException("Lost report is no longer available.")
            ElseIf nextStatus="Released" Then
                If db.Execute("UPDATE FoundItems SET Status='Released' WHERE FoundItemID=? AND Status='Reserved'",foundId)<>1 Then Throw New InvalidOperationException("Found item reservation has changed.")
                If db.Execute("UPDATE LostItems SET Status='Claimed' WHERE LostItemID=? AND Status='Matched'",lostId)<>1 Then Throw New InvalidOperationException("Lost report reservation has changed.")
            ElseIf old="Approved" AndAlso nextStatus="Cancelled" Then
                If db.Execute("UPDATE FoundItems SET Status='Available for Claim' WHERE FoundItemID=? AND Status='Reserved'",foundId)<>1 Then Throw New InvalidOperationException("Found item reservation has changed.")
                If db.Execute("UPDATE LostItems SET Status='Lost' WHERE LostItemID=? AND Status='Matched'",lostId)<>1 Then Throw New InvalidOperationException("Lost report reservation has changed.")
            End If
            Dim sql="UPDATE Claims SET Status=?,Remarks=?"
            Dim values As New List(Of Object) From {nextStatus,notes.Trim()}
            If nextStatus="Approved" OrElse nextStatus="Rejected" Then
                sql &= ",VerificationDetails=?,VerifiedBy=?,VerificationDate=?"
                values.Add(notes.Trim()) : values.Add(SessionManager.CurrentUserID) : values.Add(DateTime.Now)
            End If
            If nextStatus="Released" Then
                sql &= ",ReleaseDate=?" : values.Add(DateTime.Now)
            End If
            sql &= " WHERE ClaimID=? AND Status=?" : values.Add(id) : values.Add(old)
            If db.Execute(sql,values.ToArray())<>1 Then Throw New InvalidOperationException("The claim changed; refresh and try again.")
            db.Audit("Claim " & nextStatus,"Claim #" & id) : db.Commit()
        End Using
    End Sub
End Class
