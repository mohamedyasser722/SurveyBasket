
using SurveyBasket.Api.Entities;

namespace SurveyBasket.Api.Services;

public class PollService(ApplicationDbContext context) : IPollService
{
    private readonly ApplicationDbContext _context = context;
    private readonly DbSet<Poll> _polls = context.Set<Poll>();

    public async Task<IEnumerable<PollResponse>>? GetAllAsync(CancellationToken cancellationToken = default) => _polls.AsNoTracking().ToList().Adapt<IEnumerable<PollResponse>>();

    public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _polls.FindAsync(id, cancellationToken);
        var pollResponse = poll.Adapt<PollResponse>();
        if (pollResponse is null)
            return Result.Failure<PollResponse>(PollErrors.PollNotFound);

        return Result.Success(pollResponse);
    }
    public async Task<PollResponse> AddAsync(PollRequest pollRequest, CancellationToken cancellationToken = default)
    {
        Poll Poll = pollRequest.Adapt<Poll>();
        await _polls.AddAsync(Poll, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);


        return Poll.Adapt<PollResponse>();
    }
    public async Task<Result> UpdateAsync(int id, Poll poll, CancellationToken cancellationToken = default)
    {
        var existingPoll = await _polls
            .FindAsync(id, cancellationToken);

        if (existingPoll == null)
            return Result.Failure(PollErrors.PollNotFound);

        // Update properties
        existingPoll.Title = poll.Title;
        existingPoll.Summary = poll.Summary;
        existingPoll.StartsAt = poll.StartsAt;
        existingPoll.EndsAt = poll.EndsAt;

        // Save changes
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existingPoll = await _polls
            .FindAsync(id, cancellationToken);

        if (existingPoll is null)
            return Result.Failure(PollErrors.PollNotFound);
        _polls.Remove(existingPoll);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
    public async Task<Result> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var existingPoll = await _polls
           .FindAsync(id, cancellationToken);
        if (existingPoll is null)
            return Result.Failure(PollErrors.PollNotFound);
        existingPoll.IsPublished = !existingPoll.IsPublished;
        await _context.SaveChangesAsync(cancellationToken); 
        return Result.Success();
    }
}
