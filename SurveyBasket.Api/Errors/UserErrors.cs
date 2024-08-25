namespace SurveyBasket.Api.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials = new("invalid_credentials", "Invalid email or password");
    public static readonly Error InvalidToken = new("invalid_token", "The provided token is invalid or expired.");
    public static readonly Error UserNotFound = new("user_not_found", "User not found.");
    public static readonly Error RefreshTokenInvalid = new("refresh_token_invalid", "The provided refresh token is invalid or inactive.");
}

