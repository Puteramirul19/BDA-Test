using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BDA.Data;
using BDA.Entities;
using BDA.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BDA.Web.Controllers
{
    [Authorize(Roles = "Business Admin,ICT Admin,TGBS Banking")]
    public class FunctionController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult ManageFunction()
        {

            var model = new FunctionViewModel();
            model.Functions = Db.Function
                              .Include("Division")
                              .Where(x => x.isActive == true)
                              .ToList();
            return View(model);
            
        }

        public JsonResult GetAllFunctions(Guid? id = null, string code = null, string name = null, string divName = null, bool? status = null)
        {
            var functionList = Db.Function
                           .Select(x => new
                           {
                               id = x.Id,
                               code = x.Code,
                               name = x.Name,
                               divName = x.Division.Name,
                               divId = x.DivisionId,
                               isActive = x.isActive
                           }
                             )
                        .Where(x =>
                        (x.id == id || id == null)
                        && (x.code.Contains(code) || code == null)
                        && (x.name.Contains(name) || name == null)
                        && (x.divName.Contains(divName) || divName == null)
                        && ((x.isActive == true)))
                        //&& ((x.isActive == status) || status == null))
                        .ToList();

            return new JsonResult(functionList.ToList());
        }

        [HttpGet]
        public ActionResult Read()
        {
            var FunctionList = Db.Function.Where(x => x.isActive == true).ToList();

            return View(FunctionList);
        }


        [HttpPost]
        public JsonResult Create(FunctionViewModel model)
        {
            if (ModelState.IsValid)
            {

                //if (Db.Function.Any(x => x.Name == model.Name))
                //{
                //    return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Function with name '" + model.Name + "' already existed." });
                //    //ModelState.AddModelError("Name", "Function with name '" + model.Name + "' already existed.");
                //}
                //else
                //{
                    var entity = new Function
                    {
                        Code = model.Code,
                        Name = model.Name,
                        DivisionId = model.DivisionId
                    };

                    Db.Function.Add(entity);
                    Db.SaveChanges();

                //}

            }
            //return RedirectToAction("ManageFunction", "Function", new { sessionStatus = HttpStatusCode.Accepted.ToString() });
            return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Add New Function Successful!" });

        }

        [HttpPost]
        public JsonResult Update(FunctionViewModel model)
        {
            var entity = Db.Function.Where(x => x.Id == Guid.Parse(model.Id)).FirstOrDefault();

            if (ModelState.IsValid)
            {
                if (entity != null)
                {
                    entity.Code = model.Code;
                    entity.Name = model.Name;
                    entity.DivisionId = model.DivisionId;
                    entity.isActive = model.IsActive;

                    Db.SetModified(entity);
                    Db.SaveChanges();
                }
                else
                {
                    return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Edit Function error" });
                }

            }
            else
            {
                return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Edit Function error" });
            }

            return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Edit Function Successful!" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var function = await Db.Function.FindAsync(id);
            if (function == null)
            {
                return NotFound();
            }

            Db.Function.Remove(function);
            await Db.SaveChangesAsync();

            return Ok();
        }

        public JsonResult GetAllFunction()
        {
            var result = Db.Function.Where(x => x.isActive == true).ToList();

            return new JsonResult(result.ToList());
        }
        [HttpGet]
        public JsonResult GetAllFunctionByDivisionId(string Id)
        {
            var result = Db.Function.Where(x => x.DivisionId== Guid.Parse(Id)).Where(x => x.isActive == true).ToList();

            return new JsonResult(result.ToList());
        }
    }
}