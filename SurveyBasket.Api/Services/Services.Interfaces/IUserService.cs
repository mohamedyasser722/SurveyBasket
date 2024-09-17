namespace SurveyBasket.Api.Services.Services.Interfaces;

public interface IUserService
{
    public Task<Result<UserProfileResponse>> GetProfileAsync(string userId);
    public Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request, CancellationToken cancellationToken = default);
    public Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);
    Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> GetAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> AddAsync(CreateUserRequest request, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(string id, UpdateUserRequest request, CancellationToken cancellationToken);
    Task<Result> ToggleStatusAsync(string id);
    Task<Result> UnlockUserAsync(string id);

}
