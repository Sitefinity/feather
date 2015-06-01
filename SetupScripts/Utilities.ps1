$msbuild = "C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
$msbuildSchema = "http://schemas.microsoft.com/developer/msbuild/2003"

function Ensure-File([string]$path) {
    if ([System.IO.File]::Exists($path) -eq $false) {
        throw (New-Object 'System.IO.FileNotFoundException' -ArgumentList("${path} does not exist."))
    }
}

function LogMessage($message)
{
    $currentTime = Get-Date -Format "HH:mm:ss"
    Write-Warning "[[$currentTime]]::LOG:: $message"
}

function Get-Settings([string]$settingsPath) {
    Ensure-File -path $settingsPath

    $json = Get-Content $settingsPath -Raw
    $instance = $json | ConvertFrom-Json    

    return $instance
}

function CompileProject($projectPath)
{
	LogMessage "Start building $projectPath"    
    
	& $msbuild "$projectPath" /p:Configuration=Release
	
	LogMessage "Building completed"  
}

function RemoveSystemFolder($folderPath)
{
	if (Test-Path $folderPath){
		Remove-Item $folderPath -Recurse -Force 
	}
}

function CleanDirectory($dir){    
    Cmd /C "rmdir /S /Q $dir 2>NUL"
}

function CleanWebsiteDirectory($dir, $retryCount)
{
    for ($i=1; $i -le $retryCount; $i++)
    {
        if(Test-Path $dir)
        {        
		    IISRESET
			Start-Sleep -s 3
		    CleanDirectory $dir
            if($i -eq $retryCount)
            {
                $errorMsg = "Unable to clean "+ $dir +" directory..."
                Throw New-Object System.Exception($errorMsg)
            }
            LogMessage "Cleaning $dir... [ Retry$i ]"           
        } else {
            LogMessage "$dir cleaned successfully."
            break
        }
    }
}

function CopyTestAssemblies($workingDirectory, $destinationDirectory)
{
   write-output "Start copying test assemblies from $workingDirectory to $destinationDirectory."
   Get-ChildItem *Test*.dll -recurse -Path $workingDirectory | Copy-Item -Destination $destinationDirectory
}