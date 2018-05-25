namespace Carter.Diagnostics
{
    using System.Linq;
    using Carter.Response;

    internal class DiagnosticsModule : CarterModule
    {
        public DiagnosticsModule(CarterDiagnostics diagnostics, CarterOptions options) : base("/_CarterDiagnostics")
        {
            this.Get("/", async context =>
            {
                var responseObject = new
                {
                    Modules = diagnostics.RegisteredModules,
                    ResponseNegotiators = diagnostics.RegisteredResponseNegotiators,
                    StatusCodeHandlers = diagnostics.RegisteredStatusCodeHandlers,
                    Validators = diagnostics.RegisteredValidators,
                    Routes = diagnostics.RegisteredRoutes.Select(x => new { Module = x.Key.FullName, Path = x.Value}).GroupBy(g => g.Module, arg => arg.Path).Select(grouping => new { Module = grouping.Key, Routes = grouping})
                };
                
                await context.Response.Negotiate(responseObject);
            });
        }
    }
}
