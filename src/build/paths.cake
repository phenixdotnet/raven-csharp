public class BuildPaths
{
	public DirectoryPath BuildDirectory { get; set; }
	public DirectoryPath NuGetDirectory { get; set; }
	
	public FilePathCollection SourceProjects { get; set; }
    public FilePathCollection TestProjects { get; set; }
	
    public static BuildPaths GetPaths(
        ICakeContext context,
        string configuration,
        string semVersion
        )
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        if (string.IsNullOrEmpty(configuration))
        {
            throw new ArgumentNullException("configuration");
        }

        if (string.IsNullOrEmpty(semVersion))
        {
            semVersion = "release";
        }

		var outputDirectoryName = context.Argument<string>("outputDirectoryName", "artifacts");
		var buildDirectoryName = context.Argument<string>("buildDirectoryName", "build");
		var nugetDirectoryName = context.Argument<string>("nugetDirectoryName", "nuget");
		
		var outputDirectory = context.Directory(outputDirectoryName) + context.Directory(semVersion);
		
        return new BuildPaths
        {
			BuildDirectory = outputDirectory + context.Directory(buildDirectoryName),
            NuGetDirectory = outputDirectory + context.Directory(nugetDirectoryName),
			SourceProjects = context.GetFiles("./app/**/project.json"),
			TestProjects = context.GetFiles("./tests/**/project.json")
        };
    }
}
