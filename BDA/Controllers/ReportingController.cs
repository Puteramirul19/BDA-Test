using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BDA.Data;
using BDA.Entities;
using BDA.Services;
using BDA.ViewModel;
using Hangfire;
using Hangfire.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Rotativa.AspNetCore;

namespace BDA.Web.Controllers
{
    [Authorize]
    public class ReportingController : BaseController
    {
        private ILogService _logService;
        List<string> listCol = new List<string> { "0" };
        List<string> BdCol = new List<string> { "RefNo","BankDraftAmount", "NameOnBD","RequesterId","ApproverId","Status","InstructionLetterRefNo","SendMethod","BankDraftDate","PostageNo",
            "BankDrafNoIssued", "ReceiverContactNo", "CoverMemoRefNo","ReceiveBankDraftDate", "ReceiptNo","BankDraftAmount" };
        List<string> listColWC = new List<string> { "0" };
        List<string> listColWH = new List<string> { "0" };
        List<string> listColWCH = new List<string> { "0" };
        List<string> listColNotExistWH = new List<string> { "Alamat1", "KeteranganKerja", "JKRInvolved", "CajKod", "WBSProjekNo" };
        List<string> listColNotExistWC = new List<string> { "PONumber", "InvoiceNumber", "VendorNo", "VendorName", "BankAccount", "BankCountry", "Description" };
        List<string> listColAccountingTable = new List<string> { "GLAccount", "CONW", "CostObject", "TaxCode", "Currency", "TaxAmount", "Amount" };

        public ReportingController(ILogService logService)
        {
            this._logService = logService;
        }

        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult Lampiran1()
        {
            return View();
        }

        public IActionResult Log()
        {
            return View();
        }

        public IActionResult test()
        {
            return View();
        }

        [HttpPost]
        public string GetLogFull(LogViewModel input)
        {
            try
            {
                if (input.ColumnList == null)
                    return "Column List cannot be empty.";

                if (input.TableName == null || input.TableName == "")
                    return "TableName cannot be empty.";

                string Query = "select " + string.Join(",", input.ColumnList) + " from " + input.TableName;
                string Query1 = "";
                string Query2 = "";

                if (input.TableName == "Cancellation" || input.TableName == "Lost" || input.TableName == "Recovery")
                {
                    listCol.RemoveAt(0);
                    foreach (var column in input.ColumnList)
                    {
                        if (BdCol.Contains(column))
                        {
                            listCol.Add("BankDraft." + column);
                        }
                        else
                        {
                            listCol.Add(input.TableName + "." + column);
                        }

                    }
                    Query = "select " + string.Join(", ", listCol) + " from " + input.TableName + " inner join BankDraft on " + input.TableName + ".bankdraftid = BankDraft.id";
                }
                else if (input.TableName == "BankDraft")
                {
                    listColWC.RemoveAt(0);
                    listColWH.RemoveAt(0);
                    listColWCH.RemoveAt(0);
                    foreach (var column in input.ColumnList)
                    {
                        if (BdCol.Contains(column))
                        {
                            listColWC.Add("BankDraft." + column);
                            listColWH.Add("BankDraft." + column);
                            listColWCH.Add("BankDraft." + column);
                        }
                        else
                        {
                            if (column == "Tarikh" || column == "Bandar" || column == "Negeri" || column == "Poskod")
                            {
                                if (column == "Tarikh")
                                {
                                    listColWH.Add("WangHangus.Date as Tarikh");
                                }
                                else if (column == "Bandar")
                                {
                                    listColWH.Add("WangHangus.City as Bandar");
                                }
                                else if (column == "Negeri")
                                {
                                    listColWH.Add("WangHangus.Region as Negeri");
                                }
                                else if (column == "Poskod")
                                {
                                    listColWH.Add("WangHangus.Postcode as Poskod");
                                }


                                listColWC.Add("WangCagaran." + column);
                                listColWCH.Add("WangCagaranHangus." + column);
                            }


                            if (listColNotExistWH.Contains(column))
                            {
                                listColWH.Add("'' as " + column);
                                listColWC.Add("WangCagaran." + column);
                                listColWCH.Add("WangCagaranHangus." + column);
                            }


                            if (listColNotExistWC.Contains(column))
                            {
                                listColWC.Add("'' as " + column);
                                listColWCH.Add("'' as " + column);
                                listColWH.Add("WangHangus." + column);
                            }

                            if (listColAccountingTable.Contains(column))
                            {
                                listColWC.Add("'' as " + column);
                                listColWCH.Add("'' as " + column);
                                listColWH.Add("STUFF((SELECT ', ' + " + column
                                + " FROM AccountingTable"
                                + " WHERE AccountingTable.WangHangusId = WangHangus.Id "
                                + "FOR XML PATH('')), 1, 1, '') " + column);
                            }

                            if (!listColNotExistWH.Contains(column) && !listColNotExistWC.Contains(column) && !listColAccountingTable.Contains(column) && column != "Tarikh" && column != "Bandar" && column != "Negeri" && column != "Poskod")
                            {
                                listColWH.Add("WangHangus." + column);
                                listColWC.Add("WangCagaran." + column);
                                listColWCH.Add("WangCagaranHangus." + column);
                            }

                        }

                    }

                    Query = "select " + string.Join(", ", listColWC) + " from " + input.TableName + " inner join WangCagaran on " + input.TableName + ".Id = WangCagaran.BankDraftId";
                    Query1 = " UNION select " + string.Join(", ", listColWH) + " from " + input.TableName + " inner join WangHangus on " + input.TableName + ".Id = WangHangus.BankDraftId";
                    Query2 = " UNION select " + string.Join(", ", listColWCH) + " from " + input.TableName + " inner join WangCagaranHangus on " + input.TableName + ".Id = WangCagaranHangus.BankDraftId";
                }


                string countSql = $"select COUNT(*) from {input.TableName} ";

                if (!string.IsNullOrEmpty(input.FilterWhere))
                {
                    input.FilterWhere = input.FilterWhere.Substring(0, input.FilterWhere.Length - 3);
                    string vendorNo = input.FilterWhere.Replace("VendorNo", "'VendorNo'");
                    Query = Query + " where " + vendorNo;


                    if (Query1 != "" && Query2 != "")
                    {
                        string region = input.FilterWhere.Replace("Negeri", "Region");
                        Query1 = Query1 + " where " + region;
                        Query2 = Query2 + " where " + vendorNo;
                        Query = Query + Query1 + Query2;
                    }

                    countSql += $"where {input.FilterWhere} ";
                }


                var ds = _logService.GetLogByQuery(Query);

                return c_Serializer.GetJSON(ds, input.ColumnArray);

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost]
        public string GetLogPreview(LogViewModel input)
        {
            try
            {
                if (input.ColumnList == null)
                    return "Column List cannot be empty.";

                if (input.TableName == null || input.TableName == "")
                    return "TableName cannot be empty.";

                string Query = "select TOP(10) " + string.Join(",", input.ColumnList) + " from " + input.TableName;

                //if (!string.IsNullOrEmpty(input.FilterWhere))
                //{
                //    Query = Query + " where " + input.FilterWhere;
                //}

                var ds = _logService.GetLogByQuery(Query);

                return c_Serializer.GetJSON(ds, input.ColumnArray);

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public class c_Serializer
        {
            public static string GetJSON(DataSet ds, List<List<string>> ColumnArray)
            {
                ArrayList root = new ArrayList();
                List<Dictionary<string, object>> table;
                Dictionary<string, object> data;

                foreach (DataTable dt in ds.Tables)
                {
                    table = new List<Dictionary<string, object>>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        data = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            var colString = ColumnArray.Where(x => x.Contains(col.ColumnName)).Select(x => x[1].ToString()).FirstOrDefault();

                            if (dr[col].GetType() == typeof(DBNull))
                                data.Add(colString, "");
                            else
                                data.Add(colString, dr[col]);
                        }
                        table.Add(data);
                    }
                    //root.Add(table);
                    return JsonConvert.SerializeObject(table); //remove this for multi data table
                }
                return JsonConvert.SerializeObject(root);
            }

            public static string DataTableToJSONWithJavaScriptSerializer(DataTable table)
            {
                List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
                Dictionary<string, object> childRow;
                foreach (DataRow row in table.Rows)
                {
                    childRow = new Dictionary<string, object>();
                    foreach (DataColumn col in table.Columns)
                    {
                        childRow.Add(col.ColumnName, row[col]);
                    }
                    parentRow.Add(childRow);
                }
                return JsonConvert.SerializeObject(parentRow);
            }
        }

        public IActionResult Lampiran1PDF(string month = null, string year = null)
        {
           return View();
            //return new ViewAsPdf("Lampiran1PDF")
            //{
            //    PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 },
            //};
            //month = "5";
            //year = "2018";
            //if (month != null && year != null)
            //{ 
            //var model = new StateMonthlyAmountViewModel();

            //    model.SMAs = Db.StateMonthlyAmount
            //               .Join(Db.State,
            //               sm => sm.StateId,
            //               s => s.Id,
            //              (sm, s) => new StateMonthlyProfileViewModel
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


            //    return new ViewAsPdf("Lampiran1PDF", model)
            //    {
            //        PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 },
            //    };
            //}
            //else
            //{
            //    return new ViewAsPdf("Lampiran1PDF")
            //    {
            //        PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 },
            //    };
            //}

        }

        public IActionResult Lampiran1copy()
        {
            return View();
        }

        public IActionResult Lampiran1copy2()
        {
            return View();
        }

        public IActionResult Lampiran2()
        {
            return View();
            //return new ViewAsPdf("Lampiran2")
            //{
            //    PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 },
            //};
        }

        public IActionResult Lampiran2copy()
        {
            return View();
        }

        public IActionResult Lampiran2PDF()
        {
            return new ViewAsPdf("Lampiran2")
            {
                PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 },
                CustomSwitches = "--no-stop-slow-scripts --javascript-delay 5000 "
            };
        }
        public IActionResult Lampiran3()
        {
            return View();
        }

