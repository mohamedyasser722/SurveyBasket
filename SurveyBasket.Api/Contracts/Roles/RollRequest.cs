using SurveyBasket.Api.Contracts.Roles;

public record RollRequest(

    string Name,
    IList<string> Permissions

);