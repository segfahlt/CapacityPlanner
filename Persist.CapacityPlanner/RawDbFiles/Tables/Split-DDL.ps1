$InputFile = ".\Tables.ddl.sql"
$OutputDir = "."

$content = Get-Content $InputFile -Raw
$pattern = '^--\[\[Table\]\](.*?)\|(.*)$'

$matches = [regex]::Matches($content, $pattern, 'Multiline')

for ($i = 0; $i -lt $matches.Count; $i++) {
    $tableName = $matches[$i].Groups[1].Value.Trim()
    $fileName  = $matches[$i].Groups[2].Value.Trim()
    $start     = $matches[$i].Index + $matches[$i].Length
    $end       = if ($i -lt $matches.Count - 1) { $matches[$i + 1].Index } else { $content.Length }
    $chunk     = $content.Substring($start, $end - $start).Trim()

    $outPath = Join-Path $OutputDir $fileName
    Set-Content -Path $outPath -Value $chunk -Encoding UTF8
    Write-Host "Created $fileName"
}
