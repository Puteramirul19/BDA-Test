using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
//using BDA.Services;
using BDA.FileStorage;
using BDA.Entities;
using BDA.Web.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using BDA.ViewModel;
using Microsoft.AspNetCore.Identity;
using BDA.Identity;
using Newtonsoft.Json;
using Hangfire;
using Microsoft.AspNetCore.Authorization;

namespace BDA.Controllers
{
    [Authorize]
    public class WangHangusController : BaseController
    {
        //private BankDraftApplicationService service;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public IList<AccountingTableViewModel> accTables = new List<AccountingTableViewModel>();

        public WangHangusController(IFileStore fileStore, IHttpContextAccessor _contextAccessor, UserManager<ApplicationUser> userManager)
        {
            //this.service = service;
            this._contextAccessor = _contextAccessor;
            _userManager = userManager;
        }

        private async Task<BankDraft> TryFindBankDraft(Guid bankDraftId)
        {
            var bankDraft = await Db.BankDraft
                            .Include("WangHangus")
                            .FirstOrDefaultAsync(x => x.Id == bankDraftId);
            if (bankDraft == null)
                throw new IdNotFoundException<BankDraft>(bankDraftId);
            return bankDraft;
        }

        [Authorize(Roles = "Executive/Engineer, Manager/Senior Engineer, Head of Zone (AD)/HOZA, Senior Manager/Lead")]
        public IActionResult Create()
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;
            var requester = Db.Users.Join(Db.UserRoles,
                              u => u.Id,
                              r => r.UserId,
                              (u, r) => new
                              {
                                  Id = u.Id,
                                  RoleId = r.RoleId,
                                  UnitId = r.UnitId,
                                  ZoneId = r.ZoneId,
                                  FunctionId = r.FunctionId,
                                  DivisionId = r.DivisionId,
                                  Division = r.Division.Name,
                                  DivType = r.Division.LOAType
                              }
                          )
                      .Where(x => x.Id == user.UserName)
                      .FirstOrDefault();

            ViewBag.DivType = requester.DivType;

            return View();
        }

