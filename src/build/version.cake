#addin "Cake.Json"

public class BuildVersion
{
    public string SemVersionSuffix { get; private set; }
    public string FullSemVersion { get; private set; }

    public static BuildVersion CalculatingSemanticVersion(ICakeContext context, BuildParameters parameters)
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }
		
        string semVersion = null;
        string fullSemVersion = null;

		string version = ReadVersionInProjectJsonFile(context, parameters.FileForVersion);

		var branchName = System.Environment.GetEnvironmentVariable("BUILD_SOURCEBRANCHNAME");
		if(string.IsNullOrEmpty(branchName))
		{
			branchName = RunGitCommand(context, "rev-parse --abbrev-ref HEAD");
			branchName = branchName.Trim();	
		}

		if("master".Equals(branchName, StringComparison.InvariantCultureIgnoreCase))
		{
			semVersion = string.Empty;
			fullSemVersion = version;
		}
		else
		{
			var latestTag = RunGitCommand(context, "describe --abbrev=0 --tags");
			latestTag = latestTag.Trim();

			var countCommitSinceLastTag = RunGitCommand(context, "rev-list --count " + latestTag + "..HEAD");
			countCommitSinceLastTag = countCommitSinceLastTag.Trim();
			
			semVersion = branchName + "-" + string.Format("{0:0000}", int.Parse(countCommitSinceLastTag));
			fullSemVersion = version + "-" + semVersion;
		}

		context.Information("Calculated Semantic Version prefix: '{0}'", semVersion);
        context.Information("Calculated Full Semantic Version: '{0}'", fullSemVersion);

        return new BuildVersion
        {
            SemVersionSuffix = semVersion,
            FullSemVersion = fullSemVersion
        };
    }

	private static string ReadVersionInProjectJsonFile(ICakeContext context, string file)
	{
		context.Information("Using json file " + file + " as version source");

		var json = context.DeserializeJsonFromFile<InternalVersion>(file);
		var version = json.Version;
		
		version = version.Replace("-*", string.Empty);

		return version;
	}

	private static string RunGitCommand(ICakeContext context, string command)
	{
		System.Collections.Generic.IEnumerable<string> redirectedOutput;
		var processSettings = new ProcessSettings() {
			Arguments = command,
			RedirectStandardOutput = true
		};

		context.StartProcess("git", processSettings, out redirectedOutput);

		return redirectedOutput.First();
	}

	private class InternalVersion
	{
		public string Version {get;set;}
	}
}