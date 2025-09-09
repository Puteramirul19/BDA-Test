using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace BDA.Identity
{
    public class NotPermittedException : Exception
    {
        public NotPermittedException(ClaimsPrincipal principal, params Permission[] permission)
            : base($"User '{principal.Identity.Name}' has no permission for {String.Join("or ", permission.Select(x => "'" + x.ToText() + "'"))}.")
        {
        }
    }
}
