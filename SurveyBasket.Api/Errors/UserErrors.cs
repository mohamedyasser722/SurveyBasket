namespace SurveyBasket.Api.Errors;

public record UserErrors
{
    public static readonly Error InvalidCredentials = new("invalid_credentials", "Invalid email or password", StatusCodes.Status400BadRequest);
    public static readonly Error InvalidToken = new("invalid_token", "The provided token is invalid or expired.", StatusCodes.Status400BadRequest);
    public static readonly Error UserNotFound = new("user_not_found", "User not found.", StatusCodes.Status404NotFound);
    public static readonly Error RefreshTokenInvalid = new("refresh_token_invalid", "The provided refresh token is invalid or inactive.", StatusCodes.Status400BadRequest);
    public static readonly Error DuplicatedEmail = new("email_in_use", "The provided email is already in use.", StatusCodes.Status409Conflict);
    public static readonly Error EmailNotConfirmed = new("email_not_confirmed", "The provided email is not confirmed.", StatusCodes.Status401Unauthorized);
    public static readonly Error InvalidCode = new("invalid_code", "The provided code is invalid.", StatusCodes.Status400BadRequest);
    public static readonly Error DuplicatedConfirmation = new("email_already_confirmed", "Email already confirmed by the user", StatusCodes.Status400BadRequest);
    public static readonly Error UserIsDisabled = new("user_is_disabled", "User is disabled, please contact your adminstrator.", StatusCodes.Status400BadRequest);
    public static readonly Error LockedUser = new("locked_user", "User is locked out, please contact your adminstrator.", StatusCodes.Status400BadRequest);
    public static readonly Error InvalidRoles = new("invalid_roles", "The provided roles are invalid.", StatusCodes.Status400BadRequest);
    public static readonly Error UnableToUnlockUser = new("unable_to_unlock_user", "Unable to unlock user, please contact your adminstrator.", StatusCodes.Status400BadRequest);
}

