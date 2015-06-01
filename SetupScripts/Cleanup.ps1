. "$PSScriptRoot\Config.ps1"

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
    CleanDirectory $config.TestAgent.tempDirectory
    Start-Process -FilePath $config.TestAgent.exe
    Remove-Item  $config.Common.aspNetTempDirectory -Force -Recurse -ErrorAction Ignore
    write-output "TestAgent process started."
}
