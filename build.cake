var buildArtifacts      = Directory("./artifacts/packages");
var target          = Argument("target", "Build");
var versionSuffix          = Argument("suffix", "beta12");
var isLocalBuild        = !AppVeyor.IsRunningOnAppVeyor;
Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
	DotNetCoreBuild("Botwin.sln");
});
Task("Clean")
    .Does(() =>
{
    CleanDirectories(new DirectoryPath[] { buildArtifacts });
});
Task("Restore")
    .Does(() =>
{
  DotNetCoreRestore("Botwin.sln");
});

Task("Pack")
    .IsDependentOn("Build")
    .Does(() =>
{
    var settings = new DotNetCorePackSettings
    {
        Configuration = "Release",
        OutputDirectory = buildArtifacts,
        NoBuild = true,
        VersionSuffix = versionSuffix
    };
    DotNetCorePack("./src/Botwin.csproj", settings);
});

RunTarget(target);