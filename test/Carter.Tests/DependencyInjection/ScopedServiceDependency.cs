namespace Carter.Tests.DependencyInjection
{
    public class ScopedServiceDependency
    {
        public ScopedServiceDependency(ScopedRequestDependency scopedRequestDependency)
        {
            this.Guid = scopedRequestDependency.GetGuid();
        }

        public string Guid { get; set; }
    }
}