using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BDA.Data;
using BDA.Entities;
using Hangfire;
using Hangfire.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;

namespace BDA.Web.Controllers
{
    [Authorize]
    public class AdminConsoleController : BaseController
    {
        //private readonly IBackgroundJobClient job;
        //private readonly BdaDBContext db;

        //public ReportingController( IBackgroundJobClient job, BdaDBContext db)
        //{
        //    this.job = job;
        //    this.db = db;
        //}

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Lampiran1()
        {
            return View();
        }

        public IActionResult Lampiran1copy()
        {
            return View();
        }

        public IActionResult Lampiran1copy2()
        {
            return View();
        }

        public IActionResult Lampiran1PDF()
        {
            return new ViewAsPdf("Lampiran1PDF")
            {
                PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 },
            };
        }

        //[HttpGet]
        //public JsonResult CleanseData()
        //{
        //    var allData = Db.MR_BCRM.ToList();
        //    var delData = Db.MR_BCRM_Delete.ToList();

          
        //        //Delete exisitng data 
        //        foreach (var item in delData)
        //        {
        //           var delItem = Db.MR_BCRM.Where(x => x.ZEMPID == item.ZEMPID 
        //           && x.ZMDATETIME.Year == item.ZMDATETIME.Year
        //           && x.ZMDATETIME.Month == item.ZMDATETIME.Month
        //           && x.ZMDATETIME.Day == item.ZMDATETIME.Day
        //           && x.ZEREAD == item.ZEREAD
        //           && x.ZSREAD == item.ZSREAD).FirstOrDefault();

        //        if(delItem != null)
        //        {
        //            Db.MR_BCRM.Remove(delItem);
        //            Db.SaveChanges();
        //        }
             
        //        }
               

        //    return new JsonResult(allData.ToList());
        //}


        //[HttpGet]
        //public JsonResult EditData()
        //{
        //    var allData = Db.MR_BCRM.ToList();
        //    var editData = Db.MR_BCRM_Edit.ToList();


        //    //Delete exisitng data 
        //    foreach (var item in editData)
        //    {
        //        var editItem = Db.MR_BCRM.Where(x => x.ZEMPID == item.ZEMPID
        //        && x.ZMDATETIME.Year == item.ZMDATETIME.Year
        //        && x.ZMDATETIME.Month == item.ZMDATETIME.Month
        //        && x.ZMDATETIME.Day == item.ZMDATETIME.Day
        //        && x.ZMRID == item.ZMRID
        //        && x.ZDEVICEID == item.ZDEVICEID).FirstOrDefault();

        //        if (editItem != null)
        //        {
        //            editItem.ZAREAD = item.ZAREAD;
        //            editItem.ZEREAD = item.ZEREAD;
        //            editItem.ZLPCREAD = item.ZLPCREAD;
        //            editItem.ZSREAD = item.ZSREAD;
        //            editItem.ZSMRU = item.ZSMRU;

        //            Db.SetModified(editItem);
        //            Db.SaveChanges();
        //        }

                
        //    }
            

        //    return new JsonResult(allData.ToList());
        //}

        [HttpGet]
        public JsonResult GetReportForState(string month = null, string year = null, string coCode = null)
        {
            var monthData = Db.StateMonthlyAmount.Where(x => x.Month == Int32.Parse(month) && x.Year == Int32.Parse(year) && x.CoCode == coCode).DefaultIfEmpty().First();

            if (monthData != null)
            {
                var monthList = Db.StateMonthlyAmount.Where(x => x.Month == Int32.Parse(month) && x.Year == Int32.Parse(year) && x.CoCode == coCode).ToList();
                foreach (var item in monthList)
                {
                    Db.StateMonthlyAmount.Remove(item);
                }
                Db.SaveChanges();
            }

            var states = Db.State.ToList();

            //Count any bd data for selected month into StateMonthlyAmount
            var bd = Db.BankDraft
                 .Join(Db.WangCagaran,
                  b => b.Id,
                  w => w.BankDraftId,
                  (b, w) => new { bd = b, wg = w })
                 .Join(Db.State,
                 w => w.wg.Negeri,
                 n => n.Name,
                 (w, n) => new
                 {
                     stateId = n.Id,
                     status = w.bd.Status,
                     bdAmount = w.bd.BankDraftAmount,
                     monthIssued = w.wg.PostingDate.Value.Month,
                     yearIssued = w.wg.PostingDate.Value.Year,
                     coCode = w.wg.CoCode
                 })
                 .Where(x => x.status == "Complete" && x.yearIssued == Int32.Parse(year) && x.monthIssued == Int32.Parse(month) && x.coCode == coCode)
                 .ToList();



            var hasStates = bd.Select(x => x.stateId).Distinct().ToList();

            foreach (var hasState in hasStates)
            {
                var monthly = new StateMonthlyAmount();
                monthly.BDNoIssued = 0;
                monthly.Amount = 0;
                monthly.BDNoOutstanding = 0;
                monthly.OutstandingAmount = 0;

                foreach (var item in bd)
                {
                    if (item.stateId == hasState)
                    {
                        monthly.Id = Guid.NewGuid();
                        monthly.StateId = item.stateId;
                        monthly.BDNoIssued = monthly.BDNoIssued + 1;
                        monthly.Amount = monthly.Amount + item.bdAmount;
                        monthly.Month = Int32.Parse(month);
                        monthly.Year = Int32.Parse(year);
                        monthly.BDNoRecovered = 0;
                        monthly.RecoveryAmount = 0;
                        monthly.BDNoOutstanding = monthly.BDNoOutstanding + 1;
                        monthly.OutstandingAmount = monthly.OutstandingAmount + item.bdAmount;
                        monthly.CoCode = coCode;
                    }
                }
                Db.StateMonthlyAmount.Add(monthly);
                Db.SaveChanges();
            }

            var monthStateHasData = Db.StateMonthlyAmount.Where(x => x.Month == Int32.Parse(month) && x.Year == Int32.Parse(year) && x.CoCode == coCode).Select(x => x.StateId).ToList();
            var noData = states.Select(x => x.Id).Except(monthStateHasData).ToList();

            //Set data to zero for any selected month into StateMonthlyAmount
            foreach (var monthZero in noData)
            {
                var monthly = new StateMonthlyAmount();
       
                monthly.Id = Guid.NewGuid();
                monthly.StateId = monthZero;
                monthly.BDNoIssued = 0;
                monthly.Amount = 0;
                monthly.Month = Int32.Parse(month);
                monthly.Year = Int32.Parse(year);
                monthly.BDNoRecovered = 0;
                monthly.RecoveryAmount = 0;
                monthly.BDNoOutstanding = 0;
                monthly.OutstandingAmount = 0;
                monthly.CoCode = coCode;
                Db.StateMonthlyAmount.Add(monthly);
                Db.SaveChanges();
            }

            //Count any recovery data for selected month into StateMonthlyAmount
            var rec = Db.Recovery
                .Join(Db.BankDraft,
                  r => r.BankDraftId,
                  b => b.Id,
                  (r, b) => new { rec = r, bd = b })
                  .Join(Db.WangCagaran,
                  rx => rx.bd.Id,
                  w => w.BankDraftId,
                 (rx, w) => new { r2 = rx, w2 = w })
                .Join(Db.State,
                 r3 => r3.w2.Negeri,
                 s => s.Name,
                (r3, s) => new
                //.Join(Db.State,
                //r => r.Negeri,
                //n => n.Name,
                //(r, ba) => new
                {
                    stateId = s.Id,
                    status = r3.r2.rec.Status,
                    bdAmount = r3.r2.rec.BDAmount,
                    monthIssued = r3.r2.rec.RecoveryType == "Full" ? r3.r2.rec.PostingDate1.Value.Month : r3.r2.rec.PostingDate2.Value.Month,
                    yearIssued = r3.r2.rec.RecoveryType == "Full" ? r3.r2.rec.PostingDate1.Value.Year : r3.r2.rec.PostingDate2.Value.Year,
                    coCode = r3.r2.rec.CoCode
                })
                .Where(x => x.status == "Complete" && x.yearIssued == Int32.Parse(year) && x.monthIssued == Int32.Parse(month) && x.coCode == coCode)
                .ToList();


            foreach (var item in rec)
            {
                var monthSel = Db.StateMonthlyAmount.Where(x => x.Month == Int32.Parse(month) && x.Year == Int32.Parse(year) && x.StateId == item.stateId && x.CoCode == coCode).FirstOrDefault();
                monthSel.BDNoRecovered = monthSel.BDNoRecovered + 1;
                monthSel.RecoveryAmount = monthSel.RecoveryAmount + item.bdAmount;
                monthSel.BDNoOutstanding = monthSel.BDNoIssued - monthSel.BDNoRecovered;
                monthSel.OutstandingAmount = monthSel.Amount - monthSel.RecoveryAmount;
                Db.SetModified(monthSel);
                Db.SaveChanges();

            }

            var result = Db.StateMonthlyAmount
                       .Join(Db.State,
                       sm => sm.StateId,
                       s => s.Id,
                      (sm, s) => new
                      {
                          state = s.Name,
                          noOfBDIssued = sm.BDNoIssued,
                          amount = string.Format("{0:N}", sm.Amount),
                          amountNum = sm.Amount,
                          noOfRecovery = sm.BDNoRecovered,
                          amountRev = string.Format("{0:N}", sm.RecoveryAmount),
                          amountRevNum = sm.RecoveryAmount,
                          noOfOutstanding = sm.BDNoOutstanding,
                          outstandingAmount = string.Format("{0:N}", sm.OutstandingAmount),
                          monthName = System.Globalization.CultureInfo.GetCultureInfo("en-US").DateTimeFormat.GetMonthName(sm.Month),
                          month = sm.Month.ToString(),
                          year = sm.Year.ToString(),
                          coCode = sm.CoCode
                      })
                      .Where(x => x.month == month && x.year == year && x.coCode == coCode)
                      .OrderBy(c => c.state);

            return new JsonResult(result.ToList());
        }

