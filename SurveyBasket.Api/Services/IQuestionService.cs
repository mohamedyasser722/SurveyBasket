using SurveyBasket.Api.Contracts.Questions;
using SurveyBasket.Api.Entities;

namespace SurveyBasket.Api.Services;

public interface IQuestionService
{
    Task<Result<QuestionResponse>> AddAsync (int pollId, QuestionRequest request, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId, string userId, CancellationToken cancellationToken = default);
    Task<Result<QuestionResponse>> GetAsync(int pollId, int questionId, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(int pollId, int questionId, QuestionRequest request, CancellationToken cancellationToken = default);
    Task<Result> ToggleStatusAsync(int pollId, int questionId, CancellationToken cancellationToken = default);

}
