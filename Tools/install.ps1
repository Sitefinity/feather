param($installPath, $toolsPath, $package, $project)

  # Need to load MSBuild assembly if it's not loaded yet.
  Add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
  # Grab the loaded MSBuild project for the project
  $msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1
  $msbuild.Xml.Imports | Where-Object { $_.Project -eq 'Build\RazorGenerator.MsBuild\build\RazorGenerator.MsBuild.targets' -or $_.Project -eq 'Build\FeatherPrecompilation.targets'} | %{ $msbuild.Xml.RemoveChild($_) }
  
  $fileInfo = new-object -typename System.IO.FileInfo -ArgumentList $project.FullName
  $projectDirectory = $fileInfo.DirectoryName
  Write-Host "ProjectItems count is: $($project.ProjectItems.Count)"
  %{try { $project.ProjectItems.Item('App_Start').ProjectItems.Item('RazorGeneratorMvcStart.cs') } catch { $null }} | ?{$_ -ne $null} | %{ $_.Remove() }
  %{try { $project.ProjectItems.Item('App_Start') } catch { $null }} | ?{$_ -ne $null -and $_.ProjectItems.Count -eq 0} | %{ $_.Remove() }

  $project.Save()
  
  
  Get-ChildItem "$projectDirectory\App_Start\RazorGeneratorMvcStart.cs" | Remove-Item
  if ((Get-ChildItem "$projectDirectory\App_Start").Length -eq 0) {
    Remove-Item "$projectDirectory\App_Start"
  }