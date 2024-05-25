namespace Carter.Tests.Analyzers;

using System.IO;
using System.Threading.Tasks;
using Carter.Analyzers;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit;

public sealed class CarterModuleShouldNotHaveDependenciesTests
{
    [Theory]
    [InlineData("class")]
    [InlineData("struct")]
    [InlineData("record")]
    [InlineData("record struct")]
    public Task TypeWithDependency_Diagnostic(string type)
    {
        var code = $$"""
                     using Carter;
                     using Microsoft.AspNetCore.Routing;

                     {{type}} MyCarterModule : ICarterModule
                     {
                         internal {|#0:MyCarterModule|}(string s) {}
                         
                         public void AddRoutes(IEndpointRouteBuilder app) {}
                     }
                     """;
        
        var diagnosticResult = new DiagnosticResult(DiagnosticDescriptors.CarterModuleShouldNotHaveDependencies)
            .WithLocation(0)
            .WithArguments("MyCarterModule");

        return this.VerifyAsync(code, diagnosticResult);
    }

    [Theory]
    [InlineData("record")]
    [InlineData("record struct")]
    public Task RecordWithDependency_Diagnostic(string type)
    {
        var code = $$"""
                     using Carter;
                     using Microsoft.AspNetCore.Routing;

                     {{type}} {|#0:MyCarterModule|}(string S) : ICarterModule
                     {
                         public void AddRoutes(IEndpointRouteBuilder app) {}
                     }
                     """;
        
        var diagnosticResult = new DiagnosticResult(DiagnosticDescriptors.CarterModuleShouldNotHaveDependencies)
            .WithLocation(0)
            .WithArguments("MyCarterModule");

        return this.VerifyAsync(code, diagnosticResult);
    }
    
    [Theory]
    [InlineData("class")]
    [InlineData("struct")]
    [InlineData("record")]
    [InlineData("record struct")]
    public Task TypeWithMultipleDependencies_Diagnostic(string type)
    {
        var code = $$"""
                     using Carter;
                     using Microsoft.AspNetCore.Routing;
                     
                     {{type}} MyCarterModule : ICarterModule
                     {
                         internal {|#0:MyCarterModule|}(string s, int i) {}
                         
                         public void AddRoutes(IEndpointRouteBuilder app) {}
                     }
                     """;

        var diagnosticResult = new DiagnosticResult(DiagnosticDescriptors.CarterModuleShouldNotHaveDependencies)
            .WithLocation(0)
            .WithArguments("MyCarterModule");

        return this.VerifyAsync(code, diagnosticResult);
    }
    
    [Theory]
    [InlineData("record")]
    [InlineData("record struct")]
    public Task RecordWithMultipleDependencies_Diagnostic(string type)
    {
        var code = $$"""
                     using Carter;
                     using Microsoft.AspNetCore.Routing;
                     
                     {{type}} {|#0:MyCarterModule|}(string S, int I) : ICarterModule
                     {
                         public void AddRoutes(IEndpointRouteBuilder app) {}
                     }
                     """;

        var diagnosticResult = new DiagnosticResult(DiagnosticDescriptors.CarterModuleShouldNotHaveDependencies)
            .WithLocation(0)
            .WithArguments("MyCarterModule");

        return this.VerifyAsync(code, diagnosticResult);
    }
    
    [Theory]
    [InlineData("class")]
    [InlineData("struct")]
    [InlineData("record")]
    [InlineData("record struct")]
    public Task TypeWithDefaultDependencies_Diagnostic(string type)
    {
        var code = $$"""
                     using Carter;
                     using Microsoft.AspNetCore.Routing;
                     
                     {{type}} MyCarterModule : ICarterModule
                     {
                         internal {|#0:MyCarterModule|}(string s = "", char c = 'c') {}
                         
                         public void AddRoutes(IEndpointRouteBuilder app) {}
                     }
                     """;

        var diagnosticResult = new DiagnosticResult(DiagnosticDescriptors.CarterModuleShouldNotHaveDependencies)
            .WithLocation(0)
            .WithArguments("MyCarterModule");

        return this.VerifyAsync(code, diagnosticResult);
    }
    
    [Theory]
    [InlineData("record")]
    [InlineData("record struct")]
    public Task RecordWithDefaultDependencies_Diagnostic(string type)
    {
        var code = $$"""
                     using Carter;
                     using Microsoft.AspNetCore.Routing;
                     
                     {{type}} {|#0:MyCarterModule|}(string S = "", char C = 'c') : ICarterModule
                     {
                         public void AddRoutes(IEndpointRouteBuilder app) {}
                     }
                     """;

        var diagnosticResult = new DiagnosticResult(DiagnosticDescriptors.CarterModuleShouldNotHaveDependencies)
            .WithLocation(0)
            .WithArguments("MyCarterModule");

        return this.VerifyAsync(code, diagnosticResult);
    }
    
