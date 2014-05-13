Import-Module WebAdministration

Function CleanDirectory($dir){    
    Cmd /C "rmdir /S /Q $dir 2>NUL"
}

function CleanWebsiteDirectory($dir, $retryCount, $appPollName)
{
    for ($i=1; $i -le $retryCount; $i++)
    {
        if(Test-Path $dir)
        {            
		    Restart-WebAppPool $appPollName
			Start-Sleep -s 3
		    CleanDirectory $dir
            if($i -eq $retryCount)
            {
                $errorMsg = "Unable to clean "+ $dir +" directory..."
                Throw New-Object System.Exception($errorMsg)
            }
            write-output "Cleaning $dir... [ Retry$i ]"           
        } else {
            write-output "$dir cleaned successfully."
            break
        }
    }
}