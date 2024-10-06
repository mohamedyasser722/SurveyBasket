

namespace SurveyBasket.Api.Services;

public class PollService(ApplicationDbContext context, INotificationService notificationService) : IPollService
{
    private readonly INotificationService _notificationService = notificationService;
    private readonly ApplicationDbContext _context = context;
    private readonly DbSet<Poll> _polls = context.Set<Poll>();

    public async Task<IEnumerable<PollResponse>>? GetAllAsync(CancellationToken cancellationToken = default)
    {
        var polls = await _polls.AsNoTracking().ToListAsync(cancellationToken);
        return polls.Adapt<IEnumerable<PollResponse>>();
    }
    public async Task<IEnumerable<PollResponse>> GetCurrentAsyncV1(CancellationToken cancellationToken = default)
    {
        // Convert StartsAt and EndsAt to UTC before comparing
        // current polls are the polls that are published and the current time is between StartsAt and EndsAt
        var currentPolls = await _polls
            .Where(p => p.IsPublished &&
                        p.StartsAt <= DateTime.UtcNow &&
                        p.EndsAt >= DateTime.UtcNow)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return currentPolls.Adapt<IEnumerable<PollResponse>>();
    }
    public async Task<IEnumerable<PollResponseV2>> GetCurrentAsyncV2(CancellationToken cancellationToken = default)
    {
        // Convert StartsAt and EndsAt to UTC before comparing
        // current polls are the polls that are published and the current time is between StartsAt and EndsAt
        var currentPolls = await _polls
            .Where(p => p.StartsAt <= DateTime.UtcNow &&
                     p.EndsAt >= DateTime.UtcNow)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return currentPolls.Adapt<IEnumerable<PollResponseV2>>();
    }


    public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _polls.FindAsync(id, cancellationToken);
        var pollResponse = poll.Adapt<PollResponse>();
        if (pollResponse is null)
            return Result.Failure<PollResponse>(PollErrors.PollNotFound);

        return Result.Success(pollResponse);
    }
    public async Task<Result<PollResponse>> AddAsync(PollRequest pollRequest, CancellationToken cancellationToken = default)
    {
        var isTitleExist = await _polls.AnyAsync(p => p.Title == pollRequest.Title, cancellationToken);
        if (isTitleExist)
            return Result.Failure<PollResponse>(PollErrors.PollTitleExist);

        Poll Poll = pollRequest.Adapt<Poll>();
        await _polls.AddAsync(Poll, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);


        return Result.Success(Poll.Adapt<PollResponse>());
    }
    public async Task<Result> UpdateAsync(int id, Poll poll, CancellationToken cancellationToken = default)
    {
        var existingPoll = await _polls
            .FindAsync(id, cancellationToken);

        if (existingPoll == null)
            return Result.Failure(PollErrors.PollNotFound);

        var isTitleExist = await _polls.AnyAsync(p => p.Title == poll.Title && p.Id != id, cancellationToken);
        if (isTitleExist)
            return Result.Failure(PollErrors.PollTitleExist);

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
        var existingPoll = await _polls.FindAsync(id, cancellationToken);

        if (existingPoll is null)
            return Result.Failure(PollErrors.PollNotFound);

        existingPoll.IsPublished = !existingPoll.IsPublished;

        await _context.SaveChangesAsync(cancellationToken);

        // Send notification if the poll is published and the poll starts today because we have already passed the start date
        if (existingPoll.IsPublished && existingPoll.StartsAt >= DateTime.UtcNow.Date)
            BackgroundJob.Enqueue(() => _notificationService.SendNewPollsNotification(existingPoll.Id));

        return Result.Success();
    }
}
