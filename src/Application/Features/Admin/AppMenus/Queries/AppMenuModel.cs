using CleanArchitechture.Domain.Admin;
using static CleanArchitechture.Application.Common.DapperQueries.Constants;

namespace CleanArchitechture.Application.Features.Admin.AppMenus.Queries;

public record AppMenuModel
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string ParentName { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string RouterLink { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Tooltip { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int OrderNo { get; set; }
    public bool Visible { get; set; }
    public string Visibility { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Active { get; set; } = string.Empty;
    public Guid MenuTypeId { get; set; }
    public string MenuTypeName { get; set; } = string.Empty;
    public Dictionary<string, object> OptionsDataSources { get; set; } = [];

    public static IReadOnlyCollection<DataFieldModel> DataFields = [
        new DataFieldModel{ FieldName = "id", Caption = "Id", FieldType = TField.TString, DSName = string.Empty, DbField = "M.Id", IsSortable = false, IsGlobalFilterable = false, IsFilterable = false, IsVisible = false, SortOrder = 0 },
        new DataFieldModel{ FieldName = "label", Caption = "Label", FieldType = TField.TString, DSName = string.Empty, DbField = "M.Label", IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, IsVisible = true, SortOrder = 1 },
        new DataFieldModel{ FieldName = "routerLink", Caption = "RouterLink", FieldType = TField.TString, DSName = string.Empty, DbField = "M.RouterLink", IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, IsVisible = true,  SortOrder = 2 },
        new DataFieldModel{ FieldName = "parentName", Caption = "Parent Name", FieldType = TField.TMultiSelect, DSName = "parentSelectList", DbField = "M.ParentId", IsSortable = true, IsGlobalFilterable = false, IsFilterable = true, IsVisible = true, SortOrder = 3 },
        new DataFieldModel{ FieldName = "active", Caption = "Active", FieldType = TField.TSelect, DSName = "statusSelectList", DbField = "M.IsActive", IsSortable = true,  IsGlobalFilterable = false, IsFilterable = true, IsVisible = true, SortOrder = 4 },
        new DataFieldModel{ FieldName = "visibility", Caption = "Visible", FieldType = TField.TSelect, DSName = string.Empty, DbField = "M.Visible", IsSortable = true,  IsGlobalFilterable = false, IsFilterable = true, IsVisible = true, SortOrder = 5 },
        new DataFieldModel{ FieldName = "description", Caption = "Description", FieldType = TField.TString, DSName = string.Empty, DbField = "P.Description", IsSortable = false, IsGlobalFilterable = false,  IsFilterable = false, IsVisible = false, SortOrder = 6 },
    ];

    private class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<AppMenu, AppMenuModel>().ReverseMap();
        }
    }
}
