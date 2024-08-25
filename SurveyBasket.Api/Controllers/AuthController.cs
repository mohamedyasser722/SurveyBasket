namespace SurveyBasket.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    //[HttpPost()]
    //public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
    //{
    //    var response = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);

    //    //if (response.IsFailure)
    //    //    return Problem(statusCode: StatusCodes.Status400BadRequest, title: response.Error.code, detail: response.Error.Description);

    //    //return Ok(response.Value);

    //    return response.Match<IActionResult>(
    //        authResponse => Ok(authResponse),
    //        error => Problem(statusCode: StatusCodes.Status400BadRequest, title: error.code, detail: error.Description)
    //    );
    //}
    [HttpPost()]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);

        if (response.IsFailure)
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: response.Error.code, detail: response.Error.Description);

        return Ok(response.Value);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        if (response.IsFailure)
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: response.Error.code, detail: response.Error.Description);

        return Ok(response.Value);
    }

    [HttpPost("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshTokenAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        Result isRevoked = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        if (isRevoked.IsFailure)
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: isRevoked.Error.code, detail: isRevoked.Error.Description);

        return NoContent();
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _authService.RegisterAsync(request.Email, request.Password, request.FirstName, request.LastName, cancellationToken);

        if (response is null)
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid Registration", detail: "Invalid email or password");

        return Ok(response);
    }
}
