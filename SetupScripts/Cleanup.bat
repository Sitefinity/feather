set logsDir="C:\Tests\Logs"
for /f %%a in ('%windir%\sysnative\WindowsPowerShell\v1.0\powershell.exe -Command "Get-Date -format yyyy_MM_dd__HH_mm_ss"') do set datetime=%%a
"%windir%\sysnative\WindowsPowerShell\v1.0\powershell.exe" -executionpolicy unrestricted -file Cleanup.ps1 > %logsDir%\%datetime%_Cleanup.log 2>%logsDir%\%datetime%_Cleanup_Error.log