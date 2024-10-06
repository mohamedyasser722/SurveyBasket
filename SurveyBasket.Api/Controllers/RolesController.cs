

namespace SurveyBasket.Api.Controllers;

[ApiController]
[Route("api/v{v:apiVersion}/[controller]")]
public class RolesController(IRoleService roleService) : ControllerBase
{
    private readonly IRoleService _roleService = roleService;

    // Get all roles, supports filtering by includeDisabled
    [HttpGet]
    [HasPermission(Permissions.GetRoles)]
    public async Task<IActionResult> GetAll([FromQuery] bool? includeDisabled = false, CancellationToken cancellationToken = default)
    {
        var roles = await _roleService.GetAllAsync(includeDisabled, cancellationToken);
        return Ok(roles);
    }

    // Get a specific role by id using route parameter
    [HttpGet("{id}")] // Change from "id" to "{id}"
    [HasPermission(Permissions.GetRoles)]
    public async Task<IActionResult> Get([FromRoute] string id)
    {
        var result = await _roleService.GetAsync(id);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    // Add a new role
    [HttpPost]
    [HasPermission(Permissions.AddRoles)]
    public async Task<IActionResult> AddAsync([FromBody] RollRequest request, CancellationToken cancellationToken)
    {
        var result = await _roleService.AddAsync(request, cancellationToken);
        return result.IsSuccess ? CreatedAtAction(nameof(Get), new { result.Value.Id }, result.Value) : result.ToProblem();
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.UpdateRoles)]
    public async Task<IActionResult> UpdateAsync([FromRoute] string id, [FromBody] RollRequest request)
    {
        var result = await _roleService.UpdateAsync(id, request);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    // toggle the IsDeleted property of a role

    [HttpPut("{id}/toggle-status")]
    [HasPermission(Permissions.UpdatePolls)]
    public async Task<IActionResult> ToggleIsDeleted([FromRoute] string id)
    {
        var result = await _roleService.ToggleStatusAsync(id);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }


}
