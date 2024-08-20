using SurveyBasket.Api.Entities;

namespace SurveyBasket.Api.Mapping;

public static class MappingFunctions
{
    public static IQueryable<PollResponse> MapPollsToPollResponses(this IQueryable<Poll> polls)
        => polls.ProjectToType<PollResponse>();
}