        //[HttpGet]
        //public JsonResult GetReportForState(string month = null, string year = null)
        //{
        //    var monthData = Db.StateMonthlyAmount.Where(x => x.Month == Int32.Parse(month) && x.Year == Int32.Parse(year)).ToList();

        //    if (monthData.Count > 0)
        //    {
        //        //Delete exisitng data 
        //        foreach (var item in monthData)
        //        {
        //            Db.StateMonthlyAmount.Remove(item);
        //        }
        //        Db.SaveChanges();
        //    }

        //    var states = Db.State.ToList();

        //    //Count any bd data for selected month into StateMonthlyAmount
        //    var bd = Db.BankDraft
        //         .Join(Db.WangCagaran,
        //          b => b.Id,
        //          w => w.BankDraftId,
        //          (b, w) => new { bd = b, wg = w })
        //         .Join(Db.State,
        //         w => w.wg.Negeri,
        //         n => n.Name,
        //         (w, n) => new
        //         {
        //             stateId = n.Id,
        //             status = w.bd.Status,
        //             bdAmount = w.bd.BankDraftAmount,
        //             monthIssued = w.wg.PostingDate.Value.Month,
        //             yearIssued = w.wg.PostingDate.Value.Year
        //         })
        //         .Where(x => x.status == "Complete" && x.yearIssued == Int32.Parse(year) && x.monthIssued == Int32.Parse(month))
        //         .ToList();

        //    var monthly = new StateMonthlyAmount();
        //    monthly.Amount = 0;

        //    var hasStates = bd.Select(x => x.stateId).Distinct().ToList();

        //    foreach (var hasState in hasStates)
        //    {
        //        foreach (var item in bd)
        //        {
        //            if (item.stateId == hasState)
        //            {
        //                monthly.Id = Guid.NewGuid();
        //                monthly.StateId = item.stateId;
        //                monthly.BDNoIssued = monthly.BDNoIssued + 1;
        //                monthly.Amount = monthly.Amount + item.bdAmount;
        //                monthly.Month = Int32.Parse(month);
        //                monthly.Year = Int32.Parse(year);
        //                monthly.BDNoRecovered = 0;
        //                monthly.RecoveryAmount = 0;
        //                monthly.BDNoOutstanding = monthly.BDNoOutstanding + 1;
        //                monthly.OutstandingAmount = monthly.OutstandingAmount + item.bdAmount;
        //            }
        //        }
        //        Db.StateMonthlyAmount.Add(monthly);
        //        Db.SaveChanges();
        //    }

        //    var monthStateHasData = Db.StateMonthlyAmount.Where(x => x.Month == Int32.Parse(month) && x.Year == Int32.Parse(year)).Select(x => x.StateId).ToList();
        //    var noData = states.Select(x => x.Id).Except(monthStateHasData).ToList();

        //    //Set data to zero for any selected month into StateMonthlyAmount
        //    foreach (var monthZero in noData)
        //    {
        //        monthly.Id = Guid.NewGuid();
        //        monthly.StateId = monthZero;
        //        monthly.BDNoIssued = 0;
        //        monthly.Amount = 0;
        //        monthly.Month = Int32.Parse(month);
        //        monthly.Year = Int32.Parse(year);
        //        monthly.BDNoRecovered = 0;
        //        monthly.RecoveryAmount = 0;
        //        monthly.BDNoOutstanding = 0;
        //        monthly.OutstandingAmount = 0;
        //        Db.StateMonthlyAmount.Add(monthly);
        //        Db.SaveChanges();
        //    }

        //    //Count any recovery data for selected month into StateMonthlyAmount
        //    var rec = Db.Recovery
        //         .Join(Db.BankDraft,
        //          r => r.BankDraftId,
        //          b => b.Id,
        //          (r, b) => new { rec = r, bd = b })
        //          .Join(Db.WangCagaran,
        //          rx => rx.bd.Id,
        //          w => w.BankDraftId,
        //         (rx, w) => new { r2 = rx, w2 = w })
        //        .Join(Db.State,
        //         r3 => r3.w2.Negeri,
        //         s => s.Name,
        //        (r3, s) => new
        //        {
        //            stateId = s.Id,
        //            status = r3.r2.rec.Status,
        //            bdAmount = r3.r2.rec.BDAmount,
        //            monthIssued = r3.r2.rec.RecoveryType == "Full" ? r3.r2.rec.PostingDate1.Value.Month : r3.r2.rec.PostingDate2.Value.Month,
        //            yearIssued = r3.r2.rec.RecoveryType == "Full" ? r3.r2.rec.PostingDate1.Value.Year : r3.r2.rec.PostingDate2.Value.Year
        //        })
        //        .Where(x => x.status == "Complete" && x.yearIssued == Int32.Parse(year) && x.monthIssued == Int32.Parse(month))
        //        .ToList();


