namespace Carter
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    /// <summary>
    /// <see cref="CarterModule"/> extensions to provide security mechanisms
    /// </summary>
    public static class CarterModuleSecurity
    {
        /// <summary>
        /// A way to require authentication for your <see cref="CarterModule"/>
        /// </summary>
        /// <param name="module"><Current <see cref="CarterModule"/>/param>
        public static void RequiresAuthentication(this CarterModule module)
        {
            module.Before += context =>
            {
                var authenticated = context?.User?.Identity != null && context.User.Identity.IsAuthenticated;
                if (!authenticated)
                {
                    context.Response.StatusCode = 401;
                }
                return Task.FromResult(authenticated);
            };
        }

        /// <summary>
        /// A way to require claims for your <see cref="CarterModule"/>
        /// </summary>
        /// <param name="module">Current <see cref="CarterModule"/></param>
        /// <param name="claims">The claims required for the routes in your <see cref="CarterModule"/></param>
        public static void RequiresClaims(this CarterModule module, params Predicate<Claim>[] claims)
        {
            module.RequiresAuthentication();
            module.Before += context =>
            {
                var validClaims = context.User != null && claims.All(context.User.HasClaim);
                if (!validClaims)
                {
                    context.Response.StatusCode = 401;
                }
                return Task.FromResult(validClaims);
            };
        }
    }
}
