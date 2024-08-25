﻿
using CleanArchitechture.Application.Common.Abstractions.Identity;
using CleanArchitechture.Application.Features.Admin.AppNotifications.Queries;

namespace CleanArchitechture.Web.Endpoints.Admin;

public class AppNotifications : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost("GetAll", GetAll)
                .WithName("GetAppNotifications")
                .Produces<PaginatedResponse<AppNotificationModel>>(StatusCodes.Status200OK);

        app.MapGroup(this)
            .MapGet("GetByUser", GetByUser)
                .WithName("GetAppNotification")
                .Produces<List<AppNotificationModel>>(StatusCodes.Status200OK);

    }

    public async Task<IResult> GetAll(ISender sender, [FromBody]GetAppNotificationListQuery query)
    {
        var result = await sender.Send(query)
            .ConfigureAwait(false);

        return TypedResults.Ok(result.Value);
    }

    public async Task<IResult> GetByUser(ISender sender, IUser user)
    {
        var result = await sender.Send(new GetAppNotificationsByUserIdQuery(user.Id))
            .ConfigureAwait(false);

        return TypedResults.Ok(result.Value);
    }

}
