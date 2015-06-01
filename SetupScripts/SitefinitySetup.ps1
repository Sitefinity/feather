param($useBlobSite=$false,
      $enableFeatherModule=$false,
      $rebuildSitefinity=$true)

Import-Module WebAdministration
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SMO") | out-null
. "$PSScriptRoot\Config.ps1"
. "$PSScriptRoot\IIS.ps1"
. "$PSScriptRoot\SQL.ps1"
. "$PSScriptRoot\FeatherSetup.ps1"
. "$PSScriptRoot\DeploySitefinity.ps1"

write-output "------- Installing Sitefinity --------"

EnsureDBDeleted $config.SitefinitySite.databaseServer $config.SitefinitySite.name

DeleteAllSitesWithSameBinding $config.SitefinitySite.port

write-output "Setting up Application pool..."

Remove-WebAppPool $config.SitefinitySite.name -ErrorAction continue

New-WebAppPool $config.SitefinitySite.name -Force

Set-ItemProperty IIS:\AppPools\$($config.SitefinitySite.name) managedRuntimeVersion v4.0 -Force

#Setting application pool identity to NetworkService
Set-ItemProperty IIS:\AppPools\$($config.SitefinitySite.name) processmodel.identityType -Value 2 

DeploySitefinity $useBlobSite $rebuildSitefinity

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

if($enableFeatherModule)
{
    EnableFeatherModule
}