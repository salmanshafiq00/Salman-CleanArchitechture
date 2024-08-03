using CleanArchitechture.Domain.Admin;

namespace CleanArchitechture.Application.Common.Abstractions;

internal interface IAppNotificationService
{
    int Create(AppNotification notification);
    int Update(AppNotification notification);
    int UpdateAllUnseen(AppNotification notification);
}
