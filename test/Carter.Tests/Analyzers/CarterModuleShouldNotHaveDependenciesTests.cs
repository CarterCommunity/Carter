namespace Carter.Tests.Analyzers;

using System.Threading.Tasks;
using Carter.Analyzers;
using Microsoft.CodeAnalysis.Testing;

public sealed class CarterModuleShouldNotHaveDependenciesTests
{
    [Test]
    [Arguments("class")]
    [Arguments("struct")]
    [Arguments("record")]
    [Arguments("record struct")]
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

        return VerifyAsync(code, diagnosticResult);
    }

    [Test]
    [Arguments("record")]
    [Arguments("record struct")]
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

        return VerifyAsync(code, diagnosticResult);
    }
    
    [Test]
    [Arguments("class")]
    [Arguments("struct")]
    [Arguments("record")]
    [Arguments("record struct")]
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

        return VerifyAsync(code, diagnosticResult);
    }
    
    [Test]
    [Arguments("record")]
    [Arguments("record struct")]
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

        return VerifyAsync(code, diagnosticResult);
    }
    
    [Test]
    [Arguments("class")]
    [Arguments("struct")]
    [Arguments("record")]
    [Arguments("record struct")]
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

        return VerifyAsync(code, diagnosticResult);
    }
    
    [Test]
    [Arguments("record")]
    [Arguments("record struct")]
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

        return VerifyAsync(code, diagnosticResult);
    }
    
    [Test]
    [Arguments("class")]
    [Arguments("struct")]
    [Arguments("record")]
    [Arguments("record struct")]
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
        
        return VerifyAsync(code);
    }
    
    [Test]
    [Arguments("class")]
    [Arguments("struct")]
    [Arguments("record")]
    [Arguments("record struct")]
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
        
        return VerifyAsync(code);
    }
    
    [Test]
    [Arguments("class")]
    [Arguments("struct")]
    [Arguments("record")]
    [Arguments("record struct")]
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
        
        return VerifyAsync(code);
    }
    
    [Test]
    [Arguments("class")]
    [Arguments("struct")]
    [Arguments("record")]
    [Arguments("record struct")]
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
        
        return VerifyAsync(code);
    }
    
    [Test]
    [Arguments("class")]
    [Arguments("struct")]
    [Arguments("record")]
    [Arguments("record struct")]
    public Task NonCarterModuleWithConstructorDependencies_NoDiagnostic(string type)
    {
        var code = $$"""
                     using System;
                     
                     {{type}} MyCarterModule
                     {
                        internal MyCarterModule(string s, int i) {}
                     }
                     """;
        
        return VerifyAsync(code);
    }
    
    [Test]
    [Arguments("record")]
    [Arguments("record struct")]
    public Task RecordNonCarterModuleWithConstructorDependencies_NoDiagnostic(string type)
    {
        var code = $$"""
                     using System;
                     
                     {{type}} MyCarterModule(string S, int I)
                     {
                     }
                     """;
        
        return VerifyAsync(code);
    }
    
    [Test]
    [Arguments("class")]
    [Arguments("record")]
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
        
        return VerifyAsync(code);
    }

    [Test]
    [Arguments("class")]
    [Arguments("struct")]
    public Task EmptyPrimaryConstructor_NoDiagnostic(string type)
    {
        var code = $$"""
                     using Carter;
                     using Microsoft.AspNetCore.Routing;

                     {{type}} MyCarterModule() : ICarterModule
                     {
                         public void AddRoutes(IEndpointRouteBuilder app) {}
                     }
                     """;
        
        return VerifyAsync(code);
    }

    [Test]
    [Arguments("class", "string s")]
    [Arguments("struct", "string s")]
    [Arguments("class", "string s, int i")]
    [Arguments("struct", "string s, int i")]
    public Task PrimaryConstructor_Diagnostic(string type, string parameters)
    {
        var code = $$"""
                     using Carter;
                     using Microsoft.AspNetCore.Routing;

                     {{type}} {|#0:MyCarterModule|}({{parameters}}) : ICarterModule
                     {
                         public void AddRoutes(IEndpointRouteBuilder app) {}
                     }
                     """;
        
        var diagnosticResult = new DiagnosticResult(DiagnosticDescriptors.CarterModuleShouldNotHaveDependencies)
            .WithLocation(0)
            .WithArguments("MyCarterModule");
        
        return VerifyAsync(code, diagnosticResult);
    }

    private static Task VerifyAsync(string code, DiagnosticResult? diagnosticResult = null)
    {
        AnalyzerTest<DefaultVerifier> test = new CSharpPreviewAnalyzerTest<CarterModuleShouldNotHaveDependenciesAnalyzer>(code);
        if (diagnosticResult.HasValue)
        {
            test.ExpectedDiagnostics.Add(diagnosticResult.Value);
        }

        return test.RunAsync();
    }
}
