using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BDA.Controllers
{
    [Authorize(Roles = "Business Admin,ICT Admin,TGBS Banking")]
    public class EmailController : Controller
    {
        public IActionResult EmailQueue()
        {
            return View();
        }

        public IActionResult EmailTemplate()
        {
            return View();
        }
    }
}