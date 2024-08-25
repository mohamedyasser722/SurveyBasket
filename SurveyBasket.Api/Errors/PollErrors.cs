namespace SurveyBasket.Api.Errors;

public class PollErrors
{
    public static readonly Error PollNotFound = new("poll_not_found", "No Poll Was Found With The Given Id", StatusCodes.Status404NotFound);
  
}
