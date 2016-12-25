#tool "nuget:?package=Machine.Specifications.Runner.Console"

using System.Diagnostics;
using IO = System.IO;
using System.Linq;

var target = Argument("target", "Default");
var buildConfiguration = Argument("configuration", "Release");
var platformTarget = PlatformTarget.MSIL;

Task("Build")
  .Does(() =>
{
  NuGetRestore("Sharpenter.BootstrapperLoader.sln");
  MSBuild("Sharpenter.BootstrapperLoader.sln", new MSBuildSettings {
    Configuration = buildConfiguration,
    PlatformTarget = platformTarget
  });
});

Task("Test")
  .Does(() =>
{
  var binFolders = IO.Directory.GetDirectories(IO.Directory.GetCurrentDirectory(), "*.Tests").SelectMany(d => IO.Directory.GetDirectories(d, "bin"));
  var testDlls = binFolders.SelectMany(f => IO.Directory.GetFiles(IO.Path.Combine(f, "Release", "net45"), "*.Tests.dll", SearchOption.AllDirectories)).Select(p => "\"" + p + "\"");

  var startInfo = new ProcessStartInfo(IO.Path.Combine(IO.Directory.GetCurrentDirectory(), "tools/Machine.Specifications.Runner.Console/tools/mspec-clr4.exe"))
  {
    UseShellExecute = false,
    RedirectStandardOutput = true,
    CreateNoWindow = true,
    Arguments = "--xml=\"./TestResult.xml\" " + string.Join(" ", testDlls)
  };

  var mspecProcess = Process.Start(startInfo);
  while (!mspecProcess.StandardOutput.EndOfStream) {
      string line = mspecProcess.StandardOutput.ReadLine();
      Console.WriteLine(line);
  }
  if (mspecProcess.ExitCode != 0)
  {
    throw new CakeException(string.Format("mSpec test failure... exit code: {0}", mspecProcess.ExitCode));
}
    });

    Task("Default")
    .IsDependentOn("Build")
    .IsDependentOn("Test");

    RunTarget(target);
