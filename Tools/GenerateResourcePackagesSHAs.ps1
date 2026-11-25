$resourcePackagesDirectory = "$PSScriptRoot\..\..\FeatherPackages"

$bootstrapVersions = @("Bootstrap5")
foreach($bootstrapVersion in $bootstrapVersions)
{
    $dictionaryJsonFile = "$PSScriptRoot\$($bootstrapVersion).json"
    $dictionary = Get-Content $dictionaryJsonFile | ConvertFrom-Json
    
    $bootstrapDirectory = "$resourcePackagesDirectory\$bootstrapVersion"
    $failedFiles = @()
    if(Test-Path $bootstrapDirectory)
    {
        Get-ChildItem $bootstrapDirectory -File -Recurse | %{
            $file = $_.FullName.Replace((Resolve-Path $bootstrapDirectory).Path, "").TrimStart("\")
            
            # Dependencies installed by npm are not part of the package and must never be added to the allowlist
            if($file -like "node_modules\*" -or $_.Length -eq 0)
            {
                return
            }

            $content = Get-Content -Path $_.FullName -Encoding UTF8
            $bytes = [System.Text.Encoding]::UTF8.GetBytes($content)
            $stream = [System.IO.MemoryStream]::new($bytes)
            $hash = (Get-FileHash -InputStream $stream -Algorithm SHA256).Hash
            
            if(!$hash)
            {
                $failedFiles += $file
                return
            }

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
    
    if($failedFiles.Count -gt 0)
    {
        Write-Error "Could not compute the hash of $($failedFiles.Count) file(s), for example '$($failedFiles[0])'. '$dictionaryJsonFile' was not updated."
        exit 1
    }

    # Remove the node_modules entries that were recorded before they were excluded
    @($dictionary.PSObject.Properties | ?{ $_.Name -like "node_modules\*" }) | %{ $dictionary.PSObject.Properties.Remove($_.Name) }

    $dictionary | ConvertTo-Json | Set-Content -Path $dictionaryJsonFile -Force
}