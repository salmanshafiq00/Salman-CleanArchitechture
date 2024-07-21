using CleanArchitechture.Domain.Common;
using Microsoft.AspNetCore.Http;
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
    public string Color { get; set; } = string.Empty;
    public IFormFile UploadFile { get; set; }
    public string DescEdit { get; set; } = string.Empty;
    public List<Guid> Menus { get; set; } = [];
    public Guid SingleMenu { get; set; }
    public List<Guid> TreeSelectMenus { get; set; } = [];
    public Guid TreeSelectSingleMenu { get; set; }

    public Dictionary<string, object> OptionDataSources { get; set; } = [];


    public static IReadOnlyCollection<DataFieldModel> DataFields = [
        new DataFieldModel{ FieldName = "id", Caption = "Id", FieldType = TField.TString, DSName = string.Empty, DbField = "L.Id", IsSortable = false, IsGlobalFilterable = false, IsFilterable = false, IsVisible = false, SortOrder = 0 },
        new DataFieldModel{ FieldName = "code", Caption = "Code", FieldType = TField.TString, DSName = string.Empty, DbField = "L.Code", IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, IsVisible = true, SortOrder = 1 },
        new DataFieldModel{ FieldName = "name", Caption = "Name", FieldType = TField.TString, DSName = string.Empty, DbField = "L.Name", IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, IsVisible = true,  SortOrder = 2 },
        new DataFieldModel{ FieldName = "parentName", Caption = "Parent Name", FieldType = TField.TMultiSelect, DSName = "parentSelectList", DbField = "L.ParentId", IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, IsVisible = true, SortOrder = 3 },
        new DataFieldModel{ FieldName = "description", Caption = "Description", FieldType = TField.TString, DSName = string.Empty, DbField = "P.Description", IsSortable = false, IsGlobalFilterable = false,  IsFilterable = false, IsVisible = true, SortOrder = 4 },
        new DataFieldModel{ FieldName = "statusName", Caption = "Status", FieldType = TField.TSelect, DSName = "statusSelectList", DbField = "L.Status", IsSortable = true,  IsGlobalFilterable = false, IsFilterable = true, IsVisible = true, SortOrder = 5 },
        new DataFieldModel{ FieldName = "created", Caption = "Created", FieldType = TField.TDate, DSName = string.Empty, DbField = "L.Created", IsSortable = true,  IsGlobalFilterable = true, IsFilterable = true, IsVisible = true, SortOrder = 6 },
    ];

    private class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Lookup, LookupModel>().ReverseMap();
        }
    }
}
