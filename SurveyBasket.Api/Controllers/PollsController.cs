using Microsoft.AspNetCore.Authorization;

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
        var polls = await _pollService.GetAllAsync(cancellationToken);
        var pollsResponse = polls.Adapt<IEnumerable<PollResponse>>();
        return Ok(pollsResponse);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var poll = await _pollService.GetAsync(id, cancellationToken);
        if (poll is null)
            return NotFound();

        var pollResponse = poll.Adapt<PollResponse>();
        return Ok(pollResponse);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] PollRequest request, CancellationToken cancellationToken)
    {
        var newPoll = await _pollService.AddAsync(request.Adapt<Poll>(), cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = newPoll.Id }, newPoll.Adapt<PollResponse>());
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken)
    {
        bool isUpdated = await _pollService.UpdateAsync(id, request.Adapt<Poll>(), cancellationToken);
        if (!isUpdated)
            return NotFound();
        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        bool isDeleted = await _pollService.DeleteAsync(id, cancellationToken);
        if (!isDeleted)
            return NotFound();
        return NoContent();
    }

    [HttpPatch("{id}/toggle-publish-status")]
    public async Task<IActionResult> TogglePublishStatus([FromRoute] int id, CancellationToken cancellationToken)
    {
        bool isToggled = await _pollService.TogglePublishStatusAsync(id, cancellationToken);
        if (!isToggled)
            return NotFound();
        return NoContent();
    }

}
