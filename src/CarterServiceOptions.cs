namespace Carter
{
    using System;

    public class CarterServiceOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public Type CarterModuleProvider { get; set; } = typeof(DefaultCarterModuleProvider);
    }
}