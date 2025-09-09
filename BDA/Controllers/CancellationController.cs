using System;
using System.Collections.Generic;
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
using Rotativa.AspNetCore;

namespace BDA.Web.Controllers
{
    class BankProcessType
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }

    [Authorize]
    public class CancellationController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public CancellationController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult BankDraftList()
        {
            return View();
        }


        [Authorize(Roles = "Executive, Manager, Head of Zone, Senior Manager")]
        public IActionResult Create()
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            ViewBag.Fullname = user.FullName;
            return View();
        }

        [Authorize(Roles = "Executive, Manager, Head of Zone, Senior Manager")]
        [HttpPost]
        public JsonResult Create(CancellationViewModel model, IFormFile ScannedBankDraft, IFormFile ScannedMemo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //if (model.Status == "Submit" && ScannedBankDraft == null)
                    //{
                    //    return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Scanned Bank Draft Required!" });
                    //}
                    //else if (model.Status == "Submit" && ScannedMemo == null)
                    //{
                    //    return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Scanned Memo Required!" });
                    //}

                    var user = _userManager.GetUserAsync(HttpContext.User).Result;

                    Cancellation entity = new Cancellation();
                    entity.CreatedById = user.Id;
                    entity.CreatedByName = user.FullName;
                    entity.RequesterId = user.Id;
                    entity.ApproverId = model.ApproverId;
                    entity.DraftedOn = DateTime.Now;
                    entity.BankDraftId = Guid.Parse(model.BankDraftId);
                    entity.SubmittedOn = model.Status == "Submit" ? DateTime.Now : (DateTime?)null;
                    entity.BDNo = model.BDNo;
                    entity.BDRequesterName = model.BDRequesterName;
                    entity.BDAmount = model.BDAmount;
                    entity.ReasonCancel = model.ReasonCancel;
                    entity.OthersRemark = model.OthersRemark;
                    entity.RefNo = model.RefNo;
                    entity.Status = model.Status == "Submit" ? Data.Status.Submitted.ToString() : Data.Status.Draft.ToString();
                    entity.BDRequesterName = model.BDRequesterName;
                    entity.ERMSDocNo = model.ERMSDocNo;
                    entity.CoCode = model.CoCode;
                    entity.BA = model.BA;
                    entity.NameOnBD = model.NameOnBD;
                    entity.ProjectNo = model.ProjectNo;

                    Db.Cancellation.Add(entity);
                    Db.SaveChanges();

                    var bd = Db.BankDraft.Where(x => x.Id == entity.BankDraftId).FirstOrDefault();
                    bd.FinalApplication = "Cancellation";

                    Db.SetModified(bd);
                    Db.SaveChanges();

                    if (entity.Status == "Submitted")
                    {
                        Db.BankDraftAction.Add(new BankDraftAction
                        {
                            ApplicationType = Data.AppType.Cancellation.ToString(),
                            ActionType = Data.ActionType.Submitted.ToString(),
                            On = DateTime.Now,
                            ById = user.Id,
                            ParentId = entity.Id,
                            ActionRole = Data.ActionRole.Requester.ToString(),
                            Comment = model.Comment,
                        });
                        Db.SaveChanges();

                        //email notification to Approver for Cancellation
                        Job.Enqueue<Services.NotificationService>(x => x.NotifyCancellationApproverForApproval(entity.Id));
                    }

                    if (ScannedBankDraft != null)
                    {
                        UploadFile(ScannedBankDraft, entity.Id, Data.AttachmentType.Cancellation.ToString(), Data.BDAttachmentType.ScannedBankDraft.ToString());
                    }

                    if (ScannedMemo != null)
                    {
                        UploadFile(ScannedMemo, entity.Id, Data.AttachmentType.Cancellation.ToString(), Data.BDAttachmentType.ScannedMemo.ToString());
                    }

                    return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Cancellation Saved Successful!" });
                }
                catch (Exception e)
                {
                    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });

                }
            }
            else
            {
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Error! Please try again later." });
            }
        }

        [HttpGet]
        public IActionResult Edit(string Id)
        {
            var model = new CancellationViewModel();
            var item = Db.Cancellation.Where(x => x.Id == Guid.Parse(Id)).FirstOrDefault();
            if (item != null)
            {
                //Declare Bank Draft Cancellation Details
                model.Id = item.Id.ToString();
                model.RefNo = item.RefNo;
                model.ApproverId = item.ApproverId;
                model.RequesterId = item.RequesterId;
                model.BDNo = item.BDNo;
                model.BDAmount = item.BDAmount;
                model.Status = item.Status;
                model.InstructionLetterRefNo = item.InstructionLetterRefNo;
                model.BDRequesterName = item.BDRequesterName;
                model.ERMSDocNo = item.ERMSDocNo;
                model.CoCode = item.CoCode;
                model.BA = item.BA;
                model.NameOnBD = item.NameOnBD;
                model.BankProcessType = item.BankProcessType;
                model.ProjectNo = item.ProjectNo;
                model.ReasonCancel = item.ReasonCancel;
                model.OthersRemark = item.OthersRemark;
                model.ReceivedDate = item.ReceivedDate;

                ViewBag.ApproverId = Db.Users.Find(model.ApproverId) != null ? Db.Users.Find(model.ApproverId).FullName : "";

                model.ScannedBankDraftVM= new AttachmentViewModel();
                model.ScannedMemoVM = new AttachmentViewModel();
                model.SignedLetterVM = new AttachmentViewModel();
                model.UMAPinFormVM = new AttachmentViewModel();
                model.ScannedLetterVM = new AttachmentViewModel();
                model.BankStatementVM = new AttachmentViewModel();

                var _scannedBd = Db.Attachment.Where(x => x.ParentId == item.Id && x.FileType == Data.AttachmentType.Cancellation.ToString() && x.FileSubType == Data.BDAttachmentType.ScannedBankDraft.ToString()).FirstOrDefault();
                if (_scannedBd != null)
                {
                    model.ScannedBankDraftVM.Id = _scannedBd.Id.ToString();
                    model.ScannedBankDraftVM.FileName = _scannedBd.FileName;
                }

                var _scannedMemo = Db.Attachment.Where(x => x.ParentId == item.Id && x.FileType == Data.AttachmentType.Cancellation.ToString() && x.FileSubType == Data.BDAttachmentType.ScannedMemo.ToString()).FirstOrDefault();
                if (_scannedMemo != null)
                {
                    model.ScannedMemoVM.Id = _scannedMemo.Id.ToString();
                    model.ScannedMemoVM.FileName = _scannedMemo.FileName;
                }

                var _signedLetter = Db.Attachment.Where(x => x.ParentId == item.Id && x.FileType == Data.AttachmentType.InstructionLetter.ToString() && x.FileSubType == Data.BDAttachmentType.SignedLetter.ToString()).FirstOrDefault();
                if (_signedLetter != null)
                {
                    model.SignedLetterVM.Id = _signedLetter.Id.ToString();
                    model.SignedLetterVM.FileName = _signedLetter.FileName;
                }

                var _umaPinForm = Db.Attachment.Where(x => x.ParentId == item.Id && x.FileType == Data.AttachmentType.Cancellation.ToString() && x.FileSubType == Data.BDAttachmentType.UMAPinForm.ToString()).FirstOrDefault();
                if (_umaPinForm != null)
                {
                    model.UMAPinFormVM.Id = _umaPinForm.Id.ToString();
                    model.UMAPinFormVM.FileName = _umaPinForm.FileName;
                }

                var _scannedLetter = Db.Attachment.Where(x => x.ParentId == item.Id && x.FileType == Data.AttachmentType.Cancellation.ToString() && x.FileSubType == Data.BDAttachmentType.ScannedLetter.ToString()).FirstOrDefault();
                if (_scannedLetter != null)
                {
                    model.ScannedLetterVM.Id = _scannedLetter.Id.ToString();
                    model.ScannedLetterVM.FileName = _scannedLetter.FileName;
                }

                var _statementBank = Db.Attachment.Where(x => x.ParentId == item.Id && x.FileType == Data.AttachmentType.Cancellation.ToString() && x.FileSubType == Data.BDAttachmentType.BankStatement.ToString()).FirstOrDefault();
                if (_statementBank != null)
                {
                    model.BankStatementVM.Id = _statementBank.Id.ToString();
                    model.BankStatementVM.FileName = _statementBank.FileName;
                }

                return View(model);

              
            }
            return View();
        }

        [HttpPost]
        public JsonResult Edit(CancellationViewModel model, IFormFile ScannedBankDraft, IFormFile ScannedMemo)
        {
            bool checkFirstTimeSubmit = true;

            //if (ModelState.IsValid)
            //{
                try
                {
                    var _scannedBD = Db.Attachment.Where(x => x.ParentId == Guid.Parse(model.Id) && x.FileType == Data.AttachmentType.Cancellation.ToString() && x.FileSubType == Data.BDAttachmentType.ScannedBankDraft.ToString()).FirstOrDefault();
                    if(_scannedBD == null)
                    {
                        if (model.Status == "Submit" && ScannedBankDraft == null)
                        {
                            return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Scanned Bank Draft Required!" });
                        }
                    }


                    var user = _userManager.GetUserAsync(HttpContext.User).Result;

                    var entity = Db.Cancellation.Find(Guid.Parse(model.Id));

                if (model.BankDraftId != null)
                {
                    if (entity.BankDraftId != Guid.Parse(model.BankDraftId))
                    {

                        var bd = Db.BankDraft.Where(x => x.Id == Guid.Parse(model.BankDraftId)).FirstOrDefault();
                        bd.FinalApplication = "Cancellation";
                        Db.SetModified(bd);

                        var bd2 = Db.BankDraft.Where(x => x.Id == entity.BankDraftId).FirstOrDefault();
                        bd2.FinalApplication = "Application";

                        Db.SetModified(bd2);
                        entity.BankDraftId = Guid.Parse(model.BankDraftId);
                    }
                }

                if (entity.Status == "Rejected" || entity.Status == "Declined")
                {
                    //Checking for project availability again
                    if (model.BankDraftId == null)
                    {
                        var bd = Db.BankDraft.Where(x => x.Id == entity.BankDraftId).FirstOrDefault();
                        if(bd.FinalApplication == "Cancellation" || bd.FinalApplication == "Lost" || bd.FinalApplication == "Recovery")
                        {
                            return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Existing Project No/BD No has been used for other application ! Please use other ProjectNo/BD no to proceed." });
                        }
                        else
                        {
                            bd.FinalApplication = "Cancellation";
                        }
                        Db.SetModified(bd);
                    }
                    checkFirstTimeSubmit = false;
                }

                entity.UpdatedOn = DateTime.Now;
                  
                    entity.ApproverId = model.ApproverId == "1" ? null : model.ApproverId;
                    entity.SubmittedOn = model.Status == "Submit" ? DateTime.Now : (DateTime?)null;
                    entity.BDNo = model.BDNo;
                    entity.BDRequesterName = model.BDRequesterName;
                    entity.BDAmount = model.BDAmount;
                    entity.ReasonCancel = model.ReasonCancel;
                    entity.OthersRemark = model.OthersRemark;
                    entity.RefNo = model.RefNo;
                    entity.Status = model.Status == "Submit" ? Data.Status.Submitted.ToString() : Data.Status.Draft.ToString();
                    entity.BDRequesterName = model.BDRequesterName;
                    entity.ERMSDocNo = model.ERMSDocNo;
                    entity.CoCode = model.CoCode;
                    entity.BA = model.BA;
                    entity.NameOnBD = model.NameOnBD;
                    entity.ProjectNo = model.ProjectNo;

                   
                    entity.Status = model.Status == "Submit" ? Data.Status.Submitted.ToString() : Data.Status.Draft.ToString();

                    Db.SetModified(entity);
                    Db.SaveChanges();

                    if (ScannedBankDraft != null)
                    {
                        UploadFile(ScannedBankDraft, entity.Id , Data.AttachmentType.Cancellation.ToString(), Data.BDAttachmentType.ScannedBankDraft.ToString());
                    }

                    if (ScannedMemo != null)
                    {
                        UploadFile(ScannedMemo, entity.Id, Data.AttachmentType.Cancellation.ToString(), Data.BDAttachmentType.ScannedMemo.ToString());
                    }

                    if (model.Status == "Submit")
                    {
                        Db.BankDraftAction.Add(new BankDraftAction
                        {
                            ApplicationType = Data.AppType.Cancellation.ToString(),
                            ActionType = Data.ActionType.Submitted.ToString(),
                            On = DateTime.Now,
                            ById = user.Id,
                            ParentId = entity.Id,
                            Comment = model.Comment,
                            ActionRole = Data.ActionRole.Requester.ToString(),
                        });
                        Db.SaveChanges();

                        Job.Enqueue<Services.NotificationService>(x => x.NotifyCancellationApproverForApproval(entity.Id));
                    }

                    return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Application Saved Successful!" });
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
        public JsonResult NextAction(BankDraftViewModel model)
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
                    case "Approve":
                        model.Status = Data.Status.Approved.ToString();
                        break;
                    case "RejectApprove":
                        model.Status = Data.Status.Rejected.ToString();
                        break;
                    case "Accept":
                        model.Status = Data.Status.Accepted.ToString();
                        break;
                    case "Decline":
                        model.Status = Data.Status.Declined.ToString();
                        break;
                    default:
                        model.Status = "Invalid";
                        break;
                }

                if (model.Status != "Invalid")
                {
                    var entity = Db.Cancellation.Find(Guid.Parse(model.Id));
                    entity.UpdatedOn = DateTime.Now;

                    if (model.Status == Data.Status.Withdrawn.ToString())
                    {
                        entity.Status = model.Status;
                        entity.WithdrewOn = DateTime.Now;

                        Db.SetModified(entity);
                        Db.SaveChanges();

                        //audit trail for Withdrawn
                        actionType = Data.ActionType.Withdrawn.ToString();
                        byId = model.RequesterId;
                        actionRole = Data.ActionRole.Requester.ToString();

                        Job.Enqueue<Services.NotificationService>(x => x.NotifyCancellationApproverForWithdrawn(entity.Id));
                    }
                    else if (model.Status == Data.Status.Approved.ToString())
                    {
                        entity.Status = model.Status;
                        entity.ApprovedOn = DateTime.Now;
                        entity.ApproverComment = model.Comment;
                        entity.ApproverId = model.ApproverId;

                        Db.SetModified(entity);
                        Db.SaveChanges();

                        //audit trail for Approved
                        actionType = Data.ActionType.Approved.ToString();
                        byId = model.ApproverId;
                        actionRole = Data.ActionRole.Approver.ToString();

                        Job.Enqueue<Services.NotificationService>(x => x.NotifyCancellationTGBSBankingForAcceptance(entity.Id));
                        Job.Enqueue<Services.NotificationService>(x => x.NotifyCancellationRequesterForApproval(entity.Id));

                    }
                    else if (model.Status == Data.Status.Rejected.ToString())
                    {
                        entity.Status = model.Status;
                        entity.ApprovedOn = DateTime.Now;
                        entity.ApproverComment = model.Comment;
                        entity.ApproverId = model.ApproverId;

                        Db.SetModified(entity);
                        Db.SaveChanges();

                        //audit trail for Approved
                        actionType = Data.ActionType.Rejected.ToString();
                        byId = model.ApproverId;
                        actionRole = Data.ActionRole.Approver.ToString();

                        var bd2 = Db.BankDraft.Where(x => x.Id == entity.BankDraftId).FirstOrDefault();
                        bd2.FinalApplication = "Application";
                        Db.SetModified(bd2);

                        Job.Enqueue<Services.NotificationService>(x => x.NotifyCancellationRequesterForApprovalRejection(entity.Id));

                    }
                    else if (model.Status == Data.Status.Accepted.ToString())
                    {
                        if (entity.Status == "Accepted" || entity.Status == "Declined")
                        {
                            return Json(new { response = StatusCode(StatusCodes.Status403Forbidden), message = "Bank Draft Application has already been " + entity.Status.ToLower() + " by " + entity.TGBSAcceptanceId + "!" });
                        }
                        else
                        {
                            entity.Status = model.Status;
                            entity.TGBSAcceptedOn = DateTime.Now;
                            entity.TGBSAcceptedComment = model.Comment;
                            entity.TGBSAcceptanceId = user.Id;

                            Db.SetModified(entity);
                            Db.SaveChanges();

                            //audit trail for Accepted
                            actionType = Data.ActionType.Accepted.ToString();
                            byId = user.Id;
                            actionRole = Data.ActionRole.TGBSBanking.ToString();

                            //Job.Enqueue<Services.NotificationService>(x => x.NotifyCancellationTGBSBankingForAcceptance(entity.Id));
                            Job.Enqueue<Services.NotificationService>(x => x.NotifyCancellationRequesterForProcess(entity.Id));
                        }
                    }
                    else if (model.Status == Data.Status.Declined.ToString())
                    {
                        if (entity.Status == "Accepted" || entity.Status == "Declined")
                        {
                            return Json(new { response = StatusCode(StatusCodes.Status403Forbidden), message = "Bank Draft Application has already been " + entity.Status.ToLower() + " by " + entity.TGBSAcceptanceId + "!" });
                        }
                        else
                        {
                            entity.Status = model.Status;
                            entity.TGBSAcceptedComment = model.Comment;
                            entity.TGBSAcceptanceId = user.Id;

                            Db.SetModified(entity);
                            Db.SaveChanges();

                            //audit trail for Accepted
                            actionType = Data.ActionType.Declined.ToString();
                            byId = user.Id;
                            actionRole = Data.ActionRole.TGBSBanking.ToString();

                            var bd2 = Db.BankDraft.Where(x => x.Id == entity.BankDraftId).FirstOrDefault();
                            bd2.FinalApplication = "Application";
                            Db.SetModified(bd2);

                            Job.Enqueue<Services.NotificationService>(x => x.NotifyCancellationRequesterForAcceptanceRejection(entity.Id));
                        }
                    }
                    //Db.SetModified(entity);
                    //Db.SaveChanges();

                    //Update audit trail

                    Db.BankDraftAction.Add(new BankDraftAction
                    {
                        ApplicationType = Data.AppType.Cancellation.ToString(),
                        ActionType = actionType,
                        On = DateTime.Now,
                        ById = byId,
                        ParentId = Guid.Parse(model.Id),
                        ActionRole = actionRole,
                        Comment = model.Comment,
                    });
                    Db.SaveChanges();

                    return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Cancellation Updated Successfully!", id = entity.Id });
                }

                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Error! Invalid Action " + model.UserAction });
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

        [Authorize(Roles = "TGBS Banking, Business Admin")]
        [HttpPost]
        public JsonResult SubmitToBank(CancellationViewModel model, IFormFile SignedLetter, IFormFile UMAPinForm, IFormFile ScannedLetter)
        {
           
            var entity = Db.Cancellation.Find(Guid.Parse(model.Id));

            if (entity.Status == Data.Status.Processed.ToString())
            {
                return Json(new { response = StatusCode(StatusCodes.Status403Forbidden), message = "Bank Draft Application has already been processed by " + entity.TGBSProcesserId + "!" });
            }
            else
            {
                try
                {
                    var file = Db.Attachment.Where(x => x.ParentId == Guid.Parse(model.Id) && x.FileType == Data.AttachmentType.InstructionLetter.ToString() && x.FileSubType == Data.BDAttachmentType.SignedLetter.ToString()).FirstOrDefault();
                    if (model.UserAction == "SubmitToBank" && (SignedLetter == null && file == null) && model.BankProcessType == "1")
                    {
                        return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Signed Document Required!" });
                    }

                    var file1 = Db.Attachment.Where(x => x.ParentId == Guid.Parse(model.Id) && x.FileType == Data.AttachmentType.Cancellation.ToString() && x.FileSubType == Data.BDAttachmentType.UMAPinForm.ToString()).FirstOrDefault();
                    if (model.UserAction == "SubmitToBank" && UMAPinForm == null && file1 == null && model.BankProcessType == "2")
                    {
                        return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "UMAPinForm Document Required!" });
                    }

                    var file2 = Db.Attachment.Where(x => x.ParentId == Guid.Parse(model.Id) && x.FileType == Data.AttachmentType.Cancellation.ToString() && x.FileSubType == Data.BDAttachmentType.ScannedLetter.ToString()).FirstOrDefault();
                    if (model.UserAction == "SubmitToBank" && ScannedLetter == null && file2 == null && model.BankProcessType == "2")
                    {
                        return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Instruction Letter Document Required!" });
                    }

                    var user = _userManager.GetUserAsync(HttpContext.User).Result;

                    entity.InstructionLetterRefNo = model.InstructionLetterRefNo;
                    entity.UpdatedOn = DateTime.Now;
                    entity.BankProcessType = model.BankProcessType;
                    if (model.UserAction == "SubmitToBank")
                    {
                        entity.TGBSProcessedOn = DateTime.Now;
                        entity.TGBSProcesserId = user.Id;
                        entity.Status = Data.Status.Processed.ToString();
                    }

                    Db.SetModified(entity);
                    Db.SaveChanges();

                    var _BankDraftId = entity.Id;

                    if (SignedLetter != null)
                    {
                        UploadFile(SignedLetter, _BankDraftId, Data.AttachmentType.InstructionLetter.ToString(), Data.BDAttachmentType.SignedLetter.ToString());
                    }

                    if (UMAPinForm != null)
                    {
                        UploadFile(UMAPinForm, _BankDraftId, Data.AttachmentType.Cancellation.ToString(), Data.BDAttachmentType.UMAPinForm.ToString());
                    }

                    if (ScannedLetter != null)
                    {
                        UploadFile(ScannedLetter, _BankDraftId, Data.AttachmentType.Cancellation.ToString(), Data.BDAttachmentType.ScannedLetter.ToString());
                    }

                    if (model.UserAction == "SubmitToBank")
                    {
                        if(entity.BankProcessType == "1")
                        {
                            var letter = Db.InstructionLetter.Where(x => x.LetterRefNo == model.InstructionLetterRefNo && x.ApplicationType == "Cancellation").FirstOrDefault();
                            letter.Status = Data.Status.Processed.ToString();
                            letter.UpdatedOn = DateTime.Now;
                            Db.SetModified(letter);
                            Db.SaveChanges();

                            Job.Enqueue<Services.NotificationService>(x => x.NotifyBankForProcessingForCancellationForMaybank(letter.Id, entity.BankProcessType));
                        }
                        else if(entity.BankProcessType == "2")
                        {
                            Job.Enqueue<Services.NotificationService>(x => x.NotifyBankForProcessingForCancellationForUMA(entity.Id, entity.BankProcessType));
                        }

                        Db.BankDraftAction.Add(new BankDraftAction
                        {
                            ApplicationType = Data.AppType.Cancellation.ToString(),
                            ActionType = Data.ActionType.SubmittedToBank.ToString(),
                            On = DateTime.Now,
                            ById = user.Id,
                            ParentId = entity.Id,
                            Comment = model.Comment,
                            ActionRole = Data.ActionRole.TGBSBanking.ToString(),
                        });
                        Db.SaveChanges();

                        
                        Job.Enqueue<Services.NotificationService>(x => x.NotifyCancellationTGBSReconForReceive(entity.Id));
                    }
                    return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Application Saved Successful!" });
                }
                catch (Exception e)
                {
                    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });

                }
            }
           
        }


        [Authorize(Roles = "TGBS Reconciliation")]
        [HttpPost]
        public JsonResult ReceiveBDCancellation(CancellationViewModel model, IFormFile BankStatement)
        {
            //if (ModelState.IsValid)
            //{
            var entity = Db.Cancellation.Find(Guid.Parse(model.Id));

            if (entity.Status == Data.Status.Received.ToString())
            {
                return Json(new { response = StatusCode(StatusCodes.Status403Forbidden), message = "Bank Draft Application has already been processed by " + entity.TGBSReceiverId + "!" });
            }
            else
            {

                try
                {
                    //var file = Db.Attachment.Where(x => x.ParentId == Guid.Parse(model.Id) && x.FileType == Data.AttachmentType.Memo.ToString()).FirstOrDefault();
                    if (model.UserAction == "ReceiveBDCancellation" && BankStatement == null)
                    {
                        return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Bank Statement Required!" });
                    }

                    var user = _userManager.GetUserAsync(HttpContext.User).Result;

                    entity.ReceivedDate = model.ReceivedDate;
                    entity.UpdatedOn = DateTime.Now;

                    if (model.UserAction == "ReceiveBDCancellation")
                    {
                        entity.ReceivedOn= DateTime.Now;
                        entity.TGBSReceiverId= user.Id;
                        //entity.UpdatedOn = DateTime.Now;
                        entity.Status = Data.Status.Received.ToString();
                    }

                    Db.SetModified(entity);
                    Db.SaveChanges();

                    var _BankDraftId = entity.Id;

                    if (BankStatement != null)
                    {
                        UploadFile(BankStatement, _BankDraftId, Data.AttachmentType.Cancellation.ToString(), Data.BDAttachmentType.BankStatement.ToString());
                    }

                    if (model.UserAction == "ReceiveBDCancellation")
                    {
                       
                        Db.BankDraftAction.Add(new BankDraftAction
                        {
                            ApplicationType = Data.AppType.Cancellation.ToString(),
                            ActionType = Data.ActionType.Received.ToString(),
                            On = DateTime.Now,
                            ById = user.Id,
                            ParentId = entity.Id,
                            Comment = model.Comment,
                            ActionRole = Data.ActionRole.TGBSBanking.ToString(),
                        });
                        Db.SaveChanges();

                        Job.Enqueue<Services.NotificationService>(x => x.NotifyCancellationTGBSBankingForConfirmation(entity.Id));

                    }

                    return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Cancellation Saved Successfully!" });
                }
                catch (Exception e)
                {
                    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });

                }
            }
            //}
            //else
            //{
            //    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Error! Please try again later." });
            //}
        }

        [HttpPost]
        public JsonResult UpdateBDCancellation(CancellationViewModel model, IFormFile BankStatement)
        {   
                try
                {
                var user = _userManager.GetUserAsync(HttpContext.User).Result;
                var entity = Db.Cancellation.Where(x => x.Id == Guid.Parse(model.Id)).FirstOrDefault();

                entity.ReceivedDate = model.ReceivedDate;
                entity.UpdatedOn = DateTime.Now;

                var doc = Db.Attachment
                       .Where(x => x.FileType == "Cancellation" && x.FileSubType == "BankStatement" && x.ParentId == entity.Id)
                       .FirstOrDefault();

                var bankStatement = BankStatement;

                if (bankStatement != null)
                {
                        UploadFile(bankStatement, entity.Id, Data.AttachmentType.Cancellation.ToString(), Data.BDAttachmentType.BankStatement.ToString());
                        entity.TGBSReceiverId = user.Id;
                }

                if ((model.ReceivedDate != null || entity.ReceivedDate != null) && (BankStatement != null || doc!= null ))
                {
                    entity.Status = Data.Status.Received.ToString();
                    entity.ReceivedOn = DateTime.Now;
                    entity.TGBSReceiverId = user.Id;

                    Db.BankDraftAction.Add(new BankDraftAction
                    {
                        ApplicationType = Data.AppType.Cancellation.ToString(),
                        ActionType = Data.ActionType.Received.ToString(),
                        On = DateTime.Now,
                        ById = user.Id,
                        ParentId = entity.Id,
                        Comment = model.Comment,
                        ActionRole = Data.ActionRole.TGBSBanking.ToString(),
                    });
                    Db.SaveChanges();

                    Job.Enqueue<Services.NotificationService>(x => x.NotifyCancellationTGBSBankingForConfirmation(entity.Id));
                }

                Db.SetModified(entity);
                Db.SaveChanges();

                return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Application Saved Successful!" });

                }
                catch (Exception e)
                {
                    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });

                }
 
        }

        [Authorize(Roles = "TGBS Banking, Business Admin")]
        [HttpPost]
        public JsonResult ConfirmBDCancellation(CancellationViewModel model)
        {
            //if (ModelState.IsValid)
            //{
            var entity = Db.Cancellation.Find(Guid.Parse(model.Id));

            if (entity.Status == Data.Status.Complete.ToString())
            {
                return Json(new { response = StatusCode(StatusCodes.Status403Forbidden), message = "Bank Draft Cancellation completion has already been done by " + entity.TGBSReceiverId + "!" });
            }
            else
            {

                try
                {
                    var user = _userManager.GetUserAsync(HttpContext.User).Result;

                    entity.TGBSConfirmationComment = model.Comment;
                    entity.UpdatedOn = DateTime.Now;

                    if (model.UserAction == "Complete")
                    {
                        entity.CompletedOn = DateTime.Now;
                        entity.TGBSValidatorId = user.Id;
                        //entity.UpdatedOn = DateTime.Now;
                        entity.Status = Data.Status.Complete.ToString();
                    }

                    Db.SetModified(entity);
                    Db.SaveChanges();

                    if (model.UserAction == "Complete")
                    {

                        Db.BankDraftAction.Add(new BankDraftAction
                        {
                            ApplicationType = Data.AppType.Cancellation.ToString(),
                            ActionType = Data.ActionType.Complete.ToString(),
                            On = DateTime.Now,
                            ById = user.Id,
                            ParentId = entity.Id,
                            Comment = model.Comment,
                            ActionRole = Data.ActionRole.TGBSBanking.ToString(),
                        });
                        Db.SaveChanges();

                        Job.Enqueue<Services.NotificationService>(x => x.NotifyCancellationRequesterForComplete(entity.Id));
                    }

                        //    //Job.Enqueue<Services.NotificationService>(x => x.NotifyRequesterForBDAcceptance(entity.Id));

                        //}

                        return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Cancellation Saved Successfully!" });
                }
                catch (Exception e)
                {
                    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });

                }
            }

        }

        public void UploadFile(IFormFile file, Guid parentId, string fileType, string fileSubType = null)
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
            attachement.CreatedOn = DateTime.Now;
            attachement.UpdatedOn = DateTime.Now;
            attachement.IsActive = true;

            Db.Attachment.Add(attachement);
            Db.SaveChanges();
        }

        public string GetRunningNo()
        {
            RunningNo runningNo = new RunningNo();

            var entity = Db.RunningNo.Where(x => x.Name == "Cancellation").FirstOrDefault(); //Id for Instruction Letter
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

        [HttpGet]
        public JsonResult GetAllCancellationByStatus(string _Status = null)
        {
            if (_Status == "Processed")
            {
                var result = Db.Cancellation
           .Join(Db.Attachment,
                            c => c.Id,
                            a => a.ParentId,
                            (c, a) => new
                            {
                                id = c.Id,
                                bdRefNo = c.RefNo,
                                bdNo = c.BDNo,
                                projNo = c.ProjectNo,
                                nameOnBD = c.NameOnBD,
                                requester = c.Requester.FullName,
                                compCode = c.CoCode,
                                ba = c.BA,
                                bdAmount = string.Format("{0:C}", c.BDAmount),
                                receivedDate = c.ReceivedDate,
                                status = c.Status,
                                tgbsReceiverId = c.TGBSReceiverId,
                                bankStatement = a.FileName == null ? null : a.FileName,
                                fileSubType = a.FileSubType == null ? null : a.FileSubType
                            })
          .Where(x =>
           (x.status == _Status || _Status == null) && (x.fileSubType == "BankStatement") && x.tgbsReceiverId != null)
                  .ToList();

                var result2 = Db.Cancellation
                .Select(c => new
                {
                    id = c.Id,
                    bdRefNo = c.RefNo,
                    bdNo = c.BDNo,
                    projNo = c.ProjectNo,
                    nameOnBD = c.NameOnBD,
                    requester = c.Requester.FullName,
                    compCode = c.CoCode,
                    ba = c.BA,
                    bdAmount = string.Format("{0:C}", c.BDAmount),
                    receivedDate = c.ReceivedDate,
                    status = c.Status,
                    tgbsReceiverId = c.TGBSReceiverId,
                    bankStatement = "",
                    fileSubType = ""
                })
                .Where(x => (x.status == _Status || _Status == null) && x.tgbsReceiverId == null)
                .ToList();

                return new JsonResult(result.Concat(result2).ToList());
            }
            else
            {
                var result = Db.Cancellation
                .Join(Db.Attachment,
                            c => c.Id,
                            a => a.ParentId,
                            (c, a) => new
                            {
                                id = c.Id,
                                bdRefNo = c.RefNo,
                                bdNo = c.BDNo,
                                projNo = c.ProjectNo,
                                nameOnBD = c.NameOnBD,
                                requester = c.Requester.FullName,
                                compCode = c.CoCode,
                                ba = c.BA,
                                bdAmount = string.Format("{0:C}", c.BDAmount),
                                receivedDate = c.ReceivedDate,
                                status = c.Status,
                                tgbsReceiverId = c.TGBSReceiverId,
                                bankStatement = a.FileName == null ? null : a.FileName,
                                fileSubType = a.FileSubType == null ? null : a.FileSubType
                            })
            .Where(x =>
            (x.status == _Status || _Status == null) && (x.fileSubType == "BankStatement"))
                  .ToList();

                return new JsonResult(result.ToList());
            }
          
        }

        [HttpGet]
        public JsonResult RemoveAttachment(string Id)
        {
            try
            {
                var model = Db.Attachment.Where(x => x.ParentId == Guid.Parse(Id) && x.FileType == "Cancellation" && x.FileSubType == "BankStatement").FirstOrDefault();
                if (model != null)
                {
                    Db.Remove(model);
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
        public JsonResult GetAllProjectDetails()
        {
            var result = Db.BankDraft.Select(b => new
            {
                bdAmount = b.BankDraftAmount,
                bdNo = b.BankDrafNoIssued,
                requester = b.Requester.FullName
            });

            return new JsonResult(result.ToList());
        }

        [HttpGet]
        public JsonResult GetAllCancellationReason()
        {
            var result = Db.CancellationReason.Select(s => new
            {
                Id = s.Code,
                Name = s.Name
            }).OrderBy(s=> s.Id);

            return new JsonResult(result.ToList());
        }

        [HttpGet]
        public JsonResult GetAllBankProcessType()
        {
            List<BankProcessType> bankProcessTypes = new List<BankProcessType>()
            {
                new BankProcessType ()  {Id = 1, Name = "BD Cancellation - Maybank" },
                new BankProcessType()  { Id = 2, Name = "BD Cancellation - UMA" },
            };

            var result = bankProcessTypes.Select(s => new
            {
                Id = s.Id,
                Name = s.Name
            });

            return new JsonResult(result.ToList());
        }

        [HttpGet]
        public JsonResult GetAllCancellationApprover()
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
                        .ToList();

            string approverId = "SM";
            var result = Db.Users
                      .Join(Db.UserRoles,
                              u => u.Id,
                              r => r.UserId,
                              (u, r) => new
                              {
                                  Id = u.Id,
                                  Name = u.FullName,
                                  RoleId = r.RoleId,
                                  UnitId = r.UnitId,
                                  ZoneId = r.ZoneId,
                                  FunctionId = r.FunctionId,
                                  DivisionId = r.DivisionId,
                                  active = u.IsActive
                              }
                          )
                      .Where(x => x.RoleId == approverId && x.ZoneId == requester[0].ZoneId && x.active == true)
                      .ToList();

            var final = Db.Users
                      .Join(Db.UserRoles,
                              u => u.Id,
                              r => r.UserId,
                              (u, r) => new
                              {
                                  Id = u.Id,
                                  Name = u.FullName,
                                  RoleId = r.RoleId,
                                  UnitId = r.UnitId,
                                  ZoneId = r.ZoneId,
                                  FunctionId = r.FunctionId,
                                  DivisionId = r.DivisionId,
                                  active = u.IsActive
                              }
                          )
                      .Where(x => x.RoleId == approverId && x.ZoneId == requester[0].ZoneId && x.active == true)
                      .ToList();


            if (requester[0].RoleId == "E")
            {
                approverId = "M";

                foreach (var req in requester)
                {
                    result = Db.Users
                   .Join(Db.UserRoles,
                           u => u.Id,
                           r => r.UserId,
                           (u, r) => new
                           {
                               Id = u.Id,
                               Name = u.FullName,
                               RoleId = r.RoleId,
                               UnitId = r.UnitId,
                               ZoneId = r.ZoneId,
                               FunctionId = r.FunctionId,
                               DivisionId = r.DivisionId,
                               active = u.IsActive
                           }
                       )
                   .Where(x => x.RoleId == approverId && x.UnitId == req.UnitId && x.active == true)
                   .ToList();

                }

                result = result.Concat(final).Distinct().ToList();
            }
            else if (requester[0].RoleId == "M")
            {
                if (requester[0].DivType == 1)
                {
                    approverId = "HZ";
                }
                else
                {
                    approverId = "SM";
                }
               
                result = Db.Users
                        .Join(Db.UserRoles,
                        u => u.Id,
                        r => r.UserId,
                        (u, r) => new
                        {
                            Id = u.Id,
                            Name = u.FullName,
                            RoleId = r.RoleId,
                            UnitId = r.UnitId,
                            ZoneId = r.ZoneId,
                            FunctionId = r.FunctionId,
                            DivisionId = r.DivisionId,
                            active = u.IsActive
                        }
                        )
                    .Where(x => x.RoleId == approverId && x.ZoneId == requester[0].ZoneId && x.active == true)
                    .ToList();
            }
            else if (requester[0].RoleId == "HZ" || requester[0].RoleId == "SM")
            {
                if (requester[0].DivType == 1)
                {
                    approverId = "HOU";
                }
                else
                {
                    approverId = "GM/SGM";
                }

                result = Db.Users
                        .Join(Db.UserRoles,
                        u => u.Id,
                        r => r.UserId,
                        (u, r) => new
                        {
                            Id = u.Id,
                            Name = u.FullName,
                            RoleId = r.RoleId,
                            UnitId = r.UnitId,
                            ZoneId = r.ZoneId,
                            FunctionId = r.FunctionId,
                            DivisionId = r.DivisionId,
                            active = u.IsActive
                        }
                        )
                    .Where(x => x.RoleId == approverId && x.FunctionId == requester[0].FunctionId && x.active == true)
                    .ToList();
            }
          
            return new JsonResult(result.ToList());
        }

        public JsonResult GetSupportDocumentForCancellation()
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            var result = Db.Attachment
                        .Where(x => x.FileType == "Cancellation" && x.FileSubType == "BankStatement")
                        .OrderBy(x => x.CreatedOn)
                        .ToList();

            return new JsonResult(result.ToList());
        }

        public IActionResult _CreateDetails()
        {
            return View();
        }
        public IActionResult Approve()
        {
            return View();
        }
        public IActionResult Accept()
        {
            return View();
        }
        public IActionResult Process()
        {
            return View();
        }
        public IActionResult _ProcessDetails()
        {
            return View();
        }
        public IActionResult _ActionButton()
        {
            return View();
        }
        public IActionResult _ActionHistory()
        {
            return View();
        }
        public IActionResult _StatusBar()
        {
            return View();
        }
        public IActionResult _Document()
        {
            return View();
        }
        public IActionResult _Comments()
        {
            return View();
        }
        public IActionResult InstructionLetter()
        {
            return View();
        }
        public IActionResult FormPin()
        {
            return View();
        }
        //Hanif Cancellation Ins letter
        public IActionResult InsLetterCancellation()
        {
            return new ViewAsPdf("InsLetterCancellation")
            {
                PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 },
            };
        }
        public IActionResult Receive()
        {
            return View();
        }
        public IActionResult _ReceiveDetails()
        {
            return View();
        }
        public IActionResult Confirm()
        {
            return View();
        }
        public IActionResult Complete()
        {
            return View();
        }
        public IActionResult BulkInsLetter()
        {
            return View();
        }
        public IActionResult BulkBankDraft()
        {
            return View();
        }
        public IActionResult BulkBankDraft3()
        {
            return View();
        }
        public IActionResult _checkBoxPhysicalBD()
        {
            return View();
        }
    }
}