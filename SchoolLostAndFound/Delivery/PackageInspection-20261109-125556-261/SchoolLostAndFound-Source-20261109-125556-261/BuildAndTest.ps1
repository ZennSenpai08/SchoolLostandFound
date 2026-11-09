param([switch]$Package, [string]$MSBuildPath)
$ErrorActionPreference = 'Stop'
$projectRoot = $PSScriptRoot
$sourceDb = Join-Path $projectRoot 'SchoolLostAndFound\Database\SchoolLostAndFound.mdb'
$originalHash = (Get-FileHash -LiteralPath $sourceDb -Algorithm SHA256).Hash
$stamp = Get-Date -Format 'yyyyMMdd-HHmmss-fff'
$verification = Join-Path $projectRoot "Delivery\Verification-$stamp"
New-Item -ItemType Directory -Path $verification | Out-Null

if (-not $MSBuildPath) {
    $locator = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    if (Test-Path -LiteralPath $locator) {
        $MSBuildPath = & $locator -latest -prerelease -products '*' -requires Microsoft.Component.MSBuild -find 'MSBuild\Current\Bin\MSBuild.exe' | Select-Object -First 1
    }
}
if (-not $MSBuildPath -or -not (Test-Path -LiteralPath $MSBuildPath)) { throw 'MSBuild not found. Install the VB desktop build tools or pass -MSBuildPath.' }

foreach ($configuration in @('Debug','Release')) {
    & $MSBuildPath (Join-Path $projectRoot 'SchoolLostAndFound.sln') /t:Rebuild "/p:Configuration=$configuration" /p:Platform=x86 /nologo /v:minimal 2>&1 | Tee-Object -FilePath (Join-Path $verification "build-$configuration.txt")
    if ($LASTEXITCODE -ne 0) { throw "$configuration build failed." }
}
& $MSBuildPath (Join-Path $projectRoot 'Tests\Tests.vbproj') /t:Rebuild /p:Configuration=Release /p:Platform=x86 /nologo /v:minimal 2>&1 | Tee-Object -FilePath (Join-Path $verification 'build-tests.txt')
if ($LASTEXITCODE -ne 0) { throw 'Test runner build failed.' }
& (Join-Path $projectRoot 'Tests\bin\Tests.exe') $sourceDb 2>&1 | Tee-Object -FilePath (Join-Path $verification 'test-results.txt')
if ($LASTEXITCODE -ne 0) { throw 'Integration tests failed; no package created.' }
if ((Get-FileHash -LiteralPath $sourceDb -Algorithm SHA256).Hash -ne $originalHash) { throw 'Source database hash changed.' }
$result = Get-Content -LiteralPath (Join-Path $verification 'test-results.txt')
$artifactLine = $result | Where-Object { $_ -like 'ARTIFACTS: *' } | Select-Object -Last 1
if ($artifactLine) {
    $runPath = $artifactLine.Substring(11)
    Get-ChildItem -LiteralPath $runPath -Filter '*.png' | Copy-Item -Destination $verification
    Copy-Item -LiteralPath (Join-Path $runPath 'report.csv') -Destination $verification
}
"Source database SHA-256: $originalHash" | Set-Content -LiteralPath (Join-Path $verification 'database-hash.txt')
Write-Output "Verification saved: $verification"
if (-not $Package) { return }

& (Join-Path $projectRoot 'PackageRelease.ps1') -VerificationDirectory $verification