        [Authorize(Roles = "Executive/Engineer, Manager/Senior Engineer, Head of Zone (AD)/HOZA, Senior Manager/Lead")]
        [HttpPost]
        [Route("wanghangus/createform")]
        public JsonResult CreateForm(BankDraftViewModel model, List<IFormFile> WHFile = null, List<string> WHFileName = null)
        {

            //if (ModelState.IsValid)
            //{

            var user = _userManager.GetUserAsync(HttpContext.User).Result;
            try
            {
                BankDraft bd = new BankDraft();
                bd.RefNo = GetRunningNo("WangHangus");
                bd.CreatedById = user.Id;
                bd.CreatedByName = user.FullName;
                bd.RequesterId = user.Id;
                //bd.ApproverId = model.ApproverId;
                bd.DraftedOn = DateTime.Now;
                bd.SubmittedOn = model.Status == "Submit" ? DateTime.Now : (DateTime?)null;
                bd.ApproverId = model.ApproverId == null ? null : model.ApproverId;
                bd.Type = Data.BDType.WangHangus.ToString();
                bd.Status = model.Status == "Submit" ? Data.Status.Submitted.ToString() : Data.Status.Draft.ToString();
                bd.RequesterSubmissionComment = model.Comment;
                bd.FinalApplication = "Application";
                bd.NameOnBD = model.WangHangusViewModel.VendorName;

                Db.BankDraft.Add(bd);
                Db.SaveChanges();

                var wangHangusModel = model.WangHangusViewModel;
                WangHangus wang = new WangHangus();
                wang.BankDraftId = bd.Id;
                //wang.VendorType = wangHangusModel.VendorType == "OneTime" ? Data.VendorType.OneTime.ToString() : Data.VendorType.Registered.ToString();
                wang.InvoiceNumber = wangHangusModel.InvoiceNumber;
                wang.Name = wangHangusModel.Name;
                wang.ICNo = wangHangusModel.ICNo;
                wang.Email = wangHangusModel.Email;
                wang.SSTRegNo = wangHangusModel.SSTRegNo;
                wang.BusRegNo = wangHangusModel.BusRegNo;
                wang.Street = wangHangusModel.Street;
                wang.City = wangHangusModel.City;
                wang.Region = wangHangusModel.Region;
                wang.Postcode = wangHangusModel.Postcode;
                wang.Country = wangHangusModel.Country;
                //wang.Negeri = wangHangusModel.Negeri;
                wang.PONumber = wangHangusModel.PONumber;
                wang.Date = wangHangusModel.Date;
                wang.CoCode = wangHangusModel.CoCode;
                wang.BusinessArea = wangHangusModel.BusinessArea;
                wang.VendorNo = wangHangusModel.VendorNo;
                wang.VendorName = wangHangusModel.VendorName;
                wang.BankAccount = wangHangusModel.BankAccount;
                wang.BankCountry = wangHangusModel.BankCountry;
                wang.Description = wangHangusModel.Description;

                Db.WangHangus.Add(wang);
                Db.SaveChanges();

                model.WangHangusViewModel.Id = wang.Id.ToString();


                var str = model.WangHangusViewModel.Accounts == null ? "" : model.WangHangusViewModel.Accounts;
                var validJson = str;
                if (str.Contains("["))
                {
                    str = str.Replace("[", "");
                    validJson = "[" + str.Replace("]", ",") + "]";
                }
                //if (str.Contains("["))
                //    {
                //        validJson = str;
                //    }
                //else if (str.Contains("[]"))
                //{
                //    validJson = str.Replace("[]", "");
                //}
                else
                {
                    validJson = "[" + str.Replace("}{", "},{") + "]";
                }

                var accModel = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(validJson);
                foreach (var items in accModel)
                {
                    AccountingTable accTable = new AccountingTable();
                    foreach (var item in items)
                    {
                        accTable.WangHangusId = wang.Id;
                        accTable.DrCr = item.Key == "drCr" ? item.Value : accTable.DrCr;
                        accTable.GLAccount = item.Key == "glAccount" ? item.Value : accTable.GLAccount;
                        accTable.CONW = item.Key == "conw" ? item.Value : accTable.CONW;
                        //accTable.CONWNo = item.Key == "conwNo" ? item.Value : accTable.CONWNo;
                        accTable.CostObject = item.Key == "costObject" ? item.Value : accTable.CostObject;
                        accTable.TaxCode = item.Key == "taxCode" ? item.Value : accTable.TaxCode;
                        accTable.Currency = item.Key == "currency" ? item.Value : accTable.Currency;
                        accTable.TaxAmount = item.Key == "taxAmount" ? item.Value : accTable.TaxAmount;
                        accTable.Amount = item.Key == "amount" ? item.Value : accTable.Amount;
                    }
                    Db.AccountingTable.Add(accTable);
                    Db.SaveChanges();

                }

                decimal totalAmount = 0;
                decimal totalTax = 0;
                var projectNum = "";
                var currentAccTable = Db.AccountingTable.Where(x => x.WangHangusId == wang.Id).ToList();

                foreach (var item in currentAccTable)
                {
                    if(item.DrCr == "CR")
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

                wang.Amount = totalAmount;
                wang.TaxAmount = totalTax;
                Db.SetModified(wang);

                bd.BankDraftAmount = totalAmount + totalTax;
                bd.ProjectNo = projectNum;
                Db.SetModified(bd);

                Db.SaveChanges();

                if (model.Status == "Submit")
                {
                    Db.BankDraftAction.Add(new BankDraftAction
                    {
                        ActionType = Data.ActionType.Submitted.ToString(),
                        On = DateTime.Now,
                        ById = bd.RequesterId,
                        ParentId = bd.Id,
                        ActionRole = Data.ActionRole.Requester.ToString(),
                        Comment = model.Comment,
                    });
                    Db.SaveChanges();

                    // Notifications

                    Job.Enqueue<Services.NotificationService>(x => x.NotifyApproverForApproval(bd.Id));
                }

                var count = 0;
                foreach (var item in WHFile)
                {
                    UploadFile(item, bd.Id, Data.AttachmentType.BankDraft.ToString(), Data.BDAttachmentType.WHDocument.ToString(), WHFileName[count]);
                    count++;
                }

            }
            catch (Exception e)
            {
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });

            }

