namespace AirWaterStore.Web.Helper;

public static class UserRole
{
    public const int Customer = 1;
    public const int Staff = 2;
    public const int Admin = 3;

    public static string GetRoleName(int roleId)
    {
        return roleId switch
        {
            Customer => "Customer",
            Staff => "Staff",
            Admin => "Admin",
            _ => "Unknown"
        };
    }
}
