var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var artifactsDirectory = Directory("./Artifacts");

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(artifactsDirectory);
    });

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        DotNetCoreRestore();
    });

 Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        var projects = GetFiles("./**/*.xproj");
        foreach(var project in projects)
        {
            DotNetCoreBuild(
                project.GetDirectory().FullPath,
                new DotNetCoreBuildSettings() 
                {
                    Configuration = configuration
                });
        }
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        var projects = GetFiles("./**/*.Test.xproj");
        foreach(var project in projects)
        {
            DotNetCoreTest(
                project.GetDirectory().FullPath, 
                new DotNetCoreTestSettings()
                {
                    ArgumentCustomization = args => args
                        .Append("-xml")
                        .Append(artifactsDirectory.Path.CombineWithFilePath(project.GetFilenameWithoutExtension()).FullPath + ".xml"),
                    Configuration = configuration,
                    NoBuild = true
                });
        }
    });

Task("Pack")
    .IsDependentOn("Test")
    .Does(() =>
    {
        var buildNumber = AppVeyor.IsRunningOnAppVeyor ? Convert.ToInt32(AppVeyor.Environment.Build.Number) : 1;
        var revision = buildNumber.ToString("D4");

        var project = GetFiles("./**/Serilog.Exceptions.xproj").First();
        DotNetCorePack(
            project.GetDirectory().FullPath,
            new DotNetCorePackSettings() 
            {
                Configuration = configuration,
                OutputDirectory = artifactsDirectory,
                VersionSuffix = revision
            });
    });

Task("Upload-AppVeyor-Artifacts")
    .IsDependentOn("Pack")
    .WithCriteria(() => AppVeyor.IsRunningOnAppVeyor)
    .Does(() =>
    {
        foreach(var file in GetFiles(artifactsDirectory.Path + "/*"))
        {
            AppVeyor.UploadArtifact(file);
        }
    });

Task("Default")
    .IsDependentOn("Upload-AppVeyor-Artifacts");

RunTarget(target);