        public IActionResult Lampiran3copy()
        {
            return View();
        }

        public IActionResult Lampiran4()
        {
            return View();
        }

        public IActionResult Lampiran4b()
        {
            return View();
        }

        public IActionResult Lampiran4copy()
        {
            return View();
        }

        public IActionResult Lampiran4copy2()
        {
            return View();
        }


        public IActionResult Lampiran5()
        {
            return View();
        }
        public IActionResult Lampiran5Copy()
        {
            return View();
        }
        public IActionResult Lampiran6()
        {
            return View();
        }

        public IActionResult Lampiran6copy()
        {
            return View();
        }
        public IActionResult Lampiran7()
        {
            return View();
        }

        public IActionResult Lampiran7copy()
        {
            return View();
        }

        public IActionResult Testing()
        {
            return View();
        }

        //[HttpGet]
        //public JsonResult GetReportForState(string month = null, string year = null, string coCode = null)
        //{
        //    var exist = Db.StateMonthlyAmount.Where(x => x.Month == Int32.Parse(month) && x.Year == Int32.Parse(year)).Any();

        //    if (exist == true)
        //    {
        //        var monthData = Db.StateMonthlyAmount.Where(x => x.Month == Int32.Parse(month) && x.Year == Int32.Parse(year) && x.CoCode == coCode).ToList();
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
        //         .Join(Db.BusinessArea,
        //         w => w.wg.BusinessArea,
        //         ba => ba.Name,
        //         (w, ba) => new
        //         {
        //             stateId = ba.StateId,
        //             status = w.bd.Status,
        //             bdAmount = w.bd.BankDraftAmount,
        //             monthIssued = w.wg.PostingDate.Value.Month,
        //             yearIssued = w.wg.PostingDate.Value.Year,
        //             coCode = w.wg.CoCode
        //         })
        //         .Where(x => x.status == "Complete" && x.yearIssued == Int32.Parse(year) && x.monthIssued == Int32.Parse(month) && x.coCode == coCode)
        //         .ToList();

        //    var monthly = new StateMonthlyAmount();
        //    monthly.Amount = 0;
        //    monthly.OutstandingAmount = 0;

        //    var hasStates = bd.Select(x => x.stateId).Distinct().ToList();

        //    foreach (var hasState in hasStates)
        //    {
        //        foreach (var item in bd)
        //        {
        //            if (item.stateId == hasState)
        //            {
        //                monthly.Id = Guid.NewGuid();
        //                monthly.StateId = item.stateId;
        //                monthly.CoCode = item.coCode;
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

        //    var monthStateHasData = Db.StateMonthlyAmount.Where(x => x.Month == Int32.Parse(month) && x.Year == Int32.Parse(year) && x.CoCode == coCode).Select(x => x.StateId).ToList();
        //    var noData = states.Select(x => x.Id).Except(monthStateHasData).ToList();

        //    //Set data to zero for any selected month into StateMonthlyAmount
        //    foreach (var monthZero in noData)
        //    {
        //        monthly.Id = Guid.NewGuid();
        //        monthly.StateId = monthZero;
        //        monthly.CoCode = coCode;
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
        //        .Join(Db.BusinessArea,
        //        r => r.BA,
        //        ba => ba.Name,
        //        (r, ba) => new
        //        {
        //            stateId = ba.StateId,
        //            status = r.Status,
        //            bdAmount = r.BDAmount,
        //            monthIssued = r.RecoveryType == "Full" ? r.PostingDate1.Value.Month : r.PostingDate2.Value.Month,
        //            yearIssued = r.RecoveryType == "Full" ? r.PostingDate1.Value.Year : r.PostingDate2.Value.Year,
        //            coCode = r.CoCode
        //        })
        //        .Where(x => x.status == "Complete" && x.yearIssued == Int32.Parse(year) && x.monthIssued == Int32.Parse(month) && x.coCode == coCode)
        //        .ToList();


        //    foreach (var item in rec)
        //    {
        //        var monthSel = Db.StateMonthlyAmount.Where(x => x.Month == Int32.Parse(month) && x.Year == Int32.Parse(year) && x.StateId == item.stateId && x.CoCode == coCode).FirstOrDefault();
        //        monthSel.BDNoRecovered = monthSel.BDNoRecovered + 1;
        //        monthSel.RecoveryAmount = monthSel.RecoveryAmount + item.bdAmount;
        //        monthSel.BDNoOutstanding = monthSel.BDNoIssued - monthSel.BDNoRecovered;
        //        monthSel.OutstandingAmount = monthSel.Amount - monthSel.RecoveryAmount;

        //        Db.SetModified(monthSel);
        //        Db.SaveChanges();

        //    }

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
        //                  year = sm.Year.ToString(),
        //                  coCode = sm.CoCode
        //              })
        //              .Where(x => x.month == month && x.year == year)
        //              .OrderBy(c => c.state);

        //    return new JsonResult(result.ToList());
        //}

