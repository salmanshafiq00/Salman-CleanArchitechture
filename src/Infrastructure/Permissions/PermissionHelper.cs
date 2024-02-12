using System.Reflection;

namespace WebApi.Infrastructure.Permissions;

public static class PermissionHelper
{
    public static void GetAllConstantPermissions(this List<string> permissions, Type policy)
    {
        FieldInfo[] fields = policy.GetFields(BindingFlags.Static | BindingFlags.Public);

        foreach (FieldInfo field in fields)
        {
            permissions.Add(field.GetValue(null).ToString());
        }
    }
}
