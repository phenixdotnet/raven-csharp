public class BuildPaths
{
	public DirectoryPath BuildDirectory { get; set; }
	public DirectoryPath NuGetDirectory { get; set; }
	public DirectoryPath PublishDirectory { get; set; }
	public FilePath ZipPublishFile { get; set; }
	
	public FilePathCollection SourceProjects { get; set; }
    public FilePathCollection TestProjects { get; set; }
	public DirectoryPathCollection DeployableProjects { get; set; }
	public FilePathCollection DockerProjects { get; set; }
	
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
		var publishDirectoryName = context.Argument<string>("publishDirectoryName", "publish");
		var zipPublishFileName = context.Argument<string>("zipPublishFileName", "Archive.zip");
		
		var outputDirectory = context.Directory(outputDirectoryName) + context.Directory(semVersion);
		
		var launchSettingsFiles = context.GetFiles("./src/**/launchSettings.json");
		DirectoryPathCollection deployableProjects = new DirectoryPathCollection(PathComparer.Default);
		foreach(var launchSettingsFile in launchSettingsFiles)
		{
			var di = new System.IO.DirectoryInfo(launchSettingsFile.GetDirectory().FullPath);
			
			var fullPath = di.Parent.FullName;
			deployableProjects.Add(fullPath);
		}
		
		var dockerFiles = context.GetFiles("./src/*/Dockerfile");
		FilePathCollection dockerProjects = new FilePathCollection(PathComparer.Default);
		foreach(var dockerFile in dockerFiles)
		{
			var di = new System.IO.DirectoryInfo(dockerFile.GetDirectory().FullPath);
			
			var fullPath = di.FullName;
			dockerProjects.Add(fullPath);
		}

        return new BuildPaths
        {
			BuildDirectory = outputDirectory + context.Directory(buildDirectoryName),
            NuGetDirectory = outputDirectory + context.Directory(nugetDirectoryName),
			PublishDirectory = outputDirectory + context.Directory(publishDirectoryName),
			ZipPublishFile = outputDirectory + context.File(zipPublishFileName),
			SourceProjects = context.GetFiles("./app/**/*.csproj"),
			TestProjects = context.GetFiles("./tests/**/*.csproj"),
			DockerProjects = dockerProjects,
			DeployableProjects = deployableProjects
        };
    }
}
