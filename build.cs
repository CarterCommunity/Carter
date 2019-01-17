namespace CarterBuild
{
    using System;
    using System.IO;
    using static System.Console;
    using static Bullseye.Targets;
    using static SimpleExec.Command;

    internal class Program
    {
        internal static void Main(string[] args)
        {
            var BUILD_VERSION = "3.11.0" + Environment.GetEnvironmentVariable("build");

            Target("default", () =>
            {
                WriteLine($"Running Build : {BUILD_VERSION}");
                
                Run("git","clean -fdx", Directory.GetCurrentDirectory());

                Run("dotnet", "build Carter.sln");
            });

            Target("tests", new[] { "default" }, ForEach("test/Carter.Tests", "test/Carter.Samples.Tests"), project => Run("dotnet", $"test --no-build {project}"));

            Target("pack", new[] { "tests" }, () => Run("dotnet", $"pack --no-build src/Carter.csproj /p:Version={BUILD_VERSION}"));

            Target("publish", new[] { "pack" }, () => { });

            Target("changebuildversion", new[] { "publish" }, () => WriteLine("VERSION NUMBER : UPDATE THE MINOR BUILD VERSION FOR CI BUILDS NOW PUBLISH HAS TAKEN PLACE"));

            RunTargetsAndExit(args);
        }
    }
}
