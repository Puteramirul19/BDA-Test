using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;


namespace BDA.Web.Controllers
{
    //[Authorize]
    public class SessionController : BaseController
    {
        internal static object GetInstanceField(Type type, object instance, string fieldName)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            return field.GetValue(instance);
        }

        public JsonResult GetSessionTimeOut()
        {

            return new JsonResult(new { timeout = GetInstanceField(typeof(DistributedSession), HttpContext.Session, "_idleTimeout") });
        }

    }
}