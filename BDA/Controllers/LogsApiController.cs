using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BDA.Identity;
using BDA.Services;
using BDA.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;


namespace BDA.Controllers
{
    [Route("api/log/")]
    public class LogsApiController : Controller
    {
        private string _draw;
        private string _start;
        private string _length;
        private string _sortColumn;
        private string _sortColumnDirection;
        private string _searchValue;

        private int _pageSize;
        private int _skip;
        private int _recordsTotal;

        private ILogService _logService;
        private readonly UserManager<ApplicationUser> _userManager;
        List<string> listCol = new List<string> { "0" };
        List<string> BdCol = new List<string> { "RefNo","BankDraftAmount", "NameOnBD","RequesterId","ApproverId","Status","InstructionLetterRefNo","SendMethod","BankDraftDate","PostageNo",
            "BankDrafNoIssued", "ReceiverContactNo", "CoverMemoRefNo","ReceiveBankDraftDate", "ReceiptNo","BankDraftAmount" };
        List<string> listColWC = new List<string> { "0" };
        List<string> listColWH = new List<string> { "0" };
        List<string> listColWCH = new List<string> { "0" };
        List<string> listColNotExistWH = new List<string> { "Alamat1","KeteranganKerja", "JKRInvolved","CajKod","WBSProjekNo"};
        List<string> listColNotExistWC = new List<string> { "PONumber", "InvoiceNumber", "VendorNo", "VendorName", "BankAccount", "BankCountry","Description" };
        List<string> listColAccountingTable = new List<string> { "GLAccount", "CONW", "CostObject", "TaxCode", "Currency", "TaxAmount", "Amount" };

        public LogsApiController(
                UserManager<ApplicationUser> userManager, ILogService logService)
        {

            _logService = logService;
            _userManager = userManager;
        }

