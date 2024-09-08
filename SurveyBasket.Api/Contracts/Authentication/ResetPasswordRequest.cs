namespace SurveyBasket.Api.Contracts.Authentication;

public record ResetPasswordRequest(

    string UserId,
    string Code,
    string NewPassword

);
