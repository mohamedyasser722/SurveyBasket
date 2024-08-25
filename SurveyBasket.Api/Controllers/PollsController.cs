using SurveyBasket.Api.Contracts.Polls;

namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var pollsResponse = await _pollService.GetAllAsync(cancellationToken);
        return Ok(pollsResponse);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var pollResponse = await _pollService.GetAsync(id, cancellationToken);
        if (pollResponse.IsFailure)
            return Problem(statusCode: StatusCodes.Status404NotFound, title: pollResponse.Error.code, detail: pollResponse.Error.Description);

        return Ok(pollResponse.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] PollRequest request, CancellationToken cancellationToken)
    {
        var pollResponse = await _pollService.AddAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = pollResponse.Id }, pollResponse);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken)
    {
        Result isUpdated = await _pollService.UpdateAsync(id, request.Adapt<Poll>(), cancellationToken);
        if (isUpdated.IsFailure)
            return Problem(statusCode: StatusCodes.Status404NotFound, title: isUpdated.Error.code, detail: isUpdated.Error.Description);
        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        Result isDeleted = await _pollService.DeleteAsync(id, cancellationToken);
        if (isDeleted.IsFailure)
            return Problem(statusCode: StatusCodes.Status404NotFound, title: isDeleted.Error.code, detail: isDeleted.Error.Description);
        return NoContent();
    }

    [HttpPatch("{id}/toggle-publish-status")]
    public async Task<IActionResult> TogglePublishStatus([FromRoute] int id, CancellationToken cancellationToken)
    {
        Result isToggled = await _pollService.TogglePublishStatusAsync(id, cancellationToken);
        if (isToggled.IsFailure)
            return Problem(statusCode: StatusCodes.Status404NotFound, title: isToggled.Error.code, detail: isToggled.Error.Description);
        return NoContent();
    }

}