            //}
            return Json(new { wanghangusId = model.WangHangusViewModel.Id, response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Application Saved Successful!" });
        }



        [HttpGet]
        public ActionResult Details(Guid? wangId)
        {
            if (wangId == null)
            {
                return NotFound();
            }

            WangCagaran wang = Db.WangCagaran.Include(x => x.BankDraft).FirstOrDefault(x => x.Id == wangId);

            return View(wang);
        }

        [HttpGet]
        public IActionResult Edit(string Id)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;
            var requester = Db.Users.Join(Db.UserRoles,
                              u => u.Id,
                              r => r.UserId,
                              (u, r) => new
                              {
                                  Id = u.Id,
                                  RoleId = r.RoleId,
                                  UnitId = r.UnitId,
                                  ZoneId = r.ZoneId,
                                  FunctionId = r.FunctionId,
                                  DivisionId = r.DivisionId,
                                  //Division = r.Division.Name == ""? "" : r.Division.Name,
                                  //DivType = r.Division.LOAType == null? "": r.Division.LOAType
                              }
                          )
                      .Where(x => x.Id == user.UserName)
                      .FirstOrDefault();

            if (requester.RoleId == "TB" || requester.RoleId == "TR" || requester.RoleId == "BA" || requester.RoleId == "IA" || requester.RoleId == "BP")
            {
                ViewBag.DivType = 0;
            }
            else
            {
                var _user = Db.Users.Join(Db.UserRoles,
                              u => u.Id,
                              r => r.UserId,
                              (u, r) => new
                              {
                                  Id = u.Id,
                                  RoleId = r.RoleId,
                                  UnitId = r.UnitId,
                                  ZoneId = r.ZoneId,
                                  FunctionId = r.FunctionId,
                                  DivisionId = r.DivisionId,
                                  Division = r.Division.Name,
                                  DivType = r.Division.LOAType
                              }
                              ).Where(x => x.Id == user.UserName)
                            .FirstOrDefault();

                ViewBag.DivType = _user.DivType;
            }


            var model = new BankDraftViewModel();
            var item = Db.BankDraft.Where(x => x.Id == Guid.Parse(Id)).FirstOrDefault();
            if (item != null)
            {
                //Declare Bank Draft Details
                model.Id = item.Id.ToString();
                model.RefNo = item.RefNo;
                model.RequesterId = item.RequesterId;
                model.VerifierId = item.VerifierId;
                model.ApproverId = item.ApproverId;
                model.InstructionLetterRefNo = item.InstructionLetterRefNo;
                model.CoverMemoRefNo = item.CoverMemoRefNo;
                model.SendMethod = item.SendMethod;
                model.PostageNo = item.PostageNo;
                model.ReceiverContactNo = item.ReceiverContactNo;
                model.BankDrafNoIssued = item.BankDrafNoIssued;
                model.ReceiveBankDraftDate = item.ReceiveBankDraftDate;
                model.BankDraftDate = item.BankDraftDate;
                model.ReceiptNo = item.ReceiptNo;
                model.Status = item.Status;
                
                //Declare Wang Cagaran Details
                var _wang = Db.WangHangus.Where(x => x.BankDraftId == item.Id).FirstOrDefault();
                if (_wang != null)
                {
                    var wang = new WangHangusViewModel();
                    wang.Id = _wang.Id.ToString();
                    wang.BankDraftId = _wang.BankDraftId;
                    //wang.VendorType = _wang.VendorType == "OneTime" ? Data.VendorType.OneTime.ToString() : Data.VendorType.Registered.ToString();
                    wang.InvoiceNumber = _wang.InvoiceNumber;
                    wang.Name = _wang.Name;
                    wang.ICNo = _wang.ICNo;
                    wang.Email = _wang.Email;
                    wang.SSTRegNo = _wang.SSTRegNo;
                    wang.BusRegNo = _wang.BusRegNo;
                    wang.Street = _wang.Street;
                    wang.City = _wang.City;
                    wang.Region = _wang.Region;
                    wang.Postcode = _wang.Postcode;
                    wang.Country = _wang.Country;
                    //wang.Negeri = _wang.Negeri;
                    wang.PONumber = _wang.PONumber;
                    wang.Date = _wang.Date;
                    wang.CoCode = _wang.CoCode;
                    wang.BusinessArea = _wang.BusinessArea;
                    wang.VendorNo = _wang.VendorNo;
                    wang.VendorName = _wang.VendorName;
                    wang.BankAccount = _wang.BankAccount;
                    wang.BankCountry = _wang.BankCountry;
                    wang.Description = _wang.Description;
                    wang.Amount = _wang.Amount;
                    wang.ErmsDocNo = _wang.ErmsDocNo;
                    wang.PostingDate = _wang.PostingDate;
                    model.WangHangusViewModel = wang;

                }

                ViewBag.ApproverId = Db.Users.Find(model.ApproverId) != null? Db.Users.Find(model.ApproverId).FullName : "" ;
                ViewBag.VendorType = model.WangHangusViewModel.VendorType;

                model.WCSuratKelulusanVM = new AttachmentViewModel();
                model.WCUMAPVM = new AttachmentViewModel();
                model.SignedLetterVM = new AttachmentViewModel();
                model.SignedMemoVM = new AttachmentViewModel();
                model.EvidenceVM = new AttachmentViewModel();

                var _wcsuratkelulusan = Db.Attachment.Where(x => x.ParentId == item.Id && x.FileType == Data.AttachmentType.BankDraft.ToString() && x.FileSubType == Data.BDAttachmentType.WCSuratKelulusan.ToString()).FirstOrDefault();
                if (_wcsuratkelulusan != null)
                {
                    model.WCSuratKelulusanVM.Id = _wcsuratkelulusan.Id.ToString();
                    model.WCSuratKelulusanVM.FileName = _wcsuratkelulusan.FileName;
                }

                var _wcUMAP = Db.Attachment.Where(x => x.ParentId == item.Id && x.FileType == Data.AttachmentType.BankDraft.ToString() && x.FileSubType == Data.BDAttachmentType.WCUMAP.ToString()).FirstOrDefault();
                if (_wcUMAP != null)
                {
                    model.WCUMAPVM.Id = _wcUMAP.Id.ToString();
                    model.WCUMAPVM.FileName = _wcUMAP.FileName;
                }
                var _signedLetter = Db.Attachment.Where(x => x.ParentId == item.Id && x.FileType == Data.AttachmentType.InstructionLetter.ToString()).FirstOrDefault();
                if (_signedLetter != null)
                {
                    model.SignedLetterVM.Id = _signedLetter.Id.ToString();
                    model.SignedLetterVM.FileName = _signedLetter.FileName;
                }
                var _signedMemo = Db.Attachment.Where(x => x.ParentId == item.Id && x.FileType == Data.AttachmentType.Memo.ToString()).FirstOrDefault();
                if (_signedMemo != null)
                {
                    model.SignedMemoVM.Id = _signedMemo.Id.ToString();
                    model.SignedMemoVM.FileName = _signedMemo.FileName;
                }
                var _evidence = Db.Attachment.Where(x => x.ParentId == item.Id && x.FileType == Data.AttachmentType.Evidence.ToString()).FirstOrDefault();
                if (_evidence != null)
                {
                    model.EvidenceVM.Id = _evidence.Id.ToString();
                    model.EvidenceVM.FileName = _evidence.FileName;
                }


                return View(model);
            }
            return View();
        }