        [Route("getlog")]
        [HttpPost]
        public IActionResult GetLog(LogViewModel input)
        {

            try
            {
               
                if (input.ColumnList == null)
                    throw new Exception("Column List cannot be empty.");

                if (input.TableName == null || input.TableName == "")
                    throw new Exception("TableName cannot be empty.");


                string Query = "select " + string.Join(",", input.ColumnList) + " from " + input.TableName;
                string Query1 = "";
                string Query2 = "";

                if (input.TableName == "Cancellation" || input.TableName == "Lost" || input.TableName == "Recovery")
                {
                    listCol.RemoveAt(0);
                    foreach (var column in input.ColumnList)
                    {
                        if(BdCol.Contains(column))
                        {
                            listCol.Add("BankDraft." + column);
                        }
                        else
                        {
                            listCol.Add(input.TableName + "." + column);
                        }

                    }
                        Query = "select " + string.Join(", ", listCol) + " from " + input.TableName + " inner join BankDraft on " + input.TableName  + ".bankdraftid = BankDraft.id";
                }
                else if (input.TableName == "BankDraft")
                {
                    listCol.RemoveAt(0);
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
                            if(column == "Tarikh" || column == "Bandar" || column == "Negeri" || column == "Poskod")
                            {
                                if(column == "Tarikh")
                                {
                                    listColWH.Add("WangHangus.Date as Tarikh");
                                }
                                else if(column == "Bandar")
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
                    input.FilterWhere = input.FilterWhere.Substring(0,input.FilterWhere.Length - 3);
                    input.FilterWhere = input.FilterWhere.Replace(",", "','");
                    string vendorNo = input.FilterWhere.Replace("VendorNo", "'VendorNo'");
                    Query = Query + " where " + vendorNo;

                   

                    if(Query1 != "" && Query2 != "")
                    {
                        string region = input.FilterWhere.Replace("Negeri", "Region");
                        Query1 = Query1 + " where " + region; 
                        Query2 = Query2 + " where " + vendorNo;
                        Query = Query + Query1 + Query2;
                    }
                    
                    countSql += $"where {input.FilterWhere} ";
                }

                
                if (!string.IsNullOrEmpty(_searchValue))
                {
                    Query += !Query.Contains("where") ? " where (" : " and (";
                    countSql += !countSql.Contains("where") ? " where (" : " and (";

                    int i = 0;
                    foreach (var column in input.ColumnList)
                    {
                        Query += $" {column} like '%{_searchValue}%'";
                        countSql += $" {column} like '%{_searchValue}%'";
                        if (i >= input.ColumnList.Count - 1)
                        {
                            Query += ")";
                            countSql += ")";
                        }
                        else
                        {
                            Query += " or";
                            countSql += " or";
                        }
                        i++;
                    }

                }

                if (_sortColumn != null)
                {
                   // Query += $" order by {_sortColumn} {_sortColumnDirection ??= "asc"}";
                }

                var ds1 = _logService.GetLogByQuery(Query);

                if (_pageSize > 0)
                    Query += $" order by refno offset {_skip} rows fetch next {_pageSize} rows only";

              //  var countDs = _logService.GetLogByQuery(countSql);

                var ds = _logService.GetLogByQuery(Query);

                _recordsTotal = ds1.Tables[0].Rows.Count;
                    //countDs.Tables[0].
                    //countDs.Tables[0].Rows[0].Field<int>(0);

                var data = GetJSON(ds, input.ColumnArray);

                return Json(new { draw = _draw, recordsFiltered = _recordsTotal, recordsTotal = _recordsTotal, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            _draw = Request.Form["draw"].FirstOrDefault();
            // Skiping number of Rows count  
            _start = Request.Form["start"].FirstOrDefault();
            // Paging Length 10,20  
            _length = Request.Form["length"].FirstOrDefault();
            // Sort Column Name  
            _sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][data]"].FirstOrDefault();
            _sortColumn = _sortColumn[0].ToString().ToUpper() + _sortColumn.Remove(0, 1);
            // Sort Column Direction ( asc ,desc)  
            _sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            // Search Value from (Search box)  
            _searchValue = Request.Form["search[value]"].FirstOrDefault();

            //Paging Size (10,20,50,100)  
            _pageSize = _length != null ? Convert.ToInt32(_length) : 50;
            _skip = _start != null ? Convert.ToInt32(_start) : 0;
            _recordsTotal = 0;
        }

        private List<Dictionary<string, object>> GetJSON(DataSet ds, List<List<string>> ColumnArray, bool change = false)
        {
            ArrayList root = new ArrayList();
            List<Dictionary<string, object>> table;
            Dictionary<string, object> data;

            table = new List<Dictionary<string, object>>();

            foreach (DataTable dt in ds.Tables)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    data = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        var colString = "";

                        if (change)
                            colString += ColumnArray.Where(x => x.Contains(col.ColumnName)).Select(x => x[1].ToString()).FirstOrDefault();
                        else
                            colString += col.ColumnName;

                        if (dr[col].GetType() == typeof(DBNull))
                            data.Add(colString, "");
                        else
                            data.Add(colString, dr[col]);
                    }
                    table.Add(data);
                }
                //root.Add(table);
            }
            return table; //remove this for multi data table
        }

        private string GetRelationalData(string ColumnName, string ColumnData)
        {
            var UserColumns = new List<string>()
            {
                "created by", "issue by", "im", "pic"
            };

            var UsersColumns = new List<string>()
            {
                "notified person",
            };

            if (UserColumns.Contains(ColumnName.ToLower()))
            {
                //if (ColumnData == "" || ColumnData == null) ColumnData = "0";
                ////var UserDetails = _userManager.GetByUserId(Convert.ToInt32(ColumnData));
                //var UserDetails = _userManager.(Convert.ToInt32(ColumnData));
                //return UserDetails != null ? UserDetails.Fullname : ColumnData;
            } 
            else if (UsersColumns.Contains(ColumnName.ToLower()))
            {
                var userIds = ColumnData.Split('\u002C');
                var userFullnames = "";
                foreach (var userId in userIds)
                {
                    if (userId == "" || userId == null) continue;
                    //var UserDetails = _userService.GetByUserId(Convert.ToInt32(userId));
                    //userFullnames += $"{(UserDetails != null ? UserDetails.Fullname : userId)}, ";
                }
                return userFullnames;
            }

            return ColumnData;
        }
    }
}
