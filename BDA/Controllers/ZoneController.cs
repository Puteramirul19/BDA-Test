using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BDA.Data;
using BDA.Entities;
using BDA.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BDA.Web.Controllers
{
    [Authorize(Roles = "Business Admin,ICT Admin, TGBS Banking")]
    public class ZoneController :  BaseController
    {
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ManageZone()
        {
            var model = new ZoneViewModel();
            model.Zones = Db.Zone
                            .Include(x => x.Function.Division)
                            .Where(x => x.isActive == true)
                            .ToList();
            return View(model);
        }

        public JsonResult GetAllZones(Guid? id = null, string code = null, string name = null, string funcName= null, string divName = null,  bool? status = null)
        {
            var zoneList = Db.Zone
                           .Select(x => new
                           {
                               id = x.Id,
                               code = x.Code,
                               name = x.Name,
                               funcName = x.Function.Name,
                               funcId = x.FunctionId,
                               divName = x.Function.Division.Name,
                               divId = x.Function.Division.Id,
                               isActive = x.isActive
                           }
                             )
                        .Where(x =>
                        (x.id == id || id == null)
                        && (x.code.Contains(code) || code == null)
                        && (x.name.Contains(name) || name == null)
                        && (x.divName.Contains(divName) || divName == null)
                        && (x.funcName.Contains(funcName) || funcName == null)
                        && ((x.isActive == true)))
                        //&& ((x.isActive == status) || status == null))
                        .ToList();

            return new JsonResult(zoneList.ToList());
        }

        [HttpPost]
        public JsonResult Update(ZoneViewModel model)
        {
            var entity = Db.Zone.Where(x => x.Id == Guid.Parse(model.Id)).FirstOrDefault();

            //if (ModelState.IsValid)
            //{
                if (entity != null)
                {
                    entity.Code = model.Code;
                    entity.Name = model.Name;
                    entity.FunctionId = model.FunctionId == Guid.Empty ? entity.FunctionId : model.FunctionId;
                    entity.isActive = model.IsActive;

                    Db.SetModified(entity);
                    Db.SaveChanges();
                }
                else
                {
                    return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Edit Zone error" });
                }

            //}
            //else
            //{
            //    return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Edit Zone error" });
            //}

            return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Edit Zone Successful!" });
        }

        [HttpGet]
        public ActionResult Read ()
        {
            var ZoneList = Db.Zone.Where(x => x.isActive == true).ToList();

            return View(ZoneList);
        }


        [HttpPost]
        public JsonResult Create(ZoneViewModel model)
        {
            if (ModelState.IsValid)
            {
               
                //if (Db.Zone.Any(x => x.Name == model.Name))
                //{
                //    return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Zone with name '" + model.Name + "' already existed." });
                //    //ModelState.AddModelError("Name", "Function with name '" + model.Name + "' already existed.");
                //}
                //else
                //    {
                        var entity = new Zone
                        {
                            Code = model.Code,
                            Name = model.Name,
                            FunctionId = model.FunctionId
                        };

                        Db.Zone.Add(entity);
                        Db.SaveChanges();
                    //}
            }
            return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Add New Zone Successful!" });
        }

        public ActionResult Update(Guid id, Function model)
        {
            var entity = Db.Zone.Find(id);

            if (ModelState.IsValid)
            {
                if (id != model.Id)
                {
                    //return Status(HttpStatusCode.BadRequest);
                }

                var existingName = Db.Zone.Any(x => x.Name == model.Name && x.Id != model.Id);

               
                    if (existingName)
                        ModelState.AddModelError("Name", "Zone name '" + model.Name + "' already existed.");
                    else
                    {
                        entity.Code = model.Code;
                        entity.Name = model.Name;
                    }
               
                Db.SetModified(entity);
                Db.SaveChanges();
            }

            return View();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var zone = await Db.Zone.FindAsync(id);
            if (zone == null)
            {
                return NotFound();
            }

            Db.Zone.Remove(zone);
            await Db.SaveChangesAsync();

            return Ok();
        }


        [HttpGet]
        public JsonResult GetAllZoneByFunctionId(string Id)
        {
            var result = Db.Zone.Where(x => x.FunctionId == Guid.Parse(Id)).Where(x => x.isActive == true).ToList();

            return new JsonResult(result.ToList());
        }
    }
}