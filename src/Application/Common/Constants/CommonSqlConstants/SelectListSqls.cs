namespace CleanArchitechture.Application.Common.Constants.CommonSqlConstants;

public static class SelectListSqls
{
    public const string GetLookupSelectListSql = """
        SELECT Id, Name 
        FROM dbo.Lookups l
        WHERE 1 = 1
        ORDER BY Name
        """;

    public const string GetLookupDetailSelectListSql = """
        SELECT Id, Name 
        FROM dbo.LookupDetails ld
        WHERE 1 = 1
        ORDER BY Name
        """;

    public const string GetRoleSelectListSql = """
        SELECT Id, Name 
        FROM [identity].Roles r
        WHERE 1 = 1
        ORDER BY Name
        """;

    public const string GetAppMenuSelectListSql = """
        SELECT Id, Label AS Name
        FROM [dbo].AppMenus 
        WHERE 1 = 1
        ORDER BY Label
        """;
}
