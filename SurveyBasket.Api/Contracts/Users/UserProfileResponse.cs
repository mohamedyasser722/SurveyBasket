namespace SurveyBasket.Api.Contracts.Users;

public record UserProfileResponse(

    string email,
    string UserName,
    string firstName,
    string lastName
);