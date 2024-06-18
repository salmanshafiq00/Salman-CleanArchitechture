using CleanArchitechture.Domain.Admin;
using static CleanArchitechture.Application.Common.DapperQueries.Constants;

namespace CleanArchitechture.Application.Features.Admin.AppMenus.Queries;

public record AppMenuModel
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string ParentName { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Tooltip { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int OrderNo { get; set; }
    public bool Visible { get; set; }
    public string Visibility { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Active { get; set; } = string.Empty;
    public Dictionary<string, object> OptionsDataSources { get; set; } = [];

    public static IReadOnlyCollection<DataFieldModel> DataFields = [
        new DataFieldModel{ Field = "id", Header = "Id", FieldType = TField.TString, DSName = string.Empty, DbField = "M.Id", IsSortable = false, IsGlobalFilterable = false, IsFilterable = false, Visible = false, SortOrder = 0 },
        new DataFieldModel{ Field = "label", Header = "Label", FieldType = TField.TString, DSName = string.Empty, DbField = "M.Label", IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, Visible = true, SortOrder = 1 },
        new DataFieldModel{ Field = "url", Header = "URL", FieldType = TField.TString, DSName = string.Empty, DbField = "M.URL", IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, Visible = true,  SortOrder = 2 },
        new DataFieldModel{ Field = "parentName", Header = "Parent Name", FieldType = TField.TMultiSelect, DSName = "parentSelectList", DbField = "M.ParentId", IsSortable = true, IsGlobalFilterable = false, IsFilterable = true, Visible = true, SortOrder = 3 },
        new DataFieldModel{ Field = "active", Header = "Active", FieldType = TField.TSelect, DSName = "statusSelectList", DbField = "M.IsActive", IsSortable = true,  IsGlobalFilterable = false, IsFilterable = true, Visible = true, SortOrder = 4 },
        new DataFieldModel{ Field = "visibility", Header = "Visible", FieldType = TField.TSelect, DSName = string.Empty, DbField = "M.Visible", IsSortable = true,  IsGlobalFilterable = false, IsFilterable = true, Visible = true, SortOrder = 5 },
        new DataFieldModel{ Field = "description", Header = "Description", FieldType = TField.TString, DSName = string.Empty, DbField = "P.Description", IsSortable = false, IsGlobalFilterable = false,  IsFilterable = false, Visible = true, SortOrder = 6 },
    ];

    private class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<AppMenu, AppMenuModel>().ReverseMap();
        }
    }
}
