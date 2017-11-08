namespace Botwin
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Security.Claims;
    using System.Threading.Tasks;

    /// <summary>
    /// <see cref="BotwinModule"/> extensions to provide security mechanisms
    /// </summary>
    public static class BotwinModuleSecurity
    {
        /// <summary>
        /// A way to require authentication for your <see cref="BotwinModule"/>
        /// </summary>
        /// <param name="module"><Current <see cref="BotwinModule"/>/param>
        public static void RequiresAuthentication(this BotwinModule module)
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
        /// A way to require claims for your <see cref="BotwinModule"/>
        /// </summary>
        /// <param name="module">Current <see cref="BotwinModule"/></param>
        /// <param name="claims">The claims required for the routes in your <see cref="BotwinModule"/></param>
        public static void RequiresClaims(this BotwinModule module, params Predicate<Claim>[] claims)
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
