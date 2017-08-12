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

void Test(string framework)
{
  var settings = new DotNetCoreTestSettings
  {
      Configuration = buildConfiguration,
      Framework = framework
  };
  DotNetCoreTest("./Sharpenter.BootstrapperLoader.Tests/Sharpenter.BootstrapperLoader.Tests.csproj", settings);
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

Task("BuildNet45")
  .Does(() =>
  {
    Build("net45");
  });

Task("TestNetStandard")
  .IsDependentOn("BuildNetStandard")
  .Does(() =>
  {
    Test("netstandard2.0");
  });

Task("TestNet45")
  .IsDependentOn("BuildNet45")
  .Does(() =>
  {
    Test("net45");
  });

Task("Default")
  .IsDependentOn("Clean")
  .IsDependentOn("Restore")
  .IsDependentOn("TestNet45")
  .IsDependentOn("TestNetStandard");

RunTarget(target);
