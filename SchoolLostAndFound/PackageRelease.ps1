param([Parameter(Mandatory=$true)][string]$VerificationDirectory)
$ErrorActionPreference = 'Stop'
$projectRoot = $PSScriptRoot
$verification = (Resolve-Path -LiteralPath $VerificationDirectory).Path
$stamp = Get-Date -Format 'yyyyMMdd-HHmmss-fff'
$sourceDb = Join-Path $projectRoot 'SchoolLostAndFound\Database\SchoolLostAndFound.mdb'
$testLog = Get-Item -LiteralPath (Join-Path $verification 'test-results.txt')
if (-not (Select-String -LiteralPath $testLog.FullName -Pattern '^RESULT: [0-9]+ checks passed$' -Quiet)) { throw 'A successful full verification run is required.' }
$expectedDb = (Get-Content -LiteralPath (Join-Path $verification 'database-hash.txt')).Replace('Source database SHA-256: ','')
if ((Get-FileHash -LiteralPath $sourceDb).Hash -ne $expectedDb) { throw 'Demo database differs from verified input.' }
$releaseExe = Join-Path $projectRoot 'SchoolLostAndFound\bin\Release\SchoolLostAndFound.exe'
$testedExe = Join-Path $projectRoot 'Tests\bin\SchoolLostAndFound.exe'
if ((Get-FileHash -LiteralPath $releaseExe).Hash -ne (Get-FileHash -LiteralPath $testedExe).Hash) { throw 'Release executable differs from tested assembly.' }
foreach ($folder in @('SchoolLostAndFound','Tests')) {
    $changed = Get-ChildItem -LiteralPath (Join-Path $projectRoot $folder) -Recurse -File | Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' -and $_.Extension -in @('.vb','.vbproj','.resx') -and $_.LastWriteTimeUtc -gt $testLog.LastWriteTimeUtc }
    if ($changed) { throw 'Source changed after verification. Run BuildAndTest.ps1 again.' }
}
$docs = @('README.md','INSTALLATION_GUIDE.md','USER_MANUAL.md','TEST_REPORT.md','FINAL_COMPLETION_REPORT.md')
foreach ($doc in $docs) {
    if (-not (Test-Path -LiteralPath (Join-Path $projectRoot $doc))) { throw "Missing delivery document: $doc" }
}
$release = Join-Path $projectRoot "Delivery\SchoolLostAndFound-Release-$stamp"
$source = Join-Path $projectRoot "Delivery\SchoolLostAndFound-Source-$stamp"
New-Item -ItemType Directory -Path (Join-Path $release 'Database'), $source | Out-Null
Copy-Item -LiteralPath (Join-Path $projectRoot 'SchoolLostAndFound\bin\Release\SchoolLostAndFound.exe') -Destination $release
Copy-Item -LiteralPath $sourceDb -Destination (Join-Path $release 'Database')
Get-ChildItem -LiteralPath (Join-Path $projectRoot 'SchoolLostAndFound\bin\Release') -Filter '*.config' | Copy-Item -Destination $release
foreach ($doc in $docs) { Copy-Item -LiteralPath (Join-Path $projectRoot $doc) -Destination $release }
Copy-Item -LiteralPath (Join-Path $verification 'test-results.txt') -Destination $release

# Copy only source, schema/demo data and instructions, excluding runtime databases and recovery/test runs.
Get-ChildItem -LiteralPath $projectRoot -File | Where-Object { $_.Extension -in @('.md','.ps1','.sln') } | Copy-Item -Destination $source
foreach ($folder in @('SchoolLostAndFound','Tests')) {
    $folderRoot = Join-Path $projectRoot $folder
    foreach ($file in Get-ChildItem -LiteralPath $folderRoot -File -Recurse) {
        $relative = $file.FullName.Substring($projectRoot.Length + 1)
        if ($relative -match '\\(bin|obj)\\' -or $file.Extension -notin @('.vb','.vbproj','.resx','.mdb','.md','.settings','.myapp')) { continue }
        $destination = Join-Path $source $relative
        New-Item -ItemType Directory -Path (Split-Path -Parent $destination) -Force | Out-Null
        Copy-Item -LiteralPath $file.FullName -Destination $destination
    }
}
foreach ($folder in @($release,$source)) {
    $manifest = foreach ($file in Get-ChildItem -LiteralPath $folder -File -Recurse | Sort-Object FullName) {
        '{0}  {1}' -f (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash, $file.FullName.Substring($folder.Length + 1)
    }
    $manifest | Set-Content -LiteralPath (Join-Path $folder 'SHA256SUMS.txt')
    Compress-Archive -LiteralPath $folder -DestinationPath ($folder + '.zip')
    Write-Output "Package created: $folder.zip"
}
