namespace SurveyBasket.Api.Services.Services.Interfaces;

public interface INotificationService
{
    Task SendNewPollsNotification(int? pollId = null);
}
