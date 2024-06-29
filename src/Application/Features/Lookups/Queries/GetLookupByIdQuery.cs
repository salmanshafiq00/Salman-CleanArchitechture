﻿using System.Text.Json.Serialization;
using CleanArchitechture.Application.Common.Extensions;
using static CleanArchitechture.Application.Common.DapperQueries.SqlConstants;

namespace CleanArchitechture.Application.Features.Lookups.Queries;

[Authorize(Policy = Permissions.CommonSetup.Lookups.View)]
public record GetLookupByIdQuery(Guid? Id) : ICacheableQuery<LookupModel>
{
    [JsonIgnore]
    public string CacheKey => $"Lookup_{Id}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => false;

}

internal sealed class GetLookupByIdQueryHandler(ISqlConnectionFactory sqlConnection)
    : IQueryHandler<GetLookupByIdQuery, LookupModel>
{
    public async Task<Result<LookupModel>> Handle(GetLookupByIdQuery query, CancellationToken cancellationToken)
    {
        if (query.Id.IsNullOrEmpty())
        {
            return new LookupModel()
            {
                Created = DateTime.Now,
                CreatedDate = DateOnly.FromDateTime(DateTime.Now.Date.AddDays(-1)),
                CreatedTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(15)),
                CreatedYear = DateTime.Now.Year,
                Subjects = ["A"],
                SubjectRadio = "A"

            };
        }
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT 
                L.Id AS {nameof(LookupModel.Id)}, 
                L.Name AS {nameof(LookupModel.Name)}, 
                L.Code {nameof(LookupModel.Code)}, 
                L.ParentId AS {nameof(LookupModel.ParentId)}, 
                L.Description AS {nameof(LookupModel.Description)},
                L.Status AS {nameof(LookupModel.Status)},
                --{S.CONV}(DATE, L.Created) AS {nameof(LookupModel.Created)},
                CAST(L.Created AS DATE) AS {nameof(LookupModel.CreatedDate)},
                CAST(L.Created AS TIME) AS {nameof(LookupModel.CreatedTime)},
                L.Created AS {nameof(LookupModel.Created)},
                YEAR(L.Created) AS {nameof(LookupModel.CreatedYear)}
            FROM dbo.Lookups AS l
            LEFT JOIN dbo.Lookups AS p ON p.Id = l.ParentId
            WHERE l.Id = @Id
            """;

        return await connection.QueryFirstOrDefaultAsync<LookupModel>(sql, new { query.Id });
    }
}
