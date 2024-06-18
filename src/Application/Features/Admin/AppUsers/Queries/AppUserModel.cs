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
        new DataFieldModel{ Field = "id", Header = "Id", DbField = "U.Id", FieldType = TField.TString, DSName = string.Empty, IsSortable = false, IsGlobalFilterable = false, IsFilterable = false, Visible = false, SortOrder = 0 },
        new DataFieldModel{ Field = "username", Header = "Username", DbField = "U.Username", FieldType = TField.TString, DSName = string.Empty, IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, Visible = true, SortOrder = 1 },
        new DataFieldModel{ Field = "email", Header = "Email", DbField = "U.Email", FieldType = TField.TString, DSName = string.Empty,  IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, Visible = true,  SortOrder = 2 },
        new DataFieldModel{ Field = "firstName", Header = "First Name", DbField = "U.FirstName", FieldType = TField.TString, DSName = string.Empty,  IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, Visible = true, SortOrder = 3 },
        new DataFieldModel{ Field = "lastName", Header = "Last Name", DbField = "P.LastName", FieldType = TField.TString, DSName = string.Empty, IsSortable = false, IsGlobalFilterable = false,  IsFilterable = false, Visible = true, SortOrder = 4 },
        new DataFieldModel{ Field = "phoneNumber", Header = "Phone Number", DbField = "U.PhoneNumber", FieldType = TField.TString, DSName = string.Empty, IsSortable = true,  IsGlobalFilterable = false, IsFilterable = true, Visible = true, SortOrder = 5 },
        new DataFieldModel{ Field = "photoUrl", Header = "Photo", DbField = "U.PhotoUrl", FieldType = TField.TString, DSName = string.Empty, IsSortable = true,  IsGlobalFilterable = true, IsFilterable = true, Visible = true, SortOrder = 6 },
        new DataFieldModel{ Field = "status", Header = "Status", DbField = "U.IsActive", FieldType = TField.TSelect, DSName = "statusSelectList", IsSortable = true,  IsGlobalFilterable = false, IsFilterable = true, Visible = true, SortOrder = 7 },
        new DataFieldModel{ Field = "assignedRoles", Header = "Roles", DbField = "UR.RoleId", FieldType = TField.TMultiSelect, DSName = "roleSelectList", IsSortable = true,  IsGlobalFilterable = true, IsFilterable = true, Visible = true, SortOrder = 8 },
    ];


}
