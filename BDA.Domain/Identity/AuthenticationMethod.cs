//using BDA.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BDA.Identity
{
    public enum AuthenticationMethod
    {
        InternalDatabase,
        ActiveDirectory
    }

    public static class AuthenticationMethodExtensions
    {

        private static Dictionary<AuthenticationMethod, string> trans = new Dictionary<AuthenticationMethod, string>
        {
            { AuthenticationMethod.InternalDatabase, "Dalaman" },
            { AuthenticationMethod.ActiveDirectory,  "Active Directory" },
        };

        public static string ToText(this AuthenticationMethod me)
        {
            return trans[me];
        }
    }
}
