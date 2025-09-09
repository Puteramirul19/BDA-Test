using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BDA.Entities;
using BDA.Identity;
using BDA.Integrations;
using BDA.ViewModel;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace BDA.Web.Controllers
{
    [Authorize]
    public class BankDraftController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private IConfiguration configuration => HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        public BankDraftController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Executive/Engineer, Manager/Senior Engineer, Head of Zone (AD)/HOZA, Senior Manager/Lead")]
        [HttpPost]
        public JsonResult Create(BankDraftViewModel model, IFormFile WCSuratKelulusan, IFormFile WCUMAP, List<IFormFile> WCFile, List<string> WCFileName)
        {
            //if (ModelState.IsValid)
            //{
                try
                {
                if (model.Status == "Submit" && WCSuratKelulusan == null)
                {
                    return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Surat Kelulusan Required!" });
                }

                var user = _userManager.GetUserAsync(HttpContext.User).Result;

                    BankDraft entity = new BankDraft();
                        entity.CreatedById = user.Id;
                        entity.CreatedByName = user.FullName;
                        entity.RequesterId = user.Id;
                        entity.VerifierId = model.VerifierId;
                       //model.VerifierId == "null" ? null : 
                        entity.ApproverId = model.ApproverId;
                       //model.ApproverId == "null" ? null: 
                        entity.DraftedOn = DateTime.Now;
                        entity.SubmittedOn = model.Status == "Submit" ? DateTime.Now : (DateTime?)null;
                        entity.Type = Data.BDType.WangCagaran.ToString();
                        entity.BankDraftAmount = model.WangCagaranViewModel.Jumlah;
                        entity.NameOnBD = model.WangCagaranViewModel.NamaPemegangCagaran;
                        entity.ProjectNo = model.WangCagaranViewModel.WBSProjekNo;
                        entity.RefNo = GetRunningNo();
                        entity.Status = model.Status == "Submit" ? Data.Status.Submitted.ToString() : Data.Status.Draft.ToString();
                        entity.RequesterSubmissionComment = model.Comment;
                        entity.FinalApplication = "Application";

                    Db.BankDraft.Add(entity);
                    Db.SaveChanges();

                    var _BankDraftId = entity.Id;
                    var wangCagaranModel = model.WangCagaranViewModel;

                    WangCagaran wang = new WangCagaran();
                    wang.BankDraftId = _BankDraftId;
                    wang.ErmsDocNo = wangCagaranModel.ErmsDocNo;
                    wang.Pemula = user.FullName;
                    wang.Tarikh = DateTime.Now;
                    wang.Alamat1 = wangCagaranModel.Alamat1;
                    wang.Alamat2 = wangCagaranModel.Alamat2;
                    wang.Bandar = wangCagaranModel.Bandar;
                    wang.Poskod = wangCagaranModel.Poskod;
                    wang.Negeri = wangCagaranModel.Negeri;
                    wang.KeteranganKerja = wangCagaranModel.KeteranganKerja;
                    wang.JKRInvolved = wangCagaranModel.JKRInvolved;
                    wang.JKRType = wangCagaranModel.JKRType;
                    wang.Jumlah = wangCagaranModel.Jumlah;
                    wang.CajKod = wangCagaranModel.CajKod;
                    wang.NamaPemegangCagaran = wangCagaranModel.NamaPemegangCagaran;
                    wang.WBSProjekNo = wangCagaranModel.WBSProjekNo;
                    wang.BusinessArea = wangCagaranModel.BusinessArea;
                    wang.CoCode = wangCagaranModel.CoCode;

                    Db.WangCagaran.Add(wang);
                    Db.SaveChanges();

                    if (entity.Status == "Submitted")
                    {
                        Db.BankDraftAction.Add(new BankDraftAction
                        {
                            ActionType = Data.ActionType.Submitted.ToString(),
                            On = DateTime.Now,
                            ById = user.Id,
                            ParentId = entity.Id,
                            ActionRole = Data.ActionRole.Requester.ToString(),
                            Comment = model.Comment,
                        });
                        Db.SaveChanges();

                        Job.Enqueue<Services.NotificationService>(x => x.NotifyVerifierForVerification(entity.Id));
                    }

                    if (WCSuratKelulusan != null)
                    {
                        UploadFile(WCSuratKelulusan, _BankDraftId, Data.AttachmentType.BankDraft.ToString(), Data.BDAttachmentType.WCSuratKelulusan.ToString());
                    }

                    if (WCUMAP != null)
                    {
                        UploadFile(WCUMAP, _BankDraftId, Data.AttachmentType.BankDraft.ToString(), Data.BDAttachmentType.WCUMAP.ToString());
                    }

                    var count = 0;
                    foreach (var item in WCFile)
                    {
                        UploadFile(item, entity.Id, Data.AttachmentType.BankDraft.ToString(), Data.BDAttachmentType.WCDocument.ToString(), WCFileName[count]);
                        count++;
                    }

                    return Json(new { wanghangusId = wang.Id ,response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Application Saved Successful!" });
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

        //[HttpPost]
        //public JsonResult NextAction(BankDraftViewModel model)
        //{
        //    var user = _userManager.GetUserAsync(HttpContext.User).Result;
        //    //if (ModelState.IsValid)
        //    //{
        //        string actionType = "";
        //        string byId = "";
        //        string actionRole = "";

        //        try
        //        {
        //            //var user = _userManager.GetUserAsync(HttpContext.User).Result;

        //            switch (model.UserAction)
        //            {
        //                case "Withdrawn":
        //                    model.Status = Data.Status.Withdrawn.ToString();
        //                    break;
        //                case "Verify":
        //                    model.Status = Data.Status.Verified.ToString();
        //                    break;
        //                case "RejectVerify":
        //                    model.Status = Data.Status.RejectedVerify.ToString();
        //                    //actionType = Data.ActionType.Rejected.ToString();
        //                    //byId = model.VerifierId;
        //                    //actionRole = Data.ActionRole.Verifier.ToString();
        //                    break;
        //                case "Approve":
        //                    model.Status = Data.Status.Approved.ToString();
        //                    break;
        //                case "RejectApprove":
        //                    model.Status = Data.Status.RejectedApprove.ToString();
        //                    //actionType = Data.ActionType.Rejected.ToString();
        //                    //byId = model.ApproverId;
        //                    //actionRole = Data.ActionRole.Approver.ToString();
        //                    break;
        //                case "Accept":
        //                    model.Status = Data.Status.Accepted.ToString();
        //                    break;
        //                case "Decline":
        //                    model.Status = Data.Status.Declined.ToString();
        //                    //actionType = Data.ActionType.Declined.ToString();
        //                    //byId = model.TGBSAcceptanceId;
        //                    //actionRole = Data.ActionRole.TGBSBanking.ToString();
        //                    break;
        //                default:
        //                    model.Status = "Invalid";
        //                    break;
        //            }

        //            if(model.Status != "Invalid")
        //            {
        //                var entity = Db.BankDraft.Find(Guid.Parse(model.Id));
        //                entity.UpdatedOn = DateTime.Now;
        //                Db.SetModified(entity);
        //                Db.SaveChanges();
        //            //entity.Status = model.Status;

        //            if (model.Status == Data.Status.Withdrawn.ToString())
        //            {
        //                entity.Status = model.Status;
        //                entity.WithdrewOn = DateTime.Now;
        //                Db.SetModified(entity);
        //                Db.SaveChanges();

        //                //audit trail for Withdrawn
        //                actionType = Data.ActionType.Withdrawn.ToString();
        //                byId = user.Id;
        //                actionRole = Data.ActionRole.Requester.ToString();

        //                Job.Enqueue<Services.NotificationService>(x => x.NotifyApproverForWithdrawn(entity.Id));
        //            }
        //            else if (model.Status == Data.Status.Verified.ToString())
        //            {
        //                entity.Status = model.Status;
        //                entity.VerifiedOn = DateTime.Now;
        //                entity.VerifierComment = model.Comment;
        //                //entity.VerifierId = model.VerifierId;
        //                Db.SetModified(entity);
        //                Db.SaveChanges();

        //                //audit trail for Verified
        //                actionType = Data.ActionType.Verified.ToString();
        //                byId = user.Id;
        //                actionRole = Data.ActionRole.Verifier.ToString();

        //                Job.Enqueue<Services.NotificationService>(x => x.NotifyApproverForApproval(entity.Id));
        //                Job.Enqueue<Services.NotificationService>(x => x.NotifyRequesterForVerification(entity.Id));
        //            }
        //            else if (model.Status == Data.Status.RejectedVerify.ToString())
        //            {
        //                entity.Status = model.Status;
        //                entity.VerifiedOn = DateTime.Now;
        //                entity.VerifierComment = model.Comment;
        //                //entity.VerifierId = model.VerifierId;
        //                Db.SetModified(entity);
        //                Db.SaveChanges();

        //                //audit trail for RejectedVerify
        //                actionType = Data.ActionType.Rejected.ToString();
        //                byId = user.Id;
        //                actionRole = Data.ActionRole.Verifier.ToString();

        //                Job.Enqueue<Services.NotificationService>(x => x.NotifyRequesterForVerificationRejection(entity.Id));
        //            }
        //            else if (model.Status == Data.Status.Approved.ToString())
        //            {
        //                entity.Status = model.Status;
        //                entity.ApprovedOn = DateTime.Now;
        //                entity.ApproverComment = model.Comment;
        //                //entity.ApproverId = model.ApproverId;

        //                Db.SetModified(entity);
        //                Db.SaveChanges();

        //                //audit trail for Approved
        //                actionType = Data.ActionType.Approved.ToString();
        //                byId = user.Id;
        //                actionRole = Data.ActionRole.Approver.ToString();

        //                Job.Enqueue<Services.NotificationService>(x => x.NotifyTGBSBankingForAcceptance(entity.Id));
        //                Job.Enqueue<Services.NotificationService>(x => x.NotifyRequesterForApproval(entity.Id));

        //            }
        //            else if (model.Status == Data.Status.RejectedApprove.ToString())
        //            {
        //                entity.Status = model.Status;
        //                entity.ApprovedOn = DateTime.Now;
        //                entity.ApproverComment = model.Comment;
        //                //entity.ApproverId = model.ApproverId;

        //                Db.SetModified(entity);
        //                Db.SaveChanges();

        //                //audit trail for Approved
        //                actionType = Data.ActionType.Rejected.ToString();
        //                byId = user.Id;
        //                actionRole = Data.ActionRole.Approver.ToString();

        //                Job.Enqueue<Services.NotificationService>(x => x.NotifyRequesterForApprovalRejection(entity.Id));

        //            }
        //            else if (model.Status == Data.Status.Accepted.ToString())
        //            {
        //                if (entity.Status == "Accepted" || entity.Status == "Declined")
        //                {
        //                    return Json(new { response = StatusCode(StatusCodes.Status403Forbidden), message = "Bank Draft Application has already been " + entity.Status.ToLower() + " by " + entity.TGBSAcceptanceId + "!"});
        //                }
        //                else
        //                {
        //                    entity.Status = model.Status;
        //                    entity.TGBSAcceptedOn = DateTime.Now;
        //                    entity.TGBSAcceptedComment = model.Comment;
        //                    entity.TGBSAcceptanceId = user.Id;

        //                    if (entity.Type == "WangCagaran")
        //                    {
        //                        var wang = Db.WangCagaran.Where(x => x.BankDraftId == entity.Id).FirstOrDefault();
        //                        wang.ErmsDocNo = model.WangCagaranViewModel.ErmsDocNo;
        //                        wang.PostingDate = model.WangCagaranViewModel.PostingDate;
        //                        Db.SetModified(wang);
        //                    }
        //                    else if(entity.Type == "WangHangus")
        //                    {
        //                        var wang = Db.WangHangus.Where(x => x.BankDraftId == entity.Id).FirstOrDefault();
        //                        wang.ErmsDocNo = model.WangHangusViewModel.ErmsDocNo;
        //                        wang.PostingDate = model.WangHangusViewModel.PostingDate;
        //                        Db.SetModified(wang);
        //                    }

        //                    Db.SetModified(entity);
        //                    Db.SaveChanges();

        //                    Job.Enqueue<Services.NotificationService>(x => x.NotifyRequesterForTGBSAcceptance(entity.Id));

        //                    //audit trail for Accepted
        //                    actionType = Data.ActionType.Accepted.ToString();
        //                    byId = user.Id;
        //                    actionRole = Data.ActionRole.TGBSBanking.ToString();
        //                }
        //            }
        //            else if (model.Status == Data.Status.Declined.ToString())
        //            {
        //                if (entity.Status == "Accepted" || entity.Status == "Declined")
        //                {
        //                    return Json(new { response = StatusCode(StatusCodes.Status403Forbidden), message = "Bank Draft Application has already been " + entity.Status.ToLower() + " by " + entity.TGBSAcceptanceId + "!" });
        //                }
        //                else
        //                {
        //                    entity.Status = model.Status;
        //                    entity.TGBSAcceptedComment = model.Comment;
        //                    entity.TGBSAcceptanceId = user.Id;

        //                    Db.SetModified(entity);
        //                    Db.SaveChanges();

        //                    //audit trail for Accepted
        //                    actionType = Data.ActionType.Declined.ToString();
        //                    byId = user.Id;
        //                    actionRole = Data.ActionRole.TGBSBanking.ToString();

        //                    Job.Enqueue<Services.NotificationService>(x => x.NotifyRequesterForAcceptanceRejection(entity.Id));
        //                }
        //            }
        //                //Update audit trail

        //                Db.BankDraftAction.Add(new BankDraftAction
        //                {
        //                    ActionType = actionType,
        //                    On = DateTime.Now,
        //                    ById = byId,
        //                    ParentId = Guid.Parse(model.Id),
        //                    ActionRole = actionRole,
        //                    Comment = model.Comment,
        //                });
        //                Db.SaveChanges();

        //               if(model.Status == Data.Status.Declined.ToString())
        //                {
        //                return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "The Bank Draft Application has been declined!", id = entity.Id });
        //               }
        //               else if(model.Status == Data.Status.RejectedVerify.ToString() || model.Status == Data.Status.RejectedApprove.ToString())
        //               {
        //                return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "The Bank Draft Application has been rejected!", id = entity.Id });
        //               }
        //               else
        //                {
        //                    return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Application Updated Successful!", id = entity.Id });
        //                }

        //            }

        //            return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Error! Invalid Action " + model.UserAction });
        //        }
        //        catch (Exception e)
        //        {
        //            return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });

        //    }
        //    //}
        //    //else
        //    //{
        //    //    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Error! Please try again later." });
        //    //}
        //}

        [HttpPost]
        public JsonResult Edit(BankDraftViewModel model, IFormFile WCSuratKelulusan, IFormFile WCUMAP, List<IFormFile> WCFile, List<string> WCFileName)
        {
            bool checkFirstTimeSubmit = true;
            //if (ModelState.IsValid)
            //{
            try
            {

                var file = Db.Attachment.Where(x => x.ParentId == Guid.Parse(model.Id) && x.FileType == Data.AttachmentType.BankDraft.ToString() && x.FileSubType == Data.BDAttachmentType.WCSuratKelulusan.ToString()).FirstOrDefault();
                if (model.Status == "Submit" && WCSuratKelulusan == null && file == null)
                {
                    return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Surat Kelulusan Required!" });
                }

                var user = _userManager.GetUserAsync(HttpContext.User).Result;

                var entity = Db.BankDraft.Find(Guid.Parse(model.Id));
                //entity.RequesterId = user.Id;
                //entity.Type = Data.BDType.WangCagaran.ToString();
                //entity.DraftedOn = DateTime.Now;
                entity.UpdatedOn = DateTime.Now;
                entity.VerifierId = model.VerifierId == "1" ? null : model.VerifierId;
                entity.ApproverId = model.ApproverId == "1" ? null : model.ApproverId;
                entity.SubmittedOn = model.Status == "Submit" ? DateTime.Now : (DateTime?)null;
                entity.BankDraftAmount = model.WangCagaranViewModel.Jumlah;
                entity.NameOnBD = model.WangCagaranViewModel.NamaPemegangCagaran;
                entity.ProjectNo = model.WangCagaranViewModel.WBSProjekNo;
                entity.RequesterSubmissionComment = model.Comment;

                if (entity.Status == "RejectedVerify" || entity.Status == "RejectedApprove" || entity.Status == "Declined")
                {
                    checkFirstTimeSubmit = false;
                }
                entity.Status = model.Status == "Submit" ? Data.Status.Submitted.ToString() : Data.Status.Draft.ToString();

                Db.SetModified(entity);
                Db.SaveChanges();

                var _BankDraftId = entity.Id;
                var wangCagaranModel = model.WangCagaranViewModel;
                var wang = Db.WangCagaran.Find(Guid.Parse(model.WangCagaranViewModel.Id));
                wang.ErmsDocNo = wangCagaranModel.ErmsDocNo;
                //wang.PostingDate = wangCagaranModel.PostingDate;
                wang.Pemula = wangCagaranModel.Pemula;
                if (checkFirstTimeSubmit == false)
                {
                    wang.Tarikh = wangCagaranModel.Tarikh;
                }
                else
                {
                    wang.Tarikh = model.Status == "Submit" ? DateTime.Now : (DateTime?)null;
                }
                wang.Alamat1 = wangCagaranModel.Alamat1;
                wang.Alamat2 = wangCagaranModel.Alamat2;
                wang.Bandar = wangCagaranModel.Bandar;
                wang.Poskod = wangCagaranModel.Poskod;
                wang.Tarikh = wangCagaranModel.Tarikh;
                wang.Negeri = wangCagaranModel.Negeri;
                wang.BusinessArea = wangCagaranModel.BusinessArea;
                wang.KeteranganKerja = wangCagaranModel.KeteranganKerja;
                wang.JKRInvolved = wangCagaranModel.JKRInvolved;
                wang.JKRType = wangCagaranModel.JKRType;
                wang.Jumlah = wangCagaranModel.Jumlah;
                wang.CajKod = wangCagaranModel.CajKod;
                wang.NamaPemegangCagaran = wangCagaranModel.NamaPemegangCagaran;
                wang.WBSProjekNo = wangCagaranModel.WBSProjekNo;
                wang.CoCode = wangCagaranModel.CoCode;

                Db.SetModified(wang);
                Db.SaveChanges();

                if (WCSuratKelulusan != null)
                {
                    UploadFile(WCSuratKelulusan, _BankDraftId, Data.AttachmentType.BankDraft.ToString(), Data.BDAttachmentType.WCSuratKelulusan.ToString());
                }

                if (WCUMAP != null)
                {
                    UploadFile(WCUMAP, _BankDraftId, Data.AttachmentType.BankDraft.ToString(), Data.BDAttachmentType.WCUMAP.ToString());
                }

                if (model.Status == "Submit")
                {
                    Db.BankDraftAction.Add(new BankDraftAction
                    {
                        ActionType = Data.ActionType.Submitted.ToString(),
                        On = DateTime.Now,
                        ById = user.Id,
                        ParentId = entity.Id,
                        Comment = model.Comment,
                        ActionRole = Data.ActionRole.Requester.ToString(),
                    });
                    Db.SaveChanges();

                    Job.Enqueue<Services.NotificationService>(x => x.NotifyVerifierForVerification(entity.Id));
                }

                var count = 0;
                foreach (var item in WCFile)
                {
                    UploadFile(item, entity.Id, Data.AttachmentType.BankDraft.ToString(), Data.BDAttachmentType.WCDocument.ToString(), WCFileName[count]);
                    count++;
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
        public JsonResult NextAction(BankDraftViewModel model, IFormFile WCSuratKelulusan, IFormFile WCUMAP, List<IFormFile> WCFile, List<string> WCFileName, List<IFormFile> WHFile = null, List<string> WHFileName = null)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;
            //if (ModelState.IsValid)
            //{
            string actionType = "";
            string byId = "";
            string actionRole = "";
            string logBA = "";
            string logDoc = "";
            PostingBankDraftResult post;

            try
            {
                //var user = _userManager.GetUserAsync(HttpContext.User).Result;

                switch (model.UserAction)
                {
                    case "Withdrawn":
                        model.Status = Data.Status.Withdrawn.ToString();
                        break;
                    case "Verify":
                        model.Status = Data.Status.Verified.ToString();
                        break;
                    case "RejectVerify":
                        model.Status = Data.Status.RejectedVerify.ToString();
                        //actionType = Data.ActionType.Rejected.ToString();
                        //byId = model.VerifierId;
                        //actionRole = Data.ActionRole.Verifier.ToString();
                        break;
                    case "Approve":
                        model.Status = Data.Status.Approved.ToString();
                        break;
                    case "RejectApprove":
                        model.Status = Data.Status.RejectedApprove.ToString();
                        //actionType = Data.ActionType.Rejected.ToString();
                        //byId = model.ApproverId;
                        //actionRole = Data.ActionRole.Approver.ToString();
                        break;
                    case "Accept":
                        model.Status = Data.Status.Accepted.ToString();
                        break;
                    case "Decline":
                        model.Status = Data.Status.Declined.ToString();
                        //actionType = Data.ActionType.Declined.ToString();
                        //byId = model.TGBSAcceptanceId;
                        //actionRole = Data.ActionRole.TGBSBanking.ToString();
                        break;
                    default:
                        model.Status = "Invalid";
                        break;
                }

                if (model.Status != "Invalid")
                {
                    var entity = Db.BankDraft.Find(Guid.Parse(model.Id));
                    entity.UpdatedOn = DateTime.Now;
                    Db.SetModified(entity);
                    Db.SaveChanges();
                    //entity.Status = model.Status;

                    if (model.Status == Data.Status.Withdrawn.ToString())
                    {
                        entity.Status = model.Status;
                        entity.WithdrewOn = DateTime.Now;
                        Db.SetModified(entity);
                        Db.SaveChanges();

                        //audit trail for Withdrawn
                        actionType = Data.ActionType.Withdrawn.ToString();
                        byId = user.Id;
                        actionRole = Data.ActionRole.Requester.ToString();

                        Job.Enqueue<Services.NotificationService>(x => x.NotifyApproverForWithdrawn(entity.Id));
                    }
                    else if (model.Status == Data.Status.Verified.ToString())
                    {
                        entity.Status = model.Status;
                        entity.VerifiedOn = DateTime.Now;
                        entity.VerifierComment = model.Comment;
                        //entity.VerifierId = model.VerifierId;
                        Db.SetModified(entity);
                        Db.SaveChanges();

                        //audit trail for Verified
                        actionType = Data.ActionType.Verified.ToString();
                        byId = user.Id;
                        actionRole = Data.ActionRole.Verifier.ToString();

                        Job.Enqueue<Services.NotificationService>(x => x.NotifyApproverForApproval(entity.Id));
                        Job.Enqueue<Services.NotificationService>(x => x.NotifyRequesterForVerification(entity.Id));
                    }
                    else if (model.Status == Data.Status.RejectedVerify.ToString())
                    {
                        entity.Status = model.Status;
                        entity.VerifiedOn = DateTime.Now;
                        entity.VerifierComment = model.Comment;
                        //entity.VerifierId = model.VerifierId;
                        Db.SetModified(entity);
                        Db.SaveChanges();

                        //audit trail for RejectedVerify
                        actionType = Data.ActionType.Rejected.ToString();
                        byId = user.Id;
                        actionRole = Data.ActionRole.Verifier.ToString();

                        Job.Enqueue<Services.NotificationService>(x => x.NotifyRequesterForVerificationRejection(entity.Id));
                    }
                    else if (model.Status == Data.Status.Approved.ToString())
                    {
                        entity.Status = model.Status;
                        entity.ApprovedOn = DateTime.Now;
                        entity.ApproverComment = model.Comment;
                        //entity.ApproverId = model.ApproverId;

                        Db.SetModified(entity);
                        Db.SaveChanges();

                        //audit trail for Approved
                        actionType = Data.ActionType.Approved.ToString();
                        byId = user.Id;
                        actionRole = Data.ActionRole.Approver.ToString();

                        Job.Enqueue<Services.NotificationService>(x => x.NotifyTGBSBankingForAcceptance(entity.Id));
                        Job.Enqueue<Services.NotificationService>(x => x.NotifyRequesterForApproval(entity.Id));

                    }
                    else if (model.Status == Data.Status.RejectedApprove.ToString())
                    {
                        entity.Status = model.Status;
                        entity.ApprovedOn = DateTime.Now;
                        entity.ApproverComment = model.Comment;
                        //entity.ApproverId = model.ApproverId;

                        Db.SetModified(entity);
                        Db.SaveChanges();

                        //audit trail for Approved
                        actionType = Data.ActionType.Rejected.ToString();
                        byId = user.Id;
                        actionRole = Data.ActionRole.Approver.ToString();

                        Job.Enqueue<Services.NotificationService>(x => x.NotifyRequesterForApprovalRejection(entity.Id));

                    }
                    else if (model.Status == Data.Status.Accepted.ToString())
                    {
                        if (entity.Status == "Accepted" || entity.Status == "Declined")
                        {
                            return Json(new { response = StatusCode(StatusCodes.Status403Forbidden), message = "Bank Draft Application has already been " + entity.Status.ToLower() + " by " + entity.TGBSAcceptanceId + "!" });
                        }
                        else
                        {
                            var erms = new ErmsService(Db,configuration);

                            if (entity.Type == "WangCagaran")
                            {
                                post = erms.PostingWangCagaran(entity);

                                if (post.IsSuccess == false)
                                {
                                    return Json(new { response = StatusCode(StatusCodes.Status403Forbidden), message = post.Message });
                                }
                                entity.IntegrationId = post.IntegrationId;
                            }
                            else if (entity.Type == "WangHangus")
                            {
                                post = erms.PostingWangHangus(entity);

                                if (post.IsSuccess == false)
                                {
                                    return Json(new { response = StatusCode(StatusCodes.Status403Forbidden), message = post.Message });
                                }
                                entity.IntegrationId = post.IntegrationId;
                            }
                            else if (entity.Type == "WangCagaranHangus")
                            {
                                post = erms.PostingWangCagaranHangus(entity);

                                if (post.IsSuccess == false)
                                {
                                    return Json(new { response = StatusCode(StatusCodes.Status403Forbidden), message = post.Message });
                                }
                                entity.IntegrationId = post.IntegrationId;
                            }

                            entity.Status = "Posted";
                            entity.TGBSAcceptedOn = DateTime.Now;
                            entity.TGBSAcceptedComment = model.Comment;
                            entity.TGBSAcceptanceId = user.Id;


                            if (entity.Type == "WangCagaran")
                            {
                                var file = Db.Attachment.Where(x => x.ParentId == Guid.Parse(model.Id) && x.FileType == Data.AttachmentType.BankDraft.ToString() && x.FileSubType == Data.BDAttachmentType.WCSuratKelulusan.ToString()).FirstOrDefault();
                                if (WCSuratKelulusan == null && file == null)
                                {
                                    return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Surat Kelulusan Required!" });
                                }

                                if (WCSuratKelulusan != null)
                                {
                                    UploadFile(WCSuratKelulusan, entity.Id, Data.AttachmentType.BankDraft.ToString(), Data.BDAttachmentType.WCSuratKelulusan.ToString());
                                }

                                if (WCUMAP != null)
                                {
                                    UploadFile(WCUMAP, entity.Id, Data.AttachmentType.BankDraft.ToString(), Data.BDAttachmentType.WCUMAP.ToString());
                                    logDoc += "&&Add new document " + WCUMAP.FileName + " as UMAP";
                                }

                                var count = 0;
                                foreach (var item in WCFile)
                                {
                                    UploadFile(item, entity.Id, Data.AttachmentType.BankDraft.ToString(), Data.BDAttachmentType.WCDocument.ToString(), WCFileName[count]);
                                    count++;
                                    logDoc += "&&Add new document " + item.FileName + " as Additional Document";
                                }

                                var wang = Db.WangCagaran.Where(x => x.BankDraftId == entity.Id).FirstOrDefault();
                                if (wang.BusinessArea != model.WangCagaranViewModel.BusinessArea)
                                {
                                    logBA += "&&Business area was changed from " + wang.BusinessArea + " to " + model.WangCagaranViewModel.BusinessArea;
                                }
                                if (wang.CoCode != model.WangCagaranViewModel.CoCode)
                                {
                                    logBA += "&&Company code was changed from " + wang.CoCode + " to " + model.WangCagaranViewModel.CoCode;
                                }
                                wang.ErmsDocNo = model.WangCagaranViewModel.ErmsDocNo;
                                wang.PostingDate = model.WangCagaranViewModel.PostingDate;
                                wang.BusinessArea = model.WangCagaranViewModel.BusinessArea;
                                wang.CoCode = model.WangCagaranViewModel.CoCode;
                                Db.SetModified(wang);
                            }
                            else if (entity.Type == "WangHangus")
                            {
                                var count = 0;
                                foreach (var item in WHFile)
                                {
                                    UploadFile(item, entity.Id, Data.AttachmentType.BankDraft.ToString(), Data.BDAttachmentType.WHDocument.ToString(), WHFileName[count]);
                                    count++;
                                    logDoc += "&&Add new document " + item.FileName + " as Additional Document"; ;
                                }

                                var wang = Db.WangHangus.Where(x => x.BankDraftId == entity.Id).FirstOrDefault();
                                if (wang.BusinessArea != model.WangHangusViewModel.BusinessArea)
                                {
                                    logBA += "&&Business area was changed from " + wang.BusinessArea + " to " + model.WangHangusViewModel.BusinessArea;
                                }
                                if (wang.CoCode != model.WangHangusViewModel.CoCode)
                                {
                                    logBA += "&&Company code was changed from " + wang.CoCode + " to " + model.WangHangusViewModel.CoCode;
                                }
                                wang.ErmsDocNo = model.WangHangusViewModel.ErmsDocNo;
                                wang.PostingDate = model.WangHangusViewModel.PostingDate;
                                wang.BusinessArea = model.WangHangusViewModel.BusinessArea;
                                wang.CoCode = model.WangHangusViewModel.CoCode;
                                Db.SetModified(wang);
                            }
                            else if (entity.Type == "WangCagaranHangus")
                            {
                                var file = Db.Attachment.Where(x => x.ParentId == Guid.Parse(model.Id) && x.FileType == Data.AttachmentType.BankDraft.ToString() && x.FileSubType == Data.BDAttachmentType.WCSuratKelulusan.ToString()).FirstOrDefault();
                                if (WCSuratKelulusan == null && file == null)
                                {
                                    return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Surat Kelulusan Required!" });
                                }

                                if (WCSuratKelulusan != null)
                                {
                                    UploadFile(WCSuratKelulusan, entity.Id, Data.AttachmentType.BankDraft.ToString(), Data.BDAttachmentType.WCSuratKelulusan.ToString());
                                }

                                if (WCUMAP != null)
                                {
                                    UploadFile(WCUMAP, entity.Id, Data.AttachmentType.BankDraft.ToString(), Data.BDAttachmentType.WCUMAP.ToString());
                                    logDoc += "&&Add new document " + WCUMAP.FileName + " as UMAP";
                                }

                                var count = 0;
                                foreach (var item in WCFile)
                                {
                                    UploadFile(item, entity.Id, Data.AttachmentType.BankDraft.ToString(), Data.BDAttachmentType.WCDocument.ToString(), WCFileName[count]);
                                    count++;
                                    logDoc += "&&Add new document " + item.FileName + " as Additional Document";
                                }

                                var wang = Db.WangCagaranHangus.Where(x => x.BankDraftId == entity.Id).FirstOrDefault();
                                if (wang.BusinessArea != model.WangCagaranHangusViewModel.BusinessArea)
                                {
                                    logBA += "&&Business area was changed from " + wang.BusinessArea + " to " + model.WangCagaranHangusViewModel.BusinessArea;
                                }
                                if (wang.CoCode != model.WangCagaranHangusViewModel.CoCode)
                                {
                                    logBA += "&&Company code was changed from " + wang.CoCode + " to " + model.WangCagaranHangusViewModel.CoCode;
                                }
                                wang.ErmsDocNo = model.WangCagaranHangusViewModel.ErmsDocNo;
                                wang.PostingDate = model.WangCagaranHangusViewModel.PostingDate;
                                wang.BusinessArea = model.WangCagaranHangusViewModel.BusinessArea;
                                wang.CoCode = model.WangCagaranHangusViewModel.CoCode;
                                Db.SetModified(wang);
                            }

                            Db.SetModified(entity);
                            Db.SaveChanges();

                            //Postpone email accepted until posting to erms successfull
                            //Job.Enqueue<Services.NotificationService>(x => x.NotifyRequesterForTGBSAcceptance(entity.Id));

                            //audit trail for Accepted -> change to Posted
                            actionType = Data.ActionType.Posted.ToString();
                            byId = user.Id;
                            actionRole = Data.ActionRole.TGBSBanking.ToString();
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

                            Job.Enqueue<Services.NotificationService>(x => x.NotifyRequesterForAcceptanceRejection(entity.Id));
                        }
                    }
                    //Update audit trail

                    Db.BankDraftAction.Add(new BankDraftAction
                    {
                        ActionType = actionType,
                        On = DateTime.Now,
                        ById = byId,
                        ParentId = Guid.Parse(model.Id),
                        ActionRole = actionRole,
                        Comment = model.Comment + logBA + logDoc,
                    });
                    Db.SaveChanges();

                    if (model.Status == Data.Status.Declined.ToString())
                    {
                        return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "The Bank Draft Application has been declined!", id = entity.Id });
                    }
                    else if (model.Status == Data.Status.RejectedVerify.ToString() || model.Status == Data.Status.RejectedApprove.ToString())
                    {
                        return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "The Bank Draft Application has been rejected!", id = entity.Id });
                    }
                    else
                    {
                        return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Application Updated Successful!", id = entity.Id });
                    }

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
        public JsonResult SaveBankDetails(BankDraftViewModel model, IFormFile SignedLetter)
        {
            //if (ModelState.IsValid)
            //{
                var entity = Db.BankDraft.Find(Guid.Parse(model.Id));

                if (entity.Status == Data.Status.Processed.ToString())
                {
                    return Json(new { response = StatusCode(StatusCodes.Status403Forbidden), message = "Bank Draft Application has already been processed by " + entity.TGBSProcesserId + "!" });
                }
                else
                { 
                    try
                    {
                        var file = Db.Attachment.Where(x => x.ParentId == Guid.Parse(model.Id) && x.FileType == Data.AttachmentType.InstructionLetter.ToString()).FirstOrDefault();
                        if (model.UserAction == "SubmitToBank" && SignedLetter == null && file == null)
                        {
                            return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Signed Document Required!" });
                        }   

                        var user = _userManager.GetUserAsync(HttpContext.User).Result;
                    
                        entity.InstructionLetterRefNo = model.InstructionLetterRefNo;
                        entity.UpdatedOn = DateTime.Now;
                        if (model.UserAction == "SubmitToBank")
                        {
                            entity.TGBSProcessedOn = DateTime.Now;
                            entity.TGBSProcesserId = user.Id;
                            entity.Status = Data.Status.Processed.ToString();
                        }

                        Db.SetModified(entity);
                        Db.SaveChanges();

                    if( entity.Type == "WangCagaran")
                    {
                        var wg = Db.WangCagaran.Where(x => x.BankDraftId == Guid.Parse(model.Id)).FirstOrDefault();
                        wg.ErmsDocNo = model.WangCagaranViewModel.ErmsDocNo;
                        wg.Type = "40";

                        Db.SetModified(wg);
                        Db.SaveChanges();
                    }
                    else if(entity.Type == "WangHangus")
                    {
                        var wh = Db.WangHangus.Where(x => x.BankDraftId == Guid.Parse(model.Id)).FirstOrDefault();
                        wh.ErmsDocNo = model.WangHangusViewModel.ErmsDocNo;
                        wh.Type = "40";

                        Db.SetModified(wh);
                        Db.SaveChanges();
                    }
                      

                    var _BankDraftId = entity.Id;
                    
                        if (SignedLetter != null)
                        {
                            UploadFile(SignedLetter, _BankDraftId, Data.AttachmentType.InstructionLetter.ToString());
                        }

                        if (model.UserAction == "SubmitToBank")
                        {
                            var letter = Db.InstructionLetter.Where(x => x.LetterRefNo == model.InstructionLetterRefNo).FirstOrDefault();
                            letter.Status = Data.Status.Processed.ToString();
                            letter.UpdatedOn = DateTime.Now;
                            Db.SetModified(letter);
                            Db.SaveChanges();

                            Db.BankDraftAction.Add(new BankDraftAction
                            {
                                ActionType = Data.ActionType.SubmittedToBank.ToString(),
                                On = DateTime.Now,
                                ById = user.Id,
                                ParentId = entity.Id,
                                ActionRole = Data.ActionRole.TGBSBanking.ToString(),
                            });
                            Db.SaveChanges();

                            Job.Enqueue<Services.NotificationService>(x => x.NotifyBankForProcessing(letter.Id));
                        }
                        return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Application Saved Successful!" });
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

        [Authorize(Roles = "TGBS Banking, Business Admin")]
        [HttpPost]
        public JsonResult SaveBankIssue(BankDraftViewModel model, IFormFile SignedMemo)
        {
            //if (ModelState.IsValid)
            //{
                var entity = Db.BankDraft.Find(Guid.Parse(model.Id));

                if (entity.Status == Data.Status.Issued.ToString())
                {
                    return Json(new { response = StatusCode(StatusCodes.Status403Forbidden), message = "Bank Draft Application has already been issued by " + entity.TGBSIssuerId + "!" });
                }
                else
                {

                    try
                    {
                        var file = Db.Attachment.Where(x => x.ParentId == Guid.Parse(model.Id) && x.FileType == Data.AttachmentType.Memo.ToString()).FirstOrDefault();
                        if (model.UserAction == "SubmitToRequestor" && SignedMemo == null && file == null)
                        {
                            return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Signed Document Required!" });
                        }

                        var user = _userManager.GetUserAsync(HttpContext.User).Result;

                        entity.SendMethod = model.SendMethod == "undefined" ? entity.SendMethod : model.SendMethod;
                        entity.PostageNo = model.PostageNo;
                        entity.ReceiverContactNo = model.ReceiverContactNo;
                        entity.BankDrafNoIssued = model.BankDrafNoIssued;
                        entity.CoverMemoRefNo = model.CoverMemoRefNo;
                        entity.BankDraftDate = model.BankDraftDate; // DateTime.ParseExact(model.BankDraftDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        entity.UpdatedOn = DateTime.Now;

                        if (model.UserAction == "SubmitToRequestor")
                        {
                            entity.TGBSIssuedOn = DateTime.Now;
                            entity.TGBSIssuerId = user.Id;
                            //entity.UpdatedOn = DateTime.Now;
                            entity.Status = Data.Status.Issued.ToString();
                        }

                        Db.SetModified(entity);
                        Db.SaveChanges();

                        var _BankDraftId = entity.Id;

                        if (SignedMemo != null)
                        {
                            UploadFile(SignedMemo, _BankDraftId, Data.AttachmentType.Memo.ToString());
                        }

                        if (model.UserAction == "SubmitToRequestor")
                        {
                            var memo = Db.Memo.Where(x => x.CoverRefNo == model.CoverMemoRefNo).FirstOrDefault();
                            memo.Status = Data.Status.Issued.ToString();
                            memo.UpdatedOn = DateTime.Now;
                            Db.SetModified(memo);
                            Db.SaveChanges();

                            Db.BankDraftAction.Add(new BankDraftAction
                            {
                                ActionType = Data.ActionType.BankDraftIssued.ToString(),
                                On = DateTime.Now,
                                ById = user.Id,
                                ParentId = entity.Id,
                                ActionRole = Data.ActionRole.TGBSBanking.ToString(),
                            });
                            Db.SaveChanges();

                            Job.Enqueue<Services.NotificationService>(x => x.NotifyRequesterForBDAcceptance(entity.Id));

                        }

                        return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Application Saved Successful!" });
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
        public JsonResult UpdateBankIssue([FromBody] string value)
        {
            if (ModelState.IsValid && value != null)
            {
                try
                {
                    var item = value.Split(",");
                    var bdRefNo = item[0];
                    var entity = Db.BankDraft.Where(x => x.RefNo == bdRefNo).FirstOrDefault();
                    
                    //var memo = Db.Memo.Where(x => x.CoverRefNo == item[1]).FirstOrDefault();
                    //if(item[2] == "")
                    //{
                        //entity.CoverMemoRefNo = item[1];
                        entity.BankDrafNoIssued = item[2];
                        entity.SendMethod = item[3] == "" ? "" : item[3] == "SelfCollect" ? Data.SendingMethod.SelfCollect.ToString() : Data.SendingMethod.ByPos.ToString() ;
                        entity.PostageNo = item[4];
                        entity.ReceiverContactNo = item[5];
                        entity.BankDraftDate = item[6] != null ? Convert.ToDateTime(item[6]) : DateTime.Now;
                        Db.SetModified(entity);
                        Db.SaveChanges();

                        //if (memo.ReferenceNo != null)
                        //{
                        //    var listRefNo = memo.ReferenceNo.Split(", ").ToList();
                        //    if (!listRefNo.Contains(bdRefNo))
                        //        listRefNo.Add(bdRefNo);
                        //    memo.ReferenceNo = string.Join(", ", listRefNo);
                        //}
                        //else
                        //{
                        //    memo.ReferenceNo = bdRefNo;
                        //}
                        //Db.SetModified(entity);
                        //Db.SaveChanges();

                   
                        return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Application Saved Successful!" });
                   
                  
                    //return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Invalid Cover Memo Reference No!" });

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


        [Authorize(Roles = "Executive/Engineer, Manager/Senior Engineer, Head of Zone (AD)/HOZA, Senior Manager/Lead")]
        [HttpPost]
        public JsonResult SaveBankAcceptance(BankDraftViewModel model, IFormFile Evidence)
        {
            //if (ModelState.IsValid)
            //{
                try
                {
                    var file = Db.Attachment.Where(x => x.ParentId == Guid.Parse(model.Id) && x.FileType == Data.AttachmentType.Evidence.ToString()).FirstOrDefault(); 
                    if(model.UserAction == "Complete" && Evidence == null && file == null)
                    {
                        return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Evidence Document Required!" });
                    }

                    var user = _userManager.GetUserAsync(HttpContext.User).Result;

                    var entity = Db.BankDraft.Find(Guid.Parse(model.Id));
                    entity.ReceiveBankDraftDate = model.ReceiveBankDraftDate;
                    entity.ReceiptNo = model.ReceiptNo;
                    entity.UpdatedOn = DateTime.Now;
                    if (model.UserAction == "Complete")
                    {
                        entity.CompletedOn = DateTime.Now;
                        //entity.UpdatedOn = DateTime.Now;
                        entity.Status = Data.Status.Complete.ToString();
                    }
                    //else if (model.UserAction == "SaveComplete")
                    //{
                    //    entity.UpdatedOn = DateTime.Now;
                    //    entity.Status = Data.Status.SaveComplete.ToString();
                    //}

                    Db.SetModified(entity);
                    Db.SaveChanges();

                    var _BankDraftId = entity.Id;

                    if (Evidence != null)
                    {
                        UploadFile(Evidence, _BankDraftId, Data.AttachmentType.Evidence.ToString());
                    }

                    if (model.UserAction == "Complete")
                    {
                        Db.BankDraftAction.Add(new BankDraftAction
                        {
                            ActionType = Data.ActionType.Complete.ToString(),
                            On = DateTime.Now,
                            ById = user.Id,
                            ParentId = entity.Id,
                            ActionRole = Data.ActionRole.Requester.ToString(),
                        });
                        Db.SaveChanges();
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

        [HttpGet]
        public JsonResult RemoveAttachment(string Id)
        {
            try
            {
                var model = Db.Attachment.Where(x => x.Id == Guid.Parse(Id)).FirstOrDefault();
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
            if(fileName.Contains('%'))
            {
                fileName = fileName.Replace("%", "");
            }

            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }

        public string GetRunningNo()
        {
            RunningNo runningNo = new RunningNo();

            var entity = Db.RunningNo.Where(x => x.Name == "WangCagaran").FirstOrDefault(); //Id for Instruction Letter
            runningNo.Code = entity.Code;
            runningNo.RunNo = entity.RunNo;
            string NewCode = String.Format("{0}{1:00000}", runningNo.Code, runningNo.RunNo);

            entity.RunNo = entity.RunNo + 1;
            Db.RunningNo.Update(entity);
            Db.SaveChanges();

            return NewCode;
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

        [HttpGet]
        public JsonResult GetAllVerifier()
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
                                    DivType = r.Division.LOAType
                                }
                            )
                        .Where(x => x.Id == user.UserName)
                        .ToList();

            string verifierId = "M";
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
                      .Where(x => x.RoleId == verifierId && x.UnitId == requester[0].UnitId)
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
                     .Where(x => x.RoleId == verifierId && x.UnitId == requester[0].UnitId)
                     .ToList();

            //Wang cagaran less than 5 M
            if (requester[0].RoleId == "E")
            {
                verifierId = "M";

                foreach(var req in requester)
                {
                    final = Db.Users
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
                     .Where(x => x.RoleId == verifierId && x.UnitId == req.UnitId && x.active == true)
                     .ToList();

                    result = result.Concat(final).Distinct().ToList();
                }

            } //Wang cagaran more than 5M
            else if (requester[0].RoleId == "M")
            {
                if (requester[0].DivType == 1)
                {
                    //verifierId = "HZ";
                    verifierId = "SM";
                }
                else if (requester[0].DivType == 2)
                {
                    verifierId = "SM";
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
                      .Where(x => x.RoleId == verifierId && x.ZoneId == requester[0].ZoneId && x.active == true)
                      .ToList();
            }

            return new JsonResult(result.ToList());
        }

        //[HttpGet]
        //public JsonResult GetAllVerifier()
        //{
        //    var user = _userManager.GetUserAsync(HttpContext.User).Result;
        //    var requester = Db.Users.Join(Db.UserRoles,
        //                        u => u.Id,
        //                        r => r.UserId,
        //                        (u, r) => new
        //                        {
        //                            Id = u.Id,
        //                            RoleId = r.RoleId,
        //                            UnitId = r.UnitId,
        //                            ZoneId = r.ZoneId,
        //                            FunctionId = r.FunctionId,
        //                            DivisionId = r.DivisionId,
        //                            DivType = r.Division.LOAType
        //                        }
        //                    )
        //                .Where(x => x.Id == user.UserName)
        //                .FirstOrDefault();

        //    string verifierId = "M";
        //    var result = Db.Users
        //              .Join(Db.UserRoles,
        //                      u => u.Id,
        //                      r => r.UserId,
        //                      (u, r) => new
        //                      {
        //                          Id = u.Id,
        //                          Name = u.FullName,
        //                          RoleId = r.RoleId,
        //                          UnitId = r.UnitId,
        //                          ZoneId = r.ZoneId,
        //                          FunctionId = r.FunctionId,
        //                          DivisionId = r.DivisionId,
        //                          active = u.IsActive
        //                      }
        //                  )
        //              .Where(x => x.RoleId == verifierId && x.UnitId == requester.UnitId)
        //              .ToList();

        //    //Wang cagaran less than 5 M
        //    if (requester.RoleId == "E")
        //    {
        //        verifierId = "M";

        //        result = Db.Users
        //              .Join(Db.UserRoles,
        //                      u => u.Id,
        //                      r => r.UserId,
        //                      (u, r) => new
        //                      {
        //                          Id = u.Id,
        //                          Name = u.FullName,
        //                          RoleId = r.RoleId,
        //                          UnitId = r.UnitId,
        //                          ZoneId = r.ZoneId,
        //                          FunctionId = r.FunctionId,
        //                          DivisionId = r.DivisionId,
        //                          active = u.IsActive
        //                      }
        //                  )
        //              .Where(x => x.RoleId == verifierId && x.UnitId == requester.UnitId && x.active == true)
        //              .ToList();

        //    } //Wang cagaran more than 5M
        //    else if (requester.RoleId == "M")
        //    {
        //        if (requester.DivType == 1)
        //        {
        //            verifierId = "HZ";
        //        }
        //        else if (requester.DivType == 2)
        //        {
        //            verifierId = "SM";
        //        }

        //        result = Db.Users
        //              .Join(Db.UserRoles,
        //                      u => u.Id,
        //                      r => r.UserId,
        //                      (u, r) => new
        //                      {
        //                          Id = u.Id,
        //                          Name = u.FullName,
        //                          RoleId = r.RoleId,
        //                          UnitId = r.UnitId,
        //                          ZoneId = r.ZoneId,
        //                          FunctionId = r.FunctionId,
        //                          DivisionId = r.DivisionId,
        //                          active = u.IsActive
        //                      }
        //                  )
        //              .Where(x => x.RoleId == verifierId && x.ZoneId == requester.ZoneId && x.active == true)
        //              .ToList();
        //    }

        //    return new JsonResult(result.ToList());
        //}

        [HttpGet]
        public JsonResult GetAllVerifierForMore5M()
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
                                    DivType = r.Division.LOAType
                                }
                            )
                        .Where(x => x.Id == user.UserName)
                        .FirstOrDefault();

            string verifierId = "M";
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
                      .Where(x => x.RoleId == verifierId && x.UnitId == requester.UnitId)
                      .ToList();

            //Wang cagaran less than 5 M
            if (requester.RoleId == "E")
            {
                if (requester.DivType == 1)
                {
                    // verifierId = "HZ";
                    verifierId = "M";
                }
                else if (requester.DivType == 2)
                {
                    verifierId = "M";
    
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
                    .Where(x => x.RoleId == verifierId && x.ZoneId == requester.ZoneId && x.active == true)
                    .ToList();



            }
            else if (requester.RoleId == "M")
            {
                if (requester.DivType == 1)
                {
                    // verifierId = "HZ";
                    verifierId = "SM";
                }
                else if (requester.DivType == 2)
                {
                    verifierId = "SM";

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
                    .Where(x => x.RoleId == verifierId && x.ZoneId == requester.ZoneId && x.active == true)
                    .ToList();



            }

            return new JsonResult(result.ToList());
        }

        [HttpGet]
        public JsonResult GetAllBankDraftByStatus(string _Status = null)
        {
            var result = Db.BankDraft
             .Select(x => new { bdRefNo = x.RefNo, CoverMemoRefNo = x.CoverMemoRefNo, SendMethod = x.SendMethod, PostageNo = x.PostageNo, BankDraftNoIssued = x.BankDrafNoIssued, BankDraftDate = x.BankDraftDate, ContactNo = x.ReceiverContactNo, Status = x.Status, x.CreatedOn, x.TGBSProcessedOn, x.TGBSIssuedOn })
             .Where(x =>(x.Status == _Status || _Status == null))
             .OrderByDescending(x => x.TGBSProcessedOn)
                     .ToList();

            if (_Status == "Issued")
            {
                result = Db.BankDraft
               .Select(x => new { bdRefNo = x.RefNo, CoverMemoRefNo = x.CoverMemoRefNo, SendMethod = x.SendMethod, PostageNo = x.PostageNo, BankDraftNoIssued = x.BankDrafNoIssued, BankDraftDate = x.BankDraftDate, ContactNo = x.ReceiverContactNo, Status = x.Status, x.CreatedOn, x.TGBSProcessedOn, x.TGBSIssuedOn })
               .Where(x => (x.Status == "Issued" || x.Status == "Complete")
               && (x.TGBSIssuedOn != null))
               .OrderByDescending(x => x.TGBSIssuedOn)
                       .ToList();
            }
          
           

            return new JsonResult(result.ToList());
        }

        [HttpGet]
        public JsonResult GetAllWCApprover()
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
                      .Where(x => x.RoleId == approverId && x.ZoneId == requester.ZoneId && x.active == true)
                      .ToList();

            //
            if (requester.RoleId == "E")
                {
                    if(requester.DivType == 1)
                    {
                        approverId = "HZ";

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
                        .Where(x => (x.RoleId == "HZ" || x.RoleId == "PM" || x.RoleId == "SE") && x.ZoneId == requester.ZoneId && x.active == true)
                        .ToList();
                }
                    else if(requester.DivType == 2)
                    {
                        approverId = "SM";

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
                            .Where(x => x.RoleId == approverId && x.ZoneId == requester.ZoneId && x.active == true)
                            .ToList();
                     }


            }//
            else if (requester.RoleId == "M")
                {
                if (requester.DivType == 1)
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
                       .Where(x => (x.RoleId == "HZ" || x.RoleId == "PM" || x.RoleId == "SE") && x.ZoneId == requester.ZoneId && x.active == true)
                       .ToList();

                }
                else if (requester.DivType == 2)
                {
                    approverId = "GM/SGM";
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
                       .Where(x => (x.RoleId == "GM/SGM" || x.RoleId == "HOU") && x.FunctionId == requester.FunctionId && x.active == true)
                       .ToList();
                } 
            }

            return new JsonResult(result.ToList());
        }

        public JsonResult GetAllWCApproverForMore5M()
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
                      .Where(x => x.RoleId == approverId && x.ZoneId == requester.ZoneId && x.active == true)
                      .ToList();

            //Wang cagaran more than 5M for 
            if (requester.RoleId == "E")
            {
                if (requester.DivType == 1)
                {
                    approverId = "HZ";
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
                     .Where(x => x.RoleId == approverId && x.ZoneId == requester.ZoneId && x.active == true)
                     .ToList();

                }
                else if (requester.DivType == 2)
                {
                    approverId = "SM";
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
                       .Where(x => (x.RoleId == "SM") && x.FunctionId == requester.FunctionId && x.active == true)
                       .ToList();
                }
            }
            else if(requester.RoleId == "M")
            {
                if (requester.DivType == 1)
                {
                    approverId = "SM";
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
                     .Where(x => x.RoleId == "HZ" && x.ZoneId == requester.ZoneId && x.active == true)
                     .ToList();

                }
                else if (requester.DivType == 2)
                {
                    approverId = "GM/SGM";
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
                       .Where(x => (x.RoleId == "GM/SGM" || x.RoleId == "HOU") && x.FunctionId == requester.FunctionId && x.active == true)
                       .ToList();
                }
            }

            return new JsonResult(result.ToList());
        }

        public JsonResult GetAllWCApproverForManagerMore5M()
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
                      .Where(x => x.RoleId == approverId && x.ZoneId == requester.ZoneId && x.active == true)
                      .ToList();

            //Wang cagaran more than 5M for 
            if (requester.RoleId == "M")
            {
                if (requester.DivType == 1)
                {
                    approverId = "SM";
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
                     .Where(x => x.RoleId == approverId && x.ZoneId == requester.ZoneId && x.active == true)
                     .ToList();

                }
                else if (requester.DivType == 2)
                {
                    approverId = "GM/SGM";
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
                       .Where(x => (x.RoleId == "GM/SGM" || x.RoleId == "HOU") && x.FunctionId == requester.FunctionId && x.active == true)
                       .ToList();
                }
            }

            return new JsonResult(result.ToList());
        }

        [HttpGet]
        public JsonResult GetAllWHApprover()
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
                           .Where(x => x.RoleId == "SM" && x.DivisionId == requester.DivisionId && x.active == true)
                           .ToList();

            if (requester.DivType == 1)
            {
                if (requester.RoleId == "E" || requester.RoleId == "M" || requester.RoleId == "SM")
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
                           .Where(x => (((x.RoleId == "PM" && x.ZoneId == requester.ZoneId)|| x.RoleId == "HZ" && x.ZoneId == requester.ZoneId) && x.active == true))
                           .ToList();
                }
                //else if(requester.RoleId == "HZ")
                //{
                //    result = Db.Users
                //          .Join(Db.UserRoles,
                //                  u => u.Id,
                //                  r => r.UserId,
                //                  (u, r) => new
                //                  {
                //                      Id = u.Id,
                //                      Name = u.FullName,
                //                      RoleId = r.RoleId,
                //                      UnitId = r.UnitId,
                //                      ZoneId = r.ZoneId,
                //                      FunctionId = r.FunctionId,
                //                      DivisionId = r.DivisionId,
                //                      active = u.IsActive
                //                  }
                //              )
                //          .Where(x => (x.RoleId == "HOU" && x.FunctionId == requester.FunctionId && x.active == true))
                //          .ToList();
                //}
            }
            else if(requester.DivType == 2)
            {
                if (requester.RoleId == "E" || requester.RoleId == "M" || requester.RoleId == "SM")
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
                           .Where(x => ((x.RoleId == "GM/SGM" || x.RoleId == "HOU") && x.FunctionId == requester.FunctionId && x.active == true))
                           .ToList();
                }
            }
                return new JsonResult(result.ToList());
            }


        [HttpGet]
        public JsonResult GetAllWHApproverForManagerMore500k()
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
                           .Where(x => x.RoleId == "SM" && x.DivisionId == requester.DivisionId && x.active == true)
                           .ToList();

            if (requester.DivType == 1)
            {
                if (requester.RoleId == "M" || requester.RoleId == "PM" || requester.RoleId == "HOU")
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
                           .Where(x => (x.RoleId == "HZ" && x.FunctionId == requester.FunctionId && x.active == true))
                           .ToList();
                }
            }
            else if (requester.DivType == 2)
            {
                if (requester.RoleId == "E" || requester.RoleId == "M" || requester.RoleId == "SM")
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
                           .Where(x => ((x.RoleId == "GM/SGM" || x.RoleId == "HOU") && x.FunctionId == requester.FunctionId && x.active == true))
                           .ToList();
                }
            }
            return new JsonResult(result.ToList());
        }

        [HttpGet]
        public JsonResult GetAllState()
        {
            var result = Db.State.Select(s => new
            {
                Id = s.Id,
                Name = s.Name
            });

            return new JsonResult(result.ToList());
        }

        [HttpGet]
        public JsonResult GetAllCountry()
        {
            var result = Db.Country.Select(s => new
            {
                Id = s.Id,
                Name = s.Name
            });

            return new JsonResult(result.ToList());
        }

        [HttpGet]
        public JsonResult GetAllVendorNo()
        {

            var result = Db.VendorNo.Select(s => new
            {
                Id = s.Id,
                Name = s.Name
            }).OrderBy(s=> s.Name);

            return new JsonResult(result.ToList());
        }

        [HttpGet]
        public JsonResult GetAllCompanyCode()
        {
            var result = Db.Division.Select(s => new
            {
                Id = s.Id,
                Name = s.Name,
                Code = s.Code
            });

            return new JsonResult(result.ToList());
        }

        [HttpGet]
        public JsonResult GetBDAcceptanceList()
        {
            var result = Db.Recovery.Where(x => (x.RecoveryType == "FirstPartial" && x.Status == "Processed" && x.PartialSubmittedOn == null && x.ProcessedOn != null & x.ProcessedOn.Value.ToShortDateString() == DateTime.Now.AddDays(-1).ToShortDateString())).ToList();

            return new JsonResult(result.ToList());
        }

        [HttpGet]
        public JsonResult GetAllBusinessArea(string search = null, string coCode = null)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;
            var requester = Db.Users.Join(Db.UserRoles,
                                u => u.Id,
                                r => r.UserId,
                                (u, r) => new
                                {
                                    Id = u.Id,
                                    ZoneName = r.Zone.Name,
                                    DivisionId = r.Division.Id
                                }
                            )
                        .Where(x => x.Id == user.UserName)
                        .FirstOrDefault();

            //add correction for BA 09/09/2024
            var div = Db.Division
                .Where(x => x.Code == coCode)
                .Select(s => new
                {
                    Id = s.Id

                }).FirstOrDefault();

            var result = Db.BusinessArea
                .Where(x => x.DivisionId == div.Id && x.Name.Contains(search))
                .Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Name + " - " + s.Description

                });

            if(search == null)
            {
                result = Db.BusinessArea
                .Where(x => x.DivisionId == div.Id)
                .Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Name + " - " + s.Description
                });
            }

            return new JsonResult(result.ToList());
        }

        [HttpGet]
        public JsonResult GetSpecificCoCode(string coCode = null)
        {
            var result = Db.Division
              .Where(x => x.Code == coCode)
              .Select(s => new
              {
                  Id = s.Id,
                  Name = s.Name

              });

            return new JsonResult(result.FirstOrDefault());
        }

        [HttpGet]
        public JsonResult GetSpecificBusinessArea(string ba = null, string coCode = null)
        {

            var result = Db.BusinessArea
                .Join(Db.Division,
                                b => b.DivisionId,
                                d => d.Id,
                                (b, d) => new
                                {
                                    Id = b.Id,
                                    Name = b.Name,
                                    Description = b.Name + " - " + b.Description,
                                    DivName = d.Code
                                })
                                .Where(x => x.Name == ba && x.DivName == coCode);

                //.Where(x => x.Name == ba)
                //.Select(s => new
                //{
                //    Id = s.Id,
                //    Name = s.Name,
                //    Description = s.Name + " - " + s.Description

            //});

            return new JsonResult(result.FirstOrDefault());
        }


        //[HttpPost]
        //public JsonResult GetAllBusinessArea2()
        //{
        //    var user = _userManager.GetUserAsync(HttpContext.User).Result;
        //    var requester = Db.Users.Join(Db.UserRoles,
        //                        u => u.Id,
        //                        r => r.UserId,
        //                        (u, r) => new
        //                        {
        //                            Id = u.Id,
        //                            ZoneName = r.Zone.Name
        //                        }
        //                    )
        //                .Where(x => x.Id == user.UserName)
        //                .FirstOrDefault();

        //    var result = Db.BusinessArea
        //        .Where(x => x.Zone == requester.ZoneName)
        //        .Select(s => new
        //        {
        //            Id = s.Id,
        //            Name = s.Name
        //        });

        //    return new JsonResult(result.ToList());
        //}

        //[HttpGet]
        //public JsonResult GetAllBusinessAreaBasedOnCompanyCode(string coCode = null)
        //{
        //    var user = _userManager.GetUserAsync(HttpContext.User).Result;
        //    var requester = Db.Users.Join(Db.UserRoles,
        //                        u => u.Id,
        //                        r => r.UserId,
        //                        (u, r) => new
        //                        {
        //                            Id = u.Id,
        //                            ZoneName = r.Zone.Name,
        //                        }
        //                    )
        //                .Where(x => x.Id == user.UserName)
        //                .FirstOrDefault();

        //    //var compCode = Db.Division.Where(x => x.Name.Contains(coCode)).FirstOrDefault();

        //    var result = Db.BusinessArea
        //        .Where(x => x.Zone == requester.ZoneName && x.DivisionId == Guid.Parse(coCode))
        //        .Select(s => new
        //        {
        //            Id = s.Id,
        //            Name = s.Name
        //        });

        //    return new JsonResult(result.ToList());
        //}

        public JsonResult GetAllSendMethod()
        {
            //var result = Enum.GetValues(typeof(Data.SendingMethod))
            //               .Cast<Data.SendingMethod>()
            //               .Select(t => new
            //               {
            //                   Id = ((int)t),
            //                   Name = t.ToString()
            //               }).ToList();

            var result = Db.SendMethod.Select(s => new
            {
                Id = s.Id,
                Name = s.Type
            })
           .OrderBy(x => x.Name);

            return new JsonResult(result.ToList());
        }

        public JsonResult GetAllVendorType()
        {
            var result = Enum.GetValues(typeof(Data.VendorType))
                           .Cast<Data.VendorType>()
                           .Select(t => new
                           {
                               Id = t.ToString(),
                               Name = t.ToString()
                           }).ToList();

            return new JsonResult(result.ToList());
        }

        public JsonResult GetAllStatus()
        {
            var result = Enum.GetValues(typeof(Data.Status))
                           .Cast<Data.Status>()
                           .Select(t => new
                           {
                               Id = t.ToString(),
                               Name = t.ToString()
                           }).ToList();

            return new JsonResult(result.ToList());
        }

        //recovery
        [HttpGet]
        public JsonResult SearchBankDraftByProjectNo(string search = null)
        {
            var result = Db.BankDraft.Join(Db.WangCagaran,
                         b => b.Id,
                         w => w.BankDraftId,
                         (b, w) => new
                         {
                             bdNo = b.BankDrafNoIssued,
                             requester = b.Requester.FullName,
                             ermsDocNo = w.ErmsDocNo,
                             coCode = w.CoCode,
                             ba = w.BusinessArea,
                             nameOnBD = w.NamaPemegangCagaran,
                             bdAmount = b.BankDraftAmount,
                             ermsPostingDate = b.TGBSAcceptedOn.Value.ToShortDateString(),
                             docDate = b.SubmittedOn.Value.ToShortDateString(),
                             projNo = b.ProjectNo,
                             refNo = b.RefNo,
                             bankDraftId = b.Id,
                             status = b.Status,
                             assignment = w.Assignment,
                             keteranganKerja = w.KeteranganKerja,
                             finalApp = b.FinalApplication
                         })
                         .Where(x => ((x.projNo.Contains(search) || x.assignment.Contains(search)) && x.status.Equals("Complete") && x.finalApp != "Recovery" && x.finalApp != "Cancellation"));

         
            return new JsonResult(result.ToList());
        }


        [HttpGet]
        public JsonResult SearchBankDraftByBDNo(string search = null)
        {
            var result = Db.BankDraft.Join(Db.WangCagaran,
                         b => b.Id,
                         w => w.BankDraftId,
                         (b, w) => new
                         {
                             bdNo = b.BankDrafNoIssued,
                             requester = b.Requester.FullName,
                             ermsDocNo = w.ErmsDocNo,
                             coCode = w.CoCode,
                             ba = w.BusinessArea,
                             nameOnBD = w.NamaPemegangCagaran,
                             bdAmount = b.BankDraftAmount,
                             ermsPostingDate = b.TGBSAcceptedOn.Value.ToShortDateString(),
                             docDate = b.SubmittedOn.Value.ToShortDateString(),
                             projNo = b.ProjectNo,
                             refNo = b.RefNo,
                             bankDraftId = b.Id,
                             status = b.Status,
                             assignment = w.Assignment,
                             keteranganKerja = w.KeteranganKerja,
                             finalApp = b.FinalApplication
                         })
                         .Where(x => (x.bdNo.Contains(search) && x.status.Equals("Complete") && x.finalApp != "Recovery" && x.finalApp != "Cancellation"));

            return new JsonResult(result.ToList());
        }

        //cancellation & lost
        [HttpGet]
        public JsonResult SearchBankDraftByProjectNonBDNo(string search = null)
        {
            var result = Db.BankDraft.Join(Db.WangCagaran,
                         b => b.Id,
                         w => w.BankDraftId,
                         (b, w) => new
                         {
                             bdNo = b.BankDrafNoIssued,
                             requester = b.Requester.FullName,
                             ermsDocNo = w.ErmsDocNo,
                             coCode = w.CoCode,
                             ba = w.BusinessArea,
                             nameOnBD = w.NamaPemegangCagaran,
                             bdAmount = b.BankDraftAmount,
                             ermsPostingDate = b.TGBSAcceptedOn.Value.ToShortDateString(),
                             docDate = b.SubmittedOn.Value.ToShortDateString(),
                             projNo = b.ProjectNo,
                             refNo = b.RefNo,
                             bankDraftId = b.Id,
                             status = b.Status,
                             assignment = w.Assignment,
                             keteranganKerja = w.KeteranganKerja,
                             finalApp = b.FinalApplication
                         })
                           //.Where(x => (x.projNo == search || x.assignment == search || x.bdNo == search) && x.status.Equals("Complete"));
                           //.Where(x => (x.projNo == search || x.assignment == search || x.bdNo == search || x.refNo == search) && x.status.Equals("Complete") && x.finalApp != "Recovery" && x.finalApp != "Cancellation" && x.finalApp != "Lost");
                           .Where(x => (x.projNo.Contains(search) || x.assignment.Contains(search) || x.bdNo.Contains(search) || x.refNo.Contains(search)) && x.status.Equals("Complete") && x.finalApp != "Recovery" && x.finalApp != "Cancellation" && x.finalApp != "Lost");

            var result2 = Db.BankDraft.Join(Db.WangHangus,
                         b => b.Id,
                         w => w.BankDraftId,
                         (b, w) => new
                         {
                             bdNo = b.BankDrafNoIssued,
                             requester = b.Requester.FullName,
                             ermsDocNo = w.ErmsDocNo,
                             coCode = w.CoCode,
                             ba = w.BusinessArea,
                             nameOnBD = w.VendorName,
                             bdAmount = b.BankDraftAmount,
                             ermsPostingDate = b.TGBSAcceptedOn.Value.ToShortDateString(),
                             docDate = b.SubmittedOn.Value.ToShortDateString(),
                             projNo = b.ProjectNo,
                             refNo = b.RefNo,
                             bankDraftId = b.Id,
                             status = b.Status,
                             assignment = "",
                             keteranganKerja = "",
                            finalApp = b.FinalApplication
                         })
                //.Where(x => ((x.projNo == search || x.bdNo == search )&& x.status.Equals("Complete")));
                .Where(x => ((x.projNo == search || x.bdNo == search) && x.status.Equals("Complete") && x.finalApp != "Recovery" && x.finalApp != "Cancellation" && x.finalApp != "Lost"));
            return new JsonResult(result.Concat(result2).ToList());
        }
    }
}