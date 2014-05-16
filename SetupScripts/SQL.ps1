[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SMO") | out-null

function EnsureDBDeleted($databaseServer, $dbName)
{
    # Ensure data base is deleted
    write-output "Ensure data base is deleted"
    $Server = New-Object Microsoft.SqlServer.Management.Smo.Server($databaseServer)
    $DBObject = $Server.Databases[$dbName]
 
    #check database exists on server
    if ($DBObject)
    {

	  Try
	  {
	    #instead of drop we will use KillDatabase
        #KillDatabase drops all active connections before dropping the database.
	    write-output "Deleting $dbName database from SqlServer."
		$Server.KillAllProcesses($dbName)
	    $Server.KillDatabase($dbName)
	  }

	  Catch
	  {
		 write-output $_.Exception.Message
         write-output $_.Exception.InnerException
         write-output $_.Exception.Stack
         write-output $_.Exception.GetBaseException().Message	
		 write-output "username is:" $env:username
	  }
    }
}

function BackupDatabase($databaseServer, $dbName, $bakupFolder)
{
    New-Item -ItemType Directory -Path $bakupFolder -Force
    $dbBakFullPath = $bakupFolder+"\" + $dbName +".bak"
    SQLCMD.EXE -S $databaseServer -E -q "exit(BACKUP DATABASE [$dbName] TO DISK='$dbBakFullPath')"
}

function RestoreDatabaseWithMove($databaseServer, $dbName, $restoredDbName, $dbLocation)
{
    $dbBakFullPath = $dbLocation+"\"+$dbName+".bak"
    $dbDestinationFullPath = $dbLocation+"\"+$dbName+".mdf"
    $logDestinationFullPath = $dbLocation+"\"+$dbName+"_log.ldf"
    $logName = $dbName+'_log'
    SQLCMD.EXE -S $databaseServer -E -q "exit(RESTORE DATABASE [$restoredDbName] FROM DISK='$dbBakFullPath' WITH MOVE '$dbName' TO '$dbDestinationFullPath', MOVE '$logName' TO '$logDestinationFullPath')"
}