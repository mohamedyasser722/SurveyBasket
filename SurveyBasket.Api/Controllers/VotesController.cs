using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SurveyBasket.Api.Services.Services.Interfaces;

namespace SurveyBasket.Api.Controllers;
[Route("api/polls/{pollId}/vote")]
[ApiController]
[Authorize]
public class VotesController(IQuestionService questionService, IVoteService voteService) : ControllerBase
{
    private readonly IQuestionService _questionService = questionService;
    private readonly IVoteService _voteService = voteService;

    [HttpGet()]
    [OutputCache(Duration = 60)]
    public async Task<IActionResult> Start([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        string userId = User.GetUserId()!;

        var result = await _questionService.GetAvailableAsync(pollId, userId, cancellationToken);
        if(result.IsFailure)
            return result.ToProblem();

        return Ok(result.Value);
    }

    [HttpPost()]
    public async Task<IActionResult> Vote([FromRoute] int pollId, [FromBody] VoteRequest request, CancellationToken cancellationToken)
    {

        var result = await _voteService.AddAsync(pollId, User.GetUserId()!, request, cancellationToken);
        if (result.IsFailure)
            return result.ToProblem();

        return Created();
    }

    

}
