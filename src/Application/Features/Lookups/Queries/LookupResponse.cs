using System.Text.Json.Serialization;
using CleanArchitechture.Domain.Common;

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
        new DataFieldModel{Field = "id", Header = "Id", DataType = "Guid", DbField = "L.Id", DbDataType = "string", IsSortable = false, IsGlobalFilterable = true, IsFilterable = false, Visible = false },
        new DataFieldModel{Field = "code", Header = "Code", DataType = "string", DbField = "L.Code", DbDataType = "string", IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, Visible = true },
        new DataFieldModel{Field = "name", Header = "Name", DataType = "string", DbField = "L.Name", DbDataType = "string", IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, Visible = true },
        new DataFieldModel{Field = "parentName", Header = "Parent Name", DataType = "string", DbField = "P.Name", DbDataType = "string", IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, Visible = true },
        new DataFieldModel{Field = "description", Header = "Description", DataType = "string", DbField = "P.Description", DbDataType = "string", IsSortable = false, IsGlobalFilterable = false,  IsFilterable = false, Visible = true },
        new DataFieldModel{Field = "statusName", Header = "Status", DataType = "string", DbField = "L.Status", DbDataType = "bit", IsSortable = true,  IsGlobalFilterable = true, IsFilterable = true, Visible = true },
    ];

    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Lookup, LookupResponse>().ReverseMap();
        }
    }
}
