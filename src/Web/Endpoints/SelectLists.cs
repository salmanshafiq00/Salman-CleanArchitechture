using CleanArchitechture.Application.Common.Caching;
using CleanArchitechture.Application.Common.Constants.CommonSqlConstants;
using CleanArchitechture.Application.Common.Models;
using CleanArchitechture.Application.Features.Common.Queries;
using CleanArchitechture.Web.Extensions;

namespace CleanArchitechture.Web.Endpoints;

public class SelectLists : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetLookupSelectList, "GetLookupSelectList")
            .MapGet(GetLookupDetailSelectList, "GetLookupDetailSelectList");
    }

    public async Task<List<SelectListModel>> GetLookupSelectList(ISender sender)
    {
        var result = await sender.Send(
            new GetSelectListQuery(SelectListSqls.GetLookupSelectListSql, 
            new { },
            CacheKeys.Lookup_All_SelectList));
        return result.Value;
    }

    public async Task<List<SelectListModel>> GetLookupDetailSelectList(ISender sender)
    {
        var result = await sender.Send(
            new GetSelectListQuery(SelectListSqls.GetLookupDetailSelectListSql,
            new { },
            CacheKeys.LookupDetail_All_SelectList));

        return result.Value;
    }
}
