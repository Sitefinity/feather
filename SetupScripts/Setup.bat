set logsDir="C:\Tests\Logs"
for /f %%a in ('%windir%\sysnative\WindowsPowerShell\v1.0\powershell.exe -Command "Get-Date -format yyyy_MM_dd__HH_mm_ss"') do set datetime=%%a
if not exist %logsDir% mkdir %logsDir%
"%windir%\sysnative\WindowsPowerShell\v1.0\powershell.exe" -executionpolicy unrestricted -file Setup.ps1 > %logsDir%\%datetime%_Setup.log 2>%logsDir%\%datetime%_Setup_Error.log