namespace SurveyBasket.Api.Services.Services.Interfaces;

public interface IRoleService
{
    Task<IEnumerable<RoleResponse>> GetAllAsync(bool? includeDisabled = false, CancellationToken cancellationToken = default);

    Task<Result<RoleDetailResponse>> GetAsync(string id);
    Task<Result<RoleDetailResponse>> AddAsync(RollRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(string id, RollRequest request);
    Task<Result> ToggleStatusAsync(string id);
}