        //        foreach (var item in rec)
        //        {
        //                var monthSel = Db.StateMonthlyAmount.Where(x => x.Month == Int32.Parse(month) && x.Year == Int32.Parse(year) && x.StateId == item.stateId).FirstOrDefault();
        //                monthSel.BDNoRecovered = monthSel.BDNoRecovered + 1;
        //                monthSel.RecoveryAmount = monthSel.RecoveryAmount + item.bdAmount;
        //                monthSel.BDNoOutstanding = monthSel.BDNoIssued - monthSel.BDNoRecovered;
        //                monthSel.OutstandingAmount = monthSel.Amount - monthSel.RecoveryAmount;

        //                Db.SetModified(monthSel);
        //                Db.SaveChanges();

        //        }



        //    var result = Db.StateMonthlyAmount
        //               .Join(Db.State,
        //               sm => sm.StateId,
        //               s => s.Id,
        //              (sm, s) => new
        //              {
        //                  state = s.Name,
        //                  noOfBDIssued = sm.BDNoIssued,
        //                  amount = string.Format("{0:N}", sm.Amount),
        //                  amountNum = sm.Amount,
        //                  noOfRecovery = sm.BDNoRecovered,
        //                  amountRev = string.Format("{0:N}", sm.RecoveryAmount),
        //                  amountRevNum = sm.RecoveryAmount,
        //                  noOfOutstanding = sm.BDNoOutstanding,
        //                  outstandingAmount = string.Format("{0:N}", sm.OutstandingAmount),
        //                  monthName = System.Globalization.CultureInfo.GetCultureInfo("en-US").DateTimeFormat.GetMonthName(sm.Month),
        //                  month = sm.Month.ToString(),
        //                  year = sm.Year.ToString()
        //              })
        //              .Where(x => x.month == month && x.year == year)
        //              .OrderBy(c => c.state);

        //    return new JsonResult(result.ToList());
        //}

        [HttpPost]
        public void GenerateApplicationForLampiran2(string coCode = null)
        {
            var monthData = Db.MonthlyAmount.Where(x=> x.CoCode == coCode).ToList();

            if (monthData.Count > 0)
            {
                //Delete exisitng data 
                foreach (var item in monthData)
                {
                    Db.MonthlyAmount.Remove(item);
                }
                Db.SaveChanges();
            }

            //generate sum for bd amount & recovery amount by month an year
            for (int year = 2015; year <= 2020; year++)
            {
                for (int month = 1; month <= 12; month++)
                {
                    var sm = Db.StateMonthlyAmount
                        .Select(s => new
                        {
                            state = s.StateId,
                            noOfBDIssued = s.BDNoIssued,
                            amount = s.Amount,
                            noOfRecovery = s.BDNoRecovered,
                            amountRev = s.RecoveryAmount,
                            noOfOutstanding = s.BDNoOutstanding,
                            outstandingAmount = s.OutstandingAmount,
                            month = s.Month,
                            year = s.Year,
                            coCode = s.CoCode
                        })
                        .Where(x => x.month == month && x.year == year && x.coCode == coCode)
                        .ToList();

                    decimal? sumAmount = 0;
                    decimal? sumRecovery = 0;

                    foreach (var item in sm)
                    {

                        sumAmount += item.amount;
                        sumRecovery += item.amountRev;
                    }

                    var monthly = new MonthlyAmount();
                    monthly.Month = month;
                    monthly.Year = year;
                    monthly.CoCode = coCode;
                    monthly.sumAmount = sumAmount;
                    monthly.sumRecovery = sumRecovery;

                    Db.MonthlyAmount.Add(monthly);
                    Db.SaveChanges();
                }
            }

            var m = Db.MonthlyAmount
                   .Select(s => new
                   {
                       month = s.Month,
                       year = s.Year,
                       sumAmount = s.sumAmount,
                       sumRecovery = s.sumRecovery,
                       coCode = s.CoCode
                   })
                   .Where(x=> x.coCode == coCode)
            .ToList();


            foreach (var item in m)
            {
                var currMonth = Db.MonthlyAmount.Where(x => x.Month == item.month && x.Year == item.year && x.CoCode == coCode).FirstOrDefault();
                var prevMonth = Db.MonthlyAmount.Where(x => x.Month == 1 && x.Year == 2015 && x.CoCode == coCode).FirstOrDefault();

                currMonth.diffAmount = 0;
                currMonth.percentageAmount = 0;

                if (item.month == 1)
                {
                    if (item.year == 2015)
                    {
                        prevMonth = Db.MonthlyAmount.Where(x => x.Month == 12 && x.Year == 2020 && x.CoCode == coCode).FirstOrDefault();
                    }
                    else
                    {
                        prevMonth = Db.MonthlyAmount.Where(x => x.Month == 12 && x.Year == item.year - 1 && x.CoCode == coCode).FirstOrDefault();
                    }

                }
                else
                {
                    prevMonth = Db.MonthlyAmount.Where(x => x.Month == item.month - 1 && x.Year == item.year && x.CoCode == coCode).FirstOrDefault();
                }

                currMonth.diffAmount = currMonth.sumAmount - prevMonth.sumAmount;
                if (prevMonth.sumAmount == 0)
                {
                    currMonth.percentageAmount = 0;
                }
                else
                {
                    currMonth.percentageAmount = (currMonth.sumAmount - prevMonth.sumAmount) / prevMonth.sumAmount * 100;
                }

                currMonth.diffAmountRecovery = currMonth.sumRecovery - prevMonth.sumRecovery;
                if (prevMonth.sumRecovery == 0)
                {
                    currMonth.percentageAmountRecovery = 0;
                }
                else
                {
                    currMonth.percentageAmountRecovery = (currMonth.sumRecovery - prevMonth.sumRecovery) / prevMonth.sumRecovery * 100;
                }

                Db.SetModified(currMonth);
                Db.SaveChanges();
            }

        }

        public void GenerateApplicationForDifferenceLampiran2()
        {
            //generate sum for bd amount & recovery amount by month an year

            var m = Db.MonthlyAmount
                    .Select(s => new
                    {
                        month = s.Month,
                        year = s.Year,
                        sumAmount = s.sumAmount,
                        sumRecovery = s.sumRecovery
                    })
             .ToList();


            foreach (var item in m)
            {
                var currMonth = Db.MonthlyAmount.Where(x => x.Month == item.month && x.Year == item.year).FirstOrDefault();
                var prevMonth = Db.MonthlyAmount.Where(x => x.Month == 1 && x.Year == 2015).FirstOrDefault();
                if (item.month == 1)
                {
                    if (item.year == 2015)
                    {
                        prevMonth = Db.MonthlyAmount.Where(x => x.Month == 12 && x.Year == 2020).FirstOrDefault();
                    }
                    else
                    {
                        prevMonth = Db.MonthlyAmount.Where(x => x.Month == 12 && x.Year == item.year - 1).FirstOrDefault();
                    }

                }
                else
                {
                    prevMonth = Db.MonthlyAmount.Where(x => x.Month == item.month - 1 && x.Year == item.year).FirstOrDefault();
                }

                currMonth.diffAmount = currMonth.sumAmount - prevMonth.sumAmount;
                if (prevMonth.sumAmount == 0)
                {
                    currMonth.percentageAmount = 0;
                }
                else
                {
                    currMonth.percentageAmount = (currMonth.sumAmount - prevMonth.sumAmount) / prevMonth.sumAmount * 100;
                }

                currMonth.diffAmountRecovery = currMonth.sumRecovery - prevMonth.sumRecovery;
                if (prevMonth.sumRecovery == 0)
                {
                    currMonth.percentageAmountRecovery = 0;
                }
                else
                {
                    currMonth.percentageAmountRecovery = (currMonth.sumRecovery - prevMonth.sumRecovery) / prevMonth.sumRecovery * 100;
                }

                Db.SetModified(currMonth);
                Db.SaveChanges();
            }
        }

