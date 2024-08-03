using CleanArchitechture.Application.Common.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace CleanArchitechture.Infrastructure.Communications;

public class NotificationHub: Hub<INotificationHub>
{
    public override async Task OnConnectedAsync()
    {
        await Clients.All.ReceiveNotification("Welcome to SFS");
    }
    public async Task SendNotification(string content)
    {
        await Clients.All.ReceiveNotification(content);
    }
}
