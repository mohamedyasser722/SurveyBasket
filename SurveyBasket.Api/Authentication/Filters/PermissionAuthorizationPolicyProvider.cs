namespace SurveyBasket.Api.Authentication.Filters;

public class PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
    : DefaultAuthorizationPolicyProvider(options)
{

    private readonly AuthorizationOptions _authorizationOptions = options.Value;

    public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        var policy = await base.GetPolicyAsync(policyName);
        if(policy is not null)
            return policy;

        var policyBuilder = new AuthorizationPolicyBuilder().
            AddRequirements(new PermissionRequirment(policyName))
            .Build();

        _authorizationOptions.AddPolicy(policyName, policyBuilder);

        return policyBuilder;

    }

}