        [HttpGet]
        public JsonResult GetReportForState2(string month = null, string year = null)
        {
            var monthlyExist = Db.StateMonthlyAmount.Where(x => x.Month == Int32.Parse(month) && x.Year == Int32.Parse(year)).FirstOrDefault();

            if (monthlyExist == null)
            {
                var states = Db.State.ToList();

                var bd = Db.BankDraft
                     .Join(Db.WangCagaran,
                      b => b.Id,
                      w => w.BankDraftId,
                      (b, w) => new { bd = b, wg = w })
                     .Join(Db.State,
                     w => w.wg.Negeri,
                     n => n.Name,
                     (w, n) => new
                     {
                         stateId = n.Id,
                         status = w.bd.Status,
                         bdAmount = w.bd.BankDraftAmount,
                         monthIssued = w.wg.PostingDate.Value.Month,
                         yearIssued = w.wg.PostingDate.Value.Year
                     })
                     .Where(x => x.status == "Complete" && x.yearIssued == Int32.Parse(year) && x.monthIssued == Int32.Parse(month))
                     .ToList();

                var monthly = new StateMonthlyAmount();
                monthly.Amount = 0;

                var hasStates = bd.Select(x => x.stateId).Distinct().ToList();

                foreach (var hasState in hasStates)
                {
                    foreach (var item in bd)
                    {
                        if (item.stateId == hasState)
                        {
                            monthly.Id = Guid.NewGuid();
                            monthly.StateId = item.stateId;
                            monthly.BDNoIssued = monthly.BDNoIssued + 1;
                            monthly.Amount = monthly.Amount + item.bdAmount;
                            monthly.Month = Int32.Parse(month);
                            monthly.Year = Int32.Parse(year);
                            monthly.BDNoRecovered = 0;
                            monthly.RecoveryAmount = 0;
                            monthly.BDNoOutstanding = 0;
                            monthly.OutstandingAmount = 0;
                        }

                    }

                    Db.StateMonthlyAmount.Add(monthly);
                    Db.SaveChanges();
                }
            }

            var result = Db.StateMonthlyAmount
                         .Join(Db.State,
                         sm => sm.StateId,
                         s => s.Id,
                        (sm, s) => new
                        {
                            state = s.Name,
                            noOfBDIssued = sm.BDNoIssued,
                            amount = string.Format("{0:N}", sm.Amount),
                            amountNum = sm.Amount,
                            noOfRecovery = sm.BDNoRecovered,
                            amountRev = string.Format("{0:N}", sm.RecoveryAmount),
                            amountRevNum = sm.RecoveryAmount,
                            noOfOutstanding = sm.BDNoOutstanding,
                            outstandingAmount = string.Format("{0:N}", sm.OutstandingAmount),
                            monthName = System.Globalization.CultureInfo.GetCultureInfo("en-US").DateTimeFormat.GetMonthName(sm.Month),
                            month = sm.Month.ToString(),
                            year = sm.Year.ToString()
                        })
                        .Where( x => x.month == month && x.year == year)
                        .OrderBy(c => c.state);

            return new JsonResult(result.ToList());
        }

        //[HttpGet]
        //public JsonResult GetReportForLampiran2()
        //{
        //    var result = Db.MonthlyAmount
        //           .Select(s => new
        //           {
        //               month = s.Month,
        //               year = s.Year,
        //               sumAmount1 = s.sumAmount,
        //               sumAmount = string.Format("{0:N}", s.sumAmount),
        //               sumRecovery1 = s.sumRecovery,
        //               sumRecovery = string.Format("{0:N}", s.sumRecovery),
        //               diffAmount = string.Format("{0:N}", s.diffAmount),
        //               diffRec = string.Format("{0:N}", s.diffAmountRecovery),
        //               percentage = s.percentageAmount,
        //               percentageRec = s.percentageAmountRecovery
        //           })
        //           .Where(s => s.year == 2017)
        //    .ToList();

        //    return new JsonResult(result.ToList());
        //}

