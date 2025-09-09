using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BDA.Entities;
//using BDA.Services;
using BDA.FileStorage;
using BDA.Identity;
using BDA.ViewModel;
using BDA.Web.Controllers;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BDA.Controllers
{
    [Authorize]
    public class WangCagaranHangusController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public WangCagaranHangusController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        ////private BankDraftApplicationService service;
        //private readonly IHttpContextAccessor _contextAccessor;
        //public WangCagaranController( IFileStore fileStore, IHttpContextAccessor _contextAccessor) 
        //{
        //    //this.service = service;
        //    this._contextAccessor = _contextAccessor;
        //}

        public ActionResult Index()
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
        //[Route("wangcagaranhangus/createform")]
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
                entity.Type = Data.BDType.WangCagaranHangus.ToString();
                entity.BankDraftAmount = model.WangCagaranHangusViewModel.Jumlah;
                entity.NameOnBD = model.WangCagaranHangusViewModel.NamaPemegangCagaran;
                entity.ProjectNo = model.WangCagaranHangusViewModel.WBSProjekNo;
                entity.RefNo = GetRunningNo();
                entity.Status = model.Status == "Submit" ? Data.Status.Submitted.ToString() : Data.Status.Draft.ToString();
                entity.RequesterSubmissionComment = model.Comment;
                entity.FinalApplication = "Application";

                Db.BankDraft.Add(entity);
                Db.SaveChanges();

                var _BankDraftId = entity.Id;
                var wangCHModel = model.WangCagaranHangusViewModel;

                WangCagaranHangus wang = new WangCagaranHangus();
                wang.BankDraftId = _BankDraftId;
                wang.ErmsDocNo = wangCHModel.ErmsDocNo;
                wang.Pemula = user.FullName;
                wang.Tarikh = DateTime.Now;
                wang.Alamat1 = wangCHModel.Alamat1;
                wang.Alamat2 = wangCHModel.Alamat2;
                wang.Bandar = wangCHModel.Bandar;
                wang.Poskod = wangCHModel.Poskod;
                wang.Negeri = wangCHModel.Negeri;
                wang.KeteranganKerja = wangCHModel.KeteranganKerja;
                wang.JKRInvolved = wangCHModel.JKRInvolved;
                wang.JKRType = wangCHModel.JKRType;
                wang.Jumlah = wangCHModel.Jumlah;
                wang.CajKod = wangCHModel.CajKod;
                wang.NamaPemegangCagaran = wangCHModel.NamaPemegangCagaran;
                wang.WBSProjekNo = wangCHModel.WBSProjekNo;
                wang.BusinessArea = wangCHModel.BusinessArea;
                wang.CoCode = wangCHModel.CoCode;
                wang.RMCagaran = wangCHModel.RMCagaran;
                wang.RMHangus = wangCHModel.RMHangus;
                wang.GL = wangCHModel.GL;

                Db.WangCagaranHangus.Add(wang);
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

                return Json(new { wanghangusId = wang.Id, response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Application Saved Successful!" });
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
            var model = new BankDraftViewModel();
            var item = Db.BankDraft.Where(x => x.Id == Guid.Parse(Id)).FirstOrDefault();
            if (item != null)
            {
                //Declare Bank Draft Details
                model.Id = item.Id.ToString();
                model.RefNo = item.RefNo;
                model.VerifierId = item.VerifierId;
                model.ApproverId = item.ApproverId;
                model.RequesterId = item.RequesterId;
                model.InstructionLetterRefNo = item.InstructionLetterRefNo;
                model.CoverMemoRefNo = item.CoverMemoRefNo;
                model.SendMethod = item.SendMethod;
                model.PostageNo = item.PostageNo;
                model.ReceiverContactNo = item.ReceiverContactNo;
                model.BankDrafNoIssued = item.BankDrafNoIssued;
                model.BankDraftDate = item.BankDraftDate;
                model.ReceiveBankDraftDate = item.ReceiveBankDraftDate;
                model.ReceiptNo = item.ReceiptNo;
                model.Status = item.Status;

                //Declare Wang Cagaran Hangus Details
                var _wang = Db.WangCagaranHangus.Where(x => x.BankDraftId == item.Id).FirstOrDefault();
                if (_wang != null)
                {
                    var wang = new WangCagaranHangusViewModel();
                    wang.Id = _wang.Id.ToString();
                    wang.BankDraftId = _wang.BankDraftId;
                    wang.ErmsDocNo = _wang.ErmsDocNo == null ? "w" : _wang.ErmsDocNo;
                    wang.PostingDate = _wang.PostingDate;
                    wang.Pemula = _wang.Pemula;
                    wang.Tarikh = DateTime.Parse(_wang.Tarikh.ToString());
                    wang.Alamat1 = _wang.Alamat1;
                    wang.Alamat2 = _wang.Alamat2;
                    wang.Bandar = _wang.Bandar;
                    wang.Poskod = _wang.Poskod;
                    wang.Negeri = _wang.Negeri;
                    wang.KeteranganKerja = _wang.KeteranganKerja;
                    wang.JKRInvolved = _wang.JKRInvolved ?? false;
                    wang.JKRType = _wang.JKRType;
                    wang.Jumlah = _wang.Jumlah;
                    wang.CajKod = _wang.CajKod;
                    wang.WBSProjekNo = _wang.WBSProjekNo;
                    wang.NamaPemegangCagaran = _wang.NamaPemegangCagaran;
                    wang.BusinessArea = _wang.BusinessArea;
                    wang.ErmsDocNo = _wang.ErmsDocNo;
                    wang.CoCode = _wang.CoCode;
                    wang.RMCagaran = _wang.RMCagaran;
                    wang.RMHangus = _wang.RMHangus;
                    wang.GL = _wang.GL;

                    model.WangCagaranHangusViewModel = wang;
                }

                ViewBag.VerifierId = Db.Users.Find(model.VerifierId) != null ? Db.Users.Find(model.VerifierId).FullName : "";
                ViewBag.ApproverId = Db.Users.Find(model.ApproverId) != null ? Db.Users.Find(model.ApproverId).FullName : "";

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
                entity.BankDraftAmount = model.WangCagaranHangusViewModel.Jumlah;
                entity.NameOnBD = model.WangCagaranHangusViewModel.NamaPemegangCagaran;
                entity.ProjectNo = model.WangCagaranHangusViewModel.WBSProjekNo;
                entity.RequesterSubmissionComment = model.Comment;

                if (entity.Status == "RejectedVerify" || entity.Status == "RejectedApprove" || entity.Status == "Declined")
                {
                    checkFirstTimeSubmit = false;
                }
                entity.Status = model.Status == "Submit" ? Data.Status.Submitted.ToString() : Data.Status.Draft.ToString();

                Db.SetModified(entity);
                Db.SaveChanges();

                var _BankDraftId = entity.Id;
                var wangCagaranHangusModel = model.WangCagaranHangusViewModel;
                var wang = Db.WangCagaranHangus.Find(Guid.Parse(model.WangCagaranHangusViewModel.Id));
                wang.ErmsDocNo = wangCagaranHangusModel.ErmsDocNo;
                //wang.PostingDate = wangCagaranModel.PostingDate;
                wang.Pemula = wangCagaranHangusModel.Pemula;
                if (checkFirstTimeSubmit == false)
                {
                    wang.Tarikh = wangCagaranHangusModel.Tarikh;
                }
                else
                {
                    wang.Tarikh = model.Status == "Submit" ? DateTime.Now : (DateTime?)null;
                }
                wang.Alamat1 = wangCagaranHangusModel.Alamat1;
                wang.Alamat2 = wangCagaranHangusModel.Alamat2;
                wang.Bandar = wangCagaranHangusModel.Bandar;
                wang.Poskod = wangCagaranHangusModel.Poskod;
                wang.Tarikh = wangCagaranHangusModel.Tarikh;
                wang.Negeri = wangCagaranHangusModel.Negeri;
                wang.BusinessArea = wangCagaranHangusModel.BusinessArea;
                wang.KeteranganKerja = wangCagaranHangusModel.KeteranganKerja;
                wang.JKRInvolved = wangCagaranHangusModel.JKRInvolved;
                wang.JKRType = wangCagaranHangusModel.JKRType;
                wang.Jumlah = wangCagaranHangusModel.Jumlah;
                wang.CajKod = wangCagaranHangusModel.CajKod;
                wang.NamaPemegangCagaran = wangCagaranHangusModel.NamaPemegangCagaran;
                wang.WBSProjekNo = wangCagaranHangusModel.WBSProjekNo;
                wang.CoCode = wangCagaranHangusModel.CoCode;
                wang.RMCagaran = wangCagaranHangusModel.RMCagaran;
                wang.RMHangus = wangCagaranHangusModel.RMHangus;
                wang.GL = wangCagaranHangusModel.GL;
            
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

        public string GetRunningNo()
        {
            RunningNo runningNo = new RunningNo();

            var entity = Db.RunningNo.Where(x => x.Name == "WangCagaranHangus").FirstOrDefault(); //Id for Instruction Letter
            runningNo.Code = entity.Code;
            runningNo.RunNo = entity.RunNo;
            string NewCode = String.Format("{0}{1:00000}", runningNo.Code, runningNo.RunNo);

            entity.RunNo = entity.RunNo + 1;
            Db.RunningNo.Update(entity);
            Db.SaveChanges();

            return NewCode;
        }

        public IActionResult Apply()
        {
            return View();
        }



        public IActionResult Verify()
        {
            return View();
        }

        public IActionResult Submit()
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


        public IActionResult Issue()
        {
            return View();
        }

        public IActionResult Complete()
        {
            return View();
        }

    }
}