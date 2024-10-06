
namespace SurveyBasket.Api.Authentication.Filters;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirment>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirment requirement)
    {
        //if (context.User.Identity is not { IsAuthenticated: true } ||
        //    !context.User.Claims.Any(x => x.Value == requirement.Permession && x.Type == Permessions.Type))
        //    return;

        var user = context.User.Identity;

        if (user is null || !user.IsAuthenticated)
            return;

        var hasPermession = context.User.Claims.Any(x => x.Value == requirement.Permession && x.Type == Permissions.Type);
        if (!hasPermession)
            return;

        context.Succeed(requirement);
        return;

    }
}
