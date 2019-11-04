# How to build a template on OSX

 - Download the latest Nuget CommandLine tool.
 - Ensure mono is installed
 - Ensure there is no bin/obj directory or .DS_Store files in existance before you pack
 - Run `mono nuget.exe pack carter-template.nuspec`
 - Run `dotnet new -i /path/to/foo.nupkg`
 - Publish to nuget
