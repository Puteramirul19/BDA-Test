using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace BDA.Identity
{
    public enum Permission
    {
        SuperUser,
        ManageSettings,
        RequestWangCagaranLess10M,
        VerifyWangCagaranLess10M,
        ApproveWangCagaranLess10M,
        RequestWangCagaranMore10M,
        VerifyWangCagaranMore10M,
        ApproveWangCagaranMore10M,
        RequestWangHangus,
        ApproveWangHangus,
        ProcessingBankDraftApplication,
    }   

    public static class PermissionEnumExtensions
    {
        public static Claim ToClaim(this Permission me)
        {
            return new Claim(PermissionPolicyProvider.ClaimType, me.ToString());
        }

        private static Dictionary<Permission, string> trans = new Dictionary<Permission, string>
        {
            //
            // Permissions only
            //
            { Permission.SuperUser, "Super User" },
            { Permission.ManageSettings, "Manage Settings" },
            { Permission.RequestWangCagaranLess10M, "Request Wang Cagaran Less 10M" },
            { Permission.VerifyWangCagaranLess10M, "Verify Wang Cagaran Less 10M" },

            { Permission.ApproveWangCagaranLess10M, "Approve Wang Cagaran Less 10M" },
            { Permission.RequestWangCagaranMore10M, "Request Wang Cagaran More 10M" },
            { Permission.VerifyWangCagaranMore10M, "Verify Wang Cagaran More 10M" },
            { Permission.ApproveWangCagaranMore10M, "Approve Wang Cagaran More 10M" },
            { Permission.RequestWangHangus, "Request Wang Hangus" },
            { Permission.ApproveWangHangus, "Approve Wang Hangus" },
             { Permission.ProcessingBankDraftApplication, "Processing Bank Draft Application" },
        };

        public static string ToText(this Permission me)
        {
            return trans[me];
        }

        private static Permission[] nonActions = new[]
        {
            Permission.SuperUser,
        };

        public static bool IsAction(this Permission me)
        {
            foreach (var nonAction in nonActions)
                if (nonAction == me)
                    return false;

            return true;
        }
    }
}
