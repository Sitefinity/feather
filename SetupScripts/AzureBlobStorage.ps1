. "$PSScriptRoot\Config.ps1"
. "$PSScriptRoot\$($config.BlobScripts.common)"

$azureBlobDownloadLocation = $config.Common.azureBlobDownloadLocation
$azureStorageContainer = $config.Common.azureStorageContainer

function DeleteLocalBlobStorage
{
	Param(
		[string]$DownloadLocation = $azureBlobDownloadLocation
	)
	
	if (Test-Path $DownloadLocation)
	{
		Remove-Item $DownloadLocation -Recurse -Force -ErrorAction SilentlyContinue
	}  
	New-Item $DownloadLocation -type directory
}

function DownloadFromBlobStorage
{
	Param(
		[string]$DownloadLocation = $azureBlobDownloadLocation,
		[string]$AzureStorageContainer = $azureStorageContainer,
		[string]$BlobName,
		[string]$UnzipLocation
	)
	
	& $config.BlobScripts.copyFilesFromAzureStorageContainer -LocalPath $DownloadLocation -StorageContainer $AzureStorageContainer -BlobName $BlobName
	
	$LocalZip = (Get-ChildItem (Join-Path $DownloadLocation $AzureStorageContainer) | Where-Object {$_.Name -eq $BlobName} | % { $_.FullName })
	
	if ([string]::IsNullOrEmpty($UnzipLocation))
	{
		$UnzipLocation = $DownloadLocation
	}
	else
	{
		if (Test-Path $UnzipLocation)
		{
			Remove-Item $UnzipLocation -Recurse -Force -ErrorAction SilentlyContinue
		}  
		New-Item $UnzipLocation -type directory
	}

	Unzip $LocalZip $UnzipLocation
}