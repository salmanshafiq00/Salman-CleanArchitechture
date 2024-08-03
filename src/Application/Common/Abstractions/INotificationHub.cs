namespace CleanArchitechture.Application.Common.Abstractions;

public interface INotificationHub
{
    Task ReceiveNotification(string message);
    Task<T> SendNotification<T>(string userId, T  message);
}
