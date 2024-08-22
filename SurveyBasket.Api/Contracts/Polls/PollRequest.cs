namespace SurveyBasket.Api.Contracts.Polls;

public record PollRequest(
string Title,
string Summary,
DateTime StartsAt,
DateTime EndsAt);
