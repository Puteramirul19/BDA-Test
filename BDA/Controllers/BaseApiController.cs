using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BDA.Data;
using BDA.Entities;
using BDA.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace BDA.Web.Controllers
{
    public class BaseApiController : ControllerBase
    {
        private BdaDBContext _db = null;

        protected BdaDBContext Db
        {
            get
            {
                if (_db == null)
                    _db = HttpContext.RequestServices.GetService<BdaDBContext>();
                return _db;
            }
        }

        private ApplicationUser _currentUser = null;

        protected async Task<ApplicationUser> GetApplicationUser()
        {
            if (_currentUser == null)
            {
                var userManager = HttpContext.RequestServices.GetService<UserManager<ApplicationUser>>();
                _currentUser = await userManager.GetUserAsync(User);
            }

            return _currentUser;
        }

        //protected DataSourceResult DataSourceResult(DataSourceRequest request, IEnumerable enumerable)
        //{
        //    return enumerable.ToDataSourceResult(request);
        //}

    }
}
