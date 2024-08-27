using SurveyBasket.Api.Contracts.Polls;

namespace SurveyBasket.Api.Controllers
{
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
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken = default)
        {
            var pollsResponse = await _pollService.GetCurrentAsync(cancellationToken);
            return Ok(pollsResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken = default)
        {
            var pollResponse = await _pollService.GetAsync(id, cancellationToken);
            if (pollResponse.IsFailure)
                return pollResponse.ToProblem();

            return Ok(pollResponse.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] PollRequest request, CancellationToken cancellationToken)
        {
            var pollResult = await _pollService.AddAsync(request, cancellationToken);
            if(pollResult.IsFailure)
                return pollResult.ToProblem();

            var pollResponse = pollResult.Value;

            return CreatedAtAction(nameof(Get), new { id = pollResponse.Id }, pollResponse);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken)
        {
            Result isUpdated = await _pollService.UpdateAsync(id, request.Adapt<Poll>(), cancellationToken);
            if (isUpdated.IsFailure)
                return isUpdated.ToProblem();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            Result isDeleted = await _pollService.DeleteAsync(id, cancellationToken);
            if (isDeleted.IsFailure)
                return isDeleted.ToProblem();

            return NoContent();
        }

        [HttpPatch("{id}/toggle-publish-status")]
        public async Task<IActionResult> TogglePublishStatus([FromRoute] int id, CancellationToken cancellationToken)
        {
            Result isToggled = await _pollService.TogglePublishStatusAsync(id, cancellationToken);
            if (isToggled.IsFailure)
                return isToggled.ToProblem();

            return NoContent();
        }
    }
}

