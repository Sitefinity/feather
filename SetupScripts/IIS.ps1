Import-Module WebAdministration

function GetNextWebsiteId
{
	#There is issue for the first calling of Get-ChildItem method
	try{
		(Get-ChildItem IIS:\Sites | foreach {$_.id} | sort -Descending | select -first 1) + 1
	}catch [Exception]{
		(Get-ChildItem IIS:\Sites | foreach {$_.id} | sort -Descending | select -first 1) + 1
	}
}

function GetRequest($url)
{
	$request = [System.Net.WebRequest]::Create($url)
	$request.Method = "GET"
	$request.Timeout = 360000
	$request.AuthenticationLevel = "None"
	return $request.GetResponse()
}

function PostRequest($url)
{
	$request = [System.Net.WebRequest]::Create($url)
	$request.Method = "POST"
	$request.Timeout = 360000
    $request.ContentLength = 0
	$request.AuthenticationLevel = "None"
	return $request.GetResponse()
}

function DeleteAllSitesWithSameBinding($port)
{
	#There is issue for the first calling of Get-ChildItem method
	try{
		$websites=Get-ChildItem IIS:\Sites | where-object {$_.bindings.collection.bindinginformation.Contains("*:"+$port+":")}
	}catch [Exception]{
		$websites=Get-ChildItem IIS:\Sites | where-object {$_.bindings.collection.bindinginformation.Contains("*:"+$port+":")}
	}

    foreach ($site in $websites)
    {
	    write-output "Deleting $site.Name website from IIS."
	    Remove-Website -Name $site.Name
    }
}