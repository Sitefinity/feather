$currentPath = Split-Path $script:MyInvocation.MyCommand.Path
$variables = Join-Path $currentPath "\Variables.ps1"

. $variables
. $sitefinitySetup