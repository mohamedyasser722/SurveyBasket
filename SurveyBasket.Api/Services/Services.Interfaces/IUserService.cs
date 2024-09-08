namespace SurveyBasket.Api.Services.Services.Interfaces;

public interface IUserService
{
    public Task<Result<UserProfileResponse>> GetProfileAsync(string userId);
    public Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request, CancellationToken cancellationToken = default);
    public Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);
}
