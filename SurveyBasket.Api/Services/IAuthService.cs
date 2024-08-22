namespace SurveyBasket.Api.Services;

public interface IAuthService
{
    Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<AuthResponse?> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    Task<AuthResponse?> RegisterAsync(string email, string password, string firstName, string lastName, CancellationToken cancellationToken = default);

}
