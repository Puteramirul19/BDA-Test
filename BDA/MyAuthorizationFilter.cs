using BDA.Identity;
using Hangfire.Dashboard;

namespace BDA.Web
{
    public class MyHangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public MyHangfireAuthorizationFilter()
        {
        }

        public static PermissionChecker permissionChecker = new PermissionChecker();

        public bool Authorize(DashboardContext context)
        {
            //return permissionChecker.Check(context.GetHttpContext().User, Permission.SuperUser);
            var httpContext = context.GetHttpContext();

            // Allow all authenticated users to see the Dashboard (potentially dangerous).
            return httpContext.User.Identity.IsAuthenticated;
        }
    }
}