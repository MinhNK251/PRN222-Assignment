using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AirWaterStore.Web.Helper;

public static class PageModelExtentions
{
    public static int GetCurrentUserId(this PageModel page)
    {
        return page.HttpContext.Session.GetInt32(SessionParams.UserId) ?? 0;
    }
    // public static string GetCurrentUserName(this PageModel page)
    // {
    //     return page.HttpContext.Session.GetString(SessionParams.UserName) ?? "Unknown User";
    // }
    public static int? GetCurrentUserRole(this PageModel page)
    {
        return page.HttpContext.Session.GetInt32(SessionParams.UserRole);
    }
    public static bool IsAuthenticated(this PageModel page)
    {
        return page.HttpContext.Session.GetInt32(SessionParams.UserId).HasValue;
    }
    public static bool IsCustomer(this PageModel page)
    {
        return page.HttpContext.Session.GetInt32(SessionParams.UserRole) == UserRole.Customer;
    }
    public static bool IsStaff(this PageModel page)
    {
        return page.HttpContext.Session.GetInt32(SessionParams.UserRole) == UserRole.Staff;
    }
}
