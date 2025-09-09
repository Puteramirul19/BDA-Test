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
    public class DivisionController :  BaseController
    {
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ManageDivision()
        {
            var model = new DivisionViewModel();
            model.Divisions = Db.Division.ToList();
            return View(model);
        }

        public JsonResult GetAllDivisions(Guid? id = null, string code = null, string name = null, int loaType = 0, bool? status = null)
        {
            var divisionList = Db.Division
                           .Select(x => new
                                 {
                                     id = x.Id,
                                     code = x.Code,
                                     name = x.Name,
                                     isActive = x.isActive,
                                     loaType = x.LOAType
                                 }
                             )
                        .Where(x =>
                        (x.id == id || id == null)
                        && (x.code.Contains(code) || code == null)
                        && (x.name.Contains(name) || name == null)
                        && ((x.loaType == loaType) || loaType == 0)
                        && ((x.isActive == status) || status == null))
                        .ToList();

            return new JsonResult(divisionList.ToList());
        }

        [HttpGet]
        public JsonResult GetDivisionById(Guid id)
        {
            var divisionList = Db.Division
                           .Select(x => new
                           {
                               id = x.Id,
                               code = x.Code,
                               name = x.Name,
                               loaType = x.LOAType
                           }
                             )
                        .Where(x =>
                        (x.id == id || id == null))
                 .ToList();

            return new JsonResult(divisionList.ToList());
        }

        [HttpGet]
        public ActionResult Read()
        {
            List<Division> divisionList = Db.Division.ToList();

            return View(divisionList);
        }

        [HttpPost]
        public JsonResult Create(DivisionViewModel model)
        {
            if (ModelState.IsValid)
            {
               
                if (Db.Division.Any(x => x.Name == model.Name))
                {
                    return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Division with name '" + model.Name + "' already existed." });
                    //ModelState.AddModelError("Name", "Function with name '" + model.Name + "' already existed.");
                }
                else
                    {
                        var entity = new Division
                        {
                            Code = model.Code,
                            Name = model.Name,
                            LOAType = model.LOAType
                        };

                        Db.Division.Add(entity);
                        Db.SaveChanges();

                    }
              
            }
            //return View();
            return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Add New Division Successful!" });
        }

        [HttpPost]
        public JsonResult Update(DivisionViewModel model)
        {
            var entity = Db.Division.Where(x=> x.Id == Guid.Parse(model.Id)).FirstOrDefault();

            if (ModelState.IsValid)
            {
               if(entity != null)
                {
                    entity.Code = model.Code;
                    entity.Name = model.Name;
                    entity.isActive = model.IsActive;
                    entity.LOAType = model.LOAType;

                    Db.SetModified(entity);
                    Db.SaveChanges();
                }
               else
                {
                    return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Edit Division error" });
                }
            
            }
            else
            {
                return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Edit Division error" });
            }

            return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Edit Division Successful!" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var division = await Db.Division.FindAsync(id);
            if (division == null)
            {
                return NotFound();
            }

            Db.Division.Remove(division);
            await Db.SaveChangesAsync();

            return Ok();
        }


        public JsonResult GetAllDivision(int divType)
        {
            var result = Db.Division.Where(x => x.isActive == true && (x.LOAType == divType)).ToList();
            if ( divType == 0)
            {
                result = Db.Division.Where(x => x.isActive == true).ToList();
            }
           
            return new JsonResult(result.ToList());
        }

    }
}