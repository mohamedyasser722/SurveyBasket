
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SurveyBasket.Api.Mapping;

namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;

    [HttpGet]
    public IActionResult GetAll()
    {
        var pollsResponse = _pollService.GetAll().AsQueryable().MapPollsToPollResponses();

        return Ok(pollsResponse);
    }

    [HttpGet("{id}")]
    public IActionResult Get([FromRoute] int id)
    {
        var poll = _pollService.Get(id);
        if (poll is null)
            return NotFound();

        var pollResponse = poll.Adapt<PollResponse>();
        return Ok(pollResponse);
    }

    [HttpPost]
    public IActionResult Add([FromBody] CreatePollRequest request)
    {
        var newPoll = _pollService.Add(request.Adapt<Poll>());
        return CreatedAtAction(nameof(Get), new { id = newPoll.Id }, newPoll);
    }
    [HttpPut("{id}")]
    public IActionResult Update([FromRoute] int id, [FromBody] CreatePollRequest request)
    {
        bool isUpdated = _pollService.Update(id, request.Adapt<Poll>());
        if (!isUpdated)
            return NotFound();
        return NoContent();
    }
    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] int id)
    {
        bool isDeleted = _pollService.Delete(id);
        if (!isDeleted)
            return NotFound();
        return NoContent();
    }


}
