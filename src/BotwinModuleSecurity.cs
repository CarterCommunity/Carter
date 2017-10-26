namespace Botwin
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public static class BotwinModuleSecurity
    {
        public static void RequiresAuthentication(this BotwinModule module)
        {
            module.Before = context =>
            {
                var authenticated = context?.User?.Identity != null && context.User.Identity.IsAuthenticated;
                if (!authenticated)
                {
                    context.Response.StatusCode = 401;
                }
                return Task.FromResult(authenticated);
            };
        }

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
