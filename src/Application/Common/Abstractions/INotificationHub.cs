using CleanArchitechture.Application.Features.Admin.AppNotifications.Queries;

namespace CleanArchitechture.Application.Common.Abstractions;

public interface INotificationHub
{
    Task ReceiveNotification(AppNotificationModel notification);
    Task ReceiveRolePermissionNotify();
}
