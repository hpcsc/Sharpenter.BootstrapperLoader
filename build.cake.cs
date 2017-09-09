using System.Diagnostics;
using IO = System.IO;
using System.Linq;

var target = Argument("target", "Default");
var buildConfiguration = "Release";
var solutionFile = "Sharpenter.BootstrapperLoader.sln";
var mainProject = "./Sharpenter.BootstrapperLoader";
var testProject = "./Sharpenter.BootstrapperLoader.Tests";

Task("Clean")
  .Does(() =>
  {
    var projects = new List<string> { mainProject, testProject };
    projects.ForEach(project => {
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
    });
  });
 
Task("Restore")
  .IsDependentOn("Clean")
  .Does(() => {
    DotNetCoreRestore(solutionFile);
  });

Task("Build")
  .IsDependentOn("Restore")
  .Does(() =>
  {
    var settings = new DotNetCoreBuildSettings
    {
        Configuration = buildConfiguration
    };

    DotNetCoreBuild(mainProject, settings);
  });

Task("Test")
  .Does(() =>
  {
    DotNetCoreTool("./Sharpenter.BootstrapperLoader.Tests/Sharpenter.BootstrapperLoader.Tests.csproj", "xunit", "-xml TestResult.xml");
  });

Task("Default")
  .IsDependentOn("Build");

RunTarget(target);
