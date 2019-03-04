namespace Carter.Tests.DependencyInjection
{
    using System;
    using Microsoft.AspNetCore.Http;

    public class ScopedRequestDependencyModule : CarterModule
    {
        private readonly Guid instanceId;

        public ScopedRequestDependencyModule(ScopedRequestDependency scopedRequestDependency,
            ScopedServiceDependency scopedServiceDependency)
        {
            this.instanceId = Guid.NewGuid();
            this.Get("/scopedreqdep",
                ctx => ctx.Response.WriteAsync(scopedRequestDependency.GetGuid() + ":" + scopedServiceDependency.Guid));
            this.Get("/instanceid", ctx =>
                ctx.Response.WriteAsync(this.instanceId.ToString()));
        }
    }
}