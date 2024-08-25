
using OneOf;

namespace SurveyBasket.Api.Services;

public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider, IOptions<JwtOptions> jwtOptions) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    // using OneOf;
    //public async Task<OneOf<AuthResponse,Error>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    //{
    //    var user = await _userManager.FindByEmailAsync(email);
    //    if (user is null)
    //        return UserErrors.InvalidCredentials;

    //    bool isPasswordCorrect = await _userManager.CheckPasswordAsync(user, password);
    //    if (!isPasswordCorrect)
    //        return UserErrors.InvalidCredentials;

    //    var Result = await GenerateAuthResponseAsync(user);
    //    return Result.Value;
    //}

    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        bool isPasswordCorrect = await _userManager.CheckPasswordAsync(user, password);
        if (!isPasswordCorrect)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        return await GenerateAuthResponseAsync(user);
    }
    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        // Validate the JWT token and retrieve the user ID
        var userId = _jwtProvider.ValidateToken(token);
        if (userId is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidToken);

        // Find the user by ID
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.UserNotFound);

        // Validate the refresh token
        RefreshToken userRefreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken && rt.IsActive);
        if (userRefreshToken is null)
            return Result.Failure<AuthResponse>(UserErrors.RefreshTokenInvalid);

        // Invalidate the old refresh token
        userRefreshToken.RevokedOn = DateTime.UtcNow; // Mark it as revoked

        // Generate a new JWT token and refresh token
        var authResponse = await GenerateAuthResponseAsync(user);
        return Result.Success(authResponse.Value);
    }

    public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        // Validate the JWT token and retrieve the user ID
        var userId = _jwtProvider.ValidateToken(token);
        if (userId is null)
            return Result.Failure(UserErrors.InvalidToken);

        // Find the user by ID
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure(UserErrors.UserNotFound);

        // Validate the refresh token
        var userRefreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken && rt.IsActive);
        if (userRefreshToken is null)
            return Result.Failure(UserErrors.RefreshTokenInvalid);

        // Invalidate the old refresh token
        userRefreshToken.RevokedOn = DateTime.UtcNow;   // instead of deleting the refresh token, we mark it as revoked

        await _userManager.UpdateAsync(user);

        return Result.Success();
    }


    public async Task<Result<AuthResponse>> RegisterAsync(string email, string password, string firstName, string lastName, CancellationToken cancellationToken = default)
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

    private async Task<Result<AuthResponse>> GenerateAuthResponseAsync(ApplicationUser user)
    {
        var (token, expiresIn) = _jwtProvider.GenerateToken(user);

        var refreshToken = GenerateRefreshToken();
        user.RefreshTokens.Add(refreshToken);

        await _userManager.UpdateAsync(user);

        var authResponse = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn, refreshToken.Token, refreshToken.ExpiresOn);

        return Result.Success(authResponse);
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

