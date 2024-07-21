using static CleanArchitechture.Application.Common.DapperQueries.Constants;

namespace CleanArchitechture.Application.Features.Admin.Roles.Queries;

public record RoleModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<Guid> RoleMenus { get; set; } = [];
    public List<string> Permissions { get; set; } = [];

    public Dictionary<string, object> OptionsDataSources { get; set; } = [];

    public static IReadOnlyCollection<DataFieldModel> DataFields = [
        new DataFieldModel{ FieldName = "id", Caption = "Id", DbField = "R.Id", FieldType = TField.TString, DSName = string.Empty, IsSortable = false, IsGlobalFilterable = false, IsFilterable = false, IsVisible = false, SortOrder = 0 },
        new DataFieldModel{ FieldName = "name", Caption = "Name", DbField = "R.Name", FieldType = TField.TString, DSName = string.Empty, IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, IsVisible = true, SortOrder = 1 },
    ];


}
