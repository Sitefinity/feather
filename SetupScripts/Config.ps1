. "$PSScriptRoot\Utilities.ps1"
$config = Get-Settings "$PSScriptRoot\config.json"

# global variables
$machineName = gc env:computername
$websiteBinariesDirectory = $config.SitefinitySite.binDirectory
$azureBlobDownloadLocation = $config.Common.azureBlobDownloadLocation
$azureStorageContainer = $config.Common.azureStorageContainer
$testRunnerBlobName = "Telerik.WebTestRunner.zip"
$codeCoverageBlobName = "CodeCoverage.zip"