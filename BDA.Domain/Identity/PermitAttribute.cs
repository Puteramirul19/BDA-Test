using Microsoft.AspNetCore.Authorization;

namespace BDA.Identity
{
    public class PermitAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Permit access to the supplied list of permissions.
        /// </summary>
        /// <param name="permissions">A list of permissions to authorize</param>
        public PermitAttribute(params Permission[] permissions)
        {
            Policy = $"{PermissionPolicyProvider.ClaimType}{string.Join(",", permissions)}";
        }
    }
}