        [HttpGet]
        public JsonResult GetReportForLampiran2(string monthFrom = null, string yearFrom = null, string monthTo = null, string yearTo = null)
        {
            int yearDiff = 0;
            yearDiff = Int32.Parse(yearTo) - Int32.Parse(yearFrom);

            if (yearFrom == yearTo)
            {
                var result = Db.MonthlyAmount
                  .Select(s => new
                  {
                      month = s.Month,
                      year = s.Year,
                      sumAmount1 = s.sumAmount,
                      sumAmount = string.Format("{0:N}", s.sumAmount),
                      sumRecovery1 = s.sumRecovery,
                      sumRecovery = string.Format("{0:N}", s.sumRecovery),
                      diffAmount = string.Format("{0:N}", s.diffAmount),
                      diffRec = string.Format("{0:N}", s.diffAmountRecovery),
                      percentage = s.percentageAmount,
                      percentageRec = s.percentageAmountRecovery
                  })
                  .Where(s => s.month >= Int32.Parse(monthFrom) && s.month <= Int32.Parse(monthTo) && s.year == Int32.Parse(yearFrom))
           .ToList();

                return new JsonResult(result.ToList());

            }
            else if(Int32.Parse(yearFrom) < Int32.Parse(yearTo) && yearDiff > 1)
            {
                
                var result1 = Db.MonthlyAmount
                   .Select(s => new
                   {
                       month = s.Month,
                       year = s.Year,
                       sumAmount1 = s.sumAmount,
                       sumAmount = string.Format("{0:N}", s.sumAmount),
                       sumRecovery1 = s.sumRecovery,
                       sumRecovery = string.Format("{0:N}", s.sumRecovery),
                       diffAmount = string.Format("{0:N}", s.diffAmount),
                       diffRec = string.Format("{0:N}", s.diffAmountRecovery),
                       percentage = s.percentageAmount,
                       percentageRec = s.percentageAmountRecovery
                   })
                   .Where(s => s.month >= Int32.Parse(monthFrom) && s.year == Int32.Parse(yearFrom))
                   .ToList();

                var result2 = Db.MonthlyAmount
                  .Select(s => new
                  {
                      month = s.Month,
                      year = s.Year,
                      sumAmount1 = s.sumAmount,
                      sumAmount = string.Format("{0:N}", s.sumAmount),
                      sumRecovery1 = s.sumRecovery,
                      sumRecovery = string.Format("{0:N}", s.sumRecovery),
                      diffAmount = string.Format("{0:N}", s.diffAmount),
                      diffRec = string.Format("{0:N}", s.diffAmountRecovery),
                      percentage = s.percentageAmount,
                      percentageRec = s.percentageAmountRecovery
                  })
                  .Where(s => s.year >= (Int32.Parse(yearFrom) +1) && s.year < Int32.Parse(yearTo))
                  .ToList();

                var result3 = Db.MonthlyAmount
                   .Select(s => new
                   {
                       month = s.Month,
                       year = s.Year,
                       sumAmount1 = s.sumAmount,
                       sumAmount = string.Format("{0:N}", s.sumAmount),
                       sumRecovery1 = s.sumRecovery,
                       sumRecovery = string.Format("{0:N}", s.sumRecovery),
                       diffAmount = string.Format("{0:N}", s.diffAmount),
                       diffRec = string.Format("{0:N}", s.diffAmountRecovery),
                       percentage = s.percentageAmount,
                       percentageRec = s.percentageAmountRecovery
                   })
                   .Where(s => s.month <= Int32.Parse(monthTo) && s.year == Int32.Parse(yearTo))
                   .ToList();

                var result = result1.Concat(result2).Concat(result3);
                return new JsonResult(result.ToList());
            }
            else
            {
                var result1 = Db.MonthlyAmount
                  .Select(s => new
                  {
                      month = s.Month,
                      year = s.Year,
                      sumAmount1 = s.sumAmount,
                      sumAmount = string.Format("{0:N}", s.sumAmount),
                      sumRecovery1 = s.sumRecovery,
                      sumRecovery = string.Format("{0:N}", s.sumRecovery),
                      diffAmount = string.Format("{0:N}", s.diffAmount),
                      diffRec = string.Format("{0:N}", s.diffAmountRecovery),
                      percentage = s.percentageAmount,
                      percentageRec = s.percentageAmountRecovery
                  })
                  .Where(s => s.month >= Int32.Parse(monthFrom) && s.year == Int32.Parse(yearFrom))
           .ToList();

                var result2 = Db.MonthlyAmount
                   .Select(s => new
                   {
                       month = s.Month,
                       year = s.Year,
                       sumAmount1 = s.sumAmount,
                       sumAmount = string.Format("{0:N}", s.sumAmount),
                       sumRecovery1 = s.sumRecovery,
                       sumRecovery = string.Format("{0:N}", s.sumRecovery),
                       diffAmount = string.Format("{0:N}", s.diffAmount),
                       diffRec = string.Format("{0:N}", s.diffAmountRecovery),
                       percentage = s.percentageAmount,
                       percentageRec = s.percentageAmountRecovery
                   })
                   .Where(s => s.month <= Int32.Parse(monthTo) && s.year == Int32.Parse(yearTo))
            .ToList();

                var result = result1.Concat(result2);
                return new JsonResult(result.ToList());
            }

        }

        [HttpGet]
        public JsonResult GetReportForLampiran3(string month = null, string year = null)
        {
            var monthlyExist = Db.StateMonthlyAmount.Where(x => x.Month == Int32.Parse(month) && x.Year == Int32.Parse(year)).FirstOrDefault();

            if (monthlyExist == null)
            {
                var states = Db.State.ToList();

                var bd = Db.BankDraft
                     .Join(Db.WangCagaran,
                      b => b.Id,
                      w => w.BankDraftId,
                      (b, w) => new { bd = b, wg = w })
                     .Join(Db.State,
                     w => w.wg.Negeri,
                     n => n.Name,
                     (w, n) => new
                     {
                         stateId = n.Id,
                         status = w.bd.Status,
                         bdAmount = w.bd.BankDraftAmount,
                         monthIssued = w.bd.TGBSAcceptedOn.Value.Month,
                         yearIssued = w.bd.TGBSAcceptedOn.Value.Year
                     })
                     .Where(x => x.status == "Complete" && x.yearIssued == Int32.Parse(year) && x.monthIssued == Int32.Parse(month))
                     .ToList();

                var monthly = new StateMonthlyAmount();
                monthly.Amount = 0;

                var hasStates = bd.Select(x => x.stateId).Distinct().ToList();

                foreach (var hasState in hasStates)
                {
                    foreach (var item in bd)
                    {
                        if (item.stateId == hasState)
                        {
                            monthly.Id = Guid.NewGuid();
                            monthly.StateId = item.stateId;
                            monthly.BDNoIssued = monthly.BDNoIssued + 1;
                            monthly.Amount = monthly.Amount + item.bdAmount;
                            monthly.Month = Int32.Parse(month);
                            monthly.Year = Int32.Parse(year);
                            monthly.BDNoRecovered = 0;
                            monthly.RecoveryAmount = 0;
                            monthly.BDNoOutstanding = 0;
                            monthly.OutstandingAmount = 0;
                        }

                    }

                    Db.StateMonthlyAmount.Add(monthly);
                    Db.SaveChanges();
                }


                var noStates = states.Select(x => x.Id).Except(hasStates).ToList();


                foreach (var noState in noStates)
                {
                    monthly.Id = Guid.NewGuid();
                    monthly.StateId = noState;
                    monthly.BDNoIssued = 0;
                    monthly.Amount = 0;
                    monthly.Month = Int32.Parse(month);
                    monthly.Year = Int32.Parse(year);
                    monthly.BDNoRecovered = 0;
                    monthly.RecoveryAmount = 0;
                    monthly.BDNoOutstanding = 0;
                    monthly.OutstandingAmount = 0;
                    Db.StateMonthlyAmount.Add(monthly);
                    Db.SaveChanges();
                }
            }

            var result = Db.StateMonthlyAmount
                         .Join(Db.State,
                         sm => sm.StateId,
                         s => s.Id,
                        (sm, s) => new
                        {
                            state = s.Name,
                            noOfBDIssued = sm.BDNoIssued,
                            amount = sm.Amount,
                            month = sm.Month.ToString(),
                            year = sm.Year.ToString()
                        })
                        .Where(x => x.month == month && x.year == year)
                        .OrderBy(c => c.state);

            return new JsonResult(result.ToList());
        }


