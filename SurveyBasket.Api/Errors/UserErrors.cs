namespace SurveyBasket.Api.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials = new("invalid_credentials", "Invalid email or password",StatusCodes.Status400BadRequest);
    public static readonly Error InvalidToken = new("invalid_token", "The provided token is invalid or expired.", StatusCodes.Status400BadRequest);
    public static readonly Error UserNotFound = new("user_not_found", "User not found.", StatusCodes.Status404NotFound);
    public static readonly Error RefreshTokenInvalid = new("refresh_token_invalid", "The provided refresh token is invalid or inactive.", StatusCodes.Status400BadRequest);
    public static readonly Error EmailInUse = new("email_in_use", "The provided email is already in use.", StatusCodes.Status400BadRequest);
}

