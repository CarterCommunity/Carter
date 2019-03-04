namespace Carter.Tests.DependencyInjection
{
    using System;

    public class TransientRequestDependency
    {
        private readonly string instanceId;

        public TransientRequestDependency()
        {
            this.instanceId = Guid.NewGuid().ToString();
        }

        public string GetGuid()
        {
            return this.instanceId;
        }
    }
}