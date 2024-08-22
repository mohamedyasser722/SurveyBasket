using SurveyBasket.Api.Contracts.Polls;
using SurveyBasket.Api.Entities;

namespace SurveyBasket.Api.Mapping;

// Not Used Anywhere in the project but i just leave it for learning purpose
public static class MappingFunctions
{
    public static IQueryable<PollResponse> MapPollsToPollResponses(this IQueryable<Poll> polls)
        => polls.ProjectToType<PollResponse>();
}
