namespace Carter.Analyzers;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class CarterModuleShouldNotHaveDependenciesAnalyzer : DiagnosticAnalyzer
{
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        var carterModuleType = context.Compilation.GetTypeByMetadataName("Carter.ICarterModule");
        if (carterModuleType is null)
        {
            return;
        }

        context.RegisterSymbolAction(ctx => OnTypeAnalysis(ctx, carterModuleType), SymbolKind.NamedType);
    }

    private static void OnTypeAnalysis(SymbolAnalysisContext context, INamedTypeSymbol carterModuleType)
    {
        var typeSymbol = (INamedTypeSymbol)context.Symbol;
        if (!typeSymbol.Interfaces.Contains(carterModuleType, SymbolEqualityComparer.Default))
        {
            return;
        }

        foreach (var constructor in typeSymbol.Constructors)
        {
            if (constructor.DeclaredAccessibility == Accessibility.Private || constructor.Parameters.Length == 0)
            {
                continue;
            }

            foreach (var syntaxReference in constructor.DeclaringSyntaxReferences)
            {
                var node = syntaxReference.GetSyntax();
                SyntaxToken identifier;
                if (node is ConstructorDeclarationSyntax constructorDeclaration)
                {
                    identifier = constructorDeclaration.Identifier;
                } else if (node is RecordDeclarationSyntax recordDeclaration)
                {
                    identifier = recordDeclaration.Identifier;
                }
                else
                {
                    continue;
                }

                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.CarterModuleShouldNotHaveDependencies,
                    identifier.GetLocation(),
                    identifier.Text
                );
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [DiagnosticDescriptors.CarterModuleShouldNotHaveDependencies];
}
