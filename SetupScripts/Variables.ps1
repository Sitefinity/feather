$machineName = gc env:computername
$currentPath = Split-Path $script:MyInvocation.MyCommand.Path

$doc = New-Object System.Xml.XmlDocument
$doc.Load($currentPath + "\Variables.xml") 


#Modules
$iisModule = Join-Path $currentPath "\IIS.ps1"
$sqlModule = Join-Path $currentPath "\SQL.ps1"
$functionsModule = Join-Path $currentPath "\Functions.ps1"
$sitefinitySetup = Join-Path $currentPath "\SitefinitySetup.ps1"
$analyticsSetup = Join-Path $currentPath "\EnableAnalytics.ps1"
$multilingualSetup = Join-Path $currentPath "\Setup_Multilingual.ps1"
$siteSyncSetup = Join-Path $currentPath "\Setup_SiteSync.ps1"

#Website setup properties

$defaultWebsiteUrl = $doc.SelectSingleNode("//variables/defaultWebsiteUrl").InnerText
$defaultWebsitePort = $doc.SelectSingleNode("//variables/defaultWebsitePort").InnerText
$secondWebsiteUrl = $doc.SelectSingleNode("//variables/secondWebsiteUrl").InnerText
$secondWebsitePort = $doc.SelectSingleNode("//variables/secondWebsitePort").InnerText
$siteName = $doc.SelectSingleNode("//variables/siteName").InnerText
$secondSiteName = $doc.SelectSingleNode("//variables/secondSiteName").InnerText
$projectLocationShare = $doc.SelectSingleNode("//variables/projectLocationShare").InnerText
$projectDeploymentDirectory = $doc.SelectSingleNode("//variables/projectDeploymentDirectory").InnerText
$defaultWebsiteRootDirectory = $doc.SelectSingleNode("//variables/defaultWebsiteRootDirectory").InnerText
$secondWebsiteRootDirectory = $doc.SelectSingleNode("//variables/secondWebsiteRootDirectory").InnerText
$databaseServer = $doc.SelectSingleNode("//variables/databaseServer").InnerText
$analyticsEnabled = $doc.SelectSingleNode("//variables/analyticsEnabled").InnerText
$multilingualEnabled = $doc.SelectSingleNode("//variables/multilingualEnabled").InnerText
$siteSyncEnabled = $doc.SelectSingleNode("//variables/siteSyncEnabled").InnerText

$appPollName = $siteName
$secondAppPollName = $secondSiteName
$databaseName = $siteName
$secondSiteDatabaseName = $secondSiteName
$websiteBinariesDirectory = $defaultWebsiteRootDirectory + "/bin"

#TEST AGENT
$testAgentExe = "C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\QTAgentProcessUI.exe"
$agentTempFolder = "C:\Users\sfbuild\AppData\Local\VSEQT\QTAgent"

$aspNetTempFolder = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Temporary ASP.NET Files\root"

$setupSitefinityForSiteSyncArrangement = "AutomationTests/AutomationTestsArrangemets.svc/ExecuteArrangement/SetupSitefinityForSiteSync"

$stagingServerSetupUrl = $defaultWebsiteUrl + $setupSitefinityForSiteSyncArrangement
$productionServerSetupUrl = $secondWebsiteUrl + $setupSitefinityForSiteSyncArrangement

$enableMultilingualUrl = $defaultWebsiteUrl + "AutomationTests/AutomationTestsArrangemets.svc/ExecuteArrangement/SetupSitefinityForMultilingual/EnableLocalization"

$localBackupFileCopyPath = "C:\temp\backup"