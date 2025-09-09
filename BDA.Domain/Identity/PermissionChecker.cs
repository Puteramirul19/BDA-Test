using System.Linq;
using System.Security.Claims;

namespace BDA.Identity
{
    public class PermissionChecker
    {
        public bool Check(ClaimsPrincipal principal, params Permission[] permissions)
        {
            if (principal == null)
                return false;

            var hasPermission = false;
            foreach (var p in permissions)
            {
                var claim = p.ToClaim();
                hasPermission = hasPermission || principal.HasClaim(claim.Type, claim.Value);
                if (hasPermission)
                    return true;
            }
            return hasPermission;
        }

        public bool Check(ClaimsPrincipal principal, Permission permission, params Claim[] claims)
        {
            if (principal == null)
                return false;

            var hasPermission = principal.HasClaim(PermissionPolicyProvider.ClaimType, permission.ToString());
            if (claims == null || claims.Length == 0)
                return hasPermission;

            return hasPermission
                    && claims.All(checking => principal.HasClaim(claim => claim.Type == checking.Type && claim.Value == checking.Value));
        }
    }
}
