Import-Module WebAdministration
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SMO") | out-null
$currentPath = Split-Path $script:MyInvocation.MyCommand.Path
$variables = Join-Path $currentPath "\Variables.ps1"
. $variables
. $iisModule
. $sqlModule
. $functionsModule

write-output "------- Installing Sitefinity --------"

if (Test-Path $defaultWebsiteRootDirectory){
	CleanWebsiteDirectory $defaultWebsiteRootDirectory 10 $appPollName
}  

write-output "Sitefinity deploying from $projectLocationShare..."

Copy-Item -Path Microsoft.PowerShell.Core\FileSystem::$projectLocationShare $projectDeploymentDirectory -Recurse -ErrorAction stop

write-output "Sitefinity successfully deployed."


function CopyTestAssemblies($workingDirectory, $destinationDirectory)
{
   write-output "Start copying test assemblies from $workingDirectory to $destinationDirectory."
   Get-ChildItem *Test*.dll -recurse  -path $workingDirectory | Copy-Item -destination $destinationDirectory
}

write-output "------- Sitefinity Installed --------"
