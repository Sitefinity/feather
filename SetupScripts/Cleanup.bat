set agentTempFolder="C:\Users\sfbuild\AppData\Local\VSEQT\QTAgent"
set aspNetTempFolder="C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Temporary ASP.NET Files\root"

"%windir%\sysnative\WindowsPowerShell\v1.0\powershell.exe" -Command "Start-Process PowerShell -windowstyle hidden -ArgumentList '& Start-Sleep 30;$qtAgentServiceProcess = Get-Process QTAgentService;$qtAgentServiceProcess.Kill();$qtAgentServiceProcess.WaitForExit();Cmd /C \"rmdir /S /Q %agentTempFolder%\";Cmd /C \"rmdir /S /Q %aspNetTempFolder%\"'"