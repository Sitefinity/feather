param($installPath, $toolsPath, $package, $project)

  # Need to load MSBuild assembly if it's not loaded yet.
  Add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
  # Grab the loaded MSBuild project for the project
  $msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1
  $msbuild.Xml.Imports | Where-Object { $_.Project -eq 'Build\RazorGenerator.MsBuild\build\RazorGenerator.MsBuild.targets' -or $_.Project -eq 'Build\FeatherPrecompilation.targets'} | %{ $msbuild.Xml.RemoveChild($_) }
  
  %{try { $project.ProjectItems.Item('App_Start').ProjectItems.Item('RazorGeneratorMvcStart.cs') } catch { $null }} | ?{$_ -ne $null} | %{ $_.Remove() }
  %{try { $project.ProjectItems.Item('App_Start') } catch { $null }} | ?{$_ -ne $null -and $_.ProjectItems.Count -eq 0} | %{ $_.Remove() }

  %{try { $project.ProjectItems.Item('ResourcePackages').ProjectItems.Item('Bootstrap').ProjectItems.Item('MVC').ProjectItems.Item('Views').ProjectItems.Item('Recaptcha') } catch { $null }} | ?{$_ -ne $null} | %{ $_.Remove() }
  
  $project.Save()

  $fileInfo = new-object -typename System.IO.FileInfo -ArgumentList $project.FullName
  $projectDirectory = $fileInfo.DirectoryName
  
  if (Test-Path "$projectDirectory\App_Start\RazorGeneratorMvcStart.cs") {
	  Get-ChildItem "$projectDirectory\App_Start\RazorGeneratorMvcStart.cs" | Remove-Item -Confirm
	  if ((Get-ChildItem "$projectDirectory\App_Start").Length -eq 0) {
		Remove-Item "$projectDirectory\App_Start"
	  }
  }

  # Make sure all Resource Packages have RazorGenerator directives
  $generatorDirectivesPath = "$projectDirectory\ResourcePackages\Bootstrap\razorgenerator.directives"
  if (Test-Path $generatorDirectivesPath) {
    Get-ChildItem "$projectDirectory\ResourcePackages" -Directory -Exclude "Bootstrap" | ?{ $_.GetFiles("razorgenerator.directives").Count -eq 0 } | %{ Copy-Item $generatorDirectivesPath $_.FullName }
  }

  # Prompt to remove Recaptcha template if exists since it isn't distributed with Feather anymore
  $recaptchaTemplatesPath = "$projectDirectory\ResourcePackages\Bootstrap\MVC\Views\Recaptcha"
  if (Test-Path $recaptchaTemplatesPath) {
    Remove-Item $recaptchaTemplatesPath -Recurse -Confirm
  }

  Write-Host "Appending ControllerContainerAttribute to the AssemblyInfo..."
  $assemblyInfoPath = Join-Path $projectDirectory "Properties\AssemblyInfo.cs"
  if (Test-Path $assemblyInfoPath)
  {
    if ((Get-Content $assemblyInfoPath | ? { $_ -match "\[.*assembly.*\:.*ControllerContainer.*\]" } | group).Count -eq 0)
    {
      Add-Content $assemblyInfoPath "[assembly: Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes.ControllerContainer]"
      Write-Host "Finished appending ControllerContainerAttribute to the AssemblyInfo."
    }
    Else
    {
      Write-Host "ControllerContainerAttribute is already in the AssemblyInfo. Nothing is appended."
    }
  }
  Else
  {
    Write-Host "AssemblyInfo not found."
  }
