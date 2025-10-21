param(
    [string]$Schema = 'dbo',
    [switch]$RegenerateManifest
)

$ErrorActionPreference = 'Stop'

$here = Split-Path -Parent $MyInvocation.MyCommand.Path
$tablesFile = Join-Path $here 'Tables.ddl.sql'
$manifestPath = Join-Path $here 'table.manifest'
$outSql = Join-Path $here 'Tables.build.sql'

if (-not (Test-Path $tablesFile)) {
    throw "Tables file not found: $tablesFile"
}

function Parse-TablesFile {
    param([string]$path)
    $content = Get-Content $path -Raw
    $pattern = '^--\[\[Table\]\](?<Table>[^|]+)\|(?<File>.+)$'
    $matches = [regex]::Matches($content, $pattern, 'Multiline')

    $list = @()
    foreach ($m in $matches) {
        $tableName = $m.Groups['Table'].Value.Trim()
        $fileName  = $m.Groups['File'].Value.Trim()
        $list += [pscustomobject]@{ Table = $tableName; File = $fileName }
    }
    return $list
}

$entries = $null

if ($RegenerateManifest -or -not (Test-Path $manifestPath)) {
    $entries = Parse-TablesFile -path $tablesFile
    if ($entries.Count -eq 0) { throw "No table markers found in $tablesFile" }

    # Write/overwrite manifest from source file order
    $manifestLines = $entries | ForEach-Object { "$($_.Table)|$($_.File)" }
    Set-Content -Path $manifestPath -Value $manifestLines -Encoding UTF8
    Write-Host "Wrote manifest: $manifestPath"
} else {
    # Load existing manifest (allows manual ordering overrides)
    $manifestLines = Get-Content $manifestPath | Where-Object { $_ -and $_.Trim() -notmatch '^\s*#' }
    $entries = foreach ($line in $manifestLines) {
        if ($line -match '^\s*(?<Table>[^|]+)\|(?<File>.+?)\s*$') {
            [pscustomobject]@{ Table = $Matches.Table.Trim(); File = $Matches.File.Trim() }
        }
    }
    if ($entries.Count -eq 0) { throw "Manifest is empty or invalid: $manifestPath" }
}

# Build DROP section in reverse order
$dropBuilder = New-Object System.Text.StringBuilder
$header = @"
-- Auto-generated build script
-- Generated: $(Get-Date -Format 'u')
-- Drops tables in reverse order of manifest, then recreates in manifest order
"@
[void]$dropBuilder.AppendLine($header)

# Reverse entries (drop children first)
$entryArray = @($entries)
[array]::Reverse($entryArray)
$reverse = $entryArray
foreach ($e in $reverse) {
    $t = $e.Table
    $qualified = "[$Schema].[$t]"
    [void]$dropBuilder.AppendLine("IF OBJECT_ID(N'$qualified', N'U') IS NOT NULL DROP TABLE $qualified;")
}

# Build CREATE section by concatenating individual files
$createBuilder = New-Object System.Text.StringBuilder
[void]$createBuilder.AppendLine("")
[void]$createBuilder.AppendLine("-- Create tables in manifest order")
foreach ($e in $entries) {
    $path = Join-Path $here $e.File
    if (-not (Test-Path $path)) {
        Write-Warning "Missing file referenced by manifest: $($e.File). Skipping."
        continue
    }
    [void]$createBuilder.AppendLine("")
    [void]$createBuilder.AppendLine("-- ===== File: $($e.File) | Table: $($e.Table) =====")
    $sql = Get-Content $path -Raw
    [void]$createBuilder.AppendLine($sql.Trim())
    [void]$createBuilder.AppendLine("")
    [void]$createBuilder.AppendLine("GO")
}

$finalSql = $dropBuilder.ToString() + "`r`n" + $createBuilder.ToString()
Set-Content -Path $outSql -Value $finalSql -Encoding UTF8
Write-Host "Wrote build script: $outSql"
