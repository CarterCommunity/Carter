namespace Carter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FluentValidation;

    public class CarterDependencyScanner
    {
        private readonly DependencyContextAssemblyCatalog catalog;
        
        public CarterDependencyScanner() => this.catalog = new DependencyContextAssemblyCatalog();

        public CarterDependencyScanner(Assembly assembly) => this.catalog = new DependencyContextAssemblyCatalog(assembly);

        public IEnumerable<Type> ScanValidators() 
            => this.catalog.GetAssemblies()
                .SelectMany(ass => ass.GetTypes())
                .Where(typeof(IValidator).IsAssignableFrom)
                .Where(t => !t.GetTypeInfo().IsAbstract);
        
        public IEnumerable<Type> ScanModules() 
            => this.catalog.GetAssemblies()
                .SelectMany(x => x.GetTypes()
                    .Where(t =>
                        !t.IsAbstract &&
                        typeof(CarterModule).IsAssignableFrom(t) &&
                        t != typeof(CarterModule) &&
                        t.IsPublic
                    ));
        
        public IEnumerable<Type> ScanStatusCodeHandlers() 
            => this.catalog.GetAssemblies()
                .SelectMany(x => x.GetTypes().Where(t => 
                    typeof(IStatusCodeHandler).IsAssignableFrom(t) && t != typeof(IStatusCodeHandler)));
        
        public IEnumerable<Type> ScanResponseNegotiators() 
            => this.catalog.GetAssemblies()
                .SelectMany(x => x.GetTypes()
                    .Where(t =>
                        !t.IsAbstract &&
                        typeof(IResponseNegotiator).IsAssignableFrom(t) &&
                        t != typeof(IResponseNegotiator) &&
                        t != typeof(DefaultJsonResponseNegotiator)
                    ));
    }
}