using Microsoft.CodeAnalysis;

namespace Carter.Analyzers;

internal static class DiagnosticDescriptors
{
   public static readonly DiagnosticDescriptor CarterModuleShouldNotHaveDependencies = new(
      "CARTER1",
      "Carter module should not have dependencies",
      "'{0}' should not have dependencies",
      "Usage",
      DiagnosticSeverity.Warning,
      true,
      "When a class implements ICarterModule, it should not have any dependencies. This is because Carter uses minimal APIs and dependencies should be declared in the request delegate."
   );
}