        [HttpGet]
        public JsonResult GetReportForState(string month = null, string year = null, string coCode = null)
        {
            //var result = Db.StateMonthlyAmount
            //           .Join(Db.State,
            //           sm => sm.StateId,
            //           s => s.Id,
            //          (sm, s) => new
            //          {
            //              state = s.Name,
            //              noOfBDIssued = sm.BDNoIssued,
            //              amount = sm.Amount,
            //              //string.Format("{0:N}", sm.Amount),
            //              amountNum = sm.Amount,
            //              noOfRecovery = sm.BDNoRecovered,
            //              amountRev = sm.RecoveryAmount,
            //              //string.Format("{0:N}", sm.RecoveryAmount),
            //              //amountRevNum = sm.RecoveryAmount,
            //              noOfOutstanding = sm.BDNoOutstanding,
            //              outstandingAmount = sm.OutstandingAmount,
            //              //string.Format("{0:N}", sm.OutstandingAmount),
            //              monthName = System.Globalization.CultureInfo.GetCultureInfo("en-US").DateTimeFormat.GetMonthName(sm.Month),
            //              month = sm.Month.ToString(),
            //              year = sm.Year.ToString(),
            //              yearInt = sm.Year,
            //              monthInt = sm.Month,
            //              coCode = sm.CoCode
            //          })
            //          .Where(x => x.month == month && x.year == year && x.coCode == coCode);
            //          //.OrderBy(c => c.state);

            //if (year != "2015")
            //{
               var result1 = Db.StateMonthlyAmount
                     .Join(Db.State,
                     sm => sm.StateId,
                     s => s.Id,
                    (sm, s) => new
                    {
                        state = s.Name,
                        noOfBDIssued = sm.BDNoIssued,
                        amount = sm.Amount,
                        amountNum = sm.Amount,
                        noOfRecovery = sm.BDNoRecovered,
                        amountRev = sm.RecoveryAmount,
                        //amountRevNum = sm.RecoveryAmount,
                        noOfOutstanding = sm.BDNoOutstanding,
                        outstandingAmount = sm.OutstandingAmount,
                        //outstandingAmount = string.Format("{0:N}", sm.OutstandingAmount),
                        // monthName = System.Globalization.CultureInfo.GetCultureInfo("en-US").DateTimeFormat.GetMonthName(sm.Month),
                        month = sm.Month.ToString(),
                        year = sm.Year.ToString(),
                        yearInt = sm.Year,
                        monthInt = sm.Month,
                        coCode = sm.CoCode
                    })
                    .Where(x => x.yearInt < Int32.Parse(year) && x.coCode == coCode)
                    .OrderBy(c => c.state);
            

                var result2 = Db.StateMonthlyAmount
                       .Join(Db.State,
                       sm => sm.StateId,
                       s => s.Id,
                      (sm, s) => new
                      {
                          state = s.Name,
                          noOfBDIssued = sm.BDNoIssued,
                          amount = sm.Amount,
                          amountNum = sm.Amount,
                          noOfRecovery = sm.BDNoRecovered,
                          amountRev = sm.RecoveryAmount,
                          //amountRevNum = sm.RecoveryAmount,
                          noOfOutstanding = sm.BDNoOutstanding,
                          outstandingAmount = sm.OutstandingAmount,
                          //string.Format("{0:N}", sm.OutstandingAmount),
                          //monthName = System.Globalization.CultureInfo.GetCultureInfo("en-US").DateTimeFormat.GetMonthName(sm.Month),
                          month = sm.Month.ToString(),
                          year = sm.Year.ToString(),
                          yearInt = sm.Year,
                          monthInt = sm.Month,
                          coCode = sm.CoCode
                      })
                      .Where(x => x.monthInt <= Int32.Parse(month) && x.year == year && x.coCode == coCode)
                      .OrderBy(c => c.state);

               var resultb = result1.Concat(result2).Select(s => new
               {
                   state = s.state,
                   noOfBDIssued = s.noOfBDIssued,
                   amount = s.amount,
                   amountNum = s.amountNum,
                   noOfRecovery = s.noOfRecovery,
                   amountRev = s.amountRev,
                   //amountRevNum = sm.RecoveryAmount,
                   noOfOutstanding = s.noOfOutstanding,
                   outstandingAmount = s.outstandingAmount,
                   //string.Format("{0:N}", sm.OutstandingAmount),
                   monthName = System.Globalization.CultureInfo.GetCultureInfo("en-US").DateTimeFormat.GetMonthName(s.monthInt),
                   month = s.month.ToString(),
                   year = s.year.ToString(),
                   yearInt = s.yearInt,
                   monthInt = s.monthInt,
                   coCode = s.coCode
               }).ToList();
               
                var result5 = resultb
                    .GroupBy(r => new { r.state })
                    .Select(group => new
                    {
                        fee = group.Key,
                        state = group.Select(r => r.state).FirstOrDefault(),
                        year = group.Select(r => r.year).FirstOrDefault(),
                        noOfBDIssued = group.Sum(r => r.noOfBDIssued),
                        amount = string.Format("{0:N}", group.Sum(r => r.amount)),
                        amountNum = group.Sum(r => r.amount),
                        noOfRecovery = group.Sum(r => r.noOfRecovery),
                        amountRev = string.Format("{0:N}", group.Sum(r => r.amountRev)),
                        amountRevNum = group.Sum(r => r.amountRev),
                        noOfOutstanding = group.Sum(r => r.noOfOutstanding),
                        outstandingAmount = string.Format("{0:N}", group.Sum(r => r.outstandingAmount)),
                    })
                    .OrderBy(c => c.state);

                return new JsonResult(result5.ToList());
            //}
            //else
            //{
            //    var result1 = Db.StateMonthlyAmount
            //        .Join(Db.State,
            //        sm => sm.StateId,
            //        s => s.Id,
            //       (sm, s) => new
            //       {
            //           state = s.Name,
            //           noOfBDIssued = sm.BDNoIssued,
            //           amount = sm.Amount,
            //           amountNum = sm.Amount,
            //           noOfRecovery = sm.BDNoRecovered,
            //           amountRev = sm.RecoveryAmount,
            //            //amountRevNum = sm.RecoveryAmount,
            //            noOfOutstanding = sm.BDNoOutstanding,
            //           outstandingAmount = sm.OutstandingAmount,
            //            //outstandingAmount = string.Format("{0:N}", sm.OutstandingAmount),
            //            monthName = System.Globalization.CultureInfo.GetCultureInfo("en-US").DateTimeFormat.GetMonthName(sm.Month),
            //           month = sm.Month.ToString(),
            //           year = sm.Year.ToString(),
            //           yearInt = sm.Year,
            //           monthInt = sm.Month,
            //           coCode = sm.CoCode
            //       })
            //       .Where(x => x.yearInt < Int32.Parse(year) && x.coCode == coCode)
            //       .OrderBy(c => c.state);


            //    var result2 = Db.StateMonthlyAmount
            //           .Join(Db.State,
            //           sm => sm.StateId,
            //           s => s.Id,
            //          (sm, s) => new
            //          {
            //              state = s.Name,
            //              noOfBDIssued = sm.BDNoIssued,
            //              amount = sm.Amount,
            //              amountNum = sm.Amount,
            //              noOfRecovery = sm.BDNoRecovered,
            //              amountRev = sm.RecoveryAmount,
            //              //amountRevNum = sm.RecoveryAmount,
            //              noOfOutstanding = sm.BDNoOutstanding,
            //              outstandingAmount = sm.OutstandingAmount,
            //              //string.Format("{0:N}", sm.OutstandingAmount),
            //              monthName = System.Globalization.CultureInfo.GetCultureInfo("en-US").DateTimeFormat.GetMonthName(sm.Month),
            //              month = sm.Month.ToString(),
            //              year = sm.Year.ToString(),
            //              yearInt = sm.Year,
            //              monthInt = sm.Month,
            //              coCode = sm.CoCode
            //          })
            //          .Where(x => x.monthInt <= Int32.Parse(month) && x.year == year && x.coCode == coCode)
            //          .OrderBy(c => c.state);

            //    var resultb = result1.Concat(result2).ToList();

            //    var result5 = resultb
            //        .GroupBy(r => new { r.state })
            //        .Select(group => new
            //        {
            //            fee = group.Key,
            //            state = group.Select(r => r.state).FirstOrDefault(),
            //            year = group.Select(r => r.year).FirstOrDefault(),
            //            noOfBDIssued = group.Sum(r => r.noOfBDIssued),
            //            amount = string.Format("{0:N}", group.Sum(r => r.amount)),
            //            amountNum = group.Sum(r => r.amount),
            //            noOfRecovery = group.Sum(r => r.noOfRecovery),
            //            amountRev = string.Format("{0:N}", group.Sum(r => r.amountRev)),
            //            amountRevNum = group.Sum(r => r.amountRev),
            //            noOfOutstanding = group.Sum(r => r.noOfOutstanding),
            //            outstandingAmount = string.Format("{0:N}", group.Sum(r => r.outstandingAmount)),
            //        })
            //        .OrderBy(c => c.state);

            //    return new JsonResult(result5.ToList());
            //}

            //var result3 = result
            // .GroupBy(r => new { r.state})
            // .Select(group => new
            // {
            //     fee = group.Key,
            //     state = group.Select(r => r.state).FirstOrDefault(),
            //     year = group.Select(r => r.year).FirstOrDefault(),
            //     noOfBDIssued = group.Sum(r => r.noOfBDIssued),
            //     amount = string.Format("{0:N}", group.Sum(r => r.amount)),
            //     amountNum = group.Sum(r => r.amount),
            //     noOfRecovery = group.Sum(r => r.noOfRecovery),
            //     amountRev = string.Format("{0:N}", group.Sum(r => r.amountRev)),
            //     amountRevNum = group.Sum(r => r.amountRev),
            //     noOfOutstanding = group.Sum(r => r.noOfOutstanding),
            //     outstandingAmount = string.Format("{0:N}", group.Sum(r => r.outstandingAmount)),
            // })
            // .OrderBy(c => c.state);

            //return new JsonResult(result3.ToList());
        }


        [HttpGet]
        public JsonResult GetReportForState22(string month = null, string year = null, string coCode = null)
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

            var monthly = new StateMonthlyAmount();
            monthly.Amount = 0;
            monthly.OutstandingAmount = 0;

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


        [HttpGet]
        public JsonResult GetReportForState3(string month = null, string year = null)
        {
            var monthData = Db.StateMonthlyAmount.Where(x => x.Month == Int32.Parse(month) && x.Year == Int32.Parse(year)).ToList();

            if (monthData.Count > 0)
            {
                //Delete exisitng data 
                foreach (var item in monthData)
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
                     yearIssued = w.wg.PostingDate.Value.Year
                  
                 })
                 .Where(x => x.status == "Complete" && x.yearIssued == Int32.Parse(year) && x.monthIssued == Int32.Parse(month))
                 .ToList();

