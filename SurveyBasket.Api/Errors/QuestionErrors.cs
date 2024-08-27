namespace SurveyBasket.Api.Errors;

public static class QuestionErrors
{
    public static readonly Error QuestionNotFound = new("question_not_found", "The Question With The Given Id Was Not Found", StatusCodes.Status404NotFound);
    public static readonly Error DuplicatedQuestionContent = new("question_already_exists", "A Question With The Given Content Already Exists", StatusCodes.Status409Conflict);

}