        public void UploadFile(IFormFile file, Guid parentId, string fileType, string fileSubType = null, string title = null)
        {
            var uniqueFileName = GetUniqueFileName(file.FileName);
            var ext = Path.GetExtension(uniqueFileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/documents", uniqueFileName);
            file.CopyTo(new FileStream(filePath, FileMode.Create));

                //to do : Save uniqueFileName  to your db table 
                Attachment attachement = new Attachment();
                attachement.FileType = fileType;
                attachement.FileSubType = fileSubType;
                attachement.ParentId = parentId;
                attachement.FileName = uniqueFileName;
                attachement.FileExtension = ext;
                attachement.Title = title;
                attachement.CreatedOn = DateTime.Now;
                attachement.UpdatedOn = DateTime.Now;
                attachement.IsActive = true;

                Db.Attachment.Add(attachement);
                Db.SaveChanges();
          
        }

        public string GetUniqueFileName(string fileName)
        {
            if (fileName.Contains('%'))
            {
                fileName = fileName.Replace("%", "");
            }

            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }


        [HttpPost]
        public JsonResult Edit(BankDraftViewModel model, List<IFormFile>  WHFile, List<string> WHFileName)
        {
            //if (ModelState.IsValid)
            //{
                try
                {
                    var user = _userManager.GetUserAsync(HttpContext.User).Result;

                        var entity = Db.BankDraft.Find(Guid.Parse(model.Id));
                        //entity.RequesterId = user.Id;
                        //entity.Type = Data.BDType.WangCagaran.ToString();
                        //entity.DraftedOn = DateTime.Now;
                        entity.UpdatedOn = DateTime.Now;
                        entity.VerifierId = model.VerifierId == "1" ? null : model.VerifierId;
                        entity.ApproverId = model.ApproverId == "1" ? null : model.ApproverId;
                        entity.SubmittedOn = model.Status == "Submit" ? DateTime.Now : (DateTime?)null;
                        entity.Status = model.Status == "Submit" ? Data.Status.Submitted.ToString() : Data.Status.Draft.ToString();
                        entity.RequesterSubmissionComment = model.Comment;
                        entity.NameOnBD = model.WangHangusViewModel.VendorName;

                        Db.SetModified(entity);
                        Db.SaveChanges();

                        var wangHangusModel = model.WangHangusViewModel;
                        var wang = Db.WangHangus.Find(Guid.Parse(model.WangHangusViewModel.Id));
                        wang.BankDraftId = entity.Id;
                        //wang.VendorType = wangHangusModel.VendorType == "OneTime" ? Data.VendorType.OneTime.ToString() : Data.VendorType.Registered.ToString();
                        wang.InvoiceNumber = wangHangusModel.InvoiceNumber;
                        wang.Name = wangHangusModel.Name;
                        wang.ICNo = wangHangusModel.ICNo;
                        wang.Email = wangHangusModel.Email;
                        wang.SSTRegNo = wangHangusModel.SSTRegNo;
                        wang.BusRegNo = wangHangusModel.BusRegNo;
                        wang.Street = wangHangusModel.Street;
                        wang.City = wangHangusModel.City;
                        wang.Region = wangHangusModel.Region;
                        wang.Postcode = wangHangusModel.Postcode;
                        wang.Country = wangHangusModel.Country;
                        //wang.Negeri = wangHangusModel.Negeri;
                        wang.PONumber = wangHangusModel.PONumber;
                        wang.Date = wangHangusModel.Date;
                        wang.CoCode = wangHangusModel.CoCode;
                        wang.BusinessArea = wangHangusModel.BusinessArea;
                        wang.VendorNo = wangHangusModel.VendorNo;
                        wang.VendorName = wangHangusModel.VendorName;
                        wang.BankAccount = wangHangusModel.BankAccount;
                        wang.BankCountry = wangHangusModel.BankCountry;
                        wang.Description = wangHangusModel.Description;

                        Db.SetModified(wang);
                        Db.SaveChanges();

                var count = 0;
                foreach (var item in WHFile)
                {

                    UploadFile(item, entity.Id, Data.AttachmentType.BankDraft.ToString(), Data.BDAttachmentType.WHDocument.ToString(), WHFileName[count]);
                    count++;
                }

                if (model.Status == "Submit")
                    {
                        Db.BankDraftAction.Add(new BankDraftAction
                        {
                            ActionType = Data.ActionType.Submitted.ToString(),
                            On = DateTime.Now,
                            ById = entity.RequesterId,
                            ParentId = entity.Id,
                            ActionRole = Data.ActionRole.Requester.ToString(),
                            Comment = model.Comment,
                        });
                        Db.SaveChanges();

                        Job.Enqueue<Services.NotificationService>(x => x.NotifyApproverForApproval(entity.Id));
                    }
                
                        return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Application Saved Successful!" });
        }
                catch (Exception e)
                {
                    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message});

                }
            //}
            //else
            //{
            //    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Error! Please try again later." });
            //}
        }

        public string GetRunningNo(string type)
        {
            RunningNo runningNo = new RunningNo();

            var entity = Db.RunningNo.Where(x => x.Name == "WangCagaran").FirstOrDefault();

            if (type == "WangHangus")
            {
                entity = Db.RunningNo.Where(x => x.Name == "WangHangus").FirstOrDefault(); //Id for Instruction Letter

            }

            runningNo.Code = entity.Code;
            runningNo.RunNo = entity.RunNo;
            string NewCode = String.Format("{0}{1:00000}", runningNo.Code, runningNo.RunNo);

            entity.RunNo = entity.RunNo + 1;
            Db.RunningNo.Update(entity);
            Db.SaveChanges();

            return NewCode;
        }

        public JsonResult GetAllWangHangusAttachment(Guid parentId)
        {
            var result = Db.Attachment.Where(x => x.ParentId == parentId).ToList();

            return new JsonResult(result.ToList());
        }

            public JsonResult GetSupportDocument(Guid? parentId = null, string fileType = null, string fileSubType = null)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            var result = Db.Attachment
                        .Where(x => x.ParentId == parentId && x.FileType == fileType && x.FileSubType == fileSubType)
                        .OrderBy(x => x.CreatedOn)
                        .ToList();

            return new JsonResult(result.ToList());
        }

    }
}