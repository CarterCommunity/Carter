namespace Carter.Tests.Analyzers;

using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

public sealed class CSharpPreviewAnalyzerTest<TAnalyzer> : CSharpAnalyzerTest<TAnalyzer, XUnitVerifier>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    public CSharpPreviewAnalyzerTest(string code)
    {
        var carterPackage = new PackageIdentity("Carter", "8.1.0");
        var aspNetPackage = new PackageIdentity("Microsoft.AspNetCore.App.Ref", "8.0.0");
        var bclPackage = new PackageIdentity("Microsoft.NETCore.App.Ref", "8.0.0");
        ReferenceAssemblies = new ReferenceAssemblies("net8.0", bclPackage, Path.Combine("ref", "net8.0")).AddPackages([carterPackage, aspNetPackage]);
        TestCode = code;
        
    }
    protected override ParseOptions CreateParseOptions()
        => new CSharpParseOptions(LanguageVersion.Preview, DocumentationMode.Diagnose);
}
