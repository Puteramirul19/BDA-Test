using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDA
{
    public class AuthorizationException : Exception
    {
        public AuthorizationException()
            : base("Current user have no proper authorizations")
        {
        }

        //public AuthorizationException(params BDA.Entities.AccessRights[] accessRights)
        //    : base("Current user have no authorizations for " + String.Join("/", accessRights.Select(x => x.ToString()).ToArray()) + " access.")
        //{
        //}
    }
}