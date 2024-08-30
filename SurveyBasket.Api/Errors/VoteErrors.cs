namespace SurveyBasket.Api.Errors;

public class VoteErrors
{
    public static readonly Error DuplicatedVoted = new("user_already_voted", "The User Has Already Voted For This Poll", StatusCodes.Status409Conflict);
    public static readonly Error InvalidQuestions = new("invalid_questions", "Invalid Questions", StatusCodes.Status400BadRequest);
}
