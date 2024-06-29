using CleanArchitechture.Domain.Common;
using static CleanArchitechture.Application.Common.DapperQueries.Constants;

namespace CleanArchitechture.Application.Features.Lookups.Queries;

public record LookupModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public string ParentName { get; set; } = string.Empty;
    public DateTime? Created { get; set; }
    public DateOnly? CreatedDate { get; set; }
    public TimeOnly? CreatedTime { get; set; }
    public int? CreatedYear { get; set; }
    public List<string> Subjects { get; set; } = [];
    public string SubjectRadio { get; set; } = string.Empty;
    public Dictionary<string, object> OptionDataSources { get; set; } = [];

    public static IReadOnlyCollection<DataFieldModel> DataFields = [
        new DataFieldModel{ Field = "id", Header = "Id", FieldType = TField.TString, DSName = string.Empty, DbField = "L.Id", IsSortable = false, IsGlobalFilterable = false, IsFilterable = false, Visible = false, SortOrder = 0 },
        new DataFieldModel{ Field = "code", Header = "Code", FieldType = TField.TString, DSName = string.Empty, DbField = "L.Code", IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, Visible = true, SortOrder = 1 },
        new DataFieldModel{ Field = "name", Header = "Name", FieldType = TField.TString, DSName = string.Empty, DbField = "L.Name", IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, Visible = true,  SortOrder = 2 },
        new DataFieldModel{ Field = "parentName", Header = "Parent Name", FieldType = TField.TMultiSelect, DSName = "parentSelectList", DbField = "L.ParentId", IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, Visible = true, SortOrder = 3 },
        new DataFieldModel{ Field = "description", Header = "Description", FieldType = TField.TString, DSName = string.Empty, DbField = "P.Description", IsSortable = false, IsGlobalFilterable = false,  IsFilterable = false, Visible = true, SortOrder = 4 },
        new DataFieldModel{ Field = "statusName", Header = "Status", FieldType = TField.TSelect, DSName = "statusSelectList", DbField = "L.Status", IsSortable = true,  IsGlobalFilterable = false, IsFilterable = true, Visible = true, SortOrder = 5 },
        new DataFieldModel{ Field = "created", Header = "Created", FieldType = TField.TDate, DSName = string.Empty, DbField = "L.Created", IsSortable = true,  IsGlobalFilterable = true, IsFilterable = true, Visible = true, SortOrder = 6 },
    ];

    private class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Lookup, LookupModel>().ReverseMap();
        }
    }
}
