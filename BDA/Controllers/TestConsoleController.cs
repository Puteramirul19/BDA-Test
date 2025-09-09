using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using BDA.Data;
using BDA.Entities;
using BDA.Integrations;
using Hangfire;
using Hangfire.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rotativa.AspNetCore;

namespace BDA.Web.Controllers
{
    [Authorize]
    public class TestConsoleController : BaseController
    {

        private IConfiguration configuration => HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        public IActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //public JsonResult PostingToERMS(string refNo)
        //{
        //    var bd = Db.BankDraft.Where(x => x.RefNo == refNo).FirstOrDefault();
        //    var erms = new ErmsService(Db);
        //    erms.PostingWangHangus(bd);
            
        //    return new JsonResult(bd.NameOnBD);
        //}


        [HttpPost]
        public JsonResult SendEmailForApprover(string refNo)
        {
            var bd = Db.BankDraft.Where(x => x.RefNo == refNo).FirstOrDefault();
            var erms = new ErmsService(Db,configuration);
            Job.Enqueue<Services.NotificationService>(x => x.NotifyApproverForApproval(bd.Id));

            return new JsonResult(bd.NameOnBD);
        }

    }
}

