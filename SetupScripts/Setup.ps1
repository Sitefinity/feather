$currentPath = Split-Path $script:MyInvocation.MyCommand.Path
$variables = Join-Path $currentPath "\Variables.ps1"

. $variables
. $sitefinitySetup

Write-Output "Closing left IE processes..."
Get-Process iexplore -ErrorAction Ignore | Stop-Process