            var monthly = new StateMonthlyAmount();
            monthly.Amount = 0;
            monthly.OutstandingAmount = 0;

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
                        monthly.BDNoOutstanding = monthly.BDNoOutstanding + 1;
                        monthly.OutstandingAmount = monthly.OutstandingAmount + item.bdAmount;
                    }
                }
                Db.StateMonthlyAmount.Add(monthly);
                Db.SaveChanges();
            }

            var monthStateHasData = Db.StateMonthlyAmount.Where(x => x.Month == Int32.Parse(month) && x.Year == Int32.Parse(year)).Select(x => x.StateId).ToList();
            var noData = states.Select(x => x.Id).Except(monthStateHasData).ToList();

            //Set data to zero for any selected month into StateMonthlyAmount
            foreach (var monthZero in noData)
            {
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
                //.Join(Db.BusinessArea,
                //r => r.BA,
                //ba => ba.Name,
                //(r, ba) => new
                {
                    stateId = s.Id,
                    status = r3.r2.rec.Status,
                    bdAmount = r3.r2.rec.BDAmount,
                    monthIssued = r3.r2.rec.RecoveryType == "Full" ? r3.r2.rec.PostingDate1.Value.Month : r3.r2.rec.PostingDate2.Value.Month,
                    yearIssued = r3.r2.rec.RecoveryType == "Full" ? r3.r2.rec.PostingDate1.Value.Year : r3.r2.rec.PostingDate2.Value.Year
                })
                .Where(x => x.status == "Complete" && x.yearIssued == Int32.Parse(year) && x.monthIssued == Int32.Parse(month))
                .ToList();


            foreach (var item in rec)
            {
                var monthSel = Db.StateMonthlyAmount.Where(x => x.Month == Int32.Parse(month) && x.Year == Int32.Parse(year) && x.StateId == item.stateId).FirstOrDefault();
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
                          year = sm.Year.ToString()
                      })
                      .Where(x => x.month == month && x.year == year)
                      .OrderBy(c => c.state);

            return new JsonResult(result.ToList());
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

                //test

                //var noStates = states.Select(x => x.Id).Except(hasStates).ToList();


                //foreach (var noState in noStates)
                //{
                //    monthly.Id = Guid.NewGuid();
                //    monthly.StateId = noState;
                //    monthly.BDNoIssued = 0;
                //    monthly.Amount = 0;
                //    monthly.Month = Int32.Parse(month);
                //    monthly.Year = Int32.Parse(year);
                //    monthly.BDNoRecovered = 0;
                //    monthly.RecoveryAmount = 0;
                //    monthly.BDNoOutstanding = 0;
                //    monthly.OutstandingAmount = 0;
                //    Db.StateMonthlyAmount.Add(monthly);
                //    Db.SaveChanges();
                //}

                //var rec = Db.Recovery
                //    .Join(Db.BusinessArea,
                //    r => r.BA,
                //    ba => ba.Name,
                //    (r, ba) => new
                //    {
                //        stateId = ba.StateId,
                //        status = r.Status,
                //        bdAmount = r.BDAmount,
                //        monthIssued = r.CompletedOn.Value.Month,
                //        yearIssued = r.CompletedOn.Value.Year
                //    })
                //    .Where(x => x.status == "Complete" && x.yearIssued == Int32.Parse(year) && x.monthIssued == Int32.Parse(month))
                //    .ToList();

                //var monthEdit = Db.StateMonthlyAmount.Where(x => x.Month == Int32.Parse(month) && x.Year == Int32.Parse(year)).FirstOrDefault();
                //monthEdit.RecoveryAmount = 0;

                //var hasStates1 = bd.Select(x => x.stateId).Distinct().ToList();

                //foreach (var hasState1 in hasStates1)
                //{
                //    foreach (var item in bd)
                //    {
                //        if (item.stateId == hasState1)
                //        {
                //            monthEdit = Db.StateMonthlyAmount.Where(x => x.Month == Int32.Parse(month) && x.Year == Int32.Parse(year) && x.StateId == item.stateId).FirstOrDefault();

                //            monthEdit.BDNoIssued = 0;
                //            monthEdit.Amount = 0;
                //            monthEdit.BDNoRecovered = monthEdit.BDNoRecovered + 1;
                //            monthEdit.RecoveryAmount = monthEdit.RecoveryAmount + 1;
                //            monthEdit.BDNoOutstanding = monthEdit.BDNoIssued - monthEdit.BDNoRecovered;
                //            monthEdit.OutstandingAmount = monthEdit.Amount - monthEdit.RecoveryAmount;
                //        }

                //    }

                //    Db.SetModified(monthEdit);
                //    Db.SaveChanges();
                //}

                //foreach (var hasState1 in hasStates1)
                //{
                //    foreach (var item in bd)
                //    {
                //        if (item.stateId == hasState1)
                //        {
                //            monthEdit = Db.StateMonthlyAmount.Where(x => x.Month == Int32.Parse(month) && x.Year == Int32.Parse(year) && x.StateId == item.stateId).FirstOrDefault();

                //            monthEdit.BDNoIssued = monthEdit.BDNoIssued + 1;
                //            monthEdit.Amount = monthEdit.Amount + item.bdAmount;
                //            monthEdit.BDNoRecovered = 0;
                //            monthEdit.RecoveryAmount = 0;
                //            monthEdit.BDNoOutstanding = 0;
                //            monthEdit.OutstandingAmount = 0;
                //        }

                //    }

                //    Db.SetModified(monthEdit);
                //    Db.SaveChanges();
                //}
                //}
                //else
                //{
                //    var states = Db.State.ToList();

                //    var bd = Db.BankDraft
                //         .Join(Db.WangCagaran,
                //          b => b.Id,
                //          w => w.BankDraftId,
                //          (b, w) => new { bd = b, wg = w })
                //         .Join(Db.BusinessArea,
                //         w => w.wg.BusinessArea,
                //         ba => ba.Name,
                //         (w, ba) => new
                //         {
                //             stateId = ba.StateId,
                //             status = w.bd.Status,
                //             bdAmount = w.bd.BankDraftAmount,
                //             monthIssued = w.bd.TGBSAcceptedOn.Value.Month,
                //             yearIssued = w.bd.TGBSAcceptedOn.Value.Year
                //         })
                //         .Where(x => x.status == "Complete" && x.yearIssued == Int32.Parse(year) && x.monthIssued == Int32.Parse(month))
                //         .ToList();

                //    var monthEdit = Db.StateMonthlyAmount.Where(x => x.Month == Int32.Parse(month) && x.Year == Int32.Parse(year)).FirstOrDefault();
                //    //monthly.Amount = 0;

                //    var hasStates = bd.Select(x => x.stateId).Distinct().ToList();

                //    foreach (var hasState in hasStates)
                //    {
                //        foreach (var item in bd)
                //        {
                //            if (item.stateId == hasState)
                //            {
                //                monthEdit = Db.StateMonthlyAmount.Where(x => x.Month == Int32.Parse(month) && x.Year == Int32.Parse(year) && x.StateId == item.stateId).FirstOrDefault();

                //                monthEdit.BDNoIssued = 0;
                //                monthEdit.Amount = 0;
                //                monthEdit.BDNoRecovered = 0;
                //                monthEdit.RecoveryAmount = 0;
                //                monthEdit.BDNoOutstanding = 0;
                //                monthEdit.OutstandingAmount = 0;
                //            }

                //        }

                //        Db.SetModified(monthEdit);
                //        Db.SaveChanges();
                //    }

                //    foreach (var hasState in hasStates)
                //    {
                //        foreach (var item in bd)
                //        {
                //            if (item.stateId == hasState)
                //            {
                //                monthEdit = Db.StateMonthlyAmount.Where(x => x.Month == Int32.Parse(month) && x.Year == Int32.Parse(year) && x.StateId == item.stateId).FirstOrDefault();

                //                monthEdit.BDNoIssued = monthEdit.BDNoIssued + 1;
                //                monthEdit.Amount = monthEdit.Amount + item.bdAmount;
                //                monthEdit.BDNoRecovered = 0;
                //                monthEdit.RecoveryAmount = 0;
                //                monthEdit.BDNoOutstanding = 0;
                //                monthEdit.OutstandingAmount = 0;
                //            }

                //        }

                //        Db.SetModified(monthEdit);
                //        Db.SaveChanges();
                //    }


                //var noStates = states.Select(x => x.Id).Except(hasStates).ToList();


                //foreach (var noState in noStates)
                //{
                //    monthly.Id = Guid.NewGuid();
                //    monthly.StateId = noState;
                //    monthly.BDNoIssued = 0;
                //    monthly.Amount = 0;
                //    monthly.Month = Int32.Parse(month);
                //    monthly.Year = Int32.Parse(year);
                //    monthly.BDNoRecovered = 0;
                //    monthly.RecoveryAmount = 0;
                //    monthly.BDNoOutstanding = 0;
                //    monthly.OutstandingAmount = 0;
                //    Db.StateMonthlyAmount.Add(monthly);
                //    Db.SaveChanges();
                //}
                //}

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
        public JsonResult GetReportForLampiran2(string monthFrom = null, string yearFrom = null, string monthTo = null, string yearTo = null, string coCode = null)
        {

            var monthData = Db.MonthlyAmount.Where(x => x.Month == DateTime.Now.Month && x.Year == DateTime.Now.Year && x.CoCode == coCode).ToList();

            if (monthData.Count > 0)
            {
                //Delete exisitng data 
                foreach (var item in monthData)
                {
                    Db.MonthlyAmount.Remove(item);
                }
                Db.SaveChanges();
            }

            //generate sum for bd amount & recovery amount by month & year (currentYear)
            var sm1 = Db.StateMonthlyAmount
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
                    coCode = coCode
                })
                .Where(x => x.month == DateTime.Now.Month && x.year == DateTime.Now.Year && x.coCode == coCode)
                .ToList();

            decimal? sumAmount = 0;
            decimal? sumRecovery = 0;

            foreach (var item in sm1)
            {

                sumAmount += item.amount;
                sumRecovery += item.amountRev;
            }

            var monthly = new MonthlyAmount();
            monthly.Month = DateTime.Now.Month;
            monthly.Year = DateTime.Now.Year;
            monthly.CoCode = coCode;
            monthly.sumAmount = sumAmount;
            monthly.sumRecovery = sumRecovery;

            Db.MonthlyAmount.Add(monthly);
            Db.SaveChanges();


            var m = Db.MonthlyAmount
                   .Select(s => new
                   {
                       month = s.Month,
                       year = s.Year,
                       sumAmount = s.sumAmount,
                       sumRecovery = s.sumRecovery,
                       coCode = coCode
                   })
                   .Where(x => x.month == DateTime.Now.Month && x.year == DateTime.Now.Year && x.coCode == coCode)
            .ToList();


            foreach (var item in m)
            {
                var currMonth = Db.MonthlyAmount.Where(x => x.Month == item.month && x.Year == item.year && x.CoCode == coCode).FirstOrDefault();
                var prevMonth = Db.MonthlyAmount.Where(x => x.Month == 1 && x.Year == 2015 && x.CoCode == coCode).FirstOrDefault();
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
                      diffAmount1 = s.diffAmount,
                      diffAmount = string.Format("{0:N}", s.diffAmount),
                      diffRec1 = s.diffAmountRecovery,
                      diffRec = string.Format("{0:N}", s.diffAmountRecovery),
                      percentage = s.percentageAmount,
                      percentageRec = s.percentageAmountRecovery,
                      coCode = s.CoCode
                  })
                  .Where(s => s.month >= Int32.Parse(monthFrom) && s.month <= Int32.Parse(monthTo) && s.year == Int32.Parse(yearFrom) && s.coCode == coCode)
           .ToList();

                return new JsonResult(result.ToList());

            }
            else if (Int32.Parse(yearFrom) < Int32.Parse(yearTo) && yearDiff > 1)
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
                       diffAmount1 = s.diffAmount,
                       diffAmount = string.Format("{0:N}", s.diffAmount),
                       diffRec1 = s.diffAmountRecovery,
                       diffRec = string.Format("{0:N}", s.diffAmountRecovery),
                       percentage = s.percentageAmount,
                       percentageRec = s.percentageAmountRecovery,
                       coCode = s.CoCode
                   })
                   .Where(s => s.month >= Int32.Parse(monthFrom) && s.year == Int32.Parse(yearFrom) && s.coCode == coCode)
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
                      diffAmount1 = s.diffAmount,
                      diffAmount = string.Format("{0:N}", s.diffAmount),
                      diffRec1 = s.diffAmountRecovery,
                      diffRec = string.Format("{0:N}", s.diffAmountRecovery),
                      percentage = s.percentageAmount,
                      percentageRec = s.percentageAmountRecovery,
                      coCode = s.CoCode
                  })
                  .Where(s => s.year >= (Int32.Parse(yearFrom) + 1) && s.year < Int32.Parse(yearTo) && s.coCode == coCode)
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
                       diffAmount1 = s.diffAmount,
                       diffAmount = string.Format("{0:N}", s.diffAmount),
                       diffRec1 = s.diffAmountRecovery,
                       diffRec = string.Format("{0:N}", s.diffAmountRecovery),
                       percentage = s.percentageAmount,
                       percentageRec = s.percentageAmountRecovery,
                       coCode = s.CoCode
                   })
                   .Where(s => s.month <= Int32.Parse(monthTo) && s.year == Int32.Parse(yearTo) && s.coCode == coCode)
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
                      diffAmount1 = s.diffAmount,
                      diffAmount = string.Format("{0:N}", s.diffAmount),
                      diffRec = string.Format("{0:N}", s.diffAmountRecovery),
                      percentage = s.percentageAmount,
                      percentageRec = s.percentageAmountRecovery,
                      coCode = s.CoCode
                  })
                  .Where(s => s.month >= Int32.Parse(monthFrom) && s.year == Int32.Parse(yearFrom) && s.coCode == coCode)
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
                       diffAmount1 = s.diffAmount,
                       diffAmount = string.Format("{0:N}", s.diffAmount),
                       diffRec = string.Format("{0:N}", s.diffAmountRecovery),
                       percentage = s.percentageAmount,
                       percentageRec = s.percentageAmountRecovery,
                       coCode = s.CoCode
                   })
                   .Where(s => s.month <= Int32.Parse(monthTo) && s.year == Int32.Parse(yearTo) && s.coCode == coCode)
            .ToList();

                var result = result1.Concat(result2);
                return new JsonResult(result.ToList());
            }

        }

        [HttpGet]
        public JsonResult GetReportForLampiran2Olld(string monthFrom = null, string yearFrom = null, string monthTo = null, string yearTo = null, string coCode = null)
        {

            var monthData = Db.MonthlyAmount.Where(x => x.Month == DateTime.Now.Month && x.Year == DateTime.Now.Year).ToList();

            if (monthData.Count > 0)
            {
                //Delete exisitng data 
                foreach (var item in monthData)
                {
                    Db.MonthlyAmount.Remove(item);
                }
                Db.SaveChanges();
            }

            //generate sum for bd amount & recovery amount by month & year (currentYear)
            var sm1 = Db.StateMonthlyAmount
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
                    year = s.Year
                })
                .Where(x => x.month == DateTime.Now.Month && x.year == DateTime.Now.Year)
                .ToList();

            decimal? sumAmount = 0;
            decimal? sumRecovery = 0;

            foreach (var item in sm1)
            {

                sumAmount += item.amount;
                sumRecovery += item.amountRev;
            }

            var monthly = new MonthlyAmount();
            monthly.Month = DateTime.Now.Month;
            monthly.Year = DateTime.Now.Year;
            monthly.sumAmount = sumAmount;
            monthly.sumRecovery = sumRecovery;

            Db.MonthlyAmount.Add(monthly);
            Db.SaveChanges();


            var m = Db.MonthlyAmount
                   .Select(s => new
                   {
                       month = s.Month,
                       year = s.Year,
                       sumAmount = s.sumAmount,
                       sumRecovery = s.sumRecovery
                   })
                   .Where(x => x.month == DateTime.Now.Month && x.year == DateTime.Now.Year)
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
                      diffAmount1 = s.diffAmount,
                      diffAmount = string.Format("{0:N}", s.diffAmount),
                      diffRec1 = s.diffAmountRecovery,
                      diffRec = string.Format("{0:N}", s.diffAmountRecovery),
                      percentage = s.percentageAmount,
                      percentageRec = s.percentageAmountRecovery
                  })
                  .Where(s => s.month >= Int32.Parse(monthFrom) && s.month <= Int32.Parse(monthTo) && s.year == Int32.Parse(yearFrom))
           .ToList();

                return new JsonResult(result.ToList());

            }
            else if (Int32.Parse(yearFrom) < Int32.Parse(yearTo) && yearDiff > 1)
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
                       diffAmount1 = s.diffAmount,
                       diffAmount = string.Format("{0:N}", s.diffAmount),
                       diffRec1 = s.diffAmountRecovery,
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
                      diffAmount1 = s.diffAmount,
                      diffAmount = string.Format("{0:N}", s.diffAmount),
                      diffRec1 = s.diffAmountRecovery,
                      diffRec = string.Format("{0:N}", s.diffAmountRecovery),
                      percentage = s.percentageAmount,
                      percentageRec = s.percentageAmountRecovery
                  })
                  .Where(s => s.year >= (Int32.Parse(yearFrom) + 1) && s.year < Int32.Parse(yearTo))
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
                       diffAmount1 = s.diffAmount,
                       diffAmount = string.Format("{0:N}", s.diffAmount),
                       diffRec1 = s.diffAmountRecovery,
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
                      diffAmount1 = s.diffAmount,
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
                       diffAmount1 = s.diffAmount,
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

        //[HttpGet]
        //public JsonResult GetReportForLampiran2Old(string monthFrom = null, string yearFrom = null, string monthTo = null, string yearTo = null)
        //{
        //    int yearDiff = 0;
        //    yearDiff = Int32.Parse(yearTo) - Int32.Parse(yearFrom);

        //    if (yearFrom == yearTo)
        //    {
        //        var result = Db.MonthlyAmount
        //          .Select(s => new
        //          {
        //              month = s.Month,
        //              year = s.Year,
        //              sumAmount1 = s.sumAmount,
        //              sumAmount = string.Format("{0:N}", s.sumAmount),
        //              sumRecovery1 = s.sumRecovery,
        //              sumRecovery = string.Format("{0:N}", s.sumRecovery),
        //              diffAmount = string.Format("{0:N}", s.diffAmount),
        //              diffRec = string.Format("{0:N}", s.diffAmountRecovery),
        //              percentage = s.percentageAmount,
        //              percentageRec = s.percentageAmountRecovery
        //          })
        //          .Where(s => s.month >= Int32.Parse(monthFrom) && s.month <= Int32.Parse(monthTo) && s.year == Int32.Parse(yearFrom))
        //   .ToList();

        //        return new JsonResult(result.ToList());

        //    }
        //    else if(Int32.Parse(yearFrom) < Int32.Parse(yearTo) && yearDiff > 1)
        //    {

        //        var result1 = Db.MonthlyAmount
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
        //           .Where(s => s.month >= Int32.Parse(monthFrom) && s.year == Int32.Parse(yearFrom))
        //           .ToList();

        //        var result2 = Db.MonthlyAmount
        //          .Select(s => new
        //          {
        //              month = s.Month,
        //              year = s.Year,
        //              sumAmount1 = s.sumAmount,
        //              sumAmount = string.Format("{0:N}", s.sumAmount),
        //              sumRecovery1 = s.sumRecovery,
        //              sumRecovery = string.Format("{0:N}", s.sumRecovery),
        //              diffAmount = string.Format("{0:N}", s.diffAmount),
        //              diffRec = string.Format("{0:N}", s.diffAmountRecovery),
        //              percentage = s.percentageAmount,
        //              percentageRec = s.percentageAmountRecovery
        //          })
        //          .Where(s => s.year >= (Int32.Parse(yearFrom) +1) && s.year < Int32.Parse(yearTo))
        //          .ToList();

        //        var result3 = Db.MonthlyAmount
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
        //           .Where(s => s.month <= Int32.Parse(monthTo) && s.year == Int32.Parse(yearTo))
        //           .ToList();

        //        var result = result1.Concat(result2).Concat(result3);
        //        return new JsonResult(result.ToList());
        //    }
        //    else
        //    {
        //        var result1 = Db.MonthlyAmount
        //          .Select(s => new
        //          {
        //              month = s.Month,
        //              year = s.Year,
        //              sumAmount1 = s.sumAmount,
        //              sumAmount = string.Format("{0:N}", s.sumAmount),
        //              sumRecovery1 = s.sumRecovery,
        //              sumRecovery = string.Format("{0:N}", s.sumRecovery),
        //              diffAmount = string.Format("{0:N}", s.diffAmount),
        //              diffRec = string.Format("{0:N}", s.diffAmountRecovery),
        //              percentage = s.percentageAmount,
        //              percentageRec = s.percentageAmountRecovery
        //          })
        //          .Where(s => s.month >= Int32.Parse(monthFrom) && s.year == Int32.Parse(yearFrom))
        //   .ToList();

        //        var result2 = Db.MonthlyAmount
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
        //           .Where(s => s.month <= Int32.Parse(monthTo) && s.year == Int32.Parse(yearTo))
        //    .ToList();

        //        var result = result1.Concat(result2);
        //        return new JsonResult(result.ToList());
        //    }

        //}

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
        public JsonResult GetReportForLampiran4(string month = null, string year = null, string coCode = null)
        {
            var monthData = Db.MonthlyAmount.Where(x => x.Month == DateTime.Now.Month && x.Year == DateTime.Now.Year && x.CoCode == coCode).ToList();

            if (monthData.Count > 0)
            {
                //Delete exisitng data 
                foreach (var item in monthData)
                {
                    Db.MonthlyAmount.Remove(item);
                }
                Db.SaveChanges();
            }

            //generate sum for bd amount & recovery amount by month & year (currentYear)
                var sm1 = Db.StateMonthlyAmount
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
                    .Where(x => x.month == DateTime.Now.Month && x.year == DateTime.Now.Year && x.coCode == coCode)
                    .ToList();

                decimal? sumAmount = 0;
                decimal? sumRecovery = 0;

                foreach (var item in sm1)
                {

                    sumAmount += item.amount;
                    sumRecovery += item.amountRev;
                }

                var monthly = new MonthlyAmount();
                monthly.Month = DateTime.Now.Month;
                monthly.Year = DateTime.Now.Year;
                monthly.CoCode = coCode;
                monthly.sumAmount = sumAmount;
                monthly.sumRecovery = sumRecovery;

                Db.MonthlyAmount.Add(monthly);
                Db.SaveChanges();
            

            var m = Db.MonthlyAmount
                   .Select(s => new
                   {
                       month = s.Month,
                       year = s.Year,
                       sumAmount = s.sumAmount,
                       sumRecovery = s.sumRecovery,
                       coCode = s.CoCode
                   })
                   .Where(x => x.month == DateTime.Now.Month && x.year == DateTime.Now.Year && x.coCode == coCode)
            .ToList();


            foreach (var item in m)
            {
                var currMonth = Db.MonthlyAmount.Where(x => x.Month == item.month && x.Year == item.year && x.CoCode == coCode).FirstOrDefault();
                var prevMonth = Db.MonthlyAmount.Where(x => x.Month == 1 && x.Year == 2015 && x.CoCode == coCode).FirstOrDefault();
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
                           year = sm.Year,
                           coCode = sm.CoCode
                       })
                       .Where(x => x.year < Int32.Parse(year) && x.coCode == coCode);

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
                              year = sm.Year,
                              coCode = sm.CoCode
                          })
                          .Where(x => x.year == Int32.Parse(year) && x.month <= Int32.Parse(month) && x.coCode == coCode);

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
                            year = sm.Year,
                            coCode = sm.CoCode
                        })
                        .Where(x => x.year == Int32.Parse(year) && x.month <= Int32.Parse(month) && x.coCode == coCode);

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
        public JsonResult GetReportForLampiran4ByState(string month = null, string year = null, string coCode = null)
        {
            var monthData = Db.StateMonthlyAmount.Where(x => x.Month == Int32.Parse(month) && x.Year == Int32.Parse(year) && x.CoCode == coCode).ToList();

            if (monthData.Count > 0)
            {
                //Delete exisitng data 
                foreach (var item in monthData)
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
                monthly.Amount = 0;
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
                monthly.CoCode = coCode;
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
                        year = sm.Year,
                        coCode = sm.CoCode
                    })
                    .Where(x => x.year == Int32.Parse(year) && x.month == Int32.Parse(month) && x.coCode == coCode);

            return new JsonResult(result.ToList());

        }

        [HttpGet]
        public JsonResult GetTotalForLampiran4ByStatePreviousMonth(string month = null, string year = null, string type = null, string coCode = null)
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
                              year = s.Year,
                              coCode = s.CoCode
                          })
                         .Where(x => x.year == Int32.Parse(prevYear) && x.month == Int32.Parse(prevMonth) && x.coCode == coCode);

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
                             year = s.Year,
                             coCode = s.CoCode
                         })
                        .Where(x => x.year == Int32.Parse(year) && x.month == Int32.Parse(month) && x.coCode == coCode);

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

        public JsonResult GetReportForLampiran6(string refNo = null, string assignment = null, string ermsDocNo = null, string busA = null, string state = null, string pkType = null, string docDate = null, string ermsPostingDate = null, string bdAmount = null, string keteranganKerja = null, string coCode = null)
        {
            var result = Db.BankDraft
                .Join(Db.WangCagaran,
                    b => b.Id,
                    w => w.BankDraftId,
                    (b, w) => new { bd = b, wg = w })
                .Join(Db.BusinessArea,
                    w => w.wg.BusinessArea,
                    ba => ba.Name,
                    (w, ba) => new
                    {
                        bdNo = w.bd.BankDrafNoIssued,
                        requester = w.bd.Requester.FullName,
                        ermsDocNo = w.wg.ErmsDocNo,
                        coCode = w.wg.CoCode,
                        busA = w.wg.BusinessArea,
                        nameOnBD = w.wg.NamaPemegangCagaran,
                        bdAmount = w.bd.BankDraftAmount, //string.Format("{0:N}", w.bd.BankDraftAmount),
                        ermsPostingDate = w.wg.PostingDate, //w.wg.PostingDate.Value.ToShortDateString(),
                        //w.bd.TGBSAcceptedOn.Value.ToShortDateString(),
                        docDate = w.bd.SubmittedOn, //w.bd.SubmittedOn.Value.ToShortDateString(),
                        projNo = w.bd.ProjectNo,
                        refNo = w.bd.RefNo,
                        bankDraftId = w.bd.Id,
                        status = w.bd.Status,
                        assignment = w.wg.Assignment,
                        keteranganKerja = w.wg.KeteranganKerja,
                        finalApp = w.bd.FinalApplication,
                        pkType = w.wg.Type,
                        state = w.wg.Negeri
                    }).AsEnumerable().Select(w => new
                {
                    w.bdNo,
                    w.requester,
                    w.ermsDocNo,
                    w.coCode,
                    w.busA,
                    w.nameOnBD,
                    bdAmount = string.Format("{0:N}", w.bdAmount),
                    ermsPostingDate = w.ermsPostingDate?.ToShortDateString(),
                    //w.bd.TGBSAcceptedOn.Value.ToShortDateString(),
                    docDate = w.docDate?.ToShortDateString(),
                    w.projNo,
                    w.refNo,
                    w.bankDraftId,
                    w.status,
                    w.assignment,
                    w.keteranganKerja,
                    w.finalApp,
                    w.pkType,
                    w.state,
                });
                // .Where(x => (x.assignment.Contains(assignment) || assignment == null)
                //             && (x.refNo.Contains(refNo) || refNo == null)
                //             && (x.ermsDocNo.Contains(ermsDocNo) || ermsDocNo == null)
                //             && (x.busA.Contains(busA) || busA == null)
                //             && (x.state.Contains(state) || state == null)
                //             && (x.pkType.Contains(pkType) || pkType == null)
                //             && (x.docDate.Contains(docDate) || docDate == null)
                //             && (x.ermsPostingDate.Contains(ermsPostingDate) || ermsPostingDate == null)
                //             && (x.bdAmount.Contains(bdAmount) || bdAmount == null)
                //             && (x.keteranganKerja.Contains(keteranganKerja) || keteranganKerja == null)
                //             && (x.coCode.Contains(coCode) || coCode == null)
                //             && (x.status.Equals("Complete")));


                var result2 = Db.BankDraft
                    .Join(Db.WangHangus,
                        b => b.Id,
                        w => w.BankDraftId,
                        (b, w) => new
                        {
                            //      bd = b, wh = w })
                            // .Join(Db.BusinessArea,
                            // w => w.wh.BusinessArea,
                            // ba => ba.Name,
                            //(w, ba) => new { wH = w, ba = ba })
                            //  .Join(Db.State,
                            // ba => ba.ba.StateId,
                            // s => s.Id,
                            // (ba, s) => new
                            // {
                            bdNo = b.BankDrafNoIssued,
                            requester = b.Requester.FullName,
                            ermsDocNo = "",
                            coCode = w.CoCode,
                            busA = w.BusinessArea,
                            nameOnBD = "",
                            bdAmount = b.BankDraftAmount, //string.Format("{0:N}", b.BankDraftAmount),
                            ermsPostingDate = w.PostingDate, //w.PostingDate.Value.ToShortDateString(),
                            //b.TGBSAcceptedOn.Value.ToShortDateString(),
                            docDate = b.SubmittedOn, //b.SubmittedOn.Value.ToShortDateString(),
                            projNo = b.ProjectNo,
                            refNo = b.RefNo,
                            bankDraftId = b.Id,
                            status = b.Status,
                            assignment = "",
                            keteranganKerja = "",
                            finalApp = b.FinalApplication,
                            pkType = "",
                            state = w.Region
                        }).AsEnumerable().Select(w => new
                    {
                        w.bdNo,
                        w.requester,
                        w.ermsDocNo,
                        w.coCode,
                        w.busA,
                        w.nameOnBD,
                        bdAmount = string.Format("{0:N}", w.bdAmount),
                        ermsPostingDate = w.ermsPostingDate?.ToShortDateString(),
                        //w.bd.TGBSAcceptedOn.Value.ToShortDateString(),
                        docDate = w.docDate?.ToShortDateString(),
                        w.projNo,
                        w.refNo,
                        w.bankDraftId,
                        w.status,
                        w.assignment,
                        w.keteranganKerja,
                        w.finalApp,
                        w.pkType,
                        w.state,
                    });
                    // .Where(x => (x.assignment.Contains(assignment) || assignment == null)
                    //             && (x.refNo.Contains(refNo) || refNo == null)
                    //             && (x.ermsDocNo.Contains(ermsDocNo) || ermsDocNo == null)
                    //             && (x.busA.Contains(busA) || busA == null)
                    //             && (x.state.Contains(state) || state == null)
                    //             && (x.pkType.Contains(pkType) || pkType == null)
                    //             && (x.docDate.Contains(docDate) || docDate == null)
                    //             && (x.ermsPostingDate.Contains(ermsPostingDate) || ermsPostingDate == null)
                    //             && (x.bdAmount.Contains(bdAmount) || bdAmount == null)
                    //             && (x.keteranganKerja.Contains(keteranganKerja) || keteranganKerja == null)
                    //             && (x.coCode.Contains(coCode) || coCode == null)
                    //             && (x.status.Equals("Complete")));

            var result3 = result.Concat(result2).Where(x =>
                (string.IsNullOrEmpty(assignment) || x.assignment.Contains(assignment)) &&
                (string.IsNullOrEmpty(refNo) || x.refNo.Contains(refNo)) &&
                (string.IsNullOrEmpty(ermsDocNo) || x.ermsDocNo.Contains(ermsDocNo)) &&
                (string.IsNullOrEmpty(busA) || x.busA.Contains(busA)) &&
                (string.IsNullOrEmpty(state) || x.state.Contains(state)) &&
                (string.IsNullOrEmpty(pkType) || x.pkType.Contains(pkType)) &&
                (string.IsNullOrEmpty(docDate) || x.docDate.Contains(docDate)) &&
                (string.IsNullOrEmpty(ermsPostingDate) || x.ermsPostingDate.Contains(ermsPostingDate)) &&
                (string.IsNullOrEmpty(bdAmount) || x.bdAmount.Contains(bdAmount)) &&
                (string.IsNullOrEmpty(keteranganKerja) || x.keteranganKerja.Contains(keteranganKerja)) &&
                (string.IsNullOrEmpty(coCode) || x.coCode.Contains(coCode)) &&
                x.status == "Complete");

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

        public JsonResult GetReportForLampiran7(string coCode = null)
        {
            var wc = Db.BankDraft
                .Join(Db.WangCagaran,
                    b => b.Id,
                    w => w.BankDraftId,
                    (b, w) => new
                    {
                        refNo = b.RefNo,
                        dateSubmit = b.SubmittedOn, //.Value.ToShortDateString(),
                        dateVerify = b.VerifiedOn, //.Value.ToShortDateString(),
                        dateApprove = b.ApprovedOn, //.Value.ToShortDateString(),
                        dateAccept = b.TGBSAcceptedOn, //.Value.ToShortDateString(),
                        dateReceive = b.TGBSProcessedOn, //.Value.ToShortDateString(),
                        //dateReceive = b.ReceiveBankDraftDate == null ? "" : b.ReceiveBankDraftDate.Value.ToShortDateString(),
                        dateIssue = b.TGBSIssuedOn, //.Value.ToShortDateString(),
                        division = w.CoCode
                    }).AsEnumerable().Select(b => new
                {
                    b.refNo,
                    dateSubmit = b.dateSubmit?.ToShortDateString(),
                    dateVerify = b.dateVerify?.ToShortDateString(),
                    dateApprove = b.dateApprove?.ToShortDateString(),
                    dateAccept = b.dateAccept?.ToShortDateString(),
                    dateReceive = b.dateReceive?.ToShortDateString(),
                    dateIssue = b.dateIssue?.ToShortDateString(),
                    b.division
                });
                // .Where(x => x.refNo != null && x.division == coCode )
                // .OrderBy(b => b.refNo);

                var wh = Db.BankDraft
                    .Join(Db.WangHangus,
                        b => b.Id,
                        w => w.BankDraftId,
                        (b, w) => new
                        {
                            refNo = b.RefNo,
                            dateSubmit =
                                b.SubmittedOn, //b.SubmittedOn == null ? "" : b.SubmittedOn.Value.ToShortDateString(),
                            dateVerify =
                                b.VerifiedOn, //b.VerifiedOn == null ? "" : b.VerifiedOn.Value.ToShortDateString(),
                            dateApprove =
                                b.ApprovedOn, //b.ApprovedOn == null ? "" : b.ApprovedOn.Value.ToShortDateString(),
                            dateAccept =
                                b.TGBSAcceptedOn, //b.TGBSAcceptedOn == null ? "" : b.TGBSAcceptedOn.Value.ToShortDateString(),
                            dateReceive =
                                b.TGBSProcessedOn, //b.TGBSProcessedOn == null ? "" : b.TGBSProcessedOn.Value.ToShortDateString(),
                            dateIssue = b
                                .TGBSIssuedOn, //b.TGBSIssuedOn == null ? "" : b.TGBSIssuedOn.Value.ToShortDateString(),
                            division = w.CoCode
                        }).AsEnumerable().Select(b => new
                    {
                        b.refNo,
                        dateSubmit = b.dateSubmit?.ToShortDateString(),
                        dateVerify = b.dateVerify?.ToShortDateString(),
                        dateApprove = b.dateApprove?.ToShortDateString(),
                        dateAccept = b.dateAccept?.ToShortDateString(),
                        dateReceive = b.dateReceive?.ToShortDateString(),
                        dateIssue = b.dateIssue?.ToShortDateString(),
                        b.division
                    });
                    // .Where(x => x.refNo != null && x.division == coCode)
                    // .OrderBy(b => b.refNo);

            var result = wc.Concat(wh).OrderBy(x => x.refNo).Where(x => x.refNo != null && x.division == coCode).OrderBy(b => b.refNo);;

            if (coCode == null)
            {
                wc = Db.BankDraft
                    .Join(Db.WangCagaran,
                     b => b.Id,
                     w => w.BankDraftId,
                     (b, w) => new
                     {
                         refNo = b.RefNo,
                         dateSubmit = b.SubmittedOn == null ? "" : b.SubmittedOn.Value.ToShortDateString(),
                         dateVerify = b.VerifiedOn == null ? "" : b.VerifiedOn.Value.ToShortDateString(),
                         dateApprove = b.ApprovedOn == null ? "" : b.ApprovedOn.Value.ToShortDateString(),
                         dateAccept = b.TGBSAcceptedOn == null ? "" : b.TGBSAcceptedOn.Value.ToShortDateString(),
                         dateReceive = b.TGBSProcessedOn == null ? "" : b.TGBSProcessedOn.Value.ToShortDateString(),
                         dateIssue = b.TGBSIssuedOn == null ? "" : b.TGBSIssuedOn.Value.ToShortDateString(),
                         division = w.CoCode
                     })
                    .Where(x => x.refNo != null)
                    .OrderBy(b => b.refNo);

                 wh = Db.BankDraft
                       .Join(Db.WangHangus,
                        b => b.Id,
                        w => w.BankDraftId,
                        (b, w) => new
                        {
                            refNo = b.RefNo,
                            dateSubmit = b.SubmittedOn == null ? "" : b.SubmittedOn.Value.ToShortDateString(),
                            dateVerify = b.VerifiedOn == null ? "" : b.VerifiedOn.Value.ToShortDateString(),
                            dateApprove = b.ApprovedOn == null ? "" : b.ApprovedOn.Value.ToShortDateString(),
                            dateAccept = b.TGBSAcceptedOn == null ? "" : b.TGBSAcceptedOn.Value.ToShortDateString(),
                            dateReceive = b.TGBSProcessedOn == null ? "" : b.TGBSProcessedOn.Value.ToShortDateString(),
                            dateIssue = b.TGBSIssuedOn == null ? "" : b.TGBSIssuedOn.Value.ToShortDateString(),
                            division = w.CoCode
                        })
                       .Where(x => x.refNo != null)
                       .OrderBy(b => b.refNo);

                result = wc.Concat(wh).OrderBy(x => x.refNo);
            }

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

        public void GenerateApplicationForLampiran2()
        {
            //generate sum for bd amount & recovery amount by month an year
            for(int year = 2015; year <= 2020; year++)
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
                            year = s.Year
                        })
                        .Where(x => x.month == month && x.year == year)
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
                    monthly.sumAmount = sumAmount;
                    monthly.sumRecovery = sumRecovery;

                    Db.MonthlyAmount.Add(monthly);
                    Db.SaveChanges();
                }
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
                    if(item.year == 2015)
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
                    prevMonth = Db.MonthlyAmount.Where(x => x.Month == item.month-1 && x.Year == item.year).FirstOrDefault();
                }

                currMonth.diffAmount = currMonth.sumAmount - prevMonth.sumAmount;
                if(prevMonth.sumAmount == 0)
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
        public JsonResult GetAllCompanyCode()
        {
            var result = Db.Division.Select(s => new
            {
                Id = s.Code,
                Name = s.Name
            });

            //result = result.ToList().Add(new
            //{
            //    Id = "0000",
            //    Name = "All Divisions"
            //});

            return new JsonResult(result.ToList());
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

            //foreach (var state in states)
            //{
            //    foreach (var item in bd)
            //    {
            //        if(item.stateId == state.Id)
            //        {
            //            monthly.Id = Guid.NewGuid();
            //            monthly.StateId = item.stateId;
            //            monthly.BDNoIssued = monthly.BDNoIssued + 1;
            //            monthly.Amount = monthly.Amount + item.bdAmount;
            //            monthly.Month = 11;
            //            monthly.Year = 2018;
            //            monthly.BDNoRecovered = 0;
            //            monthly.RecoveryAmount = 0;
            //            monthly.BDNoOutstanding = 0;
            //            monthly.OutstandingAmount = 0;
            //        }

            //    }

            //    Db.StateMonthlyAmount.Add(monthly);
            //    Db.SaveChanges();
            //}



            //var recovery = Db.BankDraft
            //          .Join(Db.Recovery,
            //           b => b.Id,
            //           r => r.BankDraftId,
            //           (b, r) => new { bd = b, rev = r })
            //          .Join(Db.BusinessArea,
            //          r => r.rev.BA,
            //          ba => ba.Name,
            //          (r, ba) => new
            //          {
            //              stateId = ba.StateId,
            //              status = r.rev.Status,
            //              bdAmount = r.rev.BDAmount,
            //              monthRec = r.rev.CompletedOn.Value.Month,
            //              yearRec = r.rev.CompletedOn.Value.Year
            //          })
            //          .Where(x => x.status == "Complete" && x.yearRec == 2018 && x.monthRec == 11)
            //          .ToList();


            //var monthly2 = new StateMonthlyAmount();
            //monthly2.Amount = 0;

            //foreach (var state in states)
            //{
            //    foreach (var item in recovery)
            //    {
            //        if (item.stateId == state.Id)
            //        {
            //            monthly2.BDNoRecovered = monthly.BDNoRecovered + 1;
            //            monthly2.RecoveryAmount = monthly.RecoveryAmount + item.bdAmount;
            //            monthly2.BDNoOutstanding = monthly.BDNoIssued - monthly.BDNoRecovered;
            //            monthly2.OutstandingAmount = monthly.Amount - monthly.RecoveryAmount;
            //        }

            //    }
            //    Db.StateMonthlyAmount.Add(monthly2);
            //    Db.SaveChanges();
            //}


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
