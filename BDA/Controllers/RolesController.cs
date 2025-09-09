using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BDA.Data;
using BDA.Entities;
using BDA.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BDA.Web.Controllers
{
    [Authorize(Roles = "Business Admin,ICT Admin,TGBS Banking")]
    public class RolesController :  BaseController
    {
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ManageUser()
        {
            return View();
        }
        public IActionResult Add()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AddUser()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<ActionResult> Create(ApplicationRole model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState.Values.SelectMany(v => v.Errors).Select(error => error.ErrorMessage));
        //    }

        //    var existing = await Db.Roles.FirstOrDefaultAsync(x => x.Name == model.Name);
        //    if (existing != null)
        //        throw new Exception("Role name already exists.");

        //    var entity = new Roles();
        //    entity.Id = user.UserName;
        //    entity.LoginType = user.LoginType;
        //    entity.UserName = user.UserName; //staffNo
        //    entity.FullName = user.FullName; //staffName
        //    entity.Email = user.Email; //E-mail
        //    entity.OfficeNumber = user.OfficeNumber; //Office Number
        //    entity.PhoneNumber = user.PhoneNumber; //Hp Number
        //    entity.Division = user.Division; //Division
        //    entity.Unit = user.Unit; //Group

        //    Db.Users.Add(entity);
        //    await Db.SaveChangesAsync();

        //    user.Id = entity.Id;
        //    return Json(user);
        //}

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

        public JsonResult GetAllRoles()
        {
            var result = Db.Roles.ToList();

            return new JsonResult(result.ToList());
        }
    }
}