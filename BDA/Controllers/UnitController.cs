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
    public class UnitController :  BaseController
    {
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ManageUnit()
        {
            var model = new UnitViewModel();
            model.Units = Db.Unit
                        .Include(x => x.Zone.Function.Division) // include the "Job.Quotes" relation and data
                        .Where(x => x.isActive == true)
                        .ToList();
            return View(model);
        }

        [HttpGet]
        public ActionResult Read ()
        {
            var UnitList = Db.Unit.Where(x => x.isActive == true).ToList();

            return View(UnitList);
        }

        public JsonResult GetAllUnits(Guid? id = null, string code = null, string name = null, string funcName = null, string zoneName = null, string divName = null, bool? status = null)
        {
            var unitList = Db.Unit
                           .Select(x => new
                           {
                               id = x.Id,
                               code = x.Code,
                               name = x.Name,
                               zoneName = x.Zone.Name,
                               zoneId = x.ZoneId,
                               funcName = x.Zone.Function.Name,
                               funcId = x.Zone.FunctionId,
                               divName = x.Zone.Function.Division.Name,
                               divId = x.Zone.Function.Division.Id,
                               isActive = x.isActive
                           }
                             )
                        .Where(x =>
                        (x.id == id || id == null)
                        && (x.code.Contains(code) || code == null)
                        && (x.name.Contains(name) || name == null)
                        && (x.divName.Contains(divName) || divName == null)
                        && (x.funcName.Contains(funcName) || funcName == null)
                        && (x.zoneName.Contains(zoneName) || zoneName == null)
                        && ((x.isActive == true)))
                        //&& ((x.isActive == status) || status == null))
                        .ToList();

            return new JsonResult(unitList.ToList());
        }

        [HttpPost]
        public JsonResult Update(UnitViewModel model)
        {
            var entity = Db.Unit.Where(x => x.Id == Guid.Parse(model.Id)).FirstOrDefault();

            //if (ModelState.IsValid)
            //{
                if (entity != null)
                {
                    entity.Code = model.Code;
                    entity.Name = model.Name;
                    entity.ZoneId = model.ZoneId;
                    entity.isActive = model.IsActive;

                    Db.SetModified(entity);
                    Db.SaveChanges();
                }
                else
                {
                    return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Edit Unit error" });
                }

            //}
            //else
            //{
            //    return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Edit Unit error" });
            //}

            return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Edit Unit Successful!" });
        }

        [HttpPost]
        public JsonResult Create(UnitViewModel model)
        {
            if (ModelState.IsValid)
            {
               
                //    if (Db.Unit.Any(x => x.Name == model.Name))
                //{
                //    return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Unit with name '" + model.Name + "' already existed." });
                //}
                //else
                //    {
                        var entity = new Unit
                        {
                            Code = model.Code,
                            Name = model.Name,
                            ZoneId = model.ZoneId
                        };

                        Db.Unit.Add(entity);
                        Db.SaveChanges();

                    //}

            }
            return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Add New Unit Successful!" });
        }

        //[HttpPost]
        //public ActionResult Update(Guid id, Unit model)
        //{
        //    var entity = Db.Unit.Find(id);

        //    if (ModelState.IsValid)
        //    {
        //        if (id != model.Id)
        //        {
        //            //return Status(HttpStatusCode.BadRequest);
        //        }

        //        var existingName = Db.Unit.Any(x => x.Name == model.Name && x.Id != model.Id);

               
        //            if (existingName)
        //                ModelState.AddModelError("Name", "Unit name '" + model.Name + "' already existed.");
        //            else
        //            {
        //                entity.Code = model.Code;
        //                entity.Name = model.Name;
        //            }
               
        //        Db.SetModified(entity);
        //        Db.SaveChanges();
        //    }

        //    return View();
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var unit = await Db.Unit.FindAsync(id);
            if (unit == null)
            {
                return NotFound();
            }

            Db.Unit.Remove(unit);
            await Db.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        public JsonResult GetAllUnitByZoneId(string Id)
        {
            var result = Db.Unit.Where(x => x.ZoneId == Guid.Parse(Id)).Where(x => x.isActive == true).ToList();

            return new JsonResult(result.ToList());
        }

    }
}