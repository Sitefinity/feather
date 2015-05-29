Import-Module WebAdministration
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SMO") | out-null
$currentPath = Split-Path $script:MyInvocation.MyCommand.Path
$variables = Join-Path $currentPath "\Variables.ps1"
. $variables
. $iisModule
. $sqlModule
. $functionsModule

write-output "------- Installing Sitefinity --------"
write-output "------- DefaultWebsiteRootDirectory is '$defaultWebsiteRootDirectory' --------"
write-output "------- App poll name is '$appPollName' --------"

if (Test-Path $defaultWebsiteRootDirectory){
	CleanWebsiteDirectory $defaultWebsiteRootDirectory 10 $appPollName
}  

write-output "Sitefinity deploying from $projectLocationShare..."
write-output "ProjectDeploymentDirectory is  '$projectDeploymentDirectory'"

Copy-Item $emptyWebsite $projectDeploymentDirectory -Recurse -ErrorAction stop

write-output "Sitefinity successfully deployed."
write-output "Start building $projectPath"    

$msbuild = "C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"    
& $msbuild "C:\Tests\SitefinityWebApp\SitefinityWebApp.sln" /p:Configuration=Release /target:Rebuild /p:VisualStudioVersion=11.0 /verbosity:minimal /noconlog /nologo
	
write-output "Building completed"

function CopyTestAssemblies($workingDirectory, $destinationDirectory)
{
   write-output "Start copying test assemblies from $workingDirectory to $destinationDirectory."
   Get-ChildItem *Test*.dll -recurse  -path $workingDirectory | Copy-Item -destination $destinationDirectory
}

write-output "------- Sitefinity Installed --------"
