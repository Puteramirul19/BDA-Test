using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BDA.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BDA.Web.Controllers
{
    [Authorize]
    public class UMAController : Controller
    {
        public IActionResult List()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
