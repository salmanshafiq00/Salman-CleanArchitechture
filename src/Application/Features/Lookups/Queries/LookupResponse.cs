using CleanArchitechture.Domain.Common;
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

    //public static HashSet<string> DataFields = ["L.Code","L.Name", "P.Name", "L.Status"];
    //[JsonIgnore]
    public static IReadOnlyCollection<DataFieldModel> DataFields = [
        new DataFieldModel{Field = "id", Header = "Id", DataType = "Guid", DbField = "L.Id", DbDataType = FieldDataType.NVARCHAR, IsSortable = false, IsGlobalFilterable = false, IsFilterable = false, Visible = false, SortOrder = 0 },
        new DataFieldModel{Field = "code", Header = "Code", DataType = "string", DbField = "L.Code", DbDataType = FieldDataType.NVARCHAR, IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, Visible = true, SortOrder = 1 },
        new DataFieldModel{Field = "name", Header = "Name", DataType = "string", DbField = "L.Name", DbDataType = FieldDataType.NVARCHAR, IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, Visible = true,  SortOrder = 2 },
        new DataFieldModel{Field = "parentName", Header = "Parent Name", DataType = "string", DbField = "P.Name", DbDataType = FieldDataType.NVARCHAR, IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, Visible = true, SortOrder = 3 },
        new DataFieldModel{Field = "description", Header = "Description", DataType = "string", DbField = "P.Description", DbDataType = FieldDataType.NVARCHAR, IsSortable = false, IsGlobalFilterable = false,  IsFilterable = false, Visible = true, SortOrder = 4 },
        new DataFieldModel{Field = "statusName", Header = "Status", DataType = "dropdown", DbField = "L.Status", DbDataType = FieldDataType.BIT, IsSortable = true,  IsGlobalFilterable = false, IsFilterable = true, Visible = true, SortOrder = 5 },
    ];

    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Lookup, LookupResponse>().ReverseMap();
        }
    }
}
