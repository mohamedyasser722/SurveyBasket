namespace SurveyBasket.Api.Controllers;
[Route("api/v{v:apiVersion}/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet]
    [HasPermission(Permissions.GetUsers)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllAsync(cancellationToken);

        return Ok(users);
    }
    [HttpGet]
    [Route("{Id}")]
    [HasPermission(Permissions.GetUsers)]
    public async Task<IActionResult> Get([FromRoute] string Id, CancellationToken cancellationToken)
    {
        var result = await _userService.GetAsync(Id, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost]
    [HasPermission(Permissions.AddUsers)]
    public async Task<IActionResult> Add([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.AddAsync(request, cancellationToken);

        return result.IsSuccess ? CreatedAtAction(nameof(Get), new { Id = result.Value.Id }, result.Value) : result.ToProblem();
    }
    [HttpPut]
    [Route("{Id}")]
    [HasPermission(Permissions.UpdateUsers)]
    public async Task<IActionResult> Update([FromRoute] string Id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateAsync(Id, request, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPut]
    [Route("{Id}/toggle-status")]
    [HasPermission(Permissions.UpdateUsers)]
    public async Task<IActionResult> ToggleStatus([FromRoute] string Id)
    {
        var result = await _userService.ToggleStatusAsync(Id);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPut]
    [Route("{Id}/unlock")]
    [HasPermission(Permissions.UpdateUsers)]
    public async Task<IActionResult> Unlock([FromRoute] string Id)
    {
        var result = await _userService.UnlockUserAsync(Id);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
