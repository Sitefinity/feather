$currentPath = Split-Path $script:MyInvocation.MyCommand.Path
$variables = Join-Path $currentPath "\Variables.ps1"
. $variables
. $functionsModule

write-output "Restarting TestAgent process..."
try
{    
    $qtAgentServiceProcess = Get-Process QTAgentService
    $qtAgentServiceProcess.Kill()
    $qtAgentServiceProcess.WaitForExit()
}
catch [Exception]
{
  write-output "An exception has been thrown during killing QTagent Service when waiting for its exit. Error has been skipped."
}
finally
{
    write-output "Closing left IE processes..."
    Get-Process iexplore | stop-process
    write-output "Cleaning test agent temp folders..."
    Start-Sleep -s 5
    CleanDirectory $agentTempFolder
    Start-Process -FilePath $testAgentExe
    Remove-Item  $aspNetTempFolder -Force -Recurse -ErrorAction Ignore
    write-output "TestAgent process started."
}
