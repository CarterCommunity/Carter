# How to build a template on OSX

 - Download the latest Nuget CommandLine tool.
 - Ensure mono is installed
 - Run `mono nuget.exe pack botwin-template.nuspec`
 - Run `dotnet new -i /path/to/foo.nupkg`
 - Publish to nuget
