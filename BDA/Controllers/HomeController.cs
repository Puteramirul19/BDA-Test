using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BDA.Entities;
using BDA.Identity;
using BDA.Models;
using BDA.ViewModel;
using BDA.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BDA.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;

        //const string SessionName = "_Name";
        public HomeController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public JsonResult GetAll5RecentTask(Guid? id = null, string refNo = null, string BDNo = null,
            string projectNo = null, string applicationType = null,
            string submitDate = null, string status = null, string updatedOn = null)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            var bd2 = Db.BankDraft.AsEnumerable() // store in memory for post-processing during select
                .Select(x =>
                    new
                    {
                        x.Id,
                        refNo = x.RefNo,
                        BDNo = x.BankDrafNoIssued,
                        projectNo = x.ProjectNo,
                        applicationType = x.Type,
                        submitDate = x.SubmittedOn.HasValue ? x.SubmittedOn.Value.ToShortDateString() : "",
                        x.Status,
                        updatedOn = x.UpdatedOn.ToShortDateString(),
                        x.CreatedById,
                        x.VerifierId,
                        x.ApproverId,
                        x.SubmittedOn,
                        //x.VerifiedOn,
                        //x.ApprovedOn,
                        //x.TGBSAcceptedOn,
                    })
                .Where(x => ((x.Id == id) || id == null)
                            && (x.refNo != null && refNo != null ? x.refNo.Contains(refNo) : refNo == null)
                            && (x.BDNo != null && BDNo != null ? x.BDNo.Contains(BDNo) : BDNo == null)
                            && (x.projectNo != null && projectNo != null
                                ? x.projectNo.Contains(projectNo)
                                : projectNo == null)
                            && (x.applicationType != null && applicationType != null
                                ? x.applicationType.Contains(applicationType)
                                : applicationType == null)
                            && (x.Status == status || status == null)
                            && (x.updatedOn == updatedOn || string.IsNullOrEmpty(updatedOn))
                            && (x.submitDate == submitDate || string.IsNullOrEmpty(submitDate))
                ).OrderByDescending(x => x.updatedOn).Take(6).ToList();

            var parsedUpdatedOn = DateTime.TryParse(updatedOn, out DateTime parsedUpdatedDate)
                ? (DateTime?)parsedUpdatedDate
                : null;
            var parsedSubmitDate = DateTime.TryParse(submitDate, out DateTime parsedSubmitDateVal)
                ? (DateTime?)parsedSubmitDateVal
                : null;

            var bd = Db.BankDraft
                .Where(x => (id == null || x.Id == id)
                            && (string.IsNullOrEmpty(refNo) || x.RefNo.Contains(refNo))
                            && (string.IsNullOrEmpty(BDNo) || x.BankDrafNoIssued.Contains(BDNo))
                            && (string.IsNullOrEmpty(projectNo) || x.ProjectNo.Contains(projectNo))
                            && (string.IsNullOrEmpty(applicationType) || x.Type.Contains(applicationType))
                            && (status == null || x.Status == status)
                            && (!parsedUpdatedOn.HasValue || x.UpdatedOn.Date == parsedUpdatedOn.Value.Date)
                            && (!parsedSubmitDate.HasValue || x.SubmittedOn.HasValue &&
                                x.SubmittedOn.Value.Date == parsedSubmitDate.Value.Date)
                )
                .OrderByDescending(x => x.UpdatedOn)
                .Take(6)
                .Select(x => new
                {
                    x.Id,
                    refNo = x.RefNo,
                    BDNo = x.BankDrafNoIssued,
                    projectNo = x.ProjectNo,
                    applicationType = x.Type,
                    submitDate = x.SubmittedOn.HasValue ? x.SubmittedOn.Value.ToShortDateString() : "",
                    x.Status,
                    updatedOn = x.UpdatedOn.ToShortDateString(),
                    x.CreatedById,
                    x.VerifierId,
                    x.ApproverId,
                    x.SubmittedOn
                })
                .ToList();


            var canc = Db.Cancellation.AsEnumerable() // store in memory for post-processing during select
                .Select(x =>
                    new
                    {
                        x.Id,
                        refNo = x.RefNo,
                        BDNo = x.BDNo,
                        projectNo = x.ProjectNo,
                        applicationType = "Cancellation",
                        submitDate = x.SubmittedOn.HasValue ? x.SubmittedOn.Value.ToShortDateString() : "",
                        x.Status,
                        updatedOn = x.UpdatedOn.ToShortDateString(),
                        x.CreatedById,
                        VerifierId = "",
                        x.ApproverId,
                        x.SubmittedOn,
                    })
                .Where(x => ((x.Id == id) || id == null)
                            && (x.refNo != null && refNo != null ? x.refNo.Contains(refNo) : refNo == null)
                            && (x.BDNo != null && BDNo != null ? x.BDNo.Contains(BDNo) : BDNo == null)
                            && (x.projectNo != null && projectNo != null
                                ? x.projectNo.Contains(projectNo)
                                : projectNo == null)
                            && (x.applicationType != null && applicationType != null
                                ? x.applicationType.Contains(applicationType)
                                : applicationType == null)
                            && (x.Status == status || status == null)
                            && (x.updatedOn == updatedOn || string.IsNullOrEmpty(updatedOn)) // Remove .ToString()
                            && (x.submitDate == submitDate ||
                                string.IsNullOrEmpty(submitDate)) // Remove .ToString()
                ).OrderByDescending(x => x.updatedOn).Take(6).ToList();

            var lost = Db.Lost.AsEnumerable() // store in memory for post-processing during select
                .Select(x =>
                    new
                    {
                        x.Id,
                        refNo = x.RefNo,
                        BDNo = x.BDNo,
                        projectNo = x.ProjectNo,
                        applicationType = "Lost",
                        submitDate = x.SubmittedOn.HasValue ? x.SubmittedOn.Value.ToShortDateString() : "",
                        x.Status,
                        updatedOn = x.UpdatedOn.ToShortDateString(),
                        x.CreatedById,
                        VerifierId = "",
                        x.ApproverId,
                        x.SubmittedOn,
                    })
                .Where(x => ((x.Id == id) || id == null)
                            && (x.refNo != null && refNo != null ? x.refNo.Contains(refNo) : refNo == null)
                            && (x.BDNo != null && BDNo != null ? x.BDNo.Contains(BDNo) : BDNo == null)
                            && (x.projectNo != null && projectNo != null
                                ? x.projectNo.Contains(projectNo)
                                : projectNo == null)
                            && (x.applicationType != null && applicationType != null
                                ? x.applicationType.Contains(applicationType)
                                : applicationType == null)
                            && (x.Status == status || status == null)
                            && (x.updatedOn == updatedOn || string.IsNullOrEmpty(updatedOn)) // Remove .ToString()
                            && (x.submitDate == submitDate ||
                                string.IsNullOrEmpty(submitDate)) // Remove .ToString()
                ).OrderByDescending(x => x.updatedOn).Take(6).ToList();

            var rec = Db.Recovery.AsEnumerable() // store in memory for post-processing during select
                .Select(x =>
                    new
                    {
                        x.Id,
                        refNo = x.RefNo,
                        BDNo = x.BDNo,
                        projectNo = x.ProjNo,
                        applicationType = "Recovery",
                        submitDate = x.SubmittedOn.HasValue ? x.SubmittedOn.Value.ToShortDateString() : "",
                        x.Status,
                        updatedOn = x.UpdatedOn.ToShortDateString(),
                        x.CreatedById,
                        VerifierId = "",
                        ApproverId = "",
                        x.SubmittedOn,
                    })
                .Where(x => ((x.Id == id) || id == null)
                            && (x.refNo != null && refNo != null ? x.refNo.Contains(refNo) : refNo == null)
                            && (x.BDNo != null && BDNo != null ? x.BDNo.Contains(BDNo) : BDNo == null)
                            && (x.projectNo != null && projectNo != null
                                ? x.projectNo.Contains(projectNo)
                                : projectNo == null)
                            && (x.applicationType != null && applicationType != null
                                ? x.applicationType.Contains(applicationType)
                                : applicationType == null)
                            && (x.Status == status || status == null)
                            && (x.updatedOn == updatedOn || string.IsNullOrEmpty(updatedOn)) // Remove .ToString()
                            && (x.submitDate == submitDate ||
                                string.IsNullOrEmpty(submitDate)) // Remove .ToString()
                ).OrderByDescending(x => x.updatedOn).Take(6).ToList();

            var result = bd.Union(canc).Union(lost).Union(rec);

            var now = DateTime.Now;

            var userRole = Db.Users
                .Join(Db.UserRoles,
                    u => u.Id,
                    r => r.UserId,
                    (u, r) => new
                    {
                        Id = u.Id,
                        Name = u.FullName,
                        RoleId = r.RoleId,
                        ZoneId = r.ZoneId,
                        DivisionId = r.DivisionId
                    }
                )
                .Where(x => (x.Id == user.Id))
                .FirstOrDefault();

            if (userRole.RoleId == "E")
            {
                result = result.Where(x => x.CreatedById == user.Id).ToList();
            }
            else if (userRole.RoleId == "M")
            {
                result = result.Where(x =>
                        x.CreatedById == user.Id || (x.Status != "Draft" && x.VerifierId == user.Id))
                    .ToList();
            }
            else if (userRole.RoleId == "HZ")
            {
                result = result.Where(x =>
                    x.CreatedById == user.Id ||
                    ((x.Status != "Draft" && x.Status != "Submitted") && x.ApproverId == user.Id)).ToList();
            }
            else if (userRole.RoleId == "SM")
            {
                result = result.Where(x =>
                    x.CreatedById == user.Id ||
                    ((x.Status != "Draft" && x.Status != "Submitted") && x.ApproverId == user.Id)).ToList();
            }
            else if (userRole.RoleId == "GM")
            {
                result = result.Where(x =>
                    (x.Status != "Draft" || x.VerifierId == user.Id) ||
                    ((x.Status != "Draft" && x.Status != "Submitted") && x.ApproverId == user.Id)).ToList();
            }
            else if (userRole.RoleId == "SGM")
            {
                result = result.Where(x =>
                        (x.Status != "Draft" && x.Status != "Submitted") && x.ApproverId == user.Id)
                    .ToList();
            }
            else if (userRole.RoleId == "TB" || userRole.RoleId == "IA" || userRole.RoleId == "BA" ||
                     userRole.RoleId == "TR")
            {
                result = result.Where(x => x.Status != "Draft").ToList();
                //result = result.Where(x=> x.Status == "Approved" || (x.Status == "Accepted") || (x.Status == "Processed") || (x.Status == "Issued") || (x.Status == "Complete")).ToList();
            }

            //
            return new JsonResult(result.OrderByDescending(x => x.updatedOn).Take(6).ToList());
        }


        [HttpGet]
        public JsonResult GetAllUserManual()
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;
            var requester = Db.Users.Join(Db.UserRoles,
                    u => u.Id,
                    r => r.UserId,
                    (u, r) => new
                    {
                        Id = u.Id,
                        RoleId = r.RoleId,
                    }
                )
                .Where(x => x.Id == user.UserName)
                .FirstOrDefault();

            var result = Db.UserManual.Select(s => new
            {
                id = s.Id,
                name = s.Name,
                filename = s.FileName,
                sequence = s.Sequence,
                access = s.RoleAccess
            }).OrderBy(x => x.sequence);

            if (requester.RoleId != "IA" && requester.RoleId != "BA" && requester.RoleId != "TB" &&
                requester.RoleId != "TR")
            {
                result = Db.UserManual.Select(s => new
                    {
                        id = s.Id,
                        name = s.Name,
                        filename = s.FileName,
                        sequence = s.Sequence,
                        access = s.RoleAccess
                    })
                    .Where(x => x.access != "IA" && x.access != "BA" && x.access != "TR" && x.access != "TB")
                    .OrderBy(x => x.sequence);
            }
            else if (requester.RoleId == "TB")
            {
                result = Db.UserManual.Select(s => new
                    {
                        id = s.Id,
                        name = s.Name,
                        filename = s.FileName,
                        sequence = s.Sequence,
                        access = s.RoleAccess
                    })
                    .Where(x => x.access != "IA" && x.access != "BA" && x.access != "TR")
                    .OrderBy(x => x.sequence);
            }
            else if (requester.RoleId == "TR")
            {
                result = Db.UserManual.Select(s => new
                    {
                        id = s.Id,
                        name = s.Name,
                        filename = s.FileName,
                        sequence = s.Sequence,
                        access = s.RoleAccess
                    })
                    .Where(x => x.access != "IA" && x.access != "BA" && x.access != "TB")
                    .OrderBy(x => x.sequence);
            }
            else if (requester.RoleId == "IA" || requester.RoleId == "BA")
            {
                result = Db.UserManual.Select(s => new
                    {
                        id = s.Id,
                        name = s.Name,
                        filename = s.FileName,
                        sequence = s.Sequence,
                        access = s.RoleAccess
                    })
                    .OrderBy(x => x.sequence);
            }

            return new JsonResult(result.ToList());
        }

        public IActionResult Index()
        {
            //HttpContext.Session.SetString(SessionName, "Jarvik");
            return View();
        }

        public IActionResult Dashboard()
        {
            //ViewBag.schoolName = "Bayside Tigers";

            //ViewBag.UsernameSession = HttpContext.Session.GetString(SessionName);
            return View();
        }

        public IActionResult ApplyOneTimeVendor()
        {
            return View();
        }

        public IActionResult ApplyExistingVendor()
        {
            return View();
        }

        public IActionResult applyChoiceFirst()
        {
            return View();
        }

        public IActionResult applyWangCagaran()
        {
            return View();
        }

        public IActionResult applyChoiceWangHangus()
        {
            return View();
        }

        public IActionResult EditUser()
        {
            return View();
        }

        public IActionResult EditUser2()
        {
            return View();
        }

        public IActionResult VerifierDashboard()
        {
            return View();
        }

        public IActionResult ApproverDashboard()
        {
            return View();
        }

        public IActionResult BPDashboard()
        {
            return View();
        }

        public IActionResult TGBSFinanceDashboard()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult UserProfile()
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            var model = new UserViewModel();
            var item = Db.Users.Where(x => x.Id == user.Id).FirstOrDefault();

            if (item != null)
            {
                model.UserName = item.UserName;
                model.FullName = item.FullName;
                model.PhoneNumber = item.PhoneNumber;
                model.OfficeNo = item.OfficeNo;
                model.Email = item.Email;
                model.Designation = item.Designation;
                model.UnitName = item.Unit;
                model.DivisionName = item.Division;
                model.AuthenticationMethod = item.AuthenticationMethod.ToString() == "InternalDatabase"
                    ? "Internal"
                    : "Active Directory";
            }

            return View(model);
        }

        [HttpGet]
        public JsonResult GetAllUserRolesByStaffId(string id)
        {
            var result = Db.Users
                .Join(Db.UserRoles,
                    u => u.Id,
                    r => r.UserId,
                    (u, r) => new
                    {
                        UserId = u.Id,
                        Name = u.FullName,
                        RoleId = r.Id,
                        Role = r.Role.Name,
                        Division = r.Division.Name == null ? "" : r.Division.Name,
                        Function = r.Function.Name == null ? "" : r.Function.Name,
                        Zone = r.Zone.Name == null ? "" : r.Zone.Name,
                        Unit = r.Unit.Name == null ? "" : r.Unit.Name
                    }
                )
                .Where(x => x.UserId == id)
                .ToList();

            return new JsonResult(result.ToList());
        }

        public IActionResult UserManual()
        {
            return View();
        }

        [HttpGet]
        public ViewResult ChangePassword(string userName)
        {
            var entity = Db.Users
                .Include(x => x.Roles)
                .Select(x => new UserViewModel
                {
                    UserName = x.UserName,
                    FullName = x.FullName,
                    PhoneNumber = x.PhoneNumber,
                    OfficeNo = x.OfficeNo,
                    Email = x.Email,
                    Designation = x.Designation,
                    DivisionName = x.Division,
                    UnitName = x.Unit,
                    IsActive = x.IsActive,
                    UserRoles = x.Roles
                })
                .FirstOrDefault(x => x.UserName == userName);

            return View(entity);
        }

        [HttpPost]
        public JsonResult ChangePassword(UserViewModel model)
        {
            var user = Db.Users.Find(model.UserName);
            if (user == null)
                return Json(new
                {
                    response = StatusCode(StatusCodes.Status204NoContent), message = "Username : '" + model.UserName + "'; not found"
                });

            if (model.Password == model.Repassword)
            {
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.Password);
                user.ResetPassword = false;
                user.PasswordExpiredOn = DateTime.Now.AddMonths(3);
                user.AccessFailedCount = 0;

                var psswdHistory = Db.PasswordHistory.Where(x => x.Id == user.UserName).FirstOrDefault();

                if (psswdHistory == null)
                {
                    var user_password = new PasswordHistory
                    {
                        Id = user.UserName,
                        Password = user.PasswordHash,
                        PasswordCreateDate = DateTime.Now
                    };

                    Db.PasswordHistory.Add(user_password);
                }
                else
                {
                    //check if matching with 5 exisiting passwords
                    if (_userManager.PasswordHasher.VerifyHashedPassword(user, psswdHistory.Password, model.Password) !=
                        PasswordVerificationResult.Failed)
                    {
                        return Json(new
                        {
                            response = StatusCode(StatusCodes.Status204NoContent),
                            message = "Please dont use last 5 previous password to change password!"
                        });
                    }
                    else if (psswdHistory.Password2 != null &&
                             _userManager.PasswordHasher.VerifyHashedPassword(user, psswdHistory.Password2,
                                 model.Password) != PasswordVerificationResult.Failed)
                    {
                        return Json(new
                        {
                            response = StatusCode(StatusCodes.Status204NoContent),
                            message = "Please dont use last 5 previous password to change password!"
                        });
                    }
                    else if (psswdHistory.Password3 != null &&
                             _userManager.PasswordHasher.VerifyHashedPassword(user, psswdHistory.Password3,
                                 model.Password) != PasswordVerificationResult.Failed)
                    {
                        return Json(new
                        {
                            response = StatusCode(StatusCodes.Status204NoContent),
                            message = "Please dont use last 5 previous password to change password!"
                        });
                    }
                    else if (psswdHistory.Password4 != null &&
                             _userManager.PasswordHasher.VerifyHashedPassword(user, psswdHistory.Password4,
                                 model.Password) != PasswordVerificationResult.Failed)
                    {
                        return Json(new
                        {
                            response = StatusCode(StatusCodes.Status204NoContent),
                            message = "Please dont use last 5 previous password to change password!"
                        });
                    }
                    else if (psswdHistory.Password5 != null &&
                             _userManager.PasswordHasher.VerifyHashedPassword(user, psswdHistory.Password5,
                                 model.Password) != PasswordVerificationResult.Failed)
                    {
                        return Json(new
                        {
                            response = StatusCode(StatusCodes.Status204NoContent),
                            message = "Please dont use last 5 previous password to change password!"
                        });
                    }

                    if (psswdHistory.Password2 == null)
                    {
                        psswdHistory.Password2 = user.PasswordHash;
                        psswdHistory.Password2CreateDate = DateTime.Now;
                    }
                    else if (psswdHistory.Password3 == null && psswdHistory.Password2 != null)
                    {
                        psswdHistory.Password3 = user.PasswordHash;
                        psswdHistory.Password3CreateDate = DateTime.Now;
                    }
                    else if (psswdHistory.Password4 == null && psswdHistory.Password2 != null &&
                             psswdHistory.Password3 != null)
                    {
                        psswdHistory.Password4 = user.PasswordHash;
                        psswdHistory.Password4CreateDate = DateTime.Now;
                    }
                    else if (psswdHistory.Password5 == null && psswdHistory.Password2 != null &&
                             psswdHistory.Password3 != null && psswdHistory.Password4 != null)
                    {
                        psswdHistory.Password5 = user.PasswordHash;
                        psswdHistory.Password5CreateDate = DateTime.Now;
                    }
                    else
                    {
                        if (psswdHistory.PasswordCreateDate < psswdHistory.Password2CreateDate &&
                            psswdHistory.PasswordCreateDate < psswdHistory.Password3CreateDate &&
                            psswdHistory.PasswordCreateDate < psswdHistory.Password4CreateDate &&
                            psswdHistory.PasswordCreateDate < psswdHistory.Password5CreateDate)
                        {
                            psswdHistory.Password = user.PasswordHash;
                            psswdHistory.PasswordCreateDate = DateTime.Now;
                        }
                        else if (psswdHistory.Password2CreateDate < psswdHistory.PasswordCreateDate &&
                                 psswdHistory.Password2CreateDate < psswdHistory.Password3CreateDate &&
                                 psswdHistory.Password2CreateDate < psswdHistory.Password4CreateDate &&
                                 psswdHistory.Password2CreateDate < psswdHistory.Password5CreateDate)
                        {
                            psswdHistory.Password2 = user.PasswordHash;
                            psswdHistory.Password2CreateDate = DateTime.Now;
                        }
                        else if (psswdHistory.Password3CreateDate < psswdHistory.PasswordCreateDate &&
                                 psswdHistory.Password3CreateDate < psswdHistory.Password2CreateDate &&
                                 psswdHistory.Password3CreateDate < psswdHistory.Password4CreateDate &&
                                 psswdHistory.Password3CreateDate < psswdHistory.Password5CreateDate)
                        {
                            psswdHistory.Password3 = user.PasswordHash;
                            psswdHistory.Password3CreateDate = DateTime.Now;
                        }
                        else if (psswdHistory.Password4CreateDate < psswdHistory.PasswordCreateDate &&
                                 psswdHistory.Password4CreateDate < psswdHistory.Password2CreateDate &&
                                 psswdHistory.Password4CreateDate < psswdHistory.Password3CreateDate &&
                                 psswdHistory.Password4CreateDate < psswdHistory.Password5CreateDate)
                        {
                            psswdHistory.Password4 = user.PasswordHash;
                            psswdHistory.Password4CreateDate = DateTime.Now;
                        }
                        else if (psswdHistory.Password5CreateDate < psswdHistory.PasswordCreateDate &&
                                 psswdHistory.Password5CreateDate < psswdHistory.Password2CreateDate &&
                                 psswdHistory.Password5CreateDate < psswdHistory.Password3CreateDate &&
                                 psswdHistory.Password5CreateDate < psswdHistory.Password4CreateDate)
                        {
                            psswdHistory.Password5 = user.PasswordHash;
                            psswdHistory.Password5CreateDate = DateTime.Now;
                        }
                    }

                    Db.SetModified(psswdHistory);
                    Db.SaveChanges();
                }
            }
            else
            {
                return Json(new
                    { response = StatusCode(StatusCodes.Status403Forbidden), message = "Password not matching" });
            }

            Db.SetModified(user);
            Db.SaveChanges();

            return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Reset Password Successful!" });
        }
    }
}