namespace Carter.Tests.DependencyInjection
{
    using Microsoft.AspNetCore.Http;

    public class TransientRequestDependencyModule : CarterModule
    {
        public TransientRequestDependencyModule(TransientRequestDependency transientRequestDependency,
            TransientServiceDependency transientServiceDependency)
        {
            this.Get("/transientreqdep",
                ctx => ctx.Response.WriteAsync(transientRequestDependency.GetGuid() + ":" +
                    transientServiceDependency.TheGuid));
        }
    }
}