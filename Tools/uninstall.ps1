param($installPath, $toolsPath, $package, $project)

 # This is the MSBuild targets file to add
    $targetsFile = [System.IO.Path]::Combine($toolsPath, 'RazorGenerator.MsBuild.targets')

    # Need to load MSBuild assembly if it's not loaded yet.
    Add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
    # Grab the loaded MSBuild project for the project
    $msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1
    $projPath = Split-Path $project.FileName;

    if(Test-Path (Join-Path $projPath "Build\RazorGenerator.MsBuild\build\RazorGenerator.MsBuild.targets")){
        # Add the import and save the project
        $msbuild.Xml.AddImport("Build\RazorGenerator.MsBuild\build\RazorGenerator.MsBuild.targets") | out-null
    }

    if(Test-Path (Join-Path $projPath "Build\FeatherPrecompilation.targets")){
        # Add the import and save the project
        $msbuild.Xml.AddImport("Build\FeatherPrecompilation.targets") | out-null
    }
    $project.Save()
	
	$assemblyInfoPath = Join-Path $projPath "Properties\AssemblyInfo.cs"
	if (Test-Path $assemblyInfoPath)
	{
		$assemblyInfoPathTemp = "$assemblyInfoPath.tmp"
		Get-Content $assemblyInfoPath | ? { $_ -notmatch "\[.*assembly.*\:.*ControllerContainer.*\]" } | Set-Content $assemblyInfoPathTemp -Force
		Remove-Item $assemblyInfoPath
		Rename-Item $assemblyInfoPathTemp $assemblyInfoPath
	}
