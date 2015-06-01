param($useBlobSite=$false)

Import-Module WebAdministration
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SMO") | out-null
. "$PSScriptRoot\Config.ps1"
. "$PSScriptRoot\IIS.ps1"
. "$PSScriptRoot\SQL.ps1"

write-output "------- Installing Sitefinity --------"

EnsureDBDeleted $config.SitefinitySite.databaseServer $config.SitefinitySite.name

DeleteAllSitesWithSameBinding $config.SitefinitySite.port

write-output "Setting up Application pool..."

Remove-WebAppPool $config.SitefinitySite.name -ErrorAction continue

New-WebAppPool $config.SitefinitySite.name -Force

Set-ItemProperty IIS:\AppPools\$($config.SitefinitySite.name) managedRuntimeVersion v4.0 -Force

#Setting application pool identity to NetworkService
Set-ItemProperty IIS:\AppPools\$($config.SitefinitySite.name) processmodel.identityType -Value 2 

write-output "Deploy SitefinityWebApp to test execution machine $machineName"

if (Test-Path $config.SitefinitySite.siteDirectory){
	CleanWebsiteDirectory $config.SitefinitySite.siteDirectory 10
}  

if($useBlobSite)
{
    Write-Output "Sitefinity deploying from $($config.SitefinitySite.blobSitefinityWebApp)..."
    Copy-Item $config.SitefinitySite.blobSitefinityWebApp $config.SitefinitySite.projectDeploymentDirectory -Recurse -ErrorAction stop
} else {
    Write-Output "Sitefinity deploying from $($config.SitefinitySite.projectLocationShare)..."
    Copy-Item $config.SitefinitySite.projectLocationShare $config.SitefinitySite.projectDeploymentDirectory -Recurse -ErrorAction stop
}

if(!(Test-Path $config.SitefinitySite.configDirectory))
{
    New-Item $config.SitefinitySite.configDirectory -ItemType Directory
}
Get-ChildItem $config.SitefinitySite.configDirectory | Remove-Item -Force
Copy-Item "$PSScriptRoot\StartupConfig.config" $config.SitefinitySite.configDirectory

write-output "Sitefinity successfully deployed."

CompileProject $config.SitefinitySite.sln

$siteId = GetNextWebsiteId
write-output "Registering $($config.SitefinitySite.name) website with id $siteId in IIS."
New-WebSite -Id $siteId -Name $config.SitefinitySite.name -Port $config.SitefinitySite.port -HostHeader localhost -PhysicalPath $config.SitefinitySite.siteDirectory -ApplicationPool $config.SitefinitySite.name -Force
Start-WebSite -Name $config.SitefinitySite.name

write-output "Setting up Sitefinity..."

$installed = $false

while(!$installed){
	try{    
		$response = GetRequest $config.SitefinitySite.url
		if($response.StatusCode -eq "OK"){
			$installed = $true;
			$response
		}
	}catch [Exception]{
		Restart-WebAppPool $config.SitefinitySite.name -ErrorAction Continue
		write-output "$_.Exception.Message"
		$installed = $false
	}
}
