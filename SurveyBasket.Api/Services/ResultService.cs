namespace SurveyBasket.Api.Services;

public class ResultService(ApplicationDbContext context) : IResultService
{
    private readonly ApplicationDbContext _context = context;
    public async Task<Result<PollVotesResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken = default)
    {

        var pollVotesResponse = await _context.Polls
            .Where(p => p.Id == pollId)
            .Select(p => new PollVotesResponse(
                p.Title,
                p.Votes.Select(v => new VoteResponse(
                    $"{v.User.FirstName} {v.User.LastName}",
                    v.SubmittedOn,
                    v.VoteAnswers.Select(va => new QuestionAnswerResponse(
                        va.Question.Content,
                        va.Answer.Content

                        ))
                    ))


            )).SingleOrDefaultAsync(cancellationToken);

        if (pollVotesResponse == null)
            return Result.Failure<PollVotesResponse>(PollErrors.PollNotFound);

        return Result.Success(pollVotesResponse);
    }

    public async Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayAsync(int pollId, CancellationToken cancellationToken = default)
    {
        bool isPollExist = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);

        if (!isPollExist)
            return Result.Failure<IEnumerable<VotesPerDayResponse>>(PollErrors.PollNotFound);


        var votesPerDay = await _context.Votes
            .Where(v => v.PollId == pollId)
            .GroupBy(v => DateOnly.FromDateTime(v.SubmittedOn))
            .Select(g => new VotesPerDayResponse(g.Key, g.Count()))
            .ToListAsync(cancellationToken);


        return Result.Success<IEnumerable<VotesPerDayResponse>>(votesPerDay);
    }

    public async Task<Result<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestionAsync(int pollId, CancellationToken cancellationToken = default)
    {
        bool isPollExist = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
        if (!isPollExist)
            return Result.Failure<IEnumerable<VotesPerQuestionResponse>>(PollErrors.PollNotFound);

        // gets the votes per question without the zero count of answers

        //var votesPerQuestion = await _context.VoteAnswers
        //    .Where(va => va.Vote.PollId == pollId)
        //    .Select(va => new VotesPerQuestionResponse(
        //        va.Question.Content,
        //        va.Question.VotesAns
        //            .GroupBy(va => new { AnswerId = va.AnswerId, AnswerContent = va.Answer.Content })
        //                .Select(g => new VotesPerAnswerResponse(
        //                    g.Key.AnswerContent,
        //                    g.Count()

        //                    ))

        //    ))
        //    .ToListAsync(cancellationToken);


        // gets the votes per question with the zero count of answers

        var votesPerQuestion = _context.Questions
      .Where(q => q.PollId == pollId) // Filter questions by PollId
      .Select(q => new VotesPerQuestionResponse(
          q.Content,
          q.Answers
              .Select(a => new VotesPerAnswerResponse(
              a.Content,
                  _context.VoteAnswers
                      .Count(va => va.AnswerId == a.Id) // Count the number of votes for each answer
              ))
              .ToList()
      ))
      .ToList();









        return Result.Success<IEnumerable<VotesPerQuestionResponse>>(votesPerQuestion);
    }
}

