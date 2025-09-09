using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BDA.ActiveDirectory;
using BDA.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BDA.Web.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class DirectoryUserController : BaseApiController
    {
        private readonly IDirectoryService directoryService;

        public DirectoryUserController(IDirectoryService directoryService)
        {
            this.directoryService = directoryService;
        }

        //public ActionResult Read(string id)
        //{
        //    var user = directoryService.GetUserByStaffNo(id);
        //    //if (user == null)
        //    //{
        //    //    //return Status(HttpStatusCode.NotFound);
        //    //}

        //    //user.IsRegistered = Db.Users.Any(x => x.UserName == id);
        //    return Json(user);
        //}

        [HttpGet]
        public async Task<DirectoryUser> Get(string staffNo)
        {
            return await directoryService.GetUserByStaffNo(staffNo);
        }

        //public IActionResult DisplayObject()
        //{
        //    Destination destination = new Destination("Tokyo", "Japan", 1);
        //    return Json(destination);
        //}
    }
}
