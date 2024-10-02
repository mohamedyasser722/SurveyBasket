namespace SurveyBasket.Api.Contracts.Polls;

public record PollResponse(
    int Id,
    string Title,
    string Summary,
    bool IsPublished,
    DateTime StartsAt,
    DateTime EndsAt
);
public record PollResponseV2(
    int Id,
    string Title,
    string Summary,
    DateTime StartsAt,
    DateTime EndsAt
);