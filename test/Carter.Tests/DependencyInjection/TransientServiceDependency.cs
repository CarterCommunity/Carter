namespace Carter.Tests.DependencyInjection
{
    public class TransientServiceDependency
    {
        public TransientServiceDependency(TransientRequestDependency transientRequestDependency)
        {
            this.TheGuid = transientRequestDependency.GetGuid();
        }

        public string TheGuid { get; }
    }
}