        [HttpGet]
        public JsonResult GetReportForLampiran4(string month = null, string year = null)
        {
            if (!year.Equals("2015"))
            {
                var result = Db.StateMonthlyAmount
                        .Join(Db.State,
                        sm => sm.StateId,
                        s => s.Id,
                       (sm, s) => new
                       {
                           state = s.Name,
                           noOfBDIssued = sm.BDNoIssued,
                           amount = sm.Amount,
                           noOfRecovery = sm.BDNoRecovered,
                           amountRev = sm.RecoveryAmount,
                           noOfOutstanding = sm.BDNoOutstanding,
                           outstandingAmount = sm.OutstandingAmount,
                           month = sm.Month,
                           year = sm.Year
                       })
                       .Where(x => x.year < Int32.Parse(year));

                var result2 = Db.StateMonthlyAmount
                           .Join(Db.State,
                           sm => sm.StateId,
                           s => s.Id,
                          (sm, s) => new
                          {
                              state = s.Name,
                              noOfBDIssued = sm.BDNoIssued,
                              amount = sm.Amount,
                              noOfRecovery = sm.BDNoRecovered,
                              amountRev = sm.RecoveryAmount,
                              noOfOutstanding = sm.BDNoOutstanding,
                              outstandingAmount = sm.OutstandingAmount,
                              month = sm.Month,
                              year = sm.Year
                          })
                          .Where(x => x.year == Int32.Parse(year) && x.month <= Int32.Parse(month));

                var result4 = result.Concat(result2).ToList();

                var result3 = result4
              .GroupBy(r => new { r.state, r.year })
              .Select(group => new
              {
                  fee = group.Key,
                  state = group.Select(r => r.state).FirstOrDefault(),
                  year = group.Select(r => r.year).FirstOrDefault(),
                  noOfBDIssued = group.Sum(r => r.noOfBDIssued),
                  amount = string.Format("{0:N}", group.Sum(r => r.amount)),
                  noOfRecovery = group.Sum(r => r.noOfRecovery),
                  amountRev = string.Format("{0:N}", group.Sum(r => r.amountRev)),
              })
              .Where(x => x.year != 0)
              .OrderBy(c => c.state).ThenBy(n => n.year);

                return new JsonResult(result3.ToList());
            }
            else
            {
                var result2 = Db.StateMonthlyAmount
                         .Join(Db.State,
                         sm => sm.StateId,
                         s => s.Id,
                        (sm, s) => new
                        {
                            state = s.Name,
                            noOfBDIssued = sm.BDNoIssued,
                            amount = sm.Amount,
                            noOfRecovery = sm.BDNoRecovered,
                            amountRev = sm.RecoveryAmount,
                            noOfOutstanding = sm.BDNoOutstanding,
                            outstandingAmount = sm.OutstandingAmount,
                            month = sm.Month,
                            year = sm.Year
                        })
                        .Where(x => x.year == Int32.Parse(year) && x.month <= Int32.Parse(month));

                var result4 = result2.ToList();

                var result3 = result4
              .GroupBy(r => new { r.state, r.year })
              .Select(group => new {
                  fee = group.Key,
                  state = group.Select(r => r.state).FirstOrDefault(),
                  year = group.Select(r => r.year).FirstOrDefault(),
                  noOfBDIssued = group.Sum(r => r.noOfBDIssued),
                  amount = string.Format("{0:N}", group.Sum(r => r.amount)),
                  noOfRecovery = group.Sum(r => r.noOfRecovery),
                  amountRev = string.Format("{0:N}", group.Sum(r => r.amountRev)),
              })
              .Where(x => x.year != 0)
               .OrderBy(c => c.state).ThenBy(n => n.year);

                return new JsonResult(result3.ToList());
            }

        }

        [HttpGet]
        public JsonResult GetReportForLampiran4ByState(string month = null, string year = null)
        {
            
                var result = Db.StateMonthlyAmount
                         .Join(Db.State,
                         sm => sm.StateId,
                         s => s.Id,
                        (sm, s) => new
                        {
                            state = s.Name,
                            noOfBDIssued = sm.BDNoIssued,
                            amount = string.Format("{0:N}", sm.Amount),
                            noOfRecovery = sm.BDNoRecovered,
                            amountRev = string.Format("{0:N}", sm.RecoveryAmount),
                            noOfOutstanding = sm.BDNoOutstanding,
                            outstandingAmount = string.Format("{0:N}", sm.OutstandingAmount),
                            month = sm.Month,
                            year = sm.Year
                        })
                        .Where(x => x.year == Int32.Parse(year) && x.month == Int32.Parse(month));

                return new JsonResult(result.ToList());

        }

        [HttpGet]
        public JsonResult GetTotalForLampiran4ByStatePreviousMonth(string month = null, string year = null, string type = null)
        {
            string prevMonth = null;
            string prevYear = null;
            string prevMonthName = null;
            string monthName = null;

            if (month.Equals("1"))
            {
                prevMonth = "12";
                prevYear = (Int32.Parse(year) - 1).ToString();
            }
            else
            {
                prevMonth = (Int32.Parse(month) - 1).ToString();
                prevYear = year;
            }

            prevMonthName = System.Globalization.CultureInfo.GetCultureInfo("en-US").DateTimeFormat.GetMonthName(Int32.Parse(prevMonth));
            monthName = System.Globalization.CultureInfo.GetCultureInfo("en-US").DateTimeFormat.GetMonthName(Int32.Parse(month));

            var prevResult = Db.StateMonthlyAmount
                          .Select(s => new 
                          {
                              s.Amount,
                              s.RecoveryAmount,
                              month = s.Month,
                              year = s.Year
                          })
                         .Where(x => x.year == Int32.Parse(prevYear) && x.month == Int32.Parse(prevMonth));

            var totalPrev = prevResult.Sum(x => x.Amount);
            var totalPrevMonth = string.Format("{0:N}", totalPrev).ToString();

            var totalPrev2 = prevResult.Sum(x => x.RecoveryAmount);
            var totalPrevMonth2 = string.Format("{0:N}", totalPrev2).ToString();

            var result = Db.StateMonthlyAmount
                         .Select(s => new
                         {
                             s.Amount,
                             s.RecoveryAmount,
                             month = s.Month,
                             year = s.Year
                         })
                        .Where(x => x.year == Int32.Parse(year) && x.month == Int32.Parse(month));

            var total = result.Sum(x => x.Amount);
            var totalMonth = string.Format("{0:N}", total).ToString();

            var total2 = result.Sum(x => x.RecoveryAmount);
            var totalMonth2 = string.Format("{0:N}", total2).ToString();

            var difference = totalPrev - total;
            var diffMonth = string.Format("{0:N}", difference).ToString();
            var percentage = difference;

            var difference2 = totalPrev2 - total;
            var diffMonth2 = string.Format("{0:N}", difference2).ToString();
            var percentage2 = difference2;

            if (totalPrev != 0)
            {
                percentage = difference / totalPrev * 100;
            }
            else
            {
                percentage = 0;
            }

            if (totalPrev2 != 0)
            {
                percentage2 = difference2 / totalPrev2 * 100;
            }
            else
            {
                percentage2 = 0;
            }

            string percent = string.Format("{0:F2}", Decimal.Parse(percentage.ToString())).TrimStart('-') + "%";
            string percent2 = string.Format("{0:F2}", Decimal.Parse(percentage2.ToString())).TrimStart('-') + "%";

            var indicator = "Reduction";
            if(difference < 0)
            {
                indicator = "Addition";
            }

            if (difference2 < 0)
            {
                indicator = "Addition";
            }

            List<string> amountList = new List<string>() { prevMonthName, totalPrevMonth, monthName, totalMonth, diffMonth, percent, indicator, year };

            if (type == "recovery")
            {
                amountList = new List<string>() { prevMonthName, totalPrevMonth2, monthName, totalMonth2, diffMonth2, percent2, indicator, year }; 
            }

            return new JsonResult(amountList);

        }

