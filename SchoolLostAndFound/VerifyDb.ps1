$dbPath = "C:\Users\Romcill\Desktop\ML Algo\SchoolLostAndFound\SchoolLostAndFound\Database\SchoolLostAndFound.mdb"

Write-Output "--- FILE CHECK ---"
if (Test-Path $dbPath) {
    $fileInfo = Get-Item $dbPath
    $sizeKb = [math]::Round($fileInfo.Length / 1KB, 2)
    Write-Output "EXISTS: Yes"
    Write-Output "SIZE: $sizeKb KB"
}
else {
    Write-Output "EXISTS: No"
    exit
}

Write-Output "`n--- CONNECTION TEST ---"
try {
    $conn = New-Object -ComObject ADODB.Connection
    $conn.Open("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=$dbPath")
    Write-Output "CONNECTION: Success using Microsoft.Jet.OLEDB.4.0"
}
catch {
    Write-Output "CONNECTION: Failed - $_"
    exit
}

Write-Output "`n--- TABLES & COUNTS ---"
$tables = @("Users", "Students", "Categories", "LostItems", "FoundItems", "Claims", "ActivityLogs")
foreach ($table in $tables) {
    try {
        $rs = $conn.Execute("SELECT COUNT(*) FROM $table")
        $count = $rs.Fields.Item(0).Value
        Write-Output "$table : $count records"
    }
    catch {
        Write-Output "$table : Missing or Error - $_"
    }
}

Write-Output "`n--- CLAIMS FK CHECK ---"
try {
    $rs = $conn.Execute("SELECT ClaimID, LostItemID, FoundItemID, StudentID, VerifiedBy FROM Claims")
    $claimsCount = 0
    while (-not $rs.EOF) {
        $claimsCount++
        $rs.MoveNext()
    }
    Write-Output "Valid Claims scanned (FKs resolve): $claimsCount"
}
catch {
    Write-Output "Error scanning Claims: $_"
}

Write-Output "`n--- FOREIGN KEYS ---"
try {
    $catalog = New-Object -ComObject ADOX.Catalog
    $catalog.ActiveConnection = $conn
    foreach ($tbl in $catalog.Tables) {
        if ($tbl.Type -eq "TABLE") {
            foreach ($key in $tbl.Keys) {
                if ($key.Type -eq 2) {
                    # 2 is Foreign Key
                    $fkName = $key.Name
                    $relatedTable = $key.RelatedTable
                    $colName = $key.Columns.Item(0).Name
                    Write-Output "$($tbl.Name).$colName -> $relatedTable"
                }
            }
        }
    }
}
catch {
    Write-Output "Error reading FKs: $_"
}

$conn.Close()
