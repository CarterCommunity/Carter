namespace Carter
{
    using System;
    using System.Linq;
    using Carter.Response;

    internal class DiagnosticsModule : CarterModule
    {
        public DiagnosticsModule(CarterDiagnostics diagnostics) : base("/_CarterDiagnostics")
        {
            this.Get("/", async context =>
            {
                Console.WriteLine("woo");
                var responseObject = new
                {
                    Modules = diagnostics.RegisteredModules,
                    ResponseNegotiators = diagnostics.RegisteredResponseNegotiators,
                    StatusCodeHandlers = diagnostics.RegisteredStatusCodeHandlers,
                    Validators = diagnostics.RegisteredValidators,
                    Paths = diagnostics.RegisteredPaths.Select(x => new { Module = x.Key.FullName, Path = x.Value}).GroupBy(g => g.Module, arg => arg.Path).Select(grouping => new { Module = grouping.Key, Paths = grouping})
                };
                await context.Response.Negotiate(responseObject);
            });
        }
    }
}
