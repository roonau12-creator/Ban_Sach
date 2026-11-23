namespace BanSach.Models;

public static class UserRoles
{
    public const string Administrator = "Administrator";
    public const string Employee = "Employee";
    public const string Customer = "Customer";
}

public static class AuthorizationPolicies
{
    public const string ManageBooks = "ManageBooks";
    public const string ManageStaff = "ManageStaff";
    public const string ManageSystemSettings = "ManageSystemSettings";
    public const string ViewRevenueReports = "ViewRevenueReports";
}
