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
    public class BankDetailsController :  BaseController
    {
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ManageBankDetails()
        {
            var model = new BankDetailsViewModel();
            model.BankDetails = Db.BankDetails
                            .ToList();
            return View(model);
        }

        public JsonResult GetAllBankDetails(Guid? id = null, string bankName = null, string bankPIC = null, string bankPICPosition = null, string addLine1= null, string addLine2 = null, string state = null, string type = null, string accountNo1 = null, string accountNo2 = null, string accountNo3 = null, string chargeAccountNo = null, string email = null, bool? status = null)
        {
            var bankList = Db.BankDetails
                           .Select(x => new
                           {
                               id = x.Id,
                               bankName = x.BankName,
                               bankPIC = x.BankPIC,
                               bankPICPosition = x.BankPICPosition,
                               addLine1 = x.AddressLine1,
                               addLine2 = x.AddressLine2,
                               street = x.Street,
                               state = x.State,
                               accountNo1 = x.AccountNo1,
                               accountNo2 = x.AccountNo2,
                               accountNo3 = x.AccountNo3,
                               type = x.Type,
                               chargeAccountNo = x.ChargeAccountNo,
                               email = x.Email,
                               isActive = x.isActive
                            })
                        .Where(x =>
                        (x.id == id || id == null)
                        && (x.bankName.Contains(bankName) || bankName == null)
                        && (x.bankPIC.Contains(bankPIC) || bankPIC == null)
                        && (x.bankPICPosition.Contains(bankPICPosition) || bankPICPosition == null)
                        && (x.addLine1.Contains(addLine1) || addLine1 == null)
                        && (x.addLine2.Contains(addLine2) || addLine2 == null)
                        && (x.type.Contains(type) || type == null)
                        && (x.accountNo1.Contains(accountNo1) || accountNo1 == null)
                        && (x.accountNo2.Contains(accountNo2) || accountNo2 == null)
                        && (x.accountNo3.Contains(accountNo3) || accountNo3 == null)
                        && (x.accountNo3.Contains(accountNo3) || accountNo3 == null)
                        && (x.chargeAccountNo.Contains(chargeAccountNo) || chargeAccountNo == null)
                        && (x.email.Contains(email) || email == null)
                        && ((x.isActive == status) || status == null))
                        .ToList();

            return new JsonResult(bankList.ToList());
        }

        [HttpPost]
        public JsonResult Update(BankDetailsViewModel model)
        {
            var entity = Db.BankDetails.Where(x => x.Id == Guid.Parse(model.Id)).FirstOrDefault();

            if (ModelState.IsValid)
            {
                if (entity != null)
                {
                    entity.Id = Guid.Parse(model.Id);
                    entity.BankName = model.BankName;
                    entity.BankPIC = model.BankPIC;
                    entity.BankPICPosition = model.BankPICPosition;
                    entity.AddressLine1 = model.AddressLine1;
                    entity.AddressLine2 = model.AddressLine2;
                    entity.Street = model.Street;
                    entity.State = model.State;
                    entity.AccountNo1 = model.AccountNo1;
                    entity.AccountNo2 = model.AccountNo2;
                    entity.AccountNo3 = model.AccountNo3;
                    entity.Type = model.Type;
                    entity.ChargeAccountNo = model.ChargeAccountNo;
                    entity.Email = model.Email;
                    entity.isActive = model.IsActive;

                    Db.SetModified(entity);
                    Db.SaveChanges();
                }
                else
                {
                    return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Edit BankDetails error" });
                }

            }
            else
            {
                return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Edit BankDetails error" });
            }

            return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Edit BankDetails Successful!" });
        }

        [HttpGet]
        public ActionResult Read ()
        {
            var BankList = Db.BankDetails.ToList();

            return View(BankList);
        }


        [HttpPost]
        public JsonResult Create(BankDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                        var entity = new BankDetails
                        {
                            Id = Guid.NewGuid(),
                            BankName = model.BankName,
                            BankPIC = model.BankPIC,
                            BankPICPosition = model.BankPICPosition,
                            AddressLine1 = model.AddressLine1,
                            AddressLine2 = model.AddressLine2,
                            Street = model.Street,
                            State = model.State,
                            AccountNo1 = model.AccountNo1,
                            AccountNo2 = model.AccountNo2,
                            AccountNo3 = model.AccountNo3,
                            Type = model.Type,
                            ChargeAccountNo = model.ChargeAccountNo,
                            Email = model.Email
                        };

                        Db.BankDetails.Add(entity);
                        Db.SaveChanges();
                    //}
            }
            return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Add New BankDetails Successful!" });
        }

        public ActionResult Update(Guid id, BankDetails model)
        {
            var entity = Db.BankDetails.Find(id);

            if (ModelState.IsValid)
            {
                if (id != model.Id)
                {
                    //return Status(HttpStatusCode.BadRequest);
                }

                var existingName = Db.BankDetails.Any(x => x.BankName == model.BankName && x.BankPIC != model.BankPIC);

               
                    if (existingName)
                        ModelState.AddModelError("Name", "BankDetails name '" + model.BankName + "' already existed.");
                    else
                    {
                        entity.BankName = model.BankName;
                        entity.BankPIC = model.BankPIC;
                        entity.BankPICPosition = model.BankPICPosition;
                        entity.AddressLine1 = model.AddressLine1;
                        entity.AddressLine2 = model.AddressLine2;
                        entity.Street = model.Street;
                        entity.State = model.State;
                        entity.AccountNo1 = model.AccountNo1;
                        entity.AccountNo2 = model.AccountNo2;
                        entity.AccountNo3 = model.AccountNo3;
                        entity.Type = model.Type;
                        entity.ChargeAccountNo = model.ChargeAccountNo;
                        entity.Email = model.Email;
                        entity.isActive = model.isActive;
                }
               
                Db.SetModified(entity);
                Db.SaveChanges();
            }

            return View();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var bankDetails = await Db.BankDetails.FindAsync(id);
            if (bankDetails == null)
            {
                return NotFound();
            }

            Db.BankDetails.Remove(bankDetails);
            await Db.SaveChangesAsync();

            return Ok();
        }

       
    }
}