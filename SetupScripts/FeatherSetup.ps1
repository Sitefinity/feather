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
    $servicesNode = $doc.CreateElement("systemServices")    
    $addNode = $doc.CreateElement("add")
    $addNode.SetAttribute("title","Telerik.Sitefinity.Frontend")
    $addNode.SetAttribute("description","Telerik.Sitefinity.Frontend")
    $addNode.SetAttribute("moduleId","00000000-0000-0000-0000-000000000000")
    $addNode.SetAttribute("type","Telerik.Sitefinity.Frontend.FrontendService, Telerik.Sitefinity.Frontend")
    $addNode.SetAttribute("startupType","OnApplicationStart")
    $addNode.SetAttribute("name","Telerik.Sitefinity.Frontend")
    $servicesNode.AppendChild($addNode)
    $rootNode = $doc.SelectSingleNode("//systemConfig")
    $rootNode.AppendChild($servicesNode)
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

function InstallFeatherWidgets($featherWidgetsBinDirectory, $featherNavigationWidgetBinDirectory)
{
    Write-Output "Deploying feather widgets assembly to '$websiteBinariesDirectory'..."
    Get-ChildItem ContentBlock.dll -recurse  -path $featherWidgetsBinDirectory | Copy-Item -destination $websiteBinariesDirectory
	Get-ChildItem Navigation.dll -recurse  -path $featherNavigationWidgetBinDirectory | Copy-Item -destination $websiteBinariesDirectory
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