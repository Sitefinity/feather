. "$PSScriptRoot\Config.ps1"
. "$PSScriptRoot\SitefinitySetup.ps1"

Write-Output "Closing left IE processes..."
Get-Process iexplore -ErrorAction Ignore | Stop-Process