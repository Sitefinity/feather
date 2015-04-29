set logsDir="C:\Logs"
for /f %%a in ('%windir%\sysnative\WindowsPowerShell\v1.0\powershell.exe -Command "Get-Date -format yyyy_MM_dd__HH_mm_ss"') do set datetime=%%a
if not exist %logsDir% mkdir %logsDir%
"%windir%\sysnative\WindowsPowerShell\v1.0\powershell.exe" -Command "$setupProcess = Start-Process PowerShell -windowstyle hidden -ArgumentList '& ""%~dp0Deployment\Setup.ps1"" 2>&1 | out-file C:\Logs\%datetime%_Setup.log' -Verb RunAs -PassThru;$setupProcess | Wait-Process"