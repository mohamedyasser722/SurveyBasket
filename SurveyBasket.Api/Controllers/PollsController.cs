﻿

namespace SurveyBasket.Api.Controllers
{
    [ApiVersion(1)]
    [ApiVersion(2)]
    [Route("api/v{v:apiVersion}/[controller]")]
    [ApiController]
    public class PollsController(IPollService pollService) : ControllerBase
    {
        private readonly IPollService _pollService = pollService;

        [HttpGet]
        [HasPermission(Permissions.GetPolls)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            var pollsResponse = await _pollService.GetAllAsync(cancellationToken);
            return Ok(pollsResponse);
        }
        [MapToApiVersion(1)]
        [HttpGet("current")]
        [Authorize(Roles = DefaultRoles.Member.Name)]
        [EnableRateLimiting("userLimit")]
        public async Task<IActionResult> GetCurrentV1(CancellationToken cancellationToken = default)
        {
            var pollsResponse = await _pollService.GetCurrentAsyncV1(cancellationToken);
            return Ok(pollsResponse);
        }
        [MapToApiVersion(2)]
        [HttpGet("current")]
        [Authorize(Roles = DefaultRoles.Member.Name)]
        [EnableRateLimiting("userLimit")]
        public async Task<IActionResult> GetCurrentV2(CancellationToken cancellationToken = default)
        {
            var pollsResponse = await _pollService.GetCurrentAsyncV1(cancellationToken);
            return Ok(pollsResponse);
        }

        [HttpGet("{id}")]
        [HasPermission(Permissions.GetPolls)]
        public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken = default)
        {
            var pollResponse = await _pollService.GetAsync(id, cancellationToken);
            if (pollResponse.IsFailure)
                return pollResponse.ToProblem();

            return Ok(pollResponse.Value);
        }

        [HttpPost]
        [HasPermission(Permissions.AddPolls)]
        public async Task<IActionResult> Add([FromBody] PollRequest request, CancellationToken cancellationToken)
        {
            var pollResult = await _pollService.AddAsync(request, cancellationToken);
            if (pollResult.IsFailure)
                return pollResult.ToProblem();

            var pollResponse = pollResult.Value;

            return CreatedAtAction(nameof(Get), new { id = pollResponse.Id }, pollResponse);
        }

        [HttpPut("{id}")]
        [HasPermission(Permissions.UpdatePolls)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken)
        {
            Result isUpdated = await _pollService.UpdateAsync(id, request.Adapt<Poll>(), cancellationToken);
            if (isUpdated.IsFailure)
                return isUpdated.ToProblem();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [HasPermission(Permissions.DeletePolls)]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            Result isDeleted = await _pollService.DeleteAsync(id, cancellationToken);
            if (isDeleted.IsFailure)
                return isDeleted.ToProblem();

            return NoContent();
        }

        [HttpPatch("{id}/toggle-publish-status")]
        [HasPermission(Permissions.UpdatePolls)]
        public async Task<IActionResult> TogglePublishStatus([FromRoute] int id, CancellationToken cancellationToken)
        {
            Result isToggled = await _pollService.TogglePublishStatusAsync(id, cancellationToken);
            if (isToggled.IsFailure)
                return isToggled.ToProblem();

            return NoContent();
        }
    }
}

