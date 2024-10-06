namespace SurveyBasket.Api.Services;

public class QuestionService(ApplicationDbContext context, ICacheService cacheService, ILogger<QuestionService> logger) : IQuestionService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ICacheService _cacheService = cacheService;
    private readonly ILogger<QuestionService> _logger = logger;
    private const string CachePrefix = "availableQuestions";


    public async Task<Result<PaginatedList<QuestionResponse>>> GetAllAsync(int pollId, RequestFilters filters, CancellationToken cancellationToken = default)
    {

        bool isPollExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);

        if (!isPollExists)
            return Result.Failure<PaginatedList<QuestionResponse>>(PollErrors.PollNotFound);

        var query = _context.Questions
            .Where(p => p.PollId == pollId);


        if (!string.IsNullOrEmpty(filters.SearchValue))
            query = query.Where(q => q.Content.Contains(filters.SearchValue));

        if (!string.IsNullOrEmpty(filters.SortColumn))
            query = query.OrderBy($"{filters.SortColumn} {filters.SortDirection}"); // using System.Linq.Dynamic.Core to sort the query based on string values


        var source = query
                     .Include(q => q.Answers)
                     .Select(q => new QuestionResponse(
                         q.Id,
                         q.Content,
                         q.Answers.Select(a => new AnswerResponse(a.Id, a.Content))
                     ))
                     .ProjectToType<QuestionResponse>()
                     .AsNoTracking();

        var paginatedQuestions = await PaginatedList<QuestionResponse>.Create(source, filters.PageNumber, filters.PageSize, cancellationToken);

        return Result.Success(paginatedQuestions);
    }

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId, string userId, CancellationToken cancellationToken = default)
    {
        // check if this user has already voted on this poll before or not
        bool isUserVoted = await _context.Votes.AnyAsync(v => v.PollId == pollId && v.UserId == userId, cancellationToken);
        if (isUserVoted)
            return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.DuplicatedVoted);

        // check if the poll exists and published and not expired
        bool isPollExists = await _context.Polls.AnyAsync(p => p.Id == pollId && p.IsPublished &&
                                                            (p.StartsAt <= DateTime.UtcNow && p.EndsAt >= DateTime.UtcNow)
                                                            , cancellationToken);
        if (!isPollExists)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var cacheKey = $"{CachePrefix}-{pollId}";

        var cachedQuestions = await _cacheService.GetAsync<IEnumerable<QuestionResponse>>(cacheKey, cancellationToken);
        if (cachedQuestions != null)
        {
            _logger.LogInformation("Questions are retrieved from the cache");
            return Result.Success(cachedQuestions);
        }

        _logger.LogInformation("Questions are retrieved from the Database");
        // get all questions for this poll
        var questions = await _context.Questions
            .Where(p => p.PollId == pollId && p.IsActive)
            .Include(q => q.Answers)
            .Select(q => new QuestionResponse(
                q.Id,
                q.Content,
                q.Answers.Where(a => a.IsActive).Select(a => new AnswerResponse(a.Id, a.Content))
            ))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        await _cacheService.SetAsync(cacheKey, questions, cancellationToken);


        return Result.Success<IEnumerable<QuestionResponse>>(questions);
    }

    public async Task<Result<QuestionResponse>> GetAsync(int pollId, int questionId, CancellationToken cancellationToken = default)
    {

        var question = await _context.Questions
            .Where(q => q.PollId == pollId && q.Id == questionId)
            .Include(q => q.Answers)
            //.Select(q => new QuestionResponse(
            //    q.Id,
            //    q.Content,
            //    q.Answers.Select(a => new AnswerResponse(a.Id, a.Content))
            //))
            .ProjectToType<QuestionResponse>()
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        if (question is null)
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);

        return Result.Success(question);
    }
    public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        bool isPollExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);

        // Check if the poll exists
        if (!isPollExists)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

        // check if this poll has a question with the same content
        bool isQuestionExists = await _context.Questions.AnyAsync(q => q.PollId == pollId && q.Content == request.Content, cancellationToken);

        if (isQuestionExists)
            return Result.Failure<QuestionResponse>(QuestionErrors.DuplicatedQuestionContent);

        //Question question = new()
        //{
        //    PollId = pollId,
        //    Content = request.Content
        //};

        Question question = request.Adapt<Question>();  // matches with SOLUTION 1 in MappingConfig Class 
        question.PollId = pollId;

        //request.Answers.ForEach(answer => question.Answers.Add(new Answer { Content = answer }));     // matches with SOLUTION 2 in MappingConfig Class


        await _context.Questions.AddAsync(question, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveAsync($"{CachePrefix}-{pollId}", cancellationToken);

        return Result.Success(question.Adapt<QuestionResponse>());
    }

    public async Task<Result> UpdateAsync(int pollId, int questionId, QuestionRequest request, CancellationToken cancellationToken = default)
    {

        // check if the content is duplicated
        bool isDuplicated = await _context.Questions.AnyAsync(q => q.PollId == pollId && q.Content == request.Content && q.Id != questionId, cancellationToken);



        if (isDuplicated)
            return Result.Failure(QuestionErrors.DuplicatedQuestionContent);

        var question = await _context.Questions
            .Where(q => q.PollId == pollId && q.Id == questionId)
            .Include(q => q.Answers)
            .SingleOrDefaultAsync(cancellationToken);

        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.Content = request.Content;

        // current answers
        var currentAnswers = question.Answers.Select(a => a.Content).ToList();

        // add new answers
        var newAnswers = request.Answers.Except(currentAnswers).ToList();

        newAnswers.ForEach(answer => question.Answers.Add(new Answer { Content = answer }));

        // remove deleted answers ny marking them as inactive
        question.Answers.ToList().ForEach(answer =>
        {
            if (!request.Answers.Contains(answer.Content))
                answer.IsActive = false;
        });




        await _context.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveAsync($"{CachePrefix}-{pollId}", cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ToggleStatusAsync(int pollId, int questionId, CancellationToken cancellationToken = default)
    {
        var question = await _context.Questions
            .Where(q => q.PollId == pollId && q.Id == questionId)
            .SingleOrDefaultAsync(cancellationToken);

        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.IsActive = !question.IsActive;

        await _context.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveAsync($"{CachePrefix}-{pollId}", cancellationToken);

        return Result.Success();
    }
}
