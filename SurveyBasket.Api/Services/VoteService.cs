using Microsoft.EntityFrameworkCore;
using SurveyBasket.Api.Contracts.Votes;

namespace SurveyBasket.Api.Services;

public class VoteService(ApplicationDbContext context) : IVoteService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result> AddAsync(int pollId, string userId, VoteRequest request, CancellationToken cancellationToken = default)
    {
        // Combine the first two checks into a single query to minimize database round trips.
        var pollInfo = await _context.Polls
            .Where(p => p.Id == pollId && p.IsPublished && p.StartsAt <= DateTime.UtcNow && p.EndsAt >= DateTime.UtcNow)
            .Select(p => new
            {
                UserVoted = _context.Votes.Any(v => v.PollId == pollId && v.UserId == userId)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (pollInfo == null)
            return Result.Failure(PollErrors.PollNotFound);

        if (pollInfo.UserVoted)
            return Result.Failure(VoteErrors.DuplicatedVoted);

        // Fetch active question IDs for the poll
        var validQuestionIds = await _context.Questions
            .Where(q => q.PollId == pollId && q.IsActive)
            .Select(q => q.Id)
            .ToListAsync(cancellationToken);

        var validQuestionIdSet = validQuestionIds.ToHashSet();

        // Convert request question IDs to a HashSet for faster lookup
        var questionIdsFromRequest = request.Answers.Select(a => a.QuestionId).ToHashSet();

        // Use set-based operations to check if the sets are exactly the same
        if (!questionIdsFromRequest.SetEquals(validQuestionIdSet))
            return Result.Failure(VoteErrors.InvalidQuestions);

        // At this point, all checks have passed
        // Add vote to the database here...

        var vote = new Vote
        {
            PollId = pollId,
            UserId = userId,
            VoteAnswers = request.Answers
            //.Select(a => new VoteAnswer
            //{
            //    QuestionId = a.QuestionId,
            //    AnswerId = a.AnswerId
            //}).ToList()
            .Adapt<IEnumerable<VoteAnswer>>().ToList()      // you can use mapster or select manually to map from IEnumerable<VoteAnswerRequest> to IEnumerable<VoteAnswer>
        };

        await _context.Votes.AddAsync(vote, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }



}
