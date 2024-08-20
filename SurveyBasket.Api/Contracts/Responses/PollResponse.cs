namespace SurveyBasket.Api.Contracts.Responses;

public record PollResponse(int Id, string Title, string Summary, bool IsPublished, DateTime StartsAt, DateTime EndsAt);