﻿using CleanArchitechture.Application.Common.Models;
using CleanArchitechture.Application.Features.Common.Queries;

namespace CleanArchitechture.Web.Endpoints;

public class SelectLists : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetLookupSelectList, "GetLookupSelectList")
            .MapGet(GetLookupDetailSelectList, "GetLookupDetailSelectList")
            .MapGet(GetRoleSelectList, "GetRoleSelectList")
            .MapGet(GetMenuTypeSelectList, "GetMenuTypeSelectList");
    }

    [ProducesResponseType(typeof(List<SelectListModel>), StatusCodes.Status200OK)]
    public async Task<List<SelectListModel>> GetLookupSelectList(ISender sender, [FromQuery] bool? allowCache = null)
    {
        var result = await sender.Send(new GetSelectListQuery(
                Sql: SelectListSqls.GetLookupSelectListSql,
                Parameters: new { },
                Key: CacheKeys.Lookup_All_SelectList,
                AllowCacheList: allowCache)
            );
        return result.Value;
    }

    [ProducesResponseType(typeof(List<SelectListModel>), StatusCodes.Status200OK)]
    public async Task<List<SelectListModel>> GetMenuTypeSelectList(ISender sender, [FromQuery] bool? allowCache = null)
    {
        var result = await sender.Send(new GetSelectListQuery(
                Sql: SelectListSqls.GetLookupDetailSelectListByDevCodeSql,
                Parameters: new {DevCode = 1001 },
                Key: CacheKeys.LookupDetail_All_SelectList,
                AllowCacheList: allowCache)
            );
        return result.Value;
    }

    [ProducesResponseType(typeof(List<SelectListModel>), StatusCodes.Status200OK)]
    public async Task<List<SelectListModel>> GetLookupDetailSelectList(ISender sender, [FromQuery] bool? allowCache = null)
    {
        var result = await sender.Send(new GetSelectListQuery(
                Sql: SelectListSqls.GetLookupDetailSelectListSql,
                Parameters: new { },
                Key: CacheKeys.LookupDetail_All_SelectList,
                AllowCacheList: allowCache));

        return result.Value;
    }

    [ProducesResponseType(typeof(List<SelectListModel>), StatusCodes.Status200OK)]
    public async Task<List<SelectListModel>> GetRoleSelectList(ISender sender, [FromQuery] bool? allowCache = null)
    {
        var result = await sender.Send(new GetSelectListQuery(
                Sql: SelectListSqls.GetRoleSelectListSql,
                Parameters: new { },
                Key: CacheKeys.Role_All_SelectList,
                AllowCacheList: allowCache));

        return result.Value;
    }
}
