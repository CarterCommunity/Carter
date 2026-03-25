namespace Carter.Tests.Analyzers;

using System.Threading.Tasks;
using Carter.Analyzers;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

public sealed class CarterDerivedModuleShouldNotHaveDependenciesTests
{
    [Theory]
    [InlineData("class")]
    [InlineData("record")]
    public Task CarterSubModuleWithConstructorDependencies_Diagnostic(string type)
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
                         public {|#0:MySubCarterModule|}(string s) {}
                     }
                     """;

        var diagnosticResult = new DiagnosticResult(DiagnosticDescriptors.CarterDerivedModuleShouldNotHaveDependencies)
            .WithLocation(0)
            .WithArguments("MySubCarterModule");

        return VerifyAsync(code, diagnosticResult);
    }

    [Theory]
    [InlineData("class")]
    [InlineData("record")]
    public Task ConcreteTypeDerivedFromAbstractModuleWithDependencies_Diagnostic(string type)
    {
        var code = $$"""
                     using Carter;
                     using Microsoft.AspNetCore.Routing;

                     abstract {{type}} MyAbstractCarterModule : ICarterModule
                     {
                         public void AddRoutes(IEndpointRouteBuilder app) {}
                     }

                     {{type}} MyCarterModule : MyAbstractCarterModule
                     {
                         public {|#0:MyCarterModule|}(string s) {}
                     }
                     """;

        var diagnosticResult = new DiagnosticResult(DiagnosticDescriptors.CarterDerivedModuleShouldNotHaveDependencies)
            .WithLocation(0)
            .WithArguments("MyCarterModule");

        return VerifyAsync(code, diagnosticResult);
    }

    [Theory]
    [InlineData("class")]
    [InlineData("record")]
    public Task DerivedModuleWithPrivateConstructor_NoDiagnostic(string type)
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
                         private MySubCarterModule(string s) {}
                     }
                     """;

        return VerifyAsync(code);
    }

    [Theory]
    [InlineData("class")]
    [InlineData("record")]
    public Task AbstractDerivedModuleWithDependencies_NoDiagnostic(string type)
    {
        var code = $$"""
                     using Carter;
                     using Microsoft.AspNetCore.Routing;

                     {{type}} MyCarterModule : ICarterModule
                     {
                         public void AddRoutes(IEndpointRouteBuilder app) {}
                     }

                     abstract {{type}} MyAbstractSubCarterModule : MyCarterModule
                     {
                         public MyAbstractSubCarterModule(string s) {}
                     }
                     """;

        return VerifyAsync(code);
    }

    [Theory]
    [InlineData("class")]
    [InlineData("record")]
    public Task DerivedModuleWithoutConstructor_NoDiagnostic(string type)
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
                     }
                     """;

        return VerifyAsync(code);
    }

    [Theory]
    [InlineData("class")]
    [InlineData("record")]
    public Task ThreeLevelInheritanceWithDependencies_Diagnostic(string type)
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
                     }

                     {{type}} MySubSubCarterModule : MySubCarterModule
                     {
                         public {|#0:MySubSubCarterModule|}(string s) {}
                     }
                     """;

        var diagnosticResult = new DiagnosticResult(DiagnosticDescriptors.CarterDerivedModuleShouldNotHaveDependencies)
            .WithLocation(0)
            .WithArguments("MySubSubCarterModule");

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
