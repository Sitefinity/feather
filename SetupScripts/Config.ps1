. "$PSScriptRoot\Utilities.ps1"
$config = Get-Settings "$PSScriptRoot\config.json"
$machineName = gc env:computername