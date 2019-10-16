namespace Carter
{
    using System.Linq;

    /// <summary>
    /// <see cref="CarterModule"/> extensions to provide security mechanisms
    /// </summary>
    public static class CarterModuleSecurity
    {
        /// <summary>
        /// A way to require authentication for your <see cref="CarterModule"/>
        /// </summary>
        /// <param name="module">Current <see cref="CarterModule"/></param>
        public static void RequiresAuthorization(this CarterModule module)
        {
            module.RequiresAuth = true;
        }

        /// <summary>
        /// A way to require policies for your <see cref="CarterModule"/>
        /// </summary>
        /// <param name="module">Current <see cref="CarterModule"/></param>
        /// <param name="policyNames">The policies required for the routes in your <see cref="CarterModule"/></param>
        public static void RequiresPolicy(this CarterModule module, params string[] policyNames)
        {
            module.AuthPolicies = module.AuthPolicies.Concat(policyNames).ToArray();
        }
    }
}
