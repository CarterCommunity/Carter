namespace Botwin
{
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
    }
}
