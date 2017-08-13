using System.Diagnostics;
using IO = System.IO;
using System.Linq;

var target = Argument("target", "Default");
var buildConfiguration = "Release";
var solutionFile = "Sharpenter.BootstrapperLoader.sln";
var mainProject = "Sharpenter.BootstrapperLoader";
var testProject = "Sharpenter.BootstrapperLoader.Tests";

void Build(string targetFramework)
{
  var settings = new DotNetCoreMSBuildSettings()
                  .SetConfiguration(buildConfiguration)
                  .SetTargetFramework(targetFramework);

  DotNetCoreMSBuild(solutionFile, settings);
}

void DeleteIfExists(string project)
{
  var binDir = IO.Path.Combine(IO.Directory.GetCurrentDirectory(), project, "bin");
  if (DirectoryExists(binDir))
  {
    DeleteDirectory(binDir, recursive:true);
  }

  var objDir = IO.Path.Combine(IO.Directory.GetCurrentDirectory(), project, "obj");
  if (DirectoryExists(objDir))
  {
    DeleteDirectory(objDir, recursive:true);
  }
}

Task("Clean")
  .Does(() =>
  {
    DeleteIfExists(mainProject);
    DeleteIfExists(testProject);
  });
 
Task("Restore")
  .Does(() => {
    DotNetCoreRestore(solutionFile);
  });

Task("BuildNetStandard")
  .Does(() =>
  {
    Build("netstandard2.0");
  });

Task("BuildNet452")
  .Does(() =>
  {
    Build("net452");
  });

Task("Test")
  .IsDependentOn("BuildNetStandard")
  .IsDependentOn("BuildNet452")
  .Does(() =>
  {
    DotNetCoreTool("./Sharpenter.BootstrapperLoader.Tests/Sharpenter.BootstrapperLoader.Tests.csproj", "xunit", "-xml TestResult.xml");
  });

Task("Default")
  .IsDependentOn("Clean")
  .IsDependentOn("Restore")
  .IsDependentOn("Test");

RunTarget(target);
