namespace Carter
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Microsoft.Extensions.DependencyInjection;

    public class DefaultCarterModuleProvider : ICarterModuleProvider
    {
        private readonly IServiceProvider serviceProvider;

        public DefaultCarterModuleProvider(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            this.serviceProvider = serviceProvider;
        }

        public Func<CarterModule>[] GetModuleFactories() => this.serviceProvider
            .GetServices<CarterModule>()
            .Select(this.GetScopedModule)
            .ToArray();

        public CarterModule Get(Type carterModuleType)
            => (CarterModule)this.serviceProvider.GetRequiredService(carterModuleType);

        private Func<CarterModule> GetScopedModule(CarterModule module)
            => () =>
            {
                using (var scope = this.serviceProvider.CreateScope())
                {
                    return (CarterModule)scope.ServiceProvider.GetService(module.GetType());
                }
            };
    }
}
