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
        new DataFieldModel{ Field = "id", Header = "Id", DbField = "R.Id", FieldType = TField.TString, DSName = string.Empty, IsSortable = false, IsGlobalFilterable = false, IsFilterable = false, Visible = false, SortOrder = 0 },
        new DataFieldModel{ Field = "name", Header = "Name", DbField = "R.Name", FieldType = TField.TString, DSName = string.Empty, IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, Visible = true, SortOrder = 1 },
    ];


}
