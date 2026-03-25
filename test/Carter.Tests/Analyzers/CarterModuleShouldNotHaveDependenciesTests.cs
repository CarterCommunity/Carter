namespace Carter.Tests.Analyzers;

using System.Threading.Tasks;
using Carter.Analyzers;
using Microsoft.CodeAnalysis.Testing;
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

        return VerifyAsync(code, diagnosticResult);
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

        return VerifyAsync(code, diagnosticResult);
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

        return VerifyAsync(code, diagnosticResult);
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

        return VerifyAsync(code, diagnosticResult);
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

        return VerifyAsync(code, diagnosticResult);
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

        return VerifyAsync(code, diagnosticResult);
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

        return VerifyAsync(code);
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

        return VerifyAsync(code);
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

        return VerifyAsync(code);
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

        return VerifyAsync(code);
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

        return VerifyAsync(code);
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

        return VerifyAsync(code);
    }

    [Fact]
    public Task PrivateNestedTypeWithDependencies_NoDiagnostic()
    {
        var code = """
                   using Carter;
                   using Microsoft.AspNetCore.Routing;

                   class Outer
                   {
                       private class MyCarterModule : ICarterModule
                       {
                           public MyCarterModule(string s) {}

                           public void AddRoutes(IEndpointRouteBuilder app) {}
                       }
                   }
                   """;

        return VerifyAsync(code);
    }

    [Fact]
    public Task ImplicitlyPrivateNestedTypeWithDependencies_NoDiagnostic()
    {
        var code = """
                   using Carter;
                   using Microsoft.AspNetCore.Routing;

                   class Outer
                   {
                       class MyCarterModule : ICarterModule
                       {
                           public MyCarterModule(string s) {}

                           public void AddRoutes(IEndpointRouteBuilder app) {}
                       }
                   }
                   """;

        return VerifyAsync(code);
    }

    [Theory]
    [InlineData("class")]
    [InlineData("record")]
    public Task AbstractTypeWithDependencies_NoDiagnostic(string type)
    {
        var code = $$"""
                     using Carter;
                     using Microsoft.AspNetCore.Routing;

                     abstract {{type}} MyCarterModule : ICarterModule
                     {
                         internal MyCarterModule(string s) {}

                         public void AddRoutes(IEndpointRouteBuilder app) {}
                     }
                     """;

        return VerifyAsync(code);
    }

    [Theory]
    [InlineData("class")]
    [InlineData("struct")]
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

    [Theory]
    [InlineData("class", "string s")]
    [InlineData("struct", "string s")]
    [InlineData("class", "string s, int i")]
    [InlineData("struct", "string s, int i")]
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

    [Fact]
    public Task PublicNestedTypeWithDependencies_Diagnostic()
    {
        var code = """
                   using Carter;
                   using Microsoft.AspNetCore.Routing;

                   class Outer
                   {
                       public class MyCarterModule : ICarterModule
                       {
                           public {|#0:MyCarterModule|}(string s) {}

                           public void AddRoutes(IEndpointRouteBuilder app) {}
                       }
                   }
                   """;

        var diagnosticResult = new DiagnosticResult(DiagnosticDescriptors.CarterModuleShouldNotHaveDependencies)
            .WithLocation(0)
            .WithArguments("MyCarterModule");

        return VerifyAsync(code, diagnosticResult);
    }

    [Fact]
    public Task InternalNestedTypeWithDependencies_Diagnostic()
    {
        var code = """
                   using Carter;
                   using Microsoft.AspNetCore.Routing;

                   class Outer
                   {
                       internal class MyCarterModule : ICarterModule
                       {
                           public {|#0:MyCarterModule|}(string s) {}

                           public void AddRoutes(IEndpointRouteBuilder app) {}
                       }
                   }
                   """;

        var diagnosticResult = new DiagnosticResult(DiagnosticDescriptors.CarterModuleShouldNotHaveDependencies)
            .WithLocation(0)
            .WithArguments("MyCarterModule");

        return VerifyAsync(code, diagnosticResult);
    }

    [Theory]
    [InlineData("class")]
    [InlineData("record")]
    public Task ProtectedConstructorWithDependencies_Diagnostic(string type)
    {
        var code = $$"""
                     using Carter;
                     using Microsoft.AspNetCore.Routing;

                     {{type}} MyCarterModule : ICarterModule
                     {
                         protected {|#0:MyCarterModule|}(string s) {}

                         public void AddRoutes(IEndpointRouteBuilder app) {}
                     }
                     """;

        var diagnosticResult = new DiagnosticResult(DiagnosticDescriptors.CarterModuleShouldNotHaveDependencies)
            .WithLocation(0)
            .WithArguments("MyCarterModule");

        return VerifyAsync(code, diagnosticResult);
    }

    [Theory]
    [InlineData("class")]
    [InlineData("struct")]
    public Task TypeWithMixedConstructors_OnlyParameterizedFlagged(string type)
    {
        var code = $$"""
                     using Carter;
                     using Microsoft.AspNetCore.Routing;

                     {{type}} MyCarterModule : ICarterModule
                     {
                         public MyCarterModule() {}

                         public {|#0:MyCarterModule|}(string s) {}

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
