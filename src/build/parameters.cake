#load "./version.cake"

public class BuildParameters
{
    public string Target { get; private set; }
    public string Configuration { get; private set; }
    public bool IsLocalBuild { get; private set; }
    public bool IsRunningOnUnix { get; private set; }
    public bool IsRunningOnWindows { get; private set; }
    public bool SkipGitVersion { get; private set; }
    public BuildVersion Version { get; private set; }
	public string FileForVersion { get; private set; }

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
			
			fileForVersion = System.IO.Path.Combine(".", directory, "project.json");
		}

        return new BuildParameters {
            Target = target,
            Configuration = context.Argument("configuration", "Release"),
			FileForVersion = fileForVersion,
            IsLocalBuild = buildSystem.IsLocalBuild,
            IsRunningOnUnix = context.IsRunningOnUnix(),
            IsRunningOnWindows = context.IsRunningOnWindows(),
            SkipGitVersion = StringComparer.OrdinalIgnoreCase.Equals("True", context.EnvironmentVariable("CAKE_SKIP_GITVERSION"))
        };
    }
}
