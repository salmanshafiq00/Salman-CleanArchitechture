using CleanArchitechture.Application.Common.Abstractions;
using CleanArchitechture.Application.Common.Security;
using CleanArchitechture.Application.Features.Admin.AppNotifications.Queries;
using Microsoft.AspNetCore.SignalR;

namespace CleanArchitechture.Infrastructure.Communications;

[Authorize]
public class NotificationHub: Hub<INotificationHub>
{
    public override async Task OnConnectedAsync()
    {
        await Clients.All.ReceiveNotification(new AppNotificationModel { Title = "Welcome", Description = "Welcome to SFS"});
    }
    public async Task SendNotification(AppNotificationModel notificaiton)
    {
        await Clients.All.ReceiveNotification(notificaiton);
    }
}
