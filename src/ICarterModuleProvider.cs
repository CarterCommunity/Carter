namespace Carter
{
    using System;

    public interface ICarterModuleProvider
    {
        Func<CarterModule>[] GetModuleFactories();

        CarterModule Get(Type carterModuleType);
    }
}