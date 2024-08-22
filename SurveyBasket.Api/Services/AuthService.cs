using SurveyBasket.Api.Authentication;
using System.Security.Cryptography;

namespace SurveyBasket.Api.Services;

public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider, IOptions<JwtOptions> jwtOptions) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return null;

        bool isPasswordCorrect = await _userManager.CheckPasswordAsync(user, password);
        if (!isPasswordCorrect)
            return null;

        return await GenerateAuthResponseAsync(user);
    }
    public async Task<AuthResponse?> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        // Validate the JWT token and retrieve the user ID
        var userId = _jwtProvider.ValidateToken(token);
        if (userId is null)
            return null;

        // Find the user by ID
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return null;

        // Validate the refresh token
        RefreshToken userRefreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken && rt.IsActive);
        if (userRefreshToken is null)
            return null;

        // Invalidate the old refresh token
        userRefreshToken.RevokedOn = DateTime.UtcNow;   // instead of deleting the refresh token, we mark it as revoked

        // Generate a new JWT token and refresh token
        return await GenerateAuthResponseAsync(user);
    }
    public async Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {

        // Validate the JWT token and retrieve the user ID
        var userId = _jwtProvider.ValidateToken(token);
        if (userId is null)
            return false;

        // Find the user by ID
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return false;

        // Validate the refresh token
        RefreshToken userRefreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken && rt.IsActive);
        if (userRefreshToken is null)
            return false;

        // Invalidate the old refresh token
        userRefreshToken.RevokedOn = DateTime.UtcNow;   // instead of deleting the refresh token, we mark it as revoked

        await _userManager.UpdateAsync(user);

        return true;
    }

    public async Task<AuthResponse?> RegisterAsync(string email, string password, string firstName, string lastName, CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            Email = email,
            UserName = email,
            FirstName = firstName,
            LastName = lastName
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            return null;

        return await GenerateAuthResponseAsync(user);
    }

    private async Task<AuthResponse?> GenerateAuthResponseAsync(ApplicationUser user)
    {
        var (token, expiresIn) = _jwtProvider.GenerateToken(user);

        var refreshToken = GenerateRefreshToken();
        user.RefreshTokens.Add(refreshToken);

        await _userManager.UpdateAsync(user);

        return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn, refreshToken.Token, refreshToken.ExpiresOn);
    }

    private RefreshToken GenerateRefreshToken()
    {
        return new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiresOn = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpiryDays)
        };
    }

}

