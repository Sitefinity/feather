$resourcePackagesDirectory = "$PSScriptRoot\..\..\FeatherPackages"

$bootstrapVersions = @("Bootstrap5")
foreach($bootstrapVersion in $bootstrapVersions)
{
    $dictionaryJsonFile = "$PSScriptRoot\$($bootstrapVersion).json"
    $dictionary = Get-Content $dictionaryJsonFile | ConvertFrom-Json
    
    $bootstrapDirectory = "$resourcePackagesDirectory\$bootstrapVersion"
    if(Test-Path $bootstrapDirectory)
    {
        Get-ChildItem $bootstrapDirectory -File -Recurse | %{
            $file = $_.FullName.Replace((Resolve-Path $bootstrapDirectory).Path, "").TrimStart("\")
            
            $content = Get-Content -Path $_.FullName -Encoding UTF8
            $bytes = [System.Text.Encoding]::UTF8.GetBytes($content)
            $stream = [System.IO.MemoryStream]::new($bytes)
            $hash = (Get-FileHash -InputStream $stream -Algorithm SHA256).Hash
            
            if($dictionary."$file" -eq $null)
            {
                $dictionary | Add-Member -Name $file -Type NoteProperty -Value @($hash)
                Write-Host "[NEW] Key '$file' with hash '$hash' added."
            }
            elseif($dictionary."$file" -and !$dictionary."$file".Contains($hash))
            {
                $dictionary."$file" += $hash
                Write-Host "[UPDATE] Key '$file' with hash '$hash' added."
            }
        }
    }
    
    $dictionary | ConvertTo-Json | Set-Content -Path $dictionaryJsonFile -Force
}