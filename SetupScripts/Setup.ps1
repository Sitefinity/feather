. "$PSScriptRoot\Config.ps1"
. "$PSScriptRoot\SitefinitySetup.ps1" -useBlobSite $false -enableFeatherModule $true -rebuildSitefinity $false

Write-Output "Closing left IE processes..."
Get-Process iexplore -ErrorAction Ignore | Stop-Process