using SurveyBasket.Api.Contracts.Answers;
using SurveyBasket.Api.Contracts.Questions;

namespace SurveyBasket.Api.Services;

public class QuestionService(ApplicationDbContext context) : IQuestionService
{
    private readonly ApplicationDbContext _context = context;
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

        return Result.Success(question.Adapt<QuestionResponse>());
    }

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken = default)
    {

        bool isPollExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);

        if(!isPollExists)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var questions = await _context.Questions
            .Where(p => p.PollId == pollId)
            .Include(q => q.Answers)
            //.Select(q => new QuestionResponse(
            //    q.Id,
            //    q.Content,
            //    q.Answers.Select(a => new AnswerResponse(a.Id, a.Content))
            //))
            .ProjectToType<QuestionResponse>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

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

        return Result.Success();
    }
}
