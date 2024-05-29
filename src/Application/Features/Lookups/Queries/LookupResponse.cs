using CleanArchitechture.Domain.Common;
using Microsoft.VisualBasic.FileIO;
using static CleanArchitechture.Application.Common.DapperQueries.Constants;

namespace CleanArchitechture.Application.Features.Lookups.Queries;

public record LookupResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public string ParentName { get; set; } = string.Empty;

    public static IReadOnlyCollection<DataFieldModel> DataFields = [
        new DataFieldModel{Field = "id", Header = "Id", DataType = FieldDataType.Type_String, DbField = "L.Id", IsSortable = false, IsGlobalFilterable = false, IsFilterable = false, Visible = false, SortOrder = 0 },
        new DataFieldModel{Field = "code", Header = "Code", DataType = FieldDataType.Type_String, DbField = "L.Code", IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, Visible = true, SortOrder = 1 },
        new DataFieldModel{Field = "name", Header = "Name", DataType = FieldDataType.Type_String, DbField = "L.Name", IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, Visible = true,  SortOrder = 2 },
        new DataFieldModel{Field = "parentName", Header = "Parent Name", DataType = FieldDataType.Type_String, DbField = "P.Name", IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, Visible = true, SortOrder = 3 },
        new DataFieldModel{Field = "description", Header = "Description", DataType = FieldDataType.Type_String, DbField = "P.Description", IsSortable = false, IsGlobalFilterable = false,  IsFilterable = false, Visible = true, SortOrder = 4 },
        new DataFieldModel{Field = "statusName", Header = "Status", DataType = FieldDataType.Type_Select, DbField = "L.Status", IsSortable = true,  IsGlobalFilterable = false, IsFilterable = true, Visible = true, SortOrder = 5 },
    ];

    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Lookup, LookupResponse>().ReverseMap();
        }
    }
}
