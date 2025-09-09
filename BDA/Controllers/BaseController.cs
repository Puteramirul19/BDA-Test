using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using BDA.Data;
using BDA.Entities;
using BDA.Identity;
using BDA.Integrations;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BDA.Web.Controllers
{
    public class BaseController : Controller
    {
        private BdaDBContext _db = null;
        private ApplicationUser _currentUser = null;
    
        protected BdaDBContext Db
        {
            get
            {
                if (_db == null)
                    _db = HttpContext.RequestServices.GetService<BdaDBContext>();
                return _db;
            }
        }

        public async Task<ApplicationUser> GetApplicationUser()
        {
            if (_currentUser == null)
            {
                var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                _currentUser = await userManager.GetUserAsync(User);
            }

            return _currentUser;
        }

        protected IBackgroundJobClient Job
        {
            get
            {
                if (_job == null)
                    _job = HttpContext.RequestServices.GetRequiredService<IBackgroundJobClient>();
                return _job;
            }
        }

        private IBackgroundJobClient _job = null;

        //protected new internal JsonResult Json(object data)
        //{
        //    return base.Json(data, JsonRequestBehavior.AllowGet);
        //}

        //public HttpStatusCodeResult Ok()
        //{
        //    return Status(System.Net.HttpStatusCode.OK);
        //}

        //public ActionResult Error(string msg)
        //{
        //    Response.StatusCode = 500;
        //    return Content(msg);
        //}

        //public HttpStatusCodeResult NotFound()
        //{
        //    return Status(System.Net.HttpStatusCode.NotFound);
        //}

        //public HttpStatusCodeResult Status(HttpStatusCode statusCode)
        //{
        //    return new HttpStatusCodeResult(statusCode);
        //}

        //public Users GetCurrentUser()
        //{
        //    var user = db.Users
        //                .FirstOrDefault(x => x.Id == Users.Identity.Name);
        //    if (user == null)
        //        throw new AuthorizationException();

        //    return user;
        //}

        //public void CheckAccess(params AccessRights[] accessRights)
        //{
        //    CheckAccess(null, accessRights);
        //}

        //public void CheckAccess(Users user, params AccessRights[] accessRights)
        //{
        //    if (!HaveAccessRight(user, accessRights))
        //        throw new AuthorizationException(accessRights);
        //}

        //public bool HaveAccessRight(params AccessRights[] accessRights)
        //{
        //    return HaveAccessRight(null, accessRights);
        //}

        //public bool HaveAccessRight(Users user, params AccessRights[] accessRights)
        //{
        //    if (user == null)
        //        user = GetCurrentUser();

        //    return user.HaveAccessRight(accessRights);
        //}

    }
}