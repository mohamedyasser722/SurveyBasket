namespace SurveyBasket.Api.Authentication.Filters;

public class PermissionRequirment(string permession) : IAuthorizationRequirement
{
    public string Permession { get; } = permession;



}
