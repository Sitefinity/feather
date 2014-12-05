Import-Module WebAdministration

$currentPath = Split-Path $script:MyInvocation.MyCommand.Path
$variables = Join-Path $currentPath "\Variables.ps1"
. $variables
. $iisModule

$websiteConfigDir = $defaultWebsiteRootDirectory + "\App_Data\Sitefinity\Configuration"

function UpdateSystemConfig
{
    $systemConfig = $websiteConfigDir+"\SystemConfig.config"    
	$doc = New-Object System.Xml.XmlDocument
	$doc.Load($systemConfig)
	$modulesNode = $doc.SelectSingleNode("//systemConfig/applicationModules")            
	$featherModuleNode = $doc.CreateElement("add")
	$featherModuleNode.SetAttribute("title","Feather")
	$featherModuleNode.SetAttribute("moduleId","00000000-0000-0000-0000-000000000000")
	$featherModuleNode.SetAttribute("type","Telerik.Sitefinity.Frontend.FrontendModule, Telerik.Sitefinity.Frontend")
	$featherModuleNode.SetAttribute("startupType","OnApplicationStart")
	$featherModuleNode.SetAttribute("name","Telerik.Sitefinity.Frontend")
	$modulesNode.AppendChild($featherModuleNode)
	$doc.Save($systemConfig)
}

function InstallFeather($featherBinDirectory)
{
    Write-Output "----- Installing Feather ------"

    Write-Output "Deploying feather assemblies to '$websiteBinariesDirectory'..."
    Get-ChildItem Telerik.Sitefinity.Frontend.dll -recurse  -path $featherBinDirectory | Copy-Item -destination $websiteBinariesDirectory
    Get-ChildItem Ninject.dll -recurse  -path $featherBinDirectory | Copy-Item -destination $websiteBinariesDirectory
        
    Write-Output "Updating Sitefinity SystemConfig.config..."
    UpdateSystemConfig
    
    Write-Output "Restarting $appPollName application pool..."
    Restart-WebAppPool $appPollName -ErrorAction Continue
    GetRequest $defaultWebsiteUrl

    Write-Output "----- Feather successfully installed ------"
}

function InstallFeatherWidgets($featherWidgetsBinDirectory)
{
    Write-Output "Deploying feather widgets assembly to '$websiteBinariesDirectory'..."
    Get-ChildItem Telerik.Sitefinity.Frontend.ContentBlock.dll -recurse  -path $featherWidgetsDirectory | Copy-Item -destination $websiteBinariesDirectory
	Get-ChildItem Telerik.Sitefinity.Frontend.Navigation.dll -recurse  -path $featherWidgetsDirectory | Copy-Item -destination $websiteBinariesDirectory
	Get-ChildItem Telerik.Sitefinity.Frontend.News.dll -recurse  -path $featherWidgetsDirectory | Copy-Item -destination $websiteBinariesDirectory
	Get-ChildItem Telerik.Sitefinity.Frontend.SocialShare.dll -recurse  -path $featherWidgetsDirectory | Copy-Item -destination $websiteBinariesDirectory
	Get-ChildItem Telerik.Sitefinity.Frontend.DynamicContent.dll -recurse  -path $featherWidgetsDirectory | Copy-Item -destination $websiteBinariesDirectory

    InstallFeather $featherBinDirectory
}

function InstallFeatherPackages($featherPackagesDirectory)
{
	Write-Output "----- Create Resource Packages directory in SitefinityWebApp ------"
	
	$resourcePackagesFolder = $defaultWebsiteRootDirectory + "\ResourcePackages"
	if(!(Test-Path -Path $resourcePackagesFolder )){
		New-Item -ItemType directory -Path $resourcePackagesFolder
	}
	
	Write-Output "----- Copy packages ------"
	Get-ChildItem Bootstrap -path $featherPackagesDirectory | Copy-Item -destination $resourcePackagesFolder -recurse
	Get-ChildItem Foundation -path $featherPackagesDirectory | Copy-Item -destination $resourcePackagesFolder -recurse
	Get-ChildItem SemanticUI -path $featherPackagesDirectory | Copy-Item -destination $resourcePackagesFolder -recurse

    Write-Output "----- Feather packages successfully installed ------"
}