    [Theory]
    [InlineData("class")]
    [InlineData("struct")]
    [InlineData("record")]
    [InlineData("record struct")]
    public Task TypeWithDependencies_WhenConstructorIsPrivate_NoDiagnostic(string type)
    {
        var code = $$"""
                     using Carter;
                     using Microsoft.AspNetCore.Routing;
                     
                     {{type}} MyCarterModule : ICarterModule
                     {
                        private MyCarterModule(string s) {}
                        
                        public void AddRoutes(IEndpointRouteBuilder app) {}
                     }
                     """;
        
        return this.VerifyAsync(code);
    }
    
    [Theory]
    [InlineData("class")]
    [InlineData("struct")]
    [InlineData("record")]
    [InlineData("record struct")]
    public Task TypeWithDependencies_WhenConstructorIsImplicitlyPrivate_NoDiagnostic(string type)
    {
        var code = $$"""
                     using Carter;
                     using Microsoft.AspNetCore.Routing;
                     
                     {{type}} MyCarterModule : ICarterModule
                     {
                        MyCarterModule(string s) {}
                        
                        public void AddRoutes(IEndpointRouteBuilder app) {}
                     }
                     """;
        
        return this.VerifyAsync(code);
    }
    
    [Theory]
    [InlineData("class")]
    [InlineData("struct")]
    [InlineData("record")]
    [InlineData("record struct")]
    public Task TypeWithoutConstructor_NoDiagnostic(string type)
    {
        var code = $$"""
                     using Carter;
                     using Microsoft.AspNetCore.Routing;
                     
                     {{type}} MyCarterModule : ICarterModule
                     {
                        void M() {}
                        
                        public void AddRoutes(IEndpointRouteBuilder app) {}
                     }
                     """;
        
        return this.VerifyAsync(code);
    }
    
    [Theory]
    [InlineData("class")]
    [InlineData("struct")]
    [InlineData("record")]
    [InlineData("record struct")]
    public Task TypeWithZeroParameterConstructor_NoDiagnostic(string type)
    {
        var code = $$"""
                     using Carter;
                     using Microsoft.AspNetCore.Routing;
                     using System;
                     
                     {{type}} MyCarterModule : ICarterModule
                     {
                        public MyCarterModule()
                        {
                            Console.WriteLine("Hello World.");
                        }
                        
                        public void AddRoutes(IEndpointRouteBuilder app) {}
                     }
                     """;
        
        return this.VerifyAsync(code);
    }
    
    [Theory]
    [InlineData("class")]
    [InlineData("struct")]
    [InlineData("record")]
    [InlineData("record struct")]
    public Task NonCarterModuleWithConstructorDependencies_NoDiagnostic(string type)
    {
        var code = $$"""
                     using System;
                     
                     {{type}} MyCarterModule
                     {
                        internal MyCarterModule(string s, int i) {}
                     }
                     """;
        
        return this.VerifyAsync(code);
    }
    
    [Theory]
    [InlineData("record")]
    [InlineData("record struct")]
    public Task RecordNonCarterModuleWithConstructorDependencies_NoDiagnostic(string type)
    {
        var code = $$"""
                     using System;
                     
                     {{type}} MyCarterModule(string S, int I)
                     {
                     }
                     """;
        
        return this.VerifyAsync(code);
    }
    
    [Theory]
    [InlineData("class")]
    [InlineData("record")]
    public Task CarterSubModuleWithConstructorDependencies_NoDiagnostic(string type)
    {
        var code = $$"""
                     using Carter;
                     using Microsoft.AspNetCore.Routing;
                     
                     {{type}} MyCarterModule : ICarterModule
                     {
                         public void AddRoutes(IEndpointRouteBuilder app) {}
                     }
                     
                     {{type}} MySubCarterModule : MyCarterModule
                     {
                         public MySubCarterModule(string s) {}
                     }
                     """;
        
        return this.VerifyAsync(code);
    }

    private Task VerifyAsync(string code, DiagnosticResult? diagnosticResult = null)
    {
        var carterPackage = new PackageIdentity("Carter", "8.1.0");
        var aspNetPackage = new PackageIdentity("Microsoft.AspNetCore.App.Ref", "8.0.0");
        var bclPackage = new PackageIdentity("Microsoft.NETCore.App.Ref", "8.0.0");
        var referenceAssemblies = new ReferenceAssemblies("net8.0", bclPackage, Path.Combine("ref", "net8.0"))
            .AddPackages([carterPackage, aspNetPackage]);
        AnalyzerTest<XUnitVerifier> test = new CSharpAnalyzerTest<CarterModuleShouldNotHaveDependenciesAnalyzer, XUnitVerifier>
        {
            TestCode = code,
            ReferenceAssemblies = referenceAssemblies
        };

        if (diagnosticResult.HasValue)
        {
            test.ExpectedDiagnostics.Add(diagnosticResult.Value);
        }

        return test.RunAsync();
    }
}
