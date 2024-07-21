using static CleanArchitechture.Application.Common.DapperQueries.Constants;

namespace CleanArchitechture.Application.Features.Admin.AppUsers.Queries;

public record AppUserModel
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Status { get; set; } = string.Empty;
    public string AssignedRoles { get; set; } = string.Empty;
    public Dictionary<string, object> OptionsDataSources { get; set; } = [];

    public IList<string>? Roles { get; set; } = [];

    public static IReadOnlyCollection<DataFieldModel> DataFields = [
        new DataFieldModel{ FieldName = "id", Caption = "Id", DbField = "U.Id", FieldType = TField.TString, DSName = string.Empty, IsSortable = false, IsGlobalFilterable = false, IsFilterable = false, IsVisible = false, SortOrder = 0 },
        new DataFieldModel{ FieldName = "username", Caption = "Username", DbField = "U.Username", FieldType = TField.TString, DSName = string.Empty, IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, IsVisible = true, SortOrder = 1 },
        new DataFieldModel{ FieldName = "email", Caption = "Email", DbField = "U.Email", FieldType = TField.TString, DSName = string.Empty,  IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, IsVisible = true,  SortOrder = 2 },
        new DataFieldModel{ FieldName = "firstName", Caption = "First Name", DbField = "U.FirstName", FieldType = TField.TString, DSName = string.Empty,  IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, IsVisible = true, SortOrder = 3 },
        new DataFieldModel{ FieldName = "lastName", Caption = "Last Name", DbField = "P.LastName", FieldType = TField.TString, DSName = string.Empty, IsSortable = false, IsGlobalFilterable = false,  IsFilterable = false, IsVisible = true, SortOrder = 4 },
        new DataFieldModel{ FieldName = "phoneNumber", Caption = "Phone Number", DbField = "U.PhoneNumber", FieldType = TField.TString, DSName = string.Empty, IsSortable = true,  IsGlobalFilterable = false, IsFilterable = true, IsVisible = true, SortOrder = 5 },
        new DataFieldModel{ FieldName = "photoUrl", Caption = "Photo", DbField = "U.PhotoUrl", FieldType = TField.TString, DSName = string.Empty, IsSortable = true,  IsGlobalFilterable = true, IsFilterable = true, IsVisible = true, SortOrder = 6 },
        new DataFieldModel{ FieldName = "status", Caption = "Status", DbField = "U.IsActive", FieldType = TField.TSelect, DSName = "statusSelectList", IsSortable = true,  IsGlobalFilterable = false, IsFilterable = true, IsVisible = true, SortOrder = 7 },
        new DataFieldModel{ FieldName = "assignedRoles", Caption = "Roles", DbField = "UR.RoleId", FieldType = TField.TMultiSelect, DSName = "roleSelectList", IsSortable = true,  IsGlobalFilterable = true, IsFilterable = true, IsVisible = true, SortOrder = 8 },
    ];


}
