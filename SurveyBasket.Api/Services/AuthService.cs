namespace SurveyBasket.Api.Services;

public class AuthService

    (UserManager<ApplicationUser> userManager,
    ApplicationDbContext context,
    SignInManager<ApplicationUser> signInManager,
    IJwtProvider jwtProvider,
    IOptions<JwtOptions> jwtOptions,
    ILogger<AuthService> logger,
    IEmailSender emailSender,
    IHttpContextAccessor httpContextAccessor,
    IConfiguration configuration) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ApplicationDbContext _context = context;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly IConfiguration _configuration = configuration;

    #region example of Using OneOf for error handling Package
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
    #endregion

    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        if(user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.UserIsDisabled);

        var result = await _signInManager.PasswordSignInAsync(user, password, false, true);

        if (result.Succeeded)
            return await GenerateAuthResponseAsync(user, cancellationToken);
        
        _logger.LogWarning("Failed login attempt for Email: {Email}", email);

        if(result.IsLockedOut)
            return Result.Failure<AuthResponse>(UserErrors.LockedUser);

        return Result.Failure<AuthResponse>(result.IsNotAllowed ? UserErrors.EmailNotConfirmed : UserErrors.InvalidCredentials);

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

        if(user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.UserIsDisabled);

        if (user.LockoutEnd > DateTime.UtcNow)
            return Result.Failure<AuthResponse>(UserErrors.LockedUser);

        // Validate the refresh token
        RefreshToken userRefreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken && rt.IsActive);
        if (userRefreshToken is null)
            return Result.Failure<AuthResponse>(UserErrors.RefreshTokenInvalid);

        // Invalidate the old refresh token
        userRefreshToken.RevokedOn = DateTime.UtcNow; // Mark it as revoked

        // Generate a new JWT token and refresh token
        var authResponse = await GenerateAuthResponseAsync(user, cancellationToken);
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


    public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        bool isEmailTaken = await _userManager.FindByEmailAsync(request.Email) is not null;

        if (isEmailTaken)
            return Result.Failure<AuthResponse>(UserErrors.DuplicatedEmail);

        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var error = result.Errors.FirstOrDefault();
            return Result.Failure<AuthResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        // add this user to the default role in AspNetUserRoles table

        result = await _userManager.AddToRoleAsync(user,DefaultRoles.Member.Name);
        if (!result.Succeeded)
        {
            var error = result.Errors.FirstOrDefault();
            return Result.Failure<AuthResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }



        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);    // Generate a token for email confirmation
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));           // Encode the token to be URL-safe

        _logger.LogInformation("Confirmation Code: {code}", code);

        // Send the confirmation email with the code in background job to avoid blocking the request with hangfire

        await sendConfirmationEmailAsync(user, code);

        return Result.Success();
    }

    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null)
            return Result.Failure(UserErrors.InvalidCode);

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmation);

        var code = request.Code;

        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return Result.Failure(UserErrors.InvalidCode);
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);

        if (!result.Succeeded)
        {
            var error = result.Errors.FirstOrDefault();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        return Result.Success();
    }

    public async Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Success(); // We don't want to leak information about the user's existence

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmation);

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);    // Generate a token for email confirmation
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));           // Encode the token to be URL-safe

        // Send the confirmation email with the code

        await sendConfirmationEmailAsync(user, code);

        _logger.LogInformation("Confirmation Code: {code}", code);

        return Result.Success();

    }


    public async Task<Result> SendResetPasswordCodeAsync(ForgetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Success(); // We don't want to leak information about the user's existence
        if(!user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotConfirmed with { statusCode = StatusCodes.Status400BadRequest});

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);

        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        _logger.LogInformation("Reset Password Code: {code}", code);

        await sendResetPasswordEmail(user, code);

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null)
            return Result.Failure(UserErrors.InvalidCode);
        if (!user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotConfirmed);

        var code = request.Code;

        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return Result.Failure(UserErrors.InvalidCode);
        }

        var result = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);

        if (!result.Succeeded)
        {
            var error = result.Errors.FirstOrDefault();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized));
        }

        return Result.Success();
    }



















    private async Task<Result<AuthResponse>> GenerateAuthResponseAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        // Retrieve user roles
        var roles = await _userManager.GetRolesAsync(user);

        // Get permissions for user roles
        var permissions = await GetPermissionsForUserRolesAsync(roles, cancellationToken);


        var (token, expiresIn) = _jwtProvider.GenerateToken(user, roles, permissions);

        var refreshToken = GenerateRefreshToken();
        user.RefreshTokens.Add(refreshToken);

        await _userManager.UpdateAsync(user);

        var authResponse = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn, refreshToken.Token, refreshToken.ExpiresOn);

        return Result.Success(authResponse);
    }
    private async Task<List<string>> GetPermissionsForUserRolesAsync(IEnumerable<string> userRoles, CancellationToken cancellationToken)
    {
        //var permissions = await _context.Roles
        //    .Join(_context.RoleClaims,

        //        role => role.Id,
        //        roleClaim => roleClaim.RoleId,
        //        (role, roleClaim) => new { role, roleClaim }
        //    ).Where(x => userRoles.Contains(x.role.Name))
        //    .Select(x => x.roleClaim.ClaimValue!)
        //    .Distinct()
        //    .ToListAsync(cancellationToken);

        var permessions = await (from role in _context.Roles
                                 join roleClaim in _context.RoleClaims
                                 on role.Id equals roleClaim.RoleId
                                 where userRoles.Contains(role.Name)
                                 select roleClaim.ClaimValue)
                                 .Distinct()
                                 .ToListAsync(cancellationToken);

        return permessions;
    }


    private RefreshToken GenerateRefreshToken()
    {
        return new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiresOn = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpiryDays)
        };
    }

    private async Task sendConfirmationEmailAsync(ApplicationUser user, string code)
    {
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
        //var baseUrl = _configuration["AppSettings:BaseUrl"];
        var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",

                new Dictionary<string, string>
                {
                    { "{UserName}", $"{user.FirstName} {user.LastName}"},
                    { "[CONFIRMATION_LINK]", $"{origin}/auth/emailConfirmation?userId={user.Id}&code={code}" }
                }                                                                // also this can be send in header from frontend, and can be set in appsettings.json
            );

        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email, "Survey Basket: Email Confirmation ✅", emailBody));

        await Task.CompletedTask;   
    }

    private async Task sendResetPasswordEmail(ApplicationUser user, string code)
    {
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
        //var baseUrl = _configuration["AppSettings:BaseUrl"];
        var emailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword", 

                new Dictionary<string, string>
                {
                    { "{UserName}", $"{user.FirstName} {user.LastName}"},
                    { "[RESET_PASSWORD_LINK]", $"{origin}/auth/forgetPassword?userId={user.Id}&code={code}" }
                }                                                                // also this can be send in header from frontend, and can be set in appsettings.json
            );
        _logger.LogInformation("Sending email to userId: {userId}", user.Id);

        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email, "Survey Basket: Reset Password 🔄", emailBody));

        await Task.CompletedTask; 
    }

}