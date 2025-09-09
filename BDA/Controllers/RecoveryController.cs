using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BDA.Entities;
using BDA.Identity;
using BDA.ViewModel;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BDA.Web.Controllers
{
    class Duration
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }

    class RecoveryType
    {
        public string Type { get; set; }
        public string Name { get; set; }

    }
    [Authorize]
    public class RecoveryController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public RecoveryController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Recover()
        {
            return View();
        }
        public IActionResult _StatusBar()
        {
            return View();
        }public IActionResult BankDraftList()
        {
            return View();
        }

        [Authorize(Roles = "Executive/Engineer, Manager/Senior Engineer, Head of Zone (AD)/HOZA, Senior Manager/Lead")]
        public IActionResult Create()
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            ViewBag.Fullname = user.FullName;
            return View();
        }

        [Authorize(Roles = "Executive/Engineer, Manager/Senior Engineer, Head of Zone (AD)/HOZA, Senior Manager/Lead")]
        [HttpPost]
        public JsonResult Create(RecoveryViewModel model, IFormFile Checklist, IFormFile RecoveryLetter)
        {
            //if (ModelState.IsValid)
            //{
                try
                {
                    if (model.Status == "Create" && Checklist == null)
                    {
                        return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Marked Checklist Required!" });
                    }
                    else if (model.Status == "Create" && RecoveryLetter == null)
                    {
                        return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Recovery Letter Required!" });
                    }

                    var user = _userManager.GetUserAsync(HttpContext.User).Result;

                    Recovery entity = new Recovery();
                    entity.CreatedById = user.Id;
                    entity.CreatedByName = user.FullName;
                    entity.RequesterId = user.Id;
                    entity.DraftedOn = DateTime.Now;
                    entity.BankDraftId = Guid.Parse(model.BankDraftId);
                    entity.SubmittedOn = model.Status == "Create" ? DateTime.Now : (DateTime?)null;
                    entity.BDNo = model.BDNo;
                    entity.ERMSDocNo = model.ERMSDocNo;
                    entity.CoCode = model.CoCode;
                    entity.BA = model.BA;
                    entity.ProjNo = model.ProjNo;
                    entity.BDRequesterName = model.BDRequesterName;
                    entity.BDAmount = model.BDAmount;
                    entity.TotalRecoveryAmount = model.BDAmount;
                    entity.NameOnBD = model.NameOnBD;
                    entity.PBTEmailAddress = model.PBTEmailAddress;
                //DateTime date = DateTime.ParseExact(this.Text, "dd/MM/yyyy", null);
                //var d = model.ProjectCompletionDate.Value.Day;
                //var m = model.ProjectCompletionDate.Value.Month < 10 ? "0"+ model.ProjectCompletionDate.Value.Month : model.ProjectCompletionDate.Value.Month.ToString();
                //var y = model.ProjectCompletionDate.Value.Year;
                //var dmy = d + "/" + m + "/" + y;
                    entity.ProjectCompletionDate = model.ProjectCompletionDate;
                    //DateTime.ParseExact(model.ProjectCompletionDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //DateTime.ParseExact(model.ProjectCompletionDate.ToString(), "dd/MM/yyyy", null); 
                    entity.RefNo = model.RefNo;
                    entity.RequesterSubmissionComment = model.Comment;
                    entity.Status = model.Status == "Create" ? Data.Status.Created.ToString() : Data.Status.Draft.ToString();

                Db.Recovery.Add(entity);
                Db.SaveChanges();

                //if (model.Status == "Create")
                //{
                    var bd = Db.BankDraft.Where(x => x.Id == entity.BankDraftId).FirstOrDefault();
                    bd.FinalApplication = "Recovery";

                    Db.SetModified(bd);
                    Db.SaveChanges();
                //}
                   

                if (entity.Status == "Created")
                    {
                        Db.BankDraftAction.Add(new BankDraftAction
                        {
                            ApplicationType = Data.AppType.Recovery.ToString(),
                            ActionType = Data.ActionType.Created.ToString(),
                            On = DateTime.Now,
                            ById = user.Id,
                            ParentId = entity.Id,
                            ActionRole = Data.ActionRole.Requester.ToString(),
                            Comment = model.Comment,
                        });
                        Db.SaveChanges();
 
                     }

                    if (Checklist != null)
                    {
                        UploadFile(Checklist, entity.Id, Data.AttachmentType.Recovery.ToString(), Data.BDAttachmentType.Checklist.ToString());
                    }

                    if (RecoveryLetter != null)
                    {
                        UploadFile(RecoveryLetter, entity.Id, Data.AttachmentType.Recovery.ToString(), Data.BDAttachmentType.RecoveryLetter.ToString());
                    }

                //email notification to PBT/Agensi Kerajaan for Creation
                Job.Enqueue<Services.NotificationService>(x => x.NotifyRecoveryAgensiKerajaanForCreation(entity.Id));

                if(model.Status == "Create")
                {
                    return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Recovery Created Successfull!", id = entity.Id });
                }
                else
                {
                    return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Recovery Saved Successfull!", id = entity.Id });
                }
             
                }
                catch (Exception e)
                {
                    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });

                }
            //}
            //else
            //{
            //    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Error! Please try again later." });
            //}
        }


        [HttpGet]
        public IActionResult Edit(string Id)
        {
            var model = new RecoveryViewModel();
            var item = Db.Recovery.Where(x => x.Id == Guid.Parse(Id)).FirstOrDefault();
            if (item != null)
            {
                //Declare Bank Draft Details
                model.Id = item.Id.ToString();
                model.RequesterId = item.RequesterId;
                model.BDNo = item.BDNo;
                model.BankDraftId = item.BankDraftId.ToString();
                model.BDAmount = item.BDAmount;
                model.NameOnBD = item.NameOnBD;
                model.PBTEmailAddress = item.PBTEmailAddress;
                model.ProjectCompletionDate = item.ProjectCompletionDate;
                model.BDRequesterName = item.BDRequesterName;
                model.RefNo = item.RefNo;
                model.ERMSDocNo = item.ERMSDocNo;
                model.CoCode = item.CoCode;
                model.BA = item.BA;
                model.ProjNo = item.ProjNo;
                model.Status = item.Status;
                model.RecoveryType = item.RecoveryType;
                model.TotalRecoveryAmount = item.TotalRecoveryAmount;
                    //string.Format("{0:C}", item.TotalRecoveryAmount);
                model.FirstRecoveryAmount = item.FirstRecoveryAmount;
                model.SecondRecoveryAmount = item.SecondRecoveryAmount;
                model.SiteVisitDate = item.SiteVisitDate;
                model.CPCDate = item.CPCDate;
                model.ClaimDuration = item.ClaimDuration;
                model.ReceivedDate = item.ReceivedDate;
                model.PartialReceivedOn = item.PartialReceivedDate;
                model.ErmsDocNo1 = item.ErmsDocNo1;
                model.ErmsDocNo2 = item.ErmsDocNo2;
                model.PostingDate1 = item.PostingDate1;
                model.PostingDate2 = item.PostingDate2;
                model.ChecklistVM = new AttachmentViewModel();
                model.RecoveryLetterVM = new AttachmentViewModel();
                model.FirstPartialDocVM = new AttachmentViewModel();
                model.SecondPartialDocVM = new AttachmentViewModel();
                model.FullDocVM = new AttachmentViewModel();
                model.FirstParBankStatementVM = new AttachmentViewModel();
                model.SecondParBankStatementVM = new AttachmentViewModel();
                model.BankStatementVM = new AttachmentViewModel();

                var _checklist = Db.Attachment.Where(x => x.ParentId == item.Id && x.FileType == Data.AttachmentType.Recovery.ToString() && x.FileSubType == Data.BDAttachmentType.Checklist.ToString()).FirstOrDefault();
                if (_checklist != null)
                {
                    model.ChecklistVM.Id = _checklist.Id.ToString();
                    model.ChecklistVM.FileName = _checklist.FileName;
                }

                var _recoveryLetter = Db.Attachment.Where(x => x.ParentId == item.Id && x.FileType == Data.AttachmentType.Recovery.ToString() && x.FileSubType == Data.BDAttachmentType.RecoveryLetter.ToString()).FirstOrDefault();
                if (_recoveryLetter != null)
                {
                    model.RecoveryLetterVM.Id = _recoveryLetter.Id.ToString();
                    model.RecoveryLetterVM.FileName = _recoveryLetter.FileName;
                }

                var _firstPartialDocVM = Db.Attachment.Where(x => x.ParentId == item.Id && x.FileType == Data.AttachmentType.Recovery.ToString() && x.FileSubType == Data.BDAttachmentType.FirstPartial.ToString()).FirstOrDefault();
                if (_firstPartialDocVM != null)
                {
                    model.FirstPartialDocVM.Id = _firstPartialDocVM.Id.ToString();
                    model.FirstPartialDocVM.FileName = _firstPartialDocVM.FileName;
                }

                var _secondPartialDocVM = Db.Attachment.Where(x => x.ParentId == item.Id && x.FileType == Data.AttachmentType.Recovery.ToString() && x.FileSubType == Data.BDAttachmentType.SecondPartial.ToString()).FirstOrDefault();
                if (_secondPartialDocVM != null)
                {
                    model.SecondPartialDocVM.Id = _secondPartialDocVM.Id.ToString();
                    model.SecondPartialDocVM.FileName = _secondPartialDocVM.FileName;
                }

                var _fullDocVM = Db.Attachment.Where(x => x.ParentId == item.Id && x.FileType == Data.AttachmentType.Recovery.ToString() && x.FileSubType == Data.BDAttachmentType.FullPartial.ToString()).FirstOrDefault();
                if (_fullDocVM != null)
                {
                    model.FullDocVM.Id = _fullDocVM.Id.ToString();
                    model.FullDocVM.FileName = _fullDocVM.FileName;
                }

                var _firstParStatementBank = Db.Attachment.Where(x => x.ParentId == item.Id && x.FileType == Data.AttachmentType.Recovery.ToString() && x.FileSubType == Data.BDAttachmentType.FirstPartialBankStatement.ToString()).FirstOrDefault();
                if (_firstParStatementBank != null)
                {
                    model.FirstParBankStatementVM.Id = _firstParStatementBank.Id.ToString();
                    model.FirstParBankStatementVM.FileName = _firstParStatementBank.FileName;
                }

                var _secondParStatementBank = Db.Attachment.Where(x => x.ParentId == item.Id && x.FileType == Data.AttachmentType.Recovery.ToString() && x.FileSubType == Data.BDAttachmentType.SecondPartialBankStatement.ToString()).FirstOrDefault();
                if (_secondParStatementBank != null)
                {
                    model.SecondParBankStatementVM.Id = _secondParStatementBank.Id.ToString();
                    model.SecondParBankStatementVM.FileName = _secondParStatementBank.FileName;
                }

                var _statementBank = Db.Attachment.Where(x => x.ParentId == item.Id && x.FileType == Data.AttachmentType.Recovery.ToString() && x.FileSubType == Data.BDAttachmentType.BankStatement.ToString()).FirstOrDefault();
                if (_statementBank != null)
                {
                    model.BankStatementVM.Id = _statementBank.Id.ToString();
                    model.BankStatementVM.FileName = _statementBank.FileName;
                }

                return View(model);
            }
            return View();
        }

        public JsonResult GetSupportDocument(Guid? bankDraftId = null,  string fileType = null, string fileSubType = null)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            var result = Db.Attachment
                        .Where(x=>x.ParentId == bankDraftId && x.FileType ==  fileType && x.FileSubType == fileSubType)
                        .OrderBy(x => x.CreatedOn)
                        .ToList();
            
            return new JsonResult(result.ToList());
        }

        [HttpPost]
        public JsonResult Edit(RecoveryViewModel model, IFormFile Checklist, IFormFile RecoveryLetter)
        {

            //if (ModelState.IsValid)
            //{
                try
                {
                    var _checklist = Db.Attachment.Where(x => x.ParentId == Guid.Parse(model.Id) && x.FileType == Data.AttachmentType.Recovery.ToString() && x.FileSubType == Data.BDAttachmentType.Checklist.ToString()).FirstOrDefault();

                    if( _checklist ==  null)
                    {
                        if (model.Status == "Create" && Checklist == null)
                        {
                            return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Marked Checklist Required!" });
                        }
                    }

                    var _recoveryLetter = Db.Attachment.Where(x => x.ParentId == Guid.Parse(model.Id) && x.FileType == Data.AttachmentType.Recovery.ToString() && x.FileSubType == Data.BDAttachmentType.RecoveryLetter.ToString()).FirstOrDefault();

                    if(_recoveryLetter == null)
                    {
                        if (model.Status == "Create" && RecoveryLetter == null)
                        {
                            return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Recovery Letter Required!" });
                        }
                    }
                 

                    var user = _userManager.GetUserAsync(HttpContext.User).Result;
                    var entity = Db.Recovery.Find(Guid.Parse(model.Id));

                if (model.BankDraftId != null)
                {
                    if (entity.BankDraftId != Guid.Parse(model.BankDraftId))
                    {

                        var bd = Db.BankDraft.Where(x => x.Id == Guid.Parse(model.BankDraftId)).FirstOrDefault();
                        bd.FinalApplication = "Recovery";
                        Db.SetModified(bd);

                        var bd2 = Db.BankDraft.Where(x => x.Id == entity.BankDraftId).FirstOrDefault();
                        bd2.FinalApplication = "Application";

                        Db.SetModified(bd2);
                        entity.BankDraftId = Guid.Parse(model.BankDraftId);
                    }
                }

                    entity.RequesterId = user.Id;
                    entity.SubmittedOn = model.Status == "Create" ? DateTime.Now : (DateTime?)null;
                    entity.BDNo = model.BDNo;
                    entity.ERMSDocNo = model.ERMSDocNo;
                    entity.CoCode = model.CoCode;
                    entity.BA = model.BA;
                    entity.ProjNo = model.ProjNo;
                    entity.BDRequesterName = model.BDRequesterName;
                    entity.BDAmount = model.BDAmount;
                    entity.NameOnBD = model.NameOnBD;
                    entity.PBTEmailAddress = model.PBTEmailAddress;
                    entity.ProjectCompletionDate = model.ProjectCompletionDate;
                    entity.RefNo = model.RefNo;
                    entity.RequesterSubmissionComment = model.Comment;
                    entity.Status = model.Status == "Create" ? Data.Status.Created.ToString() : Data.Status.Draft.ToString();

                    Db.SetModified(entity);
                    Db.SaveChanges();

               

                if (Checklist != null)
                    {
                        UploadFile(Checklist, entity.Id, Data.AttachmentType.Recovery.ToString(), Data.BDAttachmentType.Checklist.ToString());
                    }

                    if (RecoveryLetter != null)
                    {
                        UploadFile(RecoveryLetter, entity.Id, Data.AttachmentType.Recovery.ToString(), Data.BDAttachmentType.RecoveryLetter.ToString());
                    }

                    if (model.Status == "Create")
                    {
                        Db.BankDraftAction.Add(new BankDraftAction
                        {
                            ApplicationType = Data.AppType.Recovery.ToString(),
                            ActionType = Data.ActionType.Created.ToString(),
                            On = DateTime.Now,
                            ById = user.Id,
                            ParentId = entity.Id,
                            Comment = model.Comment,
                            ActionRole = Data.ActionRole.Requester.ToString(),
                        });
                        Db.SaveChanges();

                        //email notification to PBT/Agensi Kerajaan for Creation
                        Job.Enqueue<Services.NotificationService>(x => x.NotifyRecoveryAgensiKerajaanForCreation(entity.Id));
                    }

                if (model.Status == "Create")
                {
                    return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Recovery Created Successfull!", id = entity.Id });
                }
                else
                {
                    return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Recovery Saved Successfull!", id = entity.Id });
                }

            }
                catch (Exception e)
                {
                    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });

                }
            //}
            //else
            //{
            //    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Error! Please try again later." });
            //}
        }

        [HttpPost]
        public JsonResult Withdrawn(RecoveryViewModel model)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            string actionType = "";
            string byId = "";
            string actionRole = "";

            try
            {
                switch (model.UserAction)
                {
                    case "Withdrawn":
                        model.Status = Data.Status.Withdrawn.ToString();
                        break;
                    default:
                        model.Status = "Invalid";
                        break;
                }

                if (model.Status != "Invalid")
                {
                    var entity = Db.Recovery.Find(Guid.Parse(model.Id));
                    entity.UpdatedOn = DateTime.Now;

                    if (model.Status == Data.Status.Withdrawn.ToString())
                    {
                        entity.Status = model.Status;
                        entity.WithdrewOn = DateTime.Now;

                        //audit trail for Withdrawn
                        actionType = Data.ActionType.Withdrawn.ToString();
                        byId = model.RequesterId;
                        actionRole = Data.ActionRole.Requester.ToString();

                        //Job.Enqueue<Services.NotificationService>(x => x.NotifyRecoveryAgensiKerajaanForCreation(entity.Id));
                    }
                  
                 
                    Db.SetModified(entity);
                    Db.SaveChanges();

                    //Update audit trail

                    Db.BankDraftAction.Add(new BankDraftAction
                    {
                        ApplicationType = Data.AppType.Recovery.ToString(),
                        ActionType = actionType,
                        On = DateTime.Now,
                        ById = byId,
                        ParentId = Guid.Parse(model.Id),
                        ActionRole = actionRole,
                        Comment = model.Comment,
                    });
                    Db.SaveChanges();

                    return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Recovery Withdrawn Successfull!" });
                }

                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Error! Invalid Action " + model.UserAction });
            }
            catch (Exception e)
            {
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });

            }
         
        }

        [Authorize(Roles = "Executive, Manager, Head of Zone, Senior Manager")]
        [HttpPost]
        public JsonResult Process(RecoveryViewModel model)
        {

            var entity = Db.Recovery.Find(Guid.Parse(model.Id));
            try
                {
                if(model.UserAction == "Withdrawn")
                {
                    entity.UpdatedOn = DateTime.Now;

                    if (model.UserAction == Data.Status.Withdrawn.ToString())
                    {
                        entity.Status = "Withdrawn";
                        entity.WithdrewOn = DateTime.Now;
  
                    }

                    Db.SetModified(entity);
                    Db.SaveChanges();

                    //Update audit trail

                    Db.BankDraftAction.Add(new BankDraftAction
                    {
                        ApplicationType = Data.AppType.Recovery.ToString(),
                        ActionType = Data.ActionType.Withdrawn.ToString(),
                        On = DateTime.Now,
                        ById = model.RequesterId,
                        ParentId = Guid.Parse(model.Id),
                        ActionRole = Data.ActionRole.Requester.ToString(),
                        Comment = model.Comment,
                    });
                    Db.SaveChanges();

                    return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Recovery Withdrawn Successfull!", id = entity.Id });
                }
                else
                {
                    var user = _userManager.GetUserAsync(HttpContext.User).Result;

                    entity.SiteVisitDate = model.SiteVisitDate;
                    entity.CPCDate = model.CPCDate;
                    entity.ClaimDuration = model.ClaimDuration;
                    entity.RequesterProcessComment = model.Comment;
                    entity.UpdatedOn = DateTime.Now;

                    if (model.UserAction == "Process")
                    {
                        entity.ProcessedOn = DateTime.Now;
                        entity.ProcesserId = user.Id;
                        entity.Status = Data.Status.Processed.ToString();

                    }

                    Db.SetModified(entity);
                    Db.SaveChanges();

                    if (model.UserAction == "Process")
                    {
                        Db.BankDraftAction.Add(new BankDraftAction
                        {
                            ApplicationType = Data.AppType.Recovery.ToString(),
                            ActionType = Data.ActionType.Processed.ToString(),
                            On = DateTime.Now,
                            ById = user.Id,
                            ParentId = entity.Id,
                            Comment = model.Comment,
                            ActionRole = Data.ActionRole.Requester.ToString(),
                        });
                        Db.SaveChanges();

                        //Job.Enqueue<Services.NotificationService>(x => x.NotifyRequesterForBDAcceptance(entity.Id));

                    }
                }

                if (model.UserAction == "Process")
                {
                    return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Recovery Process Successfull!", id = entity.Id });
                }
                else 
                {
                    return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Recovery Saved Successfull!", id = entity.Id });
                }

                //return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Recovery Saved Successfully!" });
            }
                catch (Exception e)
                {
                    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });

                }
        
            //}
            //else
            //{
            //    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Error! Please try again later." });
            //}
        }

        [Authorize(Roles = "Executive, Manager, Head of Zone, Senior Manager")]
        [HttpPost]
        public JsonResult SubmitRecovery(RecoveryViewModel model, List<IFormFile> FirstPartialDoc, List<IFormFile> SecondPartialDoc, List<IFormFile> FullDoc, List<string> FirstPartialDocName, List<string> SecondPartialDocName, List<string> FullDocName)
        {
            var entity = Db.Recovery.Find(Guid.Parse(model.Id));

            try
            {

                if (model.UserAction == "Withdrawn")
                {
                    entity.UpdatedOn = DateTime.Now;

                        entity.Status = Data.Status.Withdrawn.ToString();
                        entity.WithdrewOn = DateTime.Now;

                    Db.SetModified(entity);
                    Db.SaveChanges();

                    //Update audit trail

                    Db.BankDraftAction.Add(new BankDraftAction
                    {
                        ApplicationType = Data.AppType.Recovery.ToString(),
                        ActionType = Data.ActionType.Withdrawn.ToString(),
                        On = DateTime.Now,
                        ById = model.RequesterId,
                        ParentId = Guid.Parse(model.Id),
                        ActionRole = Data.ActionRole.Requester.ToString(),
                        Comment = model.Comment,
                    });
                    Db.SaveChanges();

                    return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Recovery Withdrawn Successfull!", id = entity.Id });
                }
                else
                {
                    var user = _userManager.GetUserAsync(HttpContext.User).Result;

                    entity.RecoveryType = model.RecoveryType;
                    entity.TotalRecoveryAmount = Convert.ToDecimal(model.TotalRecoveryAmount);
                    entity.FirstRecoveryAmount = model.FirstRecoveryAmount;
                    entity.SecondRecoveryAmount = model.SecondRecoveryAmount;
                    entity.RequesterFinalSubmissionComment = model.Comment;
                    entity.UpdatedOn = DateTime.Now;

                    if (model.UserAction == "Submit")
                    {

                        var _full = Db.Attachment.Where(x => x.ParentId == Guid.Parse(model.Id) && x.FileType == Data.AttachmentType.Recovery.ToString() && x.FileSubType == Data.BDAttachmentType.FullPartial.ToString()).FirstOrDefault();
                        var _first = Db.Attachment.Where(x => x.ParentId == Guid.Parse(model.Id) && x.FileType == Data.AttachmentType.Recovery.ToString() && x.FileSubType == Data.BDAttachmentType.FirstPartial.ToString()).FirstOrDefault();
                        var _second = Db.Attachment.Where(x => x.ParentId == Guid.Parse(model.Id) && x.FileType == Data.AttachmentType.Recovery.ToString() && x.FileSubType == Data.BDAttachmentType.SecondPartial.ToString()).FirstOrDefault();

                        if (FullDoc.Count == 0 && model.RecoveryType == "Full" && _full == null)
                        {
                            return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Supporting Document Required!" });
                        }
                        else if (FirstPartialDoc.Count == 0 && model.RecoveryType == "FirstPartial" && model.Status == "Processed" && _first == null)
                        {
                            return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Supporting Document Required!" });
                        }
                        else if (SecondPartialDoc.Count == 0 && model.RecoveryType == "SecondPartial" && model.Status == "PartialComplete" && _second == null)
                        {
                            return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Supporting Document Required!" });
                        }

                        entity.FinalSubmissionOn = DateTime.Now;
                        entity.SubmitterId = user.Id;
                        entity.Status = Data.Status.Submitted.ToString();
                    }

                    Db.SetModified(entity);
                    Db.SaveChanges();

                    var count1 = 0;
                    var count2 = 0;
                    var count3 = 0;

                    foreach (var item in FirstPartialDoc)
                    {
                        UploadFile(item, entity.Id, Data.AttachmentType.Recovery.ToString(), Data.BDAttachmentType.FirstPartial.ToString(), FirstPartialDocName[count1]);
                        count1++;
                    }

                    foreach (var item in SecondPartialDoc)
                    {
                        UploadFile(item, entity.Id, Data.AttachmentType.Recovery.ToString(), Data.BDAttachmentType.SecondPartial.ToString(), SecondPartialDocName[count2]);
                        count2++;
                    }

                    foreach (var item in FullDoc)
                    {
                        UploadFile(item, entity.Id, Data.AttachmentType.Recovery.ToString(), Data.BDAttachmentType.FullPartial.ToString(), FullDocName[count3]);
                        count3++;
                    }

                    if (model.UserAction == "Submit")
                    {
                        Db.BankDraftAction.Add(new BankDraftAction
                        {
                            ApplicationType = Data.AppType.Recovery.ToString(),
                            ActionType = Data.ActionType.Submitted.ToString(),
                            On = DateTime.Now,
                            ById = user.Id,
                            ParentId = entity.Id,
                            Comment = model.Comment,
                            ActionRole = Data.ActionRole.Requester.ToString(),
                        });
                        Db.SaveChanges();

                        if (model.RecoveryType == "Full")
                        {
                            Job.Enqueue<Services.NotificationService>(x => x.NotifyRecoveryTGBSBankingForAcceptFullSubmission(entity.Id)); 
                           // Job.Enqueue<Services.NotificationService>(x => x.NotifyRecoveryTGBSBankingForReceiveFullSubmission(entity.Id));
                        }
                        else if (model.RecoveryType == "FirstPartial")
                        {
                            Job.Enqueue<Services.NotificationService>(x => x.NotifyRecoveryTGBSBankingForAcceptFirstPartial(entity.Id));
                           // Job.Enqueue<Services.NotificationService>(x => x.NotifyRecoveryTGBSBankingForReceiveFirstPartial(entity.Id));
                        }
                        else
                        {
                            Job.Enqueue<Services.NotificationService>(x => x.NotifyRecoveryTGBSBankingForAcceptSecondPartial(entity.Id));
                           // Job.Enqueue<Services.NotificationService>(x => x.NotifyRecoveryTGBSBankingForReceiveSecondPartial(entity.Id));
                        }

                    }

                    if (model.UserAction == "Submit")
                    {
                        return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Recovery Submit Successfull!", id = entity.Id });
                    }
                    else
                    {
                        return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Recovery Saved Successfull!" });
                    }

                }
               
            }
            catch (Exception e)
            {
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });

            }

        }

        [HttpPost]
        public JsonResult UpdateBDRecovery(RecoveryViewModel model, IFormFile BankStatement)
        {
            try
            {
                var user = _userManager.GetUserAsync(HttpContext.User).Result;
                var entity = Db.Recovery.Where(x => x.Id == Guid.Parse(model.Id)).FirstOrDefault();

                if (entity.RecoveryType == "FirstPartial")
                {
                    entity.PartialReceivedDate = model.ReceivedDate;
                    entity.ErmsDocNo1 = model.ErmsDocNo1;
                }
                else
                {
                    entity.ReceivedDate = model.ReceivedDate;
                    entity.ErmsDocNo2 = model.ErmsDocNo2;
                }
              
                entity.UpdatedOn = DateTime.Now;

                var doc = Db.Attachment
                   .Where(x => x.FileType == Data.AttachmentType.Recovery.ToString() && x.FileSubType == Data.BDAttachmentType.BankStatement.ToString() && x.ParentId == entity.Id)
                   .FirstOrDefault();

                var doc1 = Db.Attachment
                       .Where(x => x.FileType == Data.AttachmentType.Recovery.ToString() && x.FileSubType == Data.BDAttachmentType.FirstPartialBankStatement.ToString() && x.ParentId == entity.Id)
                       .FirstOrDefault();

                var doc2 = Db.Attachment
                       .Where(x => x.FileType == Data.AttachmentType.Recovery.ToString() && x.FileSubType == Data.BDAttachmentType.SecondPartialBankStatement.ToString() && x.ParentId == entity.Id)
                       .FirstOrDefault();

                var bankStatement = BankStatement;

                if (bankStatement != null)
                {
                    if(entity.RecoveryType == "Full")
                    {
                        UploadFile(bankStatement, entity.Id, Data.AttachmentType.Recovery.ToString(), Data.BDAttachmentType.BankStatement.ToString());
                        entity.TGBSReceiverId = user.Id;
                    }
                    else if(entity.RecoveryType == "FirstPartial")
                    {
                        UploadFile(bankStatement, entity.Id, Data.AttachmentType.Recovery.ToString(), Data.BDAttachmentType.FirstPartialBankStatement.ToString());
                        entity.TGBSPartialReceiverId = user.Id;
                    }
                    else if (entity.RecoveryType == "SecondPartial")
                    {
                        UploadFile(bankStatement, entity.Id, Data.AttachmentType.Recovery.ToString(), Data.BDAttachmentType.SecondPartialBankStatement.ToString());
                        entity.TGBSReceiverId = user.Id;
                    }
                   
                }

                    if (entity.RecoveryType == "FirstPartial" && (BankStatement != null || doc1 != null) && (model.ReceivedDate != null || entity.PartialReceivedDate != null) && (model.ErmsDocNo1 != null || entity.ErmsDocNo1 != null))
                    {
                        entity.PartialReceivedDate = model.ReceivedDate;
                        entity.PartialReceivedOn = DateTime.Now;
                        entity.TGBSPartialReceiverId = user.Id;
                        entity.ErmsDocNo1 = model.ErmsDocNo1;
                        entity.Status = Data.Status.Received.ToString();
                    }
                    else
                    {
                        if (entity.RecoveryType == "Full" && (BankStatement != null || doc != null) && (model.ReceivedDate != null || entity.ReceivedDate != null) && (model.ErmsDocNo1 != null || entity.ErmsDocNo1 != null))
                        {
                            entity.ReceivedDate = model.ReceivedDate;
                            entity.ReceivedOn = DateTime.Now;
                            entity.TGBSReceiverId = user.Id;
                            entity.Status = Data.Status.Received.ToString();
                           entity.ErmsDocNo1 = model.ErmsDocNo1;
                    }
                        else if (entity.RecoveryType == "SecondPartial" && (BankStatement != null || doc2 != null) && (model.ReceivedDate != null || entity.ReceivedDate != null) && (model.ErmsDocNo1 != null || entity.ErmsDocNo2 != null))
                        {
                            entity.ReceivedDate = model.ReceivedDate;
                            entity.ReceivedOn = DateTime.Now;
                            entity.TGBSReceiverId = user.Id;
                            entity.ErmsDocNo1 = model.ErmsDocNo2;
                            entity.Status = Data.Status.Received.ToString();
                        }

                    }
                

                Db.SetModified(entity);
                Db.SaveChanges();

                return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Recovery Received Saved Successfull!" });

            }
            catch (Exception e)
            {
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });

            }

        }


        [Authorize(Roles = "TGBS Banking, Business Admin")]
        [HttpPost]
        public JsonResult AcceptBDRecovery(RecoveryViewModel model)
        {

            var entity = Db.Recovery.Find(Guid.Parse(model.Id));

            if (entity.Status == Data.Status.Accepted.ToString() || entity.Status == Data.Status.Declined.ToString())
            {
                return Json(new { response = StatusCode(StatusCodes.Status403Forbidden), message = "Bank Draft Recovery accept process has already been done by " + entity.TGBSReceiverId + "!" });
            }
            else
            {
               
                try
                {
                    var user = _userManager.GetUserAsync(HttpContext.User).Result;

                    if(entity.RecoveryType == "Full" || entity.RecoveryType == "SecondPartial")
                    {
                        entity.AcceptedOn = DateTime.Now;
                        entity.TGBSAcceptanceId = user.Id;
                    }
                    else if(entity.RecoveryType == "FirstPartial")
                    {
                        entity.PartialAcceptedOn = DateTime.Now;
                        entity.TGBSPartialAcceptanceId = user.Id;
                    }

                    entity.UpdatedOn = DateTime.Now;
                    entity.Status = model.UserAction;
                    Db.SetModified(entity);
                    Db.SaveChanges();

                    var _BankDraftId = entity.Id;

                        Db.BankDraftAction.Add(new BankDraftAction
                        {
                            ApplicationType = Data.AppType.Recovery.ToString(),
                            ActionType = model.UserAction,
                            On = DateTime.Now,
                            ById = user.Id,
                            ParentId = entity.Id,
                            Comment = model.Comment,
                            ActionRole = Data.ActionRole.TGBSBanking.ToString(),
                        });
                        Db.SaveChanges();

                    if(model.UserAction == "Declined")
                    {
                        Job.Enqueue<Services.NotificationService>(x => x.NotifyRequesterForRecoveryDecline(entity.Id));
                        return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Recovery Decline Successfull!", id = entity.Id });
                    }
                    else
                    { 
                        Job.Enqueue<Services.NotificationService>(x => x.NotifyRecoveryRequesterForAcceptance(entity.Id));

                    if (model.RecoveryType == "Full")
                        {
                            Job.Enqueue<Services.NotificationService>(x => x.NotifyRecoveryTGBSReconForReceiveFullSubmission(entity.Id));
                        }
                        else if (model.RecoveryType == "FirstPartial")
                        {
                            Job.Enqueue<Services.NotificationService>(x => x.NotifyRecoveryTGBSReconForReceiveFirstPartial(entity.Id));
                        }
                        else
                        {
                            Job.Enqueue<Services.NotificationService>(x => x.NotifyRecoveryTGBSReconForReceiveSecondPartial(entity.Id));
                        }

                        return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Recovery Accept Successfull!", id = entity.Id });
                    }
            
                }
                catch (Exception e)
                {
                    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });

                }
            }

        }

        [Authorize(Roles = "TGBS Reconciliation")]
        [HttpPost]
        public JsonResult ReceiveBDRecovery(RecoveryViewModel model, IFormFile BankStatement, IFormFile FirstParBankStatement, IFormFile SecondParBankStatement)
        {
          
            var entity = Db.Recovery.Find(Guid.Parse(model.Id));

            if (entity.Status == Data.Status.Received.ToString())
            {
                return Json(new { response = StatusCode(StatusCodes.Status403Forbidden), message = "Bank Draft Recovery receive process has already been done by " + entity.TGBSReceiverId + "!" });
            }
            else
            {
                if (model.UserAction == "Receive" && BankStatement == null && model.RecoveryType == "Full")
                {
                    return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Bank Statement Required!" });
                }
                else if (model.UserAction == "Receive" && FirstParBankStatement == null && model.RecoveryType == "FirstPartial")
                {
                    return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Bank Statement Required!" });
                }
                else if (model.UserAction == "Receive" && SecondParBankStatement == null && model.RecoveryType == "SecondPartial")
                {
                    return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Bank Statement Required!" });
                }

                try
                {
                    var user = _userManager.GetUserAsync(HttpContext.User).Result;

                    
                    
                    entity.UpdatedOn = DateTime.Now;

                    if (model.UserAction == "Receive")
                    {
                        if (model.RecoveryType == "FirstPartial")
                        {
                            entity.PartialReceivedDate = model.PartialReceivedDate;
                            entity.PartialReceivedOn = DateTime.Now;
                            entity.TGBSPartialReceiverId = user.Id;
                            entity.ErmsDocNo1 = model.ErmsDocNo1;
                            entity.PostingDate1 = model.PostingDate1;
                        }
                        else
                        {
                            if(model.RecoveryType == "Full")
                            {
                                entity.ErmsDocNo1 = model.ErmsDocNo1;
                                entity.PostingDate1 = model.PostingDate1;
                            }
                            else if(model.RecoveryType == "SecondPartial")
                            {
                                entity.ErmsDocNo2 = model.ErmsDocNo2;
                                entity.PostingDate2 = model.PostingDate2;
                            }
                            entity.ReceivedDate = model.ReceivedDate;
                            entity.ReceivedOn = DateTime.Now;
                            entity.TGBSReceiverId = user.Id;
                        }
                        //entity.UpdatedOn = DateTime.Now;
                        entity.Status = Data.Status.Received.ToString();
                    }

                    Db.SetModified(entity);
                    Db.SaveChanges();

                    var _BankDraftId = entity.Id;

                    if (BankStatement != null)
                    {
                        UploadFile(BankStatement, _BankDraftId, Data.AttachmentType.Recovery.ToString(), Data.BDAttachmentType.BankStatement.ToString());
                    }

                    if (FirstParBankStatement != null)
                    {
                        UploadFile(FirstParBankStatement, _BankDraftId, Data.AttachmentType.Recovery.ToString(), Data.BDAttachmentType.FirstPartialBankStatement.ToString());
                    }

                    if (SecondParBankStatement != null)
                    {
                        UploadFile(SecondParBankStatement, _BankDraftId, Data.AttachmentType.Recovery.ToString(), Data.BDAttachmentType.SecondPartialBankStatement.ToString());
                    }

                    if (model.UserAction == "Receive")
                    {
                        Db.BankDraftAction.Add(new BankDraftAction
                        {
                            ApplicationType = Data.AppType.Recovery.ToString(),
                            ActionType = Data.ActionType.Received.ToString(),
                            On = DateTime.Now,
                            ById = user.Id,
                            ParentId = entity.Id,
                            Comment = model.Comment,
                            ActionRole = Data.ActionRole.TGBSReconciliation.ToString(),
                        });
                        Db.SaveChanges();

                        //Job.Enqueue<Services.NotificationService>(x => x.NotifyRecoveryTGBSBankingForConfirmation(entity.Id));

                        if (model.RecoveryType == "Full")
                        {
                            Job.Enqueue<Services.NotificationService>(x => x.NotifyRecoveryTGBSBankingForConfirmFullSubmission(entity.Id));
                        }
                        else if (model.RecoveryType == "FirstPartial")
                        {
                            Job.Enqueue<Services.NotificationService>(x => x.NotifyRecoveryTGBSBankingForConfirmFirstPartial(entity.Id));
                        }
                        else
                        {
                            Job.Enqueue<Services.NotificationService>(x => x.NotifyRecoveryTGBSBankingForConfirmSecondPartial(entity.Id));
                        }


                    }

                    return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Recovery Submitted Successfully!" });
                }
                catch (Exception e)
                {
                    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });

                }
            }
         
        }

        [Authorize(Roles = "TGBS Banking, Business Admin")]
        [HttpPost]
        public JsonResult Complete(RecoveryViewModel model)
        {
            //if (ModelState.IsValid)
            //{
            var entity = Db.Recovery.Find(Guid.Parse(model.Id));

            if (entity.Status == Data.Status.PartialComplete.ToString() && (model.UserAction == "PartialComplete"))
            {
                 return Json(new { response = StatusCode(StatusCodes.Status403Forbidden), message = "Bank Draft Recovery confirmation has already been done by " + entity.TGBSReceiverId + "!" }); 
            }
            else if (entity.Status == Data.Status.Complete.ToString() && (model.UserAction == "Complete"))
            {
                return Json(new { response = StatusCode(StatusCodes.Status403Forbidden), message = "Bank Draft Recovery confirmation has already been done by " + entity.TGBSReceiverId + "!" });
            }
            else
            {

                try
                {
                    var user = _userManager.GetUserAsync(HttpContext.User).Result;

                    entity.TGBSConfirmationComment = model.Comment;
                    entity.UpdatedOn = DateTime.Now;

                    if (model.UserAction == "PartialComplete")
                    {
                        entity.CompletedOn = DateTime.Now;
                        entity.TGBSValidatorId = user.Id;
                        //entity.UpdatedOn = DateTime.Now;
                        entity.Status = Data.Status.PartialComplete.ToString();

                        Db.SetModified(entity);
                        Db.SaveChanges();

                        Job.Enqueue<Services.NotificationService>(x => x.NotifyRecoveryRequesterForAcknowledgement(entity.Id));
                    }
                    else
                    {
                        entity.PartialCompletedOn = DateTime.Now;
                        entity.TGBSValidatorId = user.Id;
                        //entity.UpdatedOn = DateTime.Now;
                        entity.Status = Data.Status.Complete.ToString();
                        Db.SetModified(entity);
                        Db.SaveChanges();

                        Job.Enqueue<Services.NotificationService>(x => x.NotifyRecoveryRequesterForComplete(entity.Id));
                    }

                   

                    if (model.UserAction == "PartialComplete")
                    {
                        Db.BankDraftAction.Add(new BankDraftAction
                        {
                            ApplicationType = Data.AppType.Recovery.ToString(),
                            ActionType = Data.ActionType.PartialComplete.ToString(),
                            On = DateTime.Now,
                            ById = user.Id,
                            ParentId = entity.Id,
                            Comment = model.Comment,
                            ActionRole = Data.ActionRole.TGBSBanking.ToString(),
                        });
                        Db.SaveChanges();

                        //Job.Enqueue<Services.NotificationService>(x => x.NotifyRequesterForBDAcceptance(entity.Id));

                    }
                    else
                    {
                        Db.BankDraftAction.Add(new BankDraftAction
                        {
                            ApplicationType = Data.AppType.Recovery.ToString(),
                            ActionType = Data.ActionType.Complete.ToString(),
                            On = DateTime.Now,
                            ById = user.Id,
                            ParentId = entity.Id,
                            Comment = model.Comment,
                            ActionRole = Data.ActionRole.TGBSBanking.ToString(),
                        });
                        Db.SaveChanges();
                    }

                    //if (model.UserAction == "Complete")
                    //{
                    //    //add  recovery to state report
                    //    var state = Db.BusinessArea.Where(x => x.Name == entity.BA).Select(x => x.StateId).FirstOrDefault();

                    //    var smr = Db.StateMonthlyAmount.Where(x => x.Month == DateTime.Now.Month && x.Year == DateTime.Now.Month && x.StateId == state).FirstOrDefault();
                    //    smr.BDNoRecovered = smr.BDNoRecovered + 1;
                    //    smr.RecoveryAmount = smr.RecoveryAmount + entity.TotalRecoveryAmount;
                    //    smr.OutstandingAmount = smr.Amount - smr.RecoveryAmount;
                    //    smr.BDNoOutstanding = smr.BDNoIssued - smr.BDNoRecovered;

                    //    Db.SetModified(smr);
                    //    Db.SaveChanges();
                    //}   

                    return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Recovery Completed Successfully!" });
                }
                catch (Exception e)
                {
                    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });

                }
            }

        }


        public string GetRunningNo()
        {
            RunningNo runningNo = new RunningNo();

            var entity = Db.RunningNo.Where(x => x.Name == "Recovery").FirstOrDefault(); //Id for Instruction Letter
            runningNo.Code = entity.Code;
            runningNo.RunNo = entity.RunNo;
            string NewCode = String.Format("{0}{1:00000}", runningNo.Code, runningNo.RunNo);

            entity.RunNo = entity.RunNo + 1;
            Db.RunningNo.Update(entity);
            Db.SaveChanges();

            return NewCode;
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

        [HttpGet]
        public JsonResult GetAllRecoveryByStatus(string _Status = null)
        {

            if (_Status == "Accepted")
            {
                var result = Db.Recovery
           .Join(Db.Attachment,
                            r => r.Id,
                            a => a.ParentId,
                            (r, a) => new
                            {
                                id = r.Id,
                                bdRefNo = r.RefNo,
                                bdNo = r.BDNo,
                                projNo = r.ProjNo,
                                ermsDocNo = r.ERMSDocNo,
                                projComDate = r.ProjectCompletionDate.Value.ToShortDateString(),
                                nameOnBD = r.NameOnBD,
                                requester = r.Requester.FullName,
                                compCode = r.CoCode,
                                ba = r.BA,
                                recoveryType = r.RecoveryType,
                                recoveryAmount = r.RecoveryType == "FirstPartial" ? string.Format("{0:C}", r.FirstRecoveryAmount) : r.RecoveryType == "SecondPartial" ? string.Format("{0:C}", r.SecondRecoveryAmount) : string.Format("{0:C}", r.TotalRecoveryAmount),
                                receivedDate = r.RecoveryType == "FirstPartial" ? r.PartialReceivedDate : r.ReceivedDate,
                                status = r.Status,
                                tgbsReceiverId = r.TGBSReceiverId,
                                tgbsPartialReceiverId = r.TGBSPartialReceiverId,
                                ermsDocNo1 = r.RecoveryType == "SecondPartial" ? r.ErmsDocNo2 : r.ErmsDocNo1,
                                bankStatement = a.FileName == null ? null : a.FileName,
                                fileSubType = a.FileSubType
                                /* r.RecoveryType == "FirstPartial" ? "FirstPartialBankStatement" ? r.RecoveryType = "SecondPartial" ? a.FileSubType == "SecondPartialBankStatement" : a.FileSubType == "BankStatement" */
                            })
                .Where(x => (x.status == _Status || _Status == null) && ((x.fileSubType == "BankStatement" && x.recoveryType == "Full" && x.tgbsReceiverId != null) ||
                (x.fileSubType == "FirstPartialBankStatement" && x.recoveryType == "FirstPartial" && x.tgbsPartialReceiverId != null) || (x.fileSubType == "SecondPartialBankStatement" && x.recoveryType == "SecondPartial" && x.tgbsReceiverId != null)))
                        .ToList();

                var result2 = Db.Recovery
                    .Select(r => new {
                              id = r.Id,
                              bdRefNo = r.RefNo,
                              bdNo = r.BDNo,
                              projNo = r.ProjNo,
                              ermsDocNo = r.ERMSDocNo,
                              projComDate = r.ProjectCompletionDate.Value.ToShortDateString(),
                              nameOnBD = r.NameOnBD,
                              requester = r.Requester.FullName,
                              compCode = r.CoCode,
                              ba = r.BA,
                              recoveryType = r.RecoveryType,
                              recoveryAmount = r.RecoveryType == "FirstPartial" ? string.Format("{0:C}", r.FirstRecoveryAmount) : r.RecoveryType == "SecondPartial" ? string.Format("{0:C}", r.SecondRecoveryAmount) : string.Format("{0:C}", r.TotalRecoveryAmount),
                              receivedDate = r.RecoveryType == "FirstPartial" ? r.PartialReceivedDate : r.ReceivedDate,
                              status = r.Status,
                              tgbsReceiverId = r.TGBSReceiverId,
                              tgbsPartialReceiverId = r.TGBSPartialReceiverId,
                              ermsDocNo1 = r.RecoveryType == "SecondPartial" ? r.ErmsDocNo2 : r.ErmsDocNo1,
                              bankStatement = "",
                              fileSubType = ""
                    })
                    //.Where(x => (x.status =*/= _Status || _Status == null))
                    //        .ToList();
                .Where(x => (x.status == _Status || _Status == null) && ((x.recoveryType == "Full" && x.tgbsReceiverId == null) ||
                (x.recoveryType == "FirstPartial" && x.tgbsPartialReceiverId == null) || (x.recoveryType == "SecondPartial" && x.tgbsReceiverId == null)))
                .ToList();

                return new JsonResult(result.Concat(result2).ToList());
            }
            else
            {
                var result = Db.Recovery
                .Join(Db.Attachment,
                            r => r.Id,
                            a => a.ParentId,
                            (r, a) => new
                            {
                                id = r.Id,
                                bdRefNo = r.RefNo,
                                bdNo = r.BDNo,
                                projNo = r.ProjNo,
                                ermsDocNo = r.ERMSDocNo,
                                projComDate = r.ProjectCompletionDate.Value.ToShortDateString(),
                                nameOnBD = r.NameOnBD,
                                requester = r.Requester.FullName,
                                compCode = r.CoCode,
                                ba = r.BA,
                                recoveryType = r.RecoveryType,
                                recoveryAmount = r.RecoveryType == "FirstPartial" ? string.Format("{0:C}", r.FirstRecoveryAmount) : r.RecoveryType == "SecondPartial" ? string.Format("{0:C}", r.SecondRecoveryAmount) : string.Format("{0:C}", r.TotalRecoveryAmount),
                                receivedDate = r.RecoveryType == "FirstPartial" ? r.PartialReceivedDate : r.ReceivedDate,
                                status = r.Status,
                                tgbsReceiverId = r.TGBSReceiverId,
                                tgbsPartialReceiverId = r.TGBSPartialReceiverId,
                                ermsDocNo1 = r.RecoveryType == "SecondPartial" ? r.ErmsDocNo2 : r.ErmsDocNo1,
                                bankStatement = a.FileName == null ? null : a.FileName,
                                fileSubType = a.FileSubType == null ? null : a.FileSubType
                            })
                            .Where(x => (x.status == _Status || _Status == null) && ((x.fileSubType == "BankStatement" && x.recoveryType == "Full") ||
                            (x.fileSubType == "FirstPartialBankStatement" && x.recoveryType == "FirstPartial") || (x.fileSubType == "SecondPartialBankStatement" && x.recoveryType == "SecondPartial")))
                            .ToList();

                return new JsonResult(result.ToList());
            }
        }

        [HttpGet]
        public JsonResult RemoveAttachment(string Id, string type)
        {

            string bt = "";

            try
            {
                if (type == "Full")
                {
                    bt = Data.BDAttachmentType.BankStatement.ToString();
                }
                else if (type == "FirstPartial")
                {
                    bt = Data.BDAttachmentType.FirstPartialBankStatement.ToString();
                }
                else if (type == "SecondPartial")
                {
                    bt = Data.BDAttachmentType.SecondPartialBankStatement.ToString();
                }

                var model = Db.Attachment.Where(x => x.ParentId == Guid.Parse(Id) && x.FileType == "Recovery" && x.FileSubType == bt).FirstOrDefault();
                if (model != null)
                {
                    Db.Remove(model);
                    Db.SaveChanges();

                    var rec = Db.Recovery.Where(x => x.Id == Guid.Parse(Id)).FirstOrDefault();
                    if (type == "Full")
                    {
                        rec.TGBSReceiverId = null;
                    }
                    else if (type == "FirstPartial")
                    {
                        bt = Data.BDAttachmentType.FirstPartialBankStatement.ToString();
                        rec.TGBSPartialReceiverId = null;
                    }
                    else if (type == "SecondPartial")
                    {
                        bt = Data.BDAttachmentType.SecondPartialBankStatement.ToString();
                        rec.TGBSReceiverId = null;
                    }

                    Db.SetModified(rec);
                    Db.SaveChanges();

                }

                return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Attachment Deleted Successful!" });
            }
            catch (Exception e)
            {
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });

            }
        }

        [HttpGet]
        public JsonResult GetAllDuration()
        {
            List<Duration> duration = new List<Duration>()
            {
                //new Duration ()  {Id = 0, Name = "Please Choose" },
                new Duration ()  {Id = 1, Name = "1 month" },
                new Duration ()  { Id = 2, Name = "2 months" },
                new Duration ()  { Id = 3, Name = "3 months" },
                new Duration ()  {Id = 4, Name = "4 months" },
                new Duration ()  { Id = 5, Name = "5 months" },
                new Duration ()  { Id = 6, Name = "6 months" },
                new Duration ()  {Id = 7, Name = "7 months" },
                new Duration ()  { Id = 8, Name = "8 months" },
                new Duration ()  { Id = 9, Name = "9 months" },
                new Duration ()  {Id = 10, Name = "10 months" },
                new Duration ()  { Id = 11, Name = "11 months" },
                new Duration ()  { Id = 12, Name = "12 months" },
                new Duration ()  {Id = 13, Name = "13 months" },
                new Duration ()  { Id = 14, Name = "14 months" },
                new Duration ()  { Id = 15, Name = "15 months" },
                new Duration ()  {Id = 16, Name = "16 months" },
                new Duration ()  { Id = 17, Name = "17 months" },
                new Duration ()  { Id = 18, Name = "18 months" },
                new Duration ()  {Id = 19, Name = "19 months" },
                new Duration ()  { Id = 20, Name = "20 months" },
                new Duration ()  { Id = 21, Name = "21 months" },
                new Duration ()  {Id = 22, Name = "22 months" },
                new Duration ()  { Id = 23, Name = "23 months" },
                new Duration ()  { Id = 24, Name = "24 months" },
            };

            var result = duration.Select(s => new
            {
                Id = s.Id,
                Name = s.Name
            });

            return new JsonResult(result.ToList());
        }

        [HttpGet]
        public JsonResult GetAllRecoveryType()
        {
            List<RecoveryType> recoveryType = new List<RecoveryType>()
            {
                new RecoveryType ()  { Type = "Partial", Name = "Partial Refund" },
                new RecoveryType ()  { Type = "Full", Name = "Full Refund" },
            };

            var result = recoveryType.Select(s => new
            {
                Type = s.Type,
                Name = s.Name
            });

            return new JsonResult(result.ToList());
        }

        public IActionResult Process()
        {
            return View();
        }
        public IActionResult Submit()
        {
            return View();
        }
        public IActionResult Receive()
        {
            return View();
        }
        public IActionResult Complete()
        {
            return View();
        }
        public IActionResult AllComplete()
        {
            return View();
        }
        public IActionResult BulkBankDraft()
        {
            return View();
        }
        public IActionResult BulkBankDraft1()
        {
            return View();
        }
        public IActionResult Create2()
        {
            return View();
        }
        public IActionResult _ActionButton()
        {
            return View();
        }
        public IActionResult _dateVisitCPC()
        {
            return View();
        }
        public IActionResult _CreateDetails()
        {
            return View();
        }
        public IActionResult _ProcessDetails()
        {
            return View();
        }
        public IActionResult _SubmitDetails()
        {
            return View();
        }
        public IActionResult _ReceiveDetails()
        {
            return View();
        }
        public IActionResult _CompleteDetails()
        {
            return View();
        }
        public IActionResult _StatusPendingAction()
        {
            return View();
        }
        public IActionResult _ActionHistory()
        {
            return View();
        }
        public IActionResult _Comments()
        {
            return View();
        }
    }
}