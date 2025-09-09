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
    [Authorize(Roles = "Business Admin,ICT Admin,TGBS Banking")]
    public class BusinessAreaController :  BaseController
    {
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ManageBusinessArea()
        {
            var model = new BusinessAreaViewModel();
            model.BusinessAreas = Db.BusinessArea
                            //.Include(x => x.State)
                            .ToList();
            return View(model);
        }

        public JsonResult GetAllBAs(string name = null, string zoneName = null, string stateName = null, string divId= null, string description = null)
        {

            var baList = Db.BusinessArea
                         .Select(x => new
                         {
                             id = x.Id,
                             name = x.Name,
                             divName = x.Division.Name,
                             divisionId = x.DivisionId,
                             //zoneName = x.Zone,
                             //stateName = x.State.Name,
                             //stateId = x.StateId,
                             description = x.Description
                         }
                           )
                      .Where(x =>
                      x.name.Contains(name) || name == null
                      //&& (x.divisionId == Guid.Parse(divId) || divId == null)
                      //&& (x.zoneName.Contains(zoneName) || zoneName == null)
                      && (x.description.Contains(description) || description == null))
                      //&& (x.stateName.Contains(stateName) || stateName == null))
                      .ToList();

            if (divId != null)
            {
                baList = Db.BusinessArea
                        .Select(x => new
                        {
                            id = x.Id,
                            name = x.Name,
                            divName = x.Division.Name,
                            divisionId = x.DivisionId,
                             //zoneName = x.Zone,
                             //stateName = x.State.Name,
                             //stateId = x.StateId,
                             description = x.Description
                        }
                          )
                     .Where(x =>
                     x.name.Contains(name) || name == null
                     && (x.divisionId == Guid.Parse(divId) || divId == null)
                     //&& (x.zoneName.Contains(zoneName) || zoneName == null)
                     && (x.description.Contains(description) || description == null))
                     //&& (x.stateName.Contains(stateName) || stateName == null))
                     .ToList();
            }

            return new JsonResult(baList.ToList());
        }

        [HttpPost]
        public JsonResult Update(string Id, string Name, Guid DivisionId, string Description)
        {
            var entity = Db.BusinessArea.Where(x => x.Id == Guid.Parse(Id)).FirstOrDefault();

            //if (ModelState.IsValid)
            //{
            if (entity != null)
            {
                entity.Name = Name;
                //entity.StateId = model.StateId == Guid.Empty ? entity.StateId : model.StateId;
                entity.DivisionId = DivisionId;
                //entity.Zone = model.Zone;
                entity.Description = Description;
                Db.SetModified(entity);
                Db.SaveChanges();
            }
            else
            {
                return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Edit Business Area error" });
            }

            //}
            //else
            //{
            //    return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Edit Zone error" });
            //}

            return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Edit Business Area Successful!" });
        }

        [HttpGet]
        public ActionResult Read ()
        {
            var baList = Db.BusinessArea.ToList();

            return View(baList);
        }


        [HttpPost]
        public JsonResult Create(string Name, Guid DivisionId, string Description)
        {

            if (ModelState.IsValid)
            {

                var entity = new BusinessArea
                {
                    Name = Name,
                    //StateId = model.StateId,
                    //Zone = model.Zone,
                    DivisionId = DivisionId,
                    Description = Description
                };

                Db.BusinessArea.Add(entity);
                Db.SaveChanges();

            }
            return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Add New Business Area Successful!" });
        }


        public ActionResult Update(Guid id, BusinessArea model)
        {
            var entity = Db.BusinessArea.Find(id);

            if (ModelState.IsValid)
            {
                if (id != model.Id)
                {
                    //return Status(HttpStatusCode.BadRequest);
                }

                var existingName = Db.BusinessArea.Any(x => x.Name == model.Name && x.Id != model.Id);

               
                    if (existingName)
                        ModelState.AddModelError("Name", "BusinessArea '" + model.Name + "' already existed.");
                    else
                    {
                        entity.Name = model.Name;
                        //entity.StateId = model.StateId;
                        //entity.Zone = model.Zone;
                        entity.Division = model.Division;
                        entity.Description = model.Description;
                    }
               
                Db.SetModified(entity);
                Db.SaveChanges();
            }

            return View();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var ba = await Db.BusinessArea.FindAsync(id);
            if (ba == null)
            {
                return NotFound();
            }

            Db.BusinessArea.Remove(ba);
            await Db.SaveChangesAsync();

            return Ok();
        }


        [HttpGet]
        public JsonResult GetAllBasByZoneName(string name)
        {
            var result = Db.BusinessArea.Where(x => x.Name == name).ToList();

            return new JsonResult(result.ToList());
        }
    }
}