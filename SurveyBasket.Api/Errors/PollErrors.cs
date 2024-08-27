namespace SurveyBasket.Api.Errors;

public class PollErrors
{
    public static readonly Error PollNotFound = new("poll_not_found", "No Poll Was Found With The Given Id", StatusCodes.Status404NotFound);
    public static readonly Error PollTitleExist = new("poll_title_exist", "A Poll With The Given Title Already Exists", StatusCodes.Status409Conflict);
}
