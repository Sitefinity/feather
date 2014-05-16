Import-Module WebAdministration
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SMO") | out-null
$currentPath = Split-Path $script:MyInvocation.MyCommand.Path
$variables = Join-Path $currentPath "\Variables.ps1"
. $variables
. $iisModule
. $sqlModule
. $functionsModule

write-output "------- Installing Sitefinity --------"

EnsureDBDeleted $databaseServer $databaseName

DeleteAllSitesWithSameBinding $defaultWebsitePort

write-output "Setting up Application pool..."

Remove-WebAppPool $appPollName -ErrorAction continue

New-WebAppPool $appPollName -Force

Set-ItemProperty IIS:\AppPools\$appPollName managedRuntimeVersion v4.0 -Force

#Setting application pool identity to NetworkService
Set-ItemProperty IIS:\AppPools\$appPollName processmodel.identityType -Value 2 

write-output "Deploy SitefinityWebApp to test execution machine $machineName"

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

function InstallSitefinity()
{
	$siteId = GetNextWebsiteId
	write-output "Registering $siteName website with id $siteId in IIS."
	New-WebSite -Id $siteId -Name $siteName -Port $defaultWebsitePort -HostHeader localhost -PhysicalPath $defaultWebsiteRootDirectory -ApplicationPool $appPollName -Force
	Start-WebSite -Name $siteName

	write-output "Setting up Sitefinity..."

	$installed = $false

	while(!$installed){
		try{    
			$response = GetRequest $defaultWebsiteUrl
			if($response.StatusCode -eq "OK"){
				$installed = $true;
				$response
			}
		}catch [Exception]{
			Restart-WebAppPool $appPollName -ErrorAction Continue
			write-output "$_.Exception.Message"
			$installed = $false
		}
	}

	write-output "----- Sitefinity successfully installed ------"
}

