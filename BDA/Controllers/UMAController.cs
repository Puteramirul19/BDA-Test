using BDA.Entities;
using BDA.Identity;
using BDA.ViewModel;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BDA.Web.Controllers
{
    [Authorize]
    public class UMAController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UMAController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List()
        {
            return View();
        }

        public IActionResult BankDraftList()
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
        public JsonResult Create(UMAViewModel model, IFormFile ScannedPoliceReport, IFormFile ScannedPBTDoc, List<IFormFile> OthersDoc, List<string> OthersDocName)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Status == "Submit" && ScannedPoliceReport == null)
                    {
                        return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Scanned Police Report & Memo Required!" });
                    }

                    var user = _userManager.GetUserAsync(HttpContext.User).Result;

                    UMA entity = new UMA();
                    entity.CreatedById = user.Id;
                    entity.CreatedByName = user.FullName;
                    entity.RequesterId = user.Id;
                    entity.ApproverId = model.ApproverId;
                    entity.DraftedOn = DateTime.Now;
                    entity.BankDraftId = Guid.Parse(model.BankDraftId);
                    entity.SubmittedOn = model.Status == "Submit" ? DateTime.Now : (DateTime?)null;
                    entity.Status = model.Status == "Submit" ? "Submitted" : "Draft";
                    entity.RefNo = GetRunningNo();
                    entity.BDNo = model.BDNo;
                    entity.BDAmount = model.BDAmount;
                    entity.InstructionLetterRefNo = model.InstructionLetterRefNo;
                    entity.BDRequesterName = model.BDRequesterName;
                    entity.ERMSDocNo = model.ERMSDocNo;
                    entity.CoCode = model.CoCode;
                    entity.BA = model.BA;
                    entity.NameOnBD = model.NameOnBD;
                    entity.ProjectNo = model.ProjectNo;
                    entity.Justification = model.Justification;
                    entity.ReceivedDate = model.ReceivedDate;

                    Db.UMA.Add(entity);
                    Db.SaveChanges();

                    if (ScannedPoliceReport != null)
                    {
                        UploadFile(ScannedPoliceReport, entity.Id, Data.AttachmentType.UMA.ToString(), Data.BDAttachmentType.PoliceReport.ToString());
                    }

                    if (ScannedPBTDoc != null)
                    {
                        UploadFile(ScannedPBTDoc, entity.Id, Data.AttachmentType.UMA.ToString(), Data.BDAttachmentType.PBTDoc.ToString());
                    }

                    if (OthersDoc != null)
                    {
                        for (int i = 0; i < OthersDoc.Count; i++)
                        {
                            var doc = OthersDoc[i];
                            var title = OthersDocName != null && i < OthersDocName.Count ? OthersDocName[i] : "";
                            if (doc != null)
                            {
                                UploadFile(doc, entity.Id, Data.AttachmentType.UMA.ToString(), "Others", title);
                            }
                        }
                    }

                    return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "UMA Request " + model.Status + " Successfully!" });
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

        [HttpPost]
        public JsonResult NextAction(UMAViewModel model)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            try
            {
                switch (model.UserAction)
                {
                    case "Withdrawn":
                        model.Status = "Withdrawn";
                        break;
                    case "Approve":
                        model.Status = "Approved";
                        break;
                    case "RejectApprove":
                        model.Status = "Rejected";
                        break;
                    case "Accept":
                        model.Status = "Accepted";
                        break;
                    case "Decline":
                        model.Status = "Declined";
                        break;
                    case "Resubmit":
                        model.Status = "Submitted";
                        break;
                    default:
                        model.Status = "Invalid";
                        break;
                }

                if (model.Status != "Invalid")
                {
                    var entity = Db.UMA.Find(Guid.Parse(model.Id));
                    entity.Status = model.Status;
                    entity.UpdatedOn = DateTime.Now;

                    Db.SetModified(entity);
                    Db.SaveChanges();

                    return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "UMA Request " + model.UserAction + " Successfully!" });
                }

                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Invalid Action!" });
            }
            catch (Exception e)
            {
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });
            }
        }

        [HttpGet]
        public IActionResult Edit(string Id)
        {
            var model = new UMAViewModel();
            var item = Db.UMA.Where(x => x.Id == Guid.Parse(Id)).FirstOrDefault();
            if (item != null)
            {
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
                model.ProjectNo = item.ProjectNo;
                model.Justification = item.Justification;
                model.ReceivedDate = item.ReceivedDate;

                ViewBag.ApproverId = Db.Users.Find(model.ApproverId) != null ? Db.Users.Find(model.ApproverId).FullName : "";

                model.ScannedPoliceReportVM = new AttachmentViewModel();
                model.ScannedPBTDocVM = new AttachmentViewModel();
                model.SignedLetterVM = new AttachmentViewModel();
                model.SignedIndemningFormVM = new AttachmentViewModel();
                model.BankStatementVM = new AttachmentViewModel();

                var _scannedPoliceReport = Db.Attachment.Where(x => x.ParentId == item.Id && x.FileType == Data.AttachmentType.UMA.ToString() && x.FileSubType == Data.BDAttachmentType.PoliceReport.ToString()).FirstOrDefault();
                if (_scannedPoliceReport != null)
                {
                    model.ScannedPoliceReportVM.Id = _scannedPoliceReport.Id.ToString();
                    model.ScannedPoliceReportVM.FileName = _scannedPoliceReport.FileName;
                }

                var _scannedPBTDoc = Db.Attachment.Where(x => x.ParentId == item.Id && x.FileType == Data.AttachmentType.UMA.ToString() && x.FileSubType == Data.BDAttachmentType.PBTDoc.ToString()).FirstOrDefault();
                if (_scannedPBTDoc != null)
                {
                    model.ScannedPBTDocVM.Id = _scannedPBTDoc.Id.ToString();
                    model.ScannedPBTDocVM.FileName = _scannedPBTDoc.FileName;
                }

                return View(model);
            }
            return View();
        }

        [HttpPost]
        public JsonResult Edit(UMAViewModel model, IFormFile ScannedPoliceReport, IFormFile ScannedPBTDoc, List<IFormFile> OthersDoc, List<string> OthersDocName)
        {
            bool checkFirstTimeSubmit = true;

            if (ModelState.IsValid)
            {
                try
                {
                    var _scannedPoliceReport = Db.Attachment.Where(x => x.ParentId == Guid.Parse(model.Id) && x.FileType == Data.AttachmentType.UMA.ToString() && x.FileSubType == Data.BDAttachmentType.PoliceReport.ToString()).FirstOrDefault();
                    if (_scannedPoliceReport == null)
                    {
                        if (model.Status == "Submit" && ScannedPoliceReport == null)
                        {
                            return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Scanned Police Report & Memo Required!" });
                        }
                    }

                    var entity = Db.UMA.Where(x => x.Id == Guid.Parse(model.Id)).FirstOrDefault();
                    if (entity != null)
                    {
                        if (entity.Status == "Draft" && model.Status == "Submit")
                        {
                            entity.SubmittedOn = DateTime.Now;
                            entity.Status = "Submitted";
                            checkFirstTimeSubmit = true;
                        }
                        else
                        {
                            checkFirstTimeSubmit = false;
                        }

                        entity.ApproverId = model.ApproverId;
                        entity.Justification = model.Justification;
                        entity.Comment = model.Comment;
                        entity.UpdatedOn = DateTime.Now;

                        Db.UMA.Update(entity);
                        Db.SaveChanges();

                        if (ScannedPoliceReport != null)
                        {
                            if (_scannedPoliceReport != null)
                            {
                                Db.Attachment.Remove(_scannedPoliceReport);
                                Db.SaveChanges();
                            }
                            UploadFile(ScannedPoliceReport, entity.Id, Data.AttachmentType.UMA.ToString(), Data.BDAttachmentType.PoliceReport.ToString());
                        }

                        if (ScannedPBTDoc != null)
                        {
                            var _scannedPBTDoc = Db.Attachment.Where(x => x.ParentId == entity.Id && x.FileType == Data.AttachmentType.UMA.ToString() && x.FileSubType == Data.BDAttachmentType.PBTDoc.ToString()).FirstOrDefault();
                            if (_scannedPBTDoc != null)
                            {
                                Db.Attachment.Remove(_scannedPBTDoc);
                                Db.SaveChanges();
                            }
                            UploadFile(ScannedPBTDoc, entity.Id, Data.AttachmentType.UMA.ToString(), Data.BDAttachmentType.PBTDoc.ToString());
                        }

                        if (OthersDoc != null)
                        {
                            for (int i = 0; i < OthersDoc.Count; i++)
                            {
                                var doc = OthersDoc[i];
                                var title = OthersDocName != null && i < OthersDocName.Count ? OthersDocName[i] : "";
                                if (doc != null)
                                {
                                    UploadFile(doc, entity.Id, Data.AttachmentType.UMA.ToString(), "Others", title);
                                }
                            }
                        }

                        return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "UMA Request Updated Successfully!" });
                    }

                    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "UMA Request not found!" });
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

        public void UploadFile(IFormFile file, Guid parentId, string fileType, string fileSubType = null, string title = null)
        {
            var uniqueFileName = GetUniqueFileName(file.FileName);
            var ext = Path.GetExtension(uniqueFileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/documents", uniqueFileName);
            file.CopyTo(new FileStream(filePath, FileMode.Create));

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

        public string GetRunningNo()
        {
            RunningNo runningNo = new RunningNo();

            var entity = Db.RunningNo.Where(x => x.Name == "UMA").FirstOrDefault();
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
        public JsonResult GetAllUMAByStatus(string _Status = null)
        {
            var result = Db.UMA
                .Select(x => new {
                    id = x.Id,
                    refNo = x.RefNo,
                    bdNo = x.BDNo,
                    projNo = x.ProjectNo,
                    nameOnBD = x.NameOnBD,
                    requester = x.Requester.FullName,
                    compCode = x.CoCode,
                    ba = x.BA,
                    bdAmount = x.BDAmount,
                    receivedDate = x.ReceivedDate,
                    Status = x.Status,
                    actionNeeded = x.Status == "Submitted" ? "Approval" : x.Status == "Processing" ? "Review" : "None"
                })
                .Where(x =>
                        (x.Status == _Status || _Status == null)
                        )
                        .ToList();

            return new JsonResult(result.ToList());
        }

        [HttpGet]
        public JsonResult GetMyRequestUMA(string refNo = null, string requesterName = null, string BDNo = null, string projectNo = null, string amount = null, string submitDate = null, string status = null, string updatedOn = null)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            var result = Db.UMA
                .Select(x => new {
                    id = x.Id,
                    refNo = x.RefNo,
                    requesterName = x.Requester.FullName,
                    bdNo = x.BDNo,
                    projectNo = x.ProjectNo,
                    amount = x.BDAmount,
                    applicationType = "UMA",
                    submitDate = x.SubmittedOn,
                    status = x.Status,
                    updatedOn = x.UpdatedOn
                })
                .Where(x => x.requesterName == user.FullName &&
                           (refNo == null || x.refNo.Contains(refNo)) &&
                           (requesterName == null || x.requesterName.Contains(requesterName)) &&
                           (BDNo == null || x.bdNo.Contains(BDNo)) &&
                           (projectNo == null || x.projectNo.Contains(projectNo)) &&
                           (status == null || x.status == status))
                .ToList();

            return new JsonResult(result.ToList());
        }

        [HttpGet]
        public JsonResult GetAllUMAApprover()
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;
            var requester = Db.Users.Join(Db.UserRoles,
                                u => u.Id,
                                r => r.UserId,
                                (u, r) => new
                                {
                                    UserId = u.Id,
                                    UserName = u.FullName,
                                    Unit = u.Unit,
                                    Division = u.Division,
                                    UserRole = r.RoleId,
                                    IsActive = u.IsActive
                                }
                                )
                                .Where(x => x.UserRole == "Manager/Senior Engineer" && x.IsActive == true).ToList();

            return new JsonResult(requester.ToList());
        }
    }
}