#load "./build/parameters.cake"
#load "./build/paths.cake"

#tool "nuget:?package=GitVersion.CommandLine&version=3.6.2"
#tool "nuget:?package=OpenCover&version=4.6.519"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////


//////////////////////////////////////////////////////////////////////
// Build method
//////////////////////////////////////////////////////////////////////

BuildParameters parameters = BuildParameters.GetParameters(Context, BuildSystem);
BuildPaths paths = null;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup((context) =>
{
    parameters.SetBuildVersion(
        BuildVersion.CalculatingSemanticVersion(
            context: Context,
            parameters: parameters
        )
    );

    Information("Building version {0} ({1}, {2}).",
        parameters.Version.FullSemVersion,
        parameters.Configuration,
        parameters.Target);
	
	paths = BuildPaths.GetPaths(Context, configuration, parameters.Version.SemVersionSuffix);
});

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////
Task("Clean")
    .Does(() =>
{
    CleanDirectory(paths.BuildDirectory);
	CleanDirectory(paths.NuGetDirectory);
	
	CreateDirectory(paths.BuildDirectory);
	CreateDirectory(paths.NuGetDirectory);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
	.IsDependentOn("Clean")
    .IsDependentOn("Pack")
	.IsDependentOn("Run-Unit-Test")
	.IsDependentOn("Copy-NuGet-Packages");

Task("PublishNuGet")
	.IsDependentOn("Push-NuGet-Package");
	
Task("Push-NuGet-Package")
	.IsDependentOn("Copy-NuGet-Packages")
	.Does(() => 
{
	var packages = GetFiles(".\\nuget\\*.nupkg");
	NuGetPush(packages, new NuGetPushSettings {
		Source = "nuget.org",
		ApiKey = "temp"
	});
});

Task("Copy-NuGet-Packages")
	.IsDependentOn("Pack")
	.Does(() => 
{
	string nugetDirectory = "./nuget";
	
	var nugetPackages = GetFiles(paths.NuGetDirectory + "/*.nupkg")
      - GetFiles(paths.NuGetDirectory + "/*.symbols.nupkg");
	
	if(DirectoryExists(nugetDirectory))
		CleanDirectory(nugetDirectory);
	CreateDirectory(nugetDirectory);
	
	CopyFiles(nugetPackages, nugetDirectory);
});
	
Task("Run-Unit-Test")
	.IsDependentOn("Restore-NuGet-Packages")
	.Does(() =>
{
	
	foreach(var project in paths.TestProjects)
	{
		DotNetCoreTestSettings testSettings = new DotNetCoreTestSettings
		{
			Configuration = parameters.Configuration,
			Verbose = false
		};
		
		if(!IsRunningOnWindows())
		{
			testSettings.Framework = "netcoreapp1.0";
		}

		// Set xunit results file
		var testResultFile = new FilePath("testresults_" + project.GetDirectory().GetDirectoryName() + ".xml");
		testSettings.ArgumentCustomization = args => args.Append("-xml").Append(paths.BuildDirectory.CombineWithFilePath(testResultFile).FullPath);
		
		// Define code coverage result file
		var coverageResultFile = paths.BuildDirectory.CombineWithFilePath(new FilePath("coverage_" + project.GetDirectory().GetDirectoryName() + ".xml"));
		
		if(IsRunningOnWindows()) 
		{
			OpenCover(tool => {
					tool.DotNetCoreTest(project.FullPath, testSettings);
				},
				coverageResultFile,
				new OpenCoverSettings() {
					ReturnTargetCodeOffset = 0
				}
				.WithFilter("+[*]*")
				.WithFilter("-[Elasticsearch.Net]*")
				.WithFilter("-[xunit*]*")
			);
		}
		else
		{
			DotNetCoreTest(project.FullPath, testSettings);
		}
	}
});

Task("Pack")
	.IsDependentOn("Restore-NuGet-Packages")
	.Does(() =>
{
	DotNetCorePackSettings packSettings = new DotNetCorePackSettings
	{
		VersionSuffix = parameters.Version.SemVersionSuffix,
		OutputDirectory = paths.NuGetDirectory,
		Configuration = parameters.Configuration,
		Verbose = false
	};
	
	foreach(var packProject in paths.SourceProjects)
	{
		DotNetCorePack(packProject.FullPath, packSettings);
	}
	
});

Task("Restore-NuGet-Packages")
	.IsDependentOn("Clean")
	.Does(() => 
{
	var restoreSettings = new DotNetCoreRestoreSettings
	{
		ConfigFile = ".\\NuGet.config"
	};
	
	// Restore
	DotNetCoreRestore(restoreSettings);
});

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
