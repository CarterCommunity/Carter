namespace Carter.Tests.DependencyInjection
{
    using System;

    public class ScopedRequestDependency
    {
        private string instanceId;

        public ScopedRequestDependency()
        {
            this.instanceId = Guid.NewGuid().ToString();
        }

        public string GetGuid()
        {
            return this.instanceId;
        }
    }
}