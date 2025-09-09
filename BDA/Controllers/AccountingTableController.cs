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
using Newtonsoft.Json;

namespace BDA.Web.Controllers
{
    [Authorize]
    public class AccountingTableController :  BaseController
    {
        public IList<AccountingTableViewModel> accTables = new List<AccountingTableViewModel>();

        [HttpGet]
        public ActionResult Read(Guid wangHangusId)
        {
            List<AccountingTable> accTableList = Db.AccountingTable.Where(x=> x.WangHangusId == wangHangusId).ToList();

            return View(accTableList);
        }

        [HttpPost]
        public JsonResult Add(BankDraftViewModel accTable)
        {
            //accTables.WangHangusId = Guid.Parse("92B92C61-24C9-45FA-AEC6-7BC5AA23CE36");
            accTables.Add(accTable.WangHangusViewModel.AccountingTableViewModel);
            return Json(new { data = accTable.WangHangusViewModel.AccountingTableViewModel.drCr, response = StatusCode(StatusCodes.Status200OK), message = "Add New Accounting Table Successful!" });
        }


        [HttpGet]
        public JsonResult GetAccountingTable()
        { 
            return new JsonResult(accTables.ToList());
        }

        //[HttpPost]
        //public ActionResult Create(string wangHangusId, BankDraftViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        foreach (var item in model.WangHangusViewModel.AccountingTableViewModels)
        //        {
        //            var accTable = new AccountingTable
        //            {

        //                WangHangusId = Guid.Parse(wangHangusId),
        //                DrCr = item.DrCr,
        //                GLAccount = item.GLAccount,
        //                CONW = item.CONW,
        //                CONWNo = item.CONWNo,
        //                CostObject = item.CostObject,
        //                TaxCode = item.TaxCode,
        //                Currency = item.Currency,
        //                TaxAmount = item.TaxAmount,
        //                Amount = item.Amount,
        //            };

        //            Db.AccountingTable.Add(accTable);
        //            Db.SaveChanges();
        //        }

        //    }
        //    return View();
        //}


        [HttpGet]
        public JsonResult GetAllAccountingTableByWangHangusId(Guid id)
        {
            var result = Db.AccountingTable
                .Select(x => new { id = x.Id, drCr = x.DrCr, glAccount = x.GLAccount, conw = x.CONW, conwNo = x.CONWNo, costObject = x.CostObject, taxCode = x.TaxCode, currency = x.Currency, taxAmount = x.TaxAmount, amount = x.Amount, wangHangusId = x.WangHangusId })
                .Where(x =>x.wangHangusId == id).ToList();

            return new JsonResult(result.ToList());
        }

        //public JsonResult AddAccountingTableByWangHangusId(Guid id)
        //{
        //    var accTable = Db.AccountingTable.Find(accTableId);

        //    if (ModelState.IsValid)
        //    {
        //        accTable.DrCr = accTable.DrCr;
        //        accTable.GLAccount = accTable.GLAccount;
        //        accTable.CONW = accTable.CONW;
        //        accTable.CONWNo = accTable.CONWNo;
        //        accTable.CostObject = accTable.CostObject;
        //        accTable.TaxCode = accTable.TaxCode;
        //        accTable.Currency = accTable.Currency;
        //        accTable.TaxAmount = accTable.TaxAmount;
        //        accTable.Amount = accTable.Amount;

        //        Db.SetModified(accTable);
        //        Db.SaveChanges();
        //    }

        //    return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "AccountingTable updated successfully!" });
        //}

        [HttpPost]
        public JsonResult Insert(string wangId, string model)
        {

            if (model != null)
            {
                var str = model;
                var result = JsonConvert.DeserializeObject<AccountingTableViewModel>(model);

                var accTable = new AccountingTable();
                
                accTable.WangHangusId = Guid.Parse(wangId);
                accTable.DrCr = result.drCr;
                accTable.GLAccount = result.glAccount;
                accTable.CONW = result.conw;
                //accTable.CONWNo = result.cownNo;
                accTable.CostObject = result.costObject;
                accTable.TaxCode = result.taxCode;
                accTable.Currency = result.currency;
                accTable.TaxAmount = result.taxAmount;
                accTable.Amount = result.amount;

                Db.AccountingTable.Add(accTable);
                Db.SaveChanges();

                decimal totalAmount = 0;
                decimal totalTax = 0;
                var projectNum = "";
                var currentAccTable = Db.AccountingTable.Where(x => x.WangHangusId == Guid.Parse(wangId)).ToList();

                foreach (var item in currentAccTable)
                {
                    if (item.DrCr == "CR")
                    {
                        totalAmount += Decimal.Negate(Decimal.Parse(item.Amount));
                        if (!projectNum.Contains(item.CostObject))
                            projectNum += item.CostObject + " ";
                        totalTax += Decimal.Negate(Decimal.Parse(item.TaxAmount));
                    }
                    else
                    {
                        totalAmount += Decimal.Parse(item.Amount);
                        if (!projectNum.Contains(item.CostObject))
                            projectNum += item.CostObject + " ";
                        totalTax += Decimal.Parse(item.TaxAmount);
                    }
                }

                var wang = Db.WangHangus.Where(x => x.Id == Guid.Parse(wangId)).FirstOrDefault();
                wang.Amount = totalAmount;
                wang.TaxAmount = totalTax;
                Db.SetModified(wang);

                var bd = Db.BankDraft.Where(x => x.Id == wang.BankDraftId).FirstOrDefault();
                bd.BankDraftAmount = totalAmount + totalTax;
                bd.ProjectNo = projectNum;
                Db.SetModified(bd);
                Db.SaveChanges();
            }
            else
            {
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Accounting Table Not Found" });
            }


            return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Accounting Table Inserted Successfully!" });
        }

        [HttpPost]
        public JsonResult Update(string model)
        {

            if (model != null)
            {
                var str = model;
                var result = JsonConvert.DeserializeObject<AccountingTableViewModel>(model);

                var accTable = Db.AccountingTable.Where(x => x.Id == Guid.Parse(result.Id)).FirstOrDefault();
                if(accTable != null)
                {
                    accTable.WangHangusId = result.WangHangusId;
                    accTable.DrCr = result.drCr;
                    accTable.GLAccount = result.glAccount;
                    accTable.CONW = result.conw;
                    //accTable.CONWNo = result.cownNo;
                    accTable.CostObject = result.costObject;
                    accTable.TaxCode = result.taxCode;
                    accTable.Currency = result.currency;
                    accTable.TaxAmount = result.taxAmount;
                    accTable.Amount = result.amount;

                    Db.SetModified(accTable);
                    Db.SaveChanges();
                }

                decimal totalAmount = 0;
                decimal totalTax = 0;
                var projectNum = "";
                var currentAccTable = Db.AccountingTable.Where(x => x.WangHangusId == result.WangHangusId).ToList();

                foreach (var item in currentAccTable)
                {
                    if (item.DrCr == "CR")
                    {
                        totalAmount += Decimal.Negate(Decimal.Parse(item.Amount));
                        if (!projectNum.Contains(item.CostObject))
                            projectNum += item.CostObject + " ";
                        totalTax += Decimal.Negate(Decimal.Parse(item.TaxAmount));
                    }
                    else
                    {
                        totalAmount += Decimal.Parse(item.Amount);
                        if (!projectNum.Contains(item.CostObject))
                            projectNum += item.CostObject + " ";
                        totalTax += Decimal.Parse(item.TaxAmount);
                    }
                }
                var wang = Db.WangHangus.Where(x => x.Id == result.WangHangusId).FirstOrDefault();
                wang.Amount = totalAmount;
                wang.TaxAmount = totalTax;
                Db.SetModified(wang);

                var bd = Db.BankDraft.Where(x => x.Id == wang.BankDraftId).FirstOrDefault();
                bd.BankDraftAmount = totalAmount + totalTax;
                bd.ProjectNo = projectNum;
                Db.SetModified(bd);
                Db.SaveChanges();
            }
            else
            {
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Accounting Table Not Found" });
            }
            

            return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "AccountingTable Updated Successfully!" });
        }

        [HttpDelete]
        public JsonResult Delete(Guid id)
        {
            var accTable = Db.AccountingTable.Find(id);
            if (accTable == null)
            {
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Accounting Table Not Found" });
            }

            Db.AccountingTable.Remove(accTable);
            Db.SaveChanges();

            decimal totalAmount = 0;
            decimal totalTax = 0;
            var projectNum = "";
            var currentAccTable = Db.AccountingTable.Where(x => x.WangHangusId == accTable.WangHangusId).ToList();

            foreach (var item in currentAccTable)
            {
                if (item.DrCr == "CR")
                {
                    totalAmount += Decimal.Negate(Decimal.Parse(item.Amount));
                    if (!projectNum.Contains(item.CostObject))
                        projectNum += item.CostObject + " ";
                    totalTax += Decimal.Negate(Decimal.Parse(item.TaxAmount));
                }
                else
                {
                    totalAmount += Decimal.Parse(item.Amount);
                    if (!projectNum.Contains(item.CostObject))
                        projectNum += item.CostObject + " ";
                    totalTax += Decimal.Parse(item.TaxAmount);
                }
                //totalAmount += Decimal.Parse(item.Amount);

                //if (!projectNum.Contains(item.CostObject))
                //    projectNum += item.CostObject + " ";
                //totalTax += Decimal.Parse(item.TaxAmount);
            }
            var wang = Db.WangHangus.Where(x => x.Id == accTable.WangHangusId).FirstOrDefault();
            wang.Amount = totalAmount;
            wang.TaxAmount = totalTax;
            Db.SetModified(wang);

            var bd = Db.BankDraft.Where(x => x.Id == wang.BankDraftId).FirstOrDefault();
            bd.BankDraftAmount = totalAmount + totalTax;
            bd.ProjectNo = projectNum;
            Db.SetModified(bd);
            Db.SaveChanges();

            return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "AccountingTable Deleted Successfully!" });
        }

    }
}