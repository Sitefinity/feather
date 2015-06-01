. "$PSScriptRoot\Config.ps1"

function DeploySitefinity($useBlobSite, $rebuildSitefinity = $true)
{
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

    if($rebuildSitefinity)
    {
        CompileProject $config.SitefinitySite.sln
        #there is some issue with the local assemlies copy
        CompileProject $config.SitefinitySite.sln
    }

    write-output "Sitefinity successfully deployed."
}