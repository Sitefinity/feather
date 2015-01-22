Import-Module WebAdministration
Import-Module pscx
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SMO") | out-null
$currentPath = Split-Path $script:MyInvocation.MyCommand.Path
$variables = Join-Path $currentPath "\Variables.ps1"
. $variables
. $iisModule
. $sqlModule
. $functionsModule

Write-Output "Restart website on build machine"
Restart-WebAppPool $appPollName -ErrorAction Continue
Start-Sleep 5

Function DeployFeatherWebsite
{
	Param(
		[string]$Computer,
		[string]$AppPollName,
		[string]$DefaultWebsiteUrl,
		[String]$WebsiteSource
	)
	
	$DeploymentDirectory = "\\$Computer\Tests\SitefinityWebApp\"
	
	Write-Output "Set $Computer to trusted hosts"
	Set-Item -Path WSMan:\localhost\Client\TrustedHosts -Value $Computer -Force
	
	Write-Output "Restarting website on computer $Computer"
	Invoke-Command -ComputerName $Computer -ScriptBlock {Param([string]$AppPollName); Import-Module WebAdministration; Restart-WebAppPool $AppPollName -ErrorAction Continue; Start-Sleep 5} -ArgumentList $appPollName
	
	Write-Output "Delete website from target computer"
	Remove-Item (Join-Path $DeploymentDirectory *) -Recurse -Force
	
	Write-Output "Copy new website"
	Copy-Item (Join-Path $WebsiteSource *) $DeploymentDirectory -Recurse -Force
	
	Write-Output "Request website"
	Invoke-WebRequest "http://$Computer"
}

$WebsiteSource = "\Tests\SitefinityWebApp\"
$Agent1 = "100.75.152.108"
$Agent2 = "100.75.144.104"
Write-Output "===== Start deployment to agent $Agent1 ====="
DeployFeatherWebsite -Computer $Agent1 -AppPollName $appPollName -DefaultWebsiteUrl $defaultWebsiteUrl -WebsiteSource $WebsiteSource
Write-Output "===== Start deployment to agent $Agent2 ====="
DeployFeatherWebsite -Computer $Agent2 -AppPollName $appPollName -DefaultWebsiteUrl $defaultWebsiteUrl -WebsiteSource $WebsiteSource
