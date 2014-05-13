Import-Module WebAdministration

$currentPath = Split-Path $script:MyInvocation.MyCommand.Path
$variables = Join-Path $currentPath "\Variables.ps1"
. $variables
. $functionsModule

write-output "Restarting TestAgent process..."
try{    
    $testAgentProcess = Get-Process QTAgentProcessUI
    $testAgentProcess.Kill()
    $testAgentProcess.WaitForExit()
}catch [Exception]{
}finally{
    write-output "Cleaning test agent temp folders..."
	Start-Sleep -s 5
    CleanDirectory $agentTempFolder
    Start-Process -FilePath $testAgentExe
	Remove-Item  $aspNetTempFolder -Force -Recurse -ErrorAction Continue
    write-output "TestAgent process started."
}

write-output "Restarting WebApp pools..."
Restart-WebAppPool $appPollName -ErrorAction Continue
Restart-WebAppPool $secondAppPollName -ErrorAction Continue