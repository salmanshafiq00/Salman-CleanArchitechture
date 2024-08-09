﻿using CleanArchitechture.Application.Common.Abstractions;
using CleanArchitechture.Application.Features.Admin.AppNotifications.Queries;
using Microsoft.AspNetCore.SignalR;

namespace CleanArchitechture.Infrastructure.Communications;

public class NotificationHub: Hub<INotificationHub>
{
    public override async Task OnConnectedAsync()
    {
        await Clients.All.ReceiveNotification(new AppNotificationModel { Title = "SignalR Notification", Description = "First SignalR Notification"});
    }
    public async Task SendNotification(AppNotificationModel notificaiton)
    {
        await Clients.All.ReceiveNotification(notificaiton);
    }
}