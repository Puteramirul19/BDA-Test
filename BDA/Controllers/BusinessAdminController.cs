using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BDA.Data;
using BDA.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BDA.Web.Controllers
{
    [Authorize]
    public class BusinessAdminController :  BaseController
    {
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ManageUser()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<ActionResult> AddUser(Users user)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState.Values.SelectMany(v => v.Errors).Select(error => error.ErrorMessage));
        //    }

        //    //var existing = await Db.Users.FirstOrDefaultAsync(x => x.UserName == user.UserName);
        //    //if (existing != null)
        //    //    throw new Exception("Username already exists.");

        //    //    var entity = new Users();
        //    //    entity.Id = new Guid();
        //    //    entity.LoginType = user.LoginType;
        //    //    entity.UserName = user.UserName; //staffNo
        //    //    entity.FullName = user.FullName; //staffName
        //    //    entity.Email = user.Email; //E-mail
        //    //    entity.OfficeNumber = user.OfficeNumber; //Office Number
        //    //    entity.PhoneNumber = user.PhoneNumber; //Hp Number
        //    //    entity.Division = user.Division; //Division
        //    //    entity.Unit = user.Unit; //Group

        //    //Db.Users.Add(entity);
        //    //await Db.SaveChangesAsync();

        //    //user.Id = entity.Id;
        //    //return Json(user);
        //}


        [HttpGet]
        public IActionResult AddUser()
        {
            return View();
        }

        public IActionResult EditUser()
        {
            return View();
        }

        public IActionResult ManageRole()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult AssignRole()
        {
            return View();
        }
        public IActionResult _ActionButton()
        {
            return View();
        }

        public IActionResult ManageDivision()
        {
            return View();
        }
        public IActionResult ManageFunction()
        {
            return View();
        }
        public IActionResult ManageZone()
        {
            return View();
        }
        public IActionResult ManageUnit()
        {
            return View();
        }
        public IActionResult ManageBankDetails()
        {
            return View();
        }
        public IActionResult BankDetails()
        {
            return View();
        }

    }
}