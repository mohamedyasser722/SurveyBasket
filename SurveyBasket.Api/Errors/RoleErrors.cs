namespace SurveyBasket.Api.Errors;

public static class RoleErrors
{
    public static readonly Error RoleNotFound = new("RoleNotFound", "Role not found", StatusCodes.Status404NotFound);
    public static readonly Error RoleAlreadyExist = new("RoleAlreadyExist", "Another  role with the same name already exists", StatusCodes.Status409Conflict);
    public static readonly Error InvalidPermissions = new("InvalidPermissions", "One or more permissions are invalid", StatusCodes.Status400BadRequest);
}