        [HttpGet]
        public JsonResult GetReportForLampiran4Comparison(string month = null, string year = null)
        {
            if (!year.Equals("2015"))
            {
                var result = Db.StateMonthlyAmount
                        .Join(Db.State,
                        sm => sm.StateId,
                        s => s.Id,
                       (sm, s) => new
                       {
                           state = s.Name,
                           noOfBDIssued = sm.BDNoIssued,
                           amount = sm.Amount,
                           noOfRecovery = sm.BDNoRecovered,
                           amountRev = sm.RecoveryAmount,
                           noOfOutstanding = sm.BDNoOutstanding,
                           outstandingAmount = sm.OutstandingAmount,
                           month = sm.Month,
                           year = sm.Year
                       })
                       .Where(x => x.year < Int32.Parse(year));

                var result2 = Db.StateMonthlyAmount
                           .Join(Db.State,
                           sm => sm.StateId,
                           s => s.Id,
                          (sm, s) => new
                          {
                              state = s.Name,
                              noOfBDIssued = sm.BDNoIssued,
                              amount = sm.Amount,
                              noOfRecovery = sm.BDNoRecovered,
                              amountRev = sm.RecoveryAmount,
                              noOfOutstanding = sm.BDNoOutstanding,
                              outstandingAmount = sm.OutstandingAmount,
                              month = sm.Month,
                              year = sm.Year
                          })
                          .Where(x => x.year == Int32.Parse(year) && x.month <= Int32.Parse(month));

                var result4 = result.Concat(result2).ToList();

                var result3 = result4
              .GroupBy(r => new { r.state })
              .Select(group => new
              {
                  fee = group.Key,
                  state = group.Select(r => r.state).FirstOrDefault(),
                  noOfBDIssued = group.Sum(r => r.noOfBDIssued),
                  amount = group.Sum(r => r.amount),
              })
              .OrderBy(c => c.state);

                return new JsonResult(result3.ToList());
            }
            else
            {
                var result2 = Db.StateMonthlyAmount
                         .Join(Db.State,
                         sm => sm.StateId,
                         s => s.Id,
                        (sm, s) => new
                        {
                            state = s.Name,
                            noOfBDIssued = sm.BDNoIssued,
                            amount = sm.Amount,
                            noOfRecovery = sm.BDNoRecovered,
                            amountRev = sm.RecoveryAmount,
                            noOfOutstanding = sm.BDNoOutstanding,
                            outstandingAmount = sm.OutstandingAmount,
                            month = sm.Month,
                            year = sm.Year
                        })
                        .Where(x => x.year == Int32.Parse(year) && x.month <= Int32.Parse(month));

                var result4 = result2.ToList();

                var result3 = result4
              .GroupBy(r => new { r.state })
              .Select(group => new {
                  fee = group.Key,
                  state = group.Select(r => r.state).FirstOrDefault(),
                  noOfBDIssued = group.Sum(r => r.noOfBDIssued),
                  amount = group.Sum(r => r.amount),
              })
               .OrderBy(c => c.state);

                return new JsonResult(result3.ToList());
            }

        }

        public JsonResult GetReportForLampiran6(string refNo = null, string assigment = null, string ermsDocNo = null, string busA = null, string state = null,
                                              string pkType = null, string docDate = null, string ermsPostingDate = null, string bdAmount = null, string keteranganKerja = null)
        {
            var result = Db.BankDraft
                          .Join(Db.WangCagaran,
                      b => b.Id,
                      w => w.BankDraftId,
                      (b, w) => new 
                          //bd = b, wg = w })
                     //.Join(Db.BusinessArea,
                     //w => w.wg.BusinessArea,
                     //ba => ba.Name,
                     //(w, ba) => new { wG = w, ba = ba })
                     // .Join(Db.State,
                     //ba => ba.ba.StateId,
                     //s => s.Id,
                     //(ba, s) => new
                     {
                         bdNo = b.BankDrafNoIssued,
                         requester = b.Requester.FullName,
                         ermsDocNo = w.ErmsDocNo,
                         coCode = w.CoCode,
                         busA = w.BusinessArea,
                         nameOnBD = w.NamaPemegangCagaran,
                         bdAmount = "(" + string.Format("{0:N}", b.BankDraftAmount) + ")",
                         ermsPostingDate = b.TGBSAcceptedOn.Value.ToShortDateString(),
                         docDate = b.SubmittedOn.Value.ToShortDateString(),
                         projNo = b.ProjectNo,
                         refNo = b.RefNo,
                         bankDraftId = b.Id,
                         status = b.Status,
                         assignment = w.Assignment,
                         keteranganKerja = w.KeteranganKerja,
                         finalApp = b.FinalApplication,
                         pkType = w.Type,
                         state = w.Negeri
                     })
                     .Where(x => (x.assignment.Contains(assigment) || assigment == null)
                       && (x.refNo.Contains(refNo) || refNo == null)
                                    && (x.ermsDocNo.Contains(ermsDocNo) || ermsDocNo == null)
                                    && (x.busA.Contains(busA) || busA == null)
                                    && (x.state.Contains(state) || state == null)
                                    && (x.pkType.Contains(pkType) || pkType == null)
                                    && (x.docDate.Contains(docDate) || docDate == null)
                                    && (x.ermsPostingDate.Contains(ermsPostingDate) || ermsPostingDate == null)
                                    && (x.bdAmount.ToString().Contains(bdAmount) || bdAmount == null)
                                    && (x.keteranganKerja.Contains(keteranganKerja) || keteranganKerja == null)
                                    && (x.status.Equals("Complete")));
                                    

            var result2 = Db.BankDraft
                           .Join(Db.WangHangus,
                      b => b.Id,
                      w => w.BankDraftId,
                     (b, w) => new 
                    //{ bd = b, wh = w })
                    // .Join(Db.BusinessArea,
                    // w => w.wh.BusinessArea,
                    // ba => ba.Name,
                    //(w, ba) => new { wH = w, ba = ba })
                    //  .Join(Db.State,
                    // ba => ba.ba.StateId,
                    // s => s.Id,
                    // (ba, s) => new
                     {
                         bdNo = b.BankDrafNoIssued,
                         requester = b.Requester.FullName,
                         ermsDocNo = "",
                         coCode = w.CoCode,
                         busA = w.BusinessArea,
                         nameOnBD = "",
                         bdAmount = "(" + string.Format("{0:N}", b.BankDraftAmount) + ")",
                         ermsPostingDate = b.TGBSAcceptedOn.Value.ToShortDateString(),
                         docDate = b.SubmittedOn.Value.ToShortDateString(),
                         projNo = b.ProjectNo,
                         refNo = b.RefNo,
                         bankDraftId = b.Id,
                         status = b.Status,
                         assignment = "",
                         keteranganKerja = "",
                         finalApp = b.FinalApplication,
                         pkType = "",
                         state = w.Region
                     })
                     .Where(x => (x.assignment.Contains(assigment) || assigment == null)
                      && (x.refNo.Contains(refNo) || refNo == null)
                                    && (x.ermsDocNo.Contains(ermsDocNo) || ermsDocNo == null)
                                    && (x.busA.Contains(busA) || busA == null)
                                    && (x.state.Contains(state) || state == null)
                                    && (x.pkType.Contains(pkType) || pkType == null)
                                    && (x.docDate.Contains(docDate) || docDate == null)
                                    && (x.ermsPostingDate.Contains(ermsPostingDate) || ermsPostingDate == null)
                                    && (x.bdAmount.ToString().Contains(bdAmount) || bdAmount == null)
                                    && (x.keteranganKerja.Contains(keteranganKerja) || keteranganKerja == null)
                                    && (x.status.Equals("Complete")));

            var result3 = result.Concat(result2);

            return new JsonResult(result3.AsEnumerable()
                .OrderBy(a => a.ermsPostingDate)
               .Select((x, index) => new
               {
                   index,
                   x.refNo,
                   x.assignment,
                   x.ermsDocNo,
                   x.busA,
                   x.state,
                   x.pkType,
                   x.docDate,
                   x.ermsPostingDate,
                   x.bdAmount,
                   x.keteranganKerja
               }).ToList());

            //return new JsonResult(result3.ToList());

            //.Select((x, index) => new
            // {
            //     index,
            //     x.assignment,
            //     x.ermsDocNo,
            //     x.ba,
            //     x.state,
            //     x.pkType,
            //     x.docDate,
            //     x.ermsPostingDate,
            //     x.bdAmount,
            //     x.keteranganKerja
            // })
            //return new JsonResult(result3.ToList());


        }

