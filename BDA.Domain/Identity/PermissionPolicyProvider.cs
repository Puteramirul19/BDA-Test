using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace BDA.Identity
{
    //
    // Adapted from https://hackernoon.com/permission-based-authorization-asp-net-core-with-authorizationpolicyprovider-af4933d575ee
    //
    public class PermissionPolicyProvider:DefaultAuthorizationPolicyProvider, IAuthorizationPolicyProvider
    {
        public const string ClaimType = "__permission";

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
            : base(options)
        {
        }

        public override Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (!policyName.StartsWith(PermissionPolicyProvider.ClaimType, StringComparison.OrdinalIgnoreCase))
            {
                return base.GetPolicyAsync(policyName);
            }

            var permissionNames = policyName.Substring(PermissionPolicyProvider.ClaimType.Length).Split(',');

            var policy = new AuthorizationPolicyBuilder()
                .RequireClaim(PermissionPolicyProvider.ClaimType, permissionNames)
                .Build();

            return Task.FromResult(policy);
        }
    }
}
