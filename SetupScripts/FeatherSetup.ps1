Import-Module WebAdministration

. "$PSScriptRoot\Config.ps1"
. "$PSScriptRoot\IIS.ps1"

function UpdateSystemConfig
{
    $systemConfig = "$($config.SitefinitySite.configDirectory)\SystemConfig.config"    
	$doc = New-Object System.Xml.XmlDocument
	$doc.Load($systemConfig)
	$modulesNode = $doc.SelectSingleNode("//systemConfig/applicationModules")            
	$featherModuleNode = $doc.CreateElement("add")
	$featherModuleNode.SetAttribute("title","Feather")
	$featherModuleNode.SetAttribute("moduleId","00000000-0000-0000-0000-000000000000")
	$featherModuleNode.SetAttribute("type","Telerik.Sitefinity.Frontend.FrontendModule, Telerik.Sitefinity.Frontend")
	$featherModuleNode.SetAttribute("startupType","OnApplicationStart")
	$featherModuleNode.SetAttribute("name","Feather")
	$modulesNode.AppendChild($featherModuleNode)
	$doc.Save($systemConfig)
}

function InstallFeather($featherBinDirectory)
{
    Write-Output "----- Installing Feather ------"

    Write-Output "Deploying feather assemblies to '$($config.SitefinitySite.binDirectory)' from '$featherBinDirectory'"
    Get-ChildItem Telerik.Sitefinity.Frontend.dll -force -recurse -path $featherBinDirectory | Copy-Item -destination $config.SitefinitySite.binDirectory
    Get-ChildItem Ninject.dll -force -recurse -path $featherBinDirectory | Copy-Item -destination $config.SitefinitySite.binDirectory
        
    Write-Output "Updating Sitefinity SystemConfig.config..."
    UpdateSystemConfig
    
    Write-Output "Restarting $($config.SitefinitySite.name) application pool..."
    Restart-WebAppPool $config.SitefinitySite.name -ErrorAction Continue
    GetRequest $config.SitefinitySite.url

    Write-Output "----- Feather successfully installed ------"
}

function InstallFeatherWidgets($featherWidgetsDirectory)
{
    Write-Output "Deploying feather widgets assembly to '$($config.SitefinitySite.binDirectory)' from '$featherBinDirectory'"
	Get-ChildItem $featherWidgetsDirectory -Include Telerik.Sitefinity.Frontend.*.dll -Recurse | Copy-Item -destination $config.SitefinitySite.binDirectory
    InstallFeather $featherBinDirectory
}

function DeleteFeatherWidgets
{
    Write-Output "Deleting feather widgets assemblies from '$($config.SitefinitySite.binDirectory)'"
    Get-ChildItem $config.SitefinitySite.binDirectory -Include Telerik.Sitefinity.Frontend.*.dll -Recurse | Remove-Item -Force
}

function InstallFeatherPackages($featherPackagesDirectory)
{
	Write-Output "----- Feather packages directory is '$featherPackagesDirectory' ------"
	Write-Output "----- Create Resource Packages directory in SitefinityWebApp ------"
	Write-Output "----- DefaultWebsiteRootDirectory is directory is '$($config.SitefinitySite.siteDirectory)' ------"
	$resourcePackagesFolder = $config.SitefinitySite.siteDirectory + "\ResourcePackages"
	Write-Output "----- ResourcePackagesFolder is directory is '$resourcePackagesFolder' ------"
	if(!(Test-Path -Path $resourcePackagesFolder )){
		New-Item -ItemType directory -Path $resourcePackagesFolder
	}
	
	Write-Output "----- Copy packages ------"
	Get-ChildItem Bootstrap -path $featherPackagesDirectory | Copy-Item -destination $resourcePackagesFolder -force -recurse
	Get-ChildItem Foundation -path $featherPackagesDirectory | Copy-Item -destination $resourcePackagesFolder -force -recurse
	Get-ChildItem SemanticUI -path $featherPackagesDirectory | Copy-Item -destination $resourcePackagesFolder -force -recurse

    Write-Output "----- Feather packages successfully installed ------"
}