#load "./version.cake"

public class BuildParameters
{
    public string Target { get; private set; }
    public string Configuration { get; private set; }
    public bool IsLocalBuild { get; private set; }
    public bool IsRunningOnUnix { get; private set; }
    public bool IsRunningOnWindows { get; private set; }
    public bool IsPullRequest { get; private set; }
    public bool IsTagged { get; private set; }
    public bool IsPublishBuild { get; private set; }
    public bool IsReleaseBuild { get; private set; }
    public bool SkipGitVersion { get; private set; }
    public BuildVersion Version { get; private set; }
	public string FileForVersion { get; private set; }
    public string DockerHostConnection { get; private set; }

    public void SetBuildVersion(BuildVersion version)
    {
        Version  = version;
    }

    
    public static BuildParameters GetParameters(
        ICakeContext context,
        BuildSystem buildSystem
        )
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        var target = context.Argument("target", "Default");
		var fileForVersion = context.Argument("FileForVersion", string.Empty);
		if(string.IsNullOrEmpty(fileForVersion))
		{
			context.Information("No FileForVersion parameter set, using the first project in src directory as FileForVersion");
			var directories = System.IO.Directory.GetDirectories("app");
            var sortedDirectories = new List<string>(directories);
            sortedDirectories.Sort();
            var directory = sortedDirectories[0];
			
			fileForVersion = System.IO.Directory.GetFiles(directory, "*.csproj").First();
			
			//fileForVersion = System.IO.Path.Combine(".", directory, "*.csproj");
		}

        return new BuildParameters {
            Target = target,
            Configuration = context.Argument("configuration", "Release"),
			FileForVersion = fileForVersion,
            IsLocalBuild = buildSystem.IsLocalBuild,
            IsRunningOnUnix = context.IsRunningOnUnix(),
            IsRunningOnWindows = context.IsRunningOnWindows(),
            IsPullRequest = buildSystem.AppVeyor.Environment.PullRequest.IsPullRequest,
            IsPublishBuild = new [] {
                "ReleaseNotes",
                "Create-Release-Notes"
            }.Any(
                releaseTarget => StringComparer.OrdinalIgnoreCase.Equals(releaseTarget, target)
            ),
            IsReleaseBuild = new [] {
                "Publish",
                "Publish-NuGet"
            }.Any(
                publishTarget => StringComparer.OrdinalIgnoreCase.Equals(publishTarget, target)
            ),
            SkipGitVersion = StringComparer.OrdinalIgnoreCase.Equals("True", context.EnvironmentVariable("CAKE_SKIP_GITVERSION")),
            DockerHostConnection = context.Argument("dockerHost", string.Empty)
        };
    }
}
