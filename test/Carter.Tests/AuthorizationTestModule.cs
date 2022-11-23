using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Carter.Tests;

public abstract class AuthorizationTestModuleBase : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/authorizedendpoint", async (HttpContext context) => await context.Response.WriteAsync("Authorized endpoint"));
    }
}

/// <summary>
/// Test module that requires default authorization for all the added routes.
/// </summary>
public class DefaultAuthorizationTestModule : AuthorizationTestModuleBase
{
    public DefaultAuthorizationTestModule()
    {
        RequireAuthorization();
    }
}

/// <summary>
/// Test module that requires authorization specified by the <see cref="SpecificPolicyOne"/> and <see cref="SpecificPolicyTwo"/> policies for all the added routes.
/// </summary>
public class SpecificPolicyAuthorizationTestModule : AuthorizationTestModuleBase
{
    public const string SpecificPolicyOne = nameof(SpecificPolicyOne);
    public const string SpecificPolicyTwo = nameof(SpecificPolicyTwo);

    public SpecificPolicyAuthorizationTestModule()
    {
        RequireAuthorization(SpecificPolicyOne, SpecificPolicyTwo);
    }
}