        public JsonResult GetReportForLampiran7()
        {
            var result = Db.BankDraft
                         .Select(b => new
                        {
                            refNo = b.RefNo,
                            dateSubmit = b.SubmittedOn == null ? "" : b.SubmittedOn.Value.ToShortDateString(),
                            dateVerify = b.VerifiedOn == null ? "" : b.VerifiedOn.Value.ToShortDateString(),
                            dateApprove = b.ApprovedOn == null ? "" : b.ApprovedOn.Value.ToShortDateString(),
                            dateAccept = b.TGBSAcceptedOn == null ? "" : b.TGBSAcceptedOn.Value.ToShortDateString(),
                            dateReceive = b.ReceiveBankDraftDate == null ? "" : b.ReceiveBankDraftDate.Value.ToShortDateString(),
                            dateIssue = b.TGBSIssuedOn == null ? "" : b.TGBSIssuedOn.Value.ToShortDateString()
                         })
                       .Where(x=> x.refNo != null)
                       .OrderBy(b=> b.refNo);

            return new JsonResult(result.ToList());
        }

        public void GenerateApplication()
        {
            var bd = Db.BankDraft
                     .Join(Db.WangCagaran,
                      b => b.Id,
                      w => w.BankDraftId,
                      (b, w) => new { bd = b, wg = w })
                     .Join(Db.State,
                     w => w.wg.Negeri,
                     n => n.Name,
                     (w, n) => new
                     {
                         stateId = n.Id,
                         status = w.bd.Status,
                         bdAmount = w.bd.BankDraftAmount,
                         monthIssued = w.bd.TGBSAcceptedOn.Value.Month,
                         yearIssued = w.bd.TGBSAcceptedOn.Value.Year
                     })
                     .Where(x => x.status == "Complete" && x.yearIssued >= 2020)
                     .ToList();

            foreach (var item in bd)
            {
                var monthly = Db.StateMonthlyAmount.Where(x => x.Month == item.monthIssued && x.Year == item.yearIssued && x.StateId == item.stateId).FirstOrDefault();

                monthly.StateId = item.stateId;
                monthly.BDNoIssued = monthly.BDNoIssued + 1;
                monthly.Amount = monthly.Amount + item.bdAmount;

                Db.SetModified(monthly);
                Db.SaveChanges();
            }
        }

        [HttpPost]
        public void GenerateReportforState()
        {
            var states = Db.State.ToList();

            //foreach (var state in states)
            //{
            //    int count = 0;

            //    for (int year = 2015; year <= 2020; year++)
            //    {

            //        for (int month = 1; month <= 12; month++)
            //        {
            //            StateMonthlyAmount monthly = new StateMonthlyAmount();
            //            monthly.Amount = 0;
            //            monthly.StateId = state.Id;

            //            monthly.BDNoIssued = 0;
            //            monthly.Amount = 0;
            //            monthly.Month = month;
            //            monthly.Year = year;

            //            Db.StateMonthlyAmount.Add(monthly);
            //            Db.SaveChanges();
            //        }

            //    }

            //}


            var bd = Db.BankDraft
                   .Join(Db.WangCagaran,
                    b => b.Id,
                    w => w.BankDraftId,
                    (b, w) => new { bd = b, wg = w })
                   .Join(Db.State,
                   w => w.wg.Negeri,
                   n => n.Name,
                   (w, n) => new
                   {
                       stateId = n.Id,
                       status = w.bd.Status,
                       bdAmount = w.bd.BankDraftAmount,
                       monthIssued = w.bd.TGBSAcceptedOn.Value.Month,
                       yearIssued = w.bd.TGBSAcceptedOn.Value.Year
                   })
                   .Where(x => x.status == "Complete" && x.yearIssued == 2018 && x.monthIssued == 11)
                   .ToList();

            var monthly = new StateMonthlyAmount();
            monthly.Amount = 0;

            var hasStates = bd.Select(x => x.stateId).Distinct().ToList();

            foreach (var hasState in hasStates)
            {
                foreach (var item in bd)
                {
                    if (item.stateId == hasState)
                    {
                        monthly.Id = Guid.NewGuid();
                        monthly.StateId = item.stateId;
                        monthly.BDNoIssued = monthly.BDNoIssued + 1;
                        monthly.Amount = monthly.Amount + item.bdAmount;
                        monthly.Month = 11;
                        monthly.Year = 2018;
                        monthly.BDNoRecovered = 0;
                        monthly.RecoveryAmount = 0;
                        monthly.BDNoOutstanding = 0;
                        monthly.OutstandingAmount = 0;
                    }

                }

                Db.StateMonthlyAmount.Add(monthly);
                Db.SaveChanges();
            }


            var noStates = states.Select(x => x.Id).Except(hasStates).ToList();


            foreach (var noState in noStates)
            {
                        monthly.Id = Guid.NewGuid();
                        monthly.StateId = noState;
                        monthly.BDNoIssued = 0;
                        monthly.Amount = 0;
                        monthly.Month = 11;
                        monthly.Year = 2018;
                        monthly.BDNoRecovered = 0;
                        monthly.RecoveryAmount = 0;
                        monthly.BDNoOutstanding = 0;
                        monthly.OutstandingAmount = 0;
                        Db.StateMonthlyAmount.Add(monthly);
                        Db.SaveChanges();
            }

           

        }


    }
}

//var entity = Db.BankDraft
//         .Join(Db.Recovery,
//         b => b.Id,
//         r => r.BankDraftId,
//         (b, r) => new { bd = b, r = r })
//        .Join(Db.WangCagaran,
//         b => b.bd.Id,
//         w => w.BankDraftId,
//         (b, w) => new { bd = b, wg = w })
//        .Join(Db.BusinessArea,
//        w => w.wg.BusinessArea,
//        ba => ba.Name,
//        (w, ba) => new
//        {
//            stateId = ba.StateId,
//            status = w.bd.bd.Status,
//            bdAmount = w.bd.bd.BankDraftAmount,
//            monthIssued = w.bd.bd.TGBSAcceptedOn.Value.Month,
//            yearIssued = w.bd.bd.TGBSAcceptedOn.Value.Year,
//            recoveryAmount = w.bd.r.BDAmount,
//            monthRev = w.bd.r.CompletedOn.Value.Month,
//            yearRev = w.bd.r.CompletedOn.Value.Year
//        })
//        .Where(x => x.status == "Complete");


//foreach (var state in states)
//{
//    for (int year = 2015; year <= 2020; year++)
//    {
//        for (int month = 1; month <= 12; month++)
//        {
