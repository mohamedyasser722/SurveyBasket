using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SurveyBasket.Api.Controllers;
[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;


    [HttpPost()]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);

        if (response is null)
            return BadRequest("Invalid email or password");
        return Ok(response);
    }
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        if (response is null)
            return BadRequest("Invalid token or refresh token or token revoked");    

        return Ok(response);
    }
    [HttpPost("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshTokenAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        bool isRevoked = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        if (isRevoked is false)
            return BadRequest("Invalid token or refresh token or token revoked");

        return Ok();
    }
    // register
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _authService.RegisterAsync(request.Email, request.Password, request.FirstName, request.LastName, cancellationToken);

        if (response is null)
            return BadRequest("Invalid email or password");
        return Ok(response);
    }

} 
