using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BDA.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BDA.Web.Controllers
{
    [Authorize]
    public class ApplicationController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public ApplicationController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult MyApplication()
        {
            return View();
        }

        public IActionResult MyApplication2()
        {
            return View();
        }

        public IActionResult ManageApplication()
        {
            return View();
        }

        public IActionResult ManageApplicationV2()
        {
            return View();
        }

        public IActionResult MyCancellation()
        {
            return View();
        }

        public IActionResult ManageCancellation()
        {
            return View();
        }

        public IActionResult ManageCancellationV2()
        {
            return View();
        }

        public IActionResult MyRecovery()
        {
            return View();
        }

        public IActionResult ManageRecovery()
        {
            return View();
        }

        public IActionResult ManageRecoveryV2()
        {
            return View();
        }

        public IActionResult MyLost()
        {
            return View();
        }

        public IActionResult ManageLost()
        {
            return View();
        }

        public IActionResult ManageLostV2()
        {
            return View();
        }

        public JsonResult GetMyRequestCancellation(Guid? id = null, string refNo = null, string bdNo = null,
                                      string projectNo = null, string nameOnBD = null,
                                      string requester = null, string coCode = null, string BA = null, string bdAmount = null, string status = null)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            var result = Db.Cancellation
                        .Join(Db.UserRoles,
                                  c => c.RequesterId,
                                  u => u.UserId,
                                  (c, u) => new
                                  {
                                      c.Id,
                                      refNo = c.RefNo,
                                      bdNo = c.BDNo,
                                      projectNo = c.BankDraft.ProjectNo,
                                      nameOnBd = c.NameOnBD,
                                      requester = c.Requester.FullName,
                                      coCode = c.CoCode,
                                      ba = c.BA,
                                      bdAmount = string.Format("{0:N}", c.BDAmount),
                                      status = c.Status,
                                      c.RequesterId,
                                      c.ApproverId,
                                      unit = u.Unit.Name,
                                      //c.Status,
                                      zone = u.Zone.Name,
                                      function = u.Function.Name,
                                      division = u.Division.Name,

                                  })
                        .Where(x => (x.Id == id || id == null)
                                    && (x.refNo.Contains(refNo) || refNo == null)
                                    && (x.bdNo.Contains(bdNo) || bdNo == null)
                                    && (x.projectNo.Contains(projectNo) || projectNo == null)
                                    && (x.requester.Contains(requester) || requester == null)
                                    && (x.coCode.Contains(coCode) || coCode == null)
                                    && (x.ba.Contains(BA) || BA == null)
                                    && (x.bdAmount.ToString().Contains(bdAmount) || bdAmount == null)
                                    && (x.status.Contains(status) || status == null)
                                    && (x.status != "Complete")
                                    )
                        .OrderByDescending(x => x.refNo)
                        .ToList();

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
                                      division = r.Division.Name,
                                      zone = r.Zone.Name,
                                      function = r.Function.Name,
                                      unit = r.Unit.Name,
                                  }
                              )
                          .Where(x => (x.Id == user.Id))
                          .FirstOrDefault();

            if (userRole.RoleId == "E")
            {
                result = result.Where(x => (x.RequesterId == user.Id)).ToList();
            }
            else if (userRole.RoleId == "M")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "HZ")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "SM")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "HOU")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "GM/SGM")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "TB" || userRole.RoleId == "IA" || userRole.RoleId == "BA" || userRole.RoleId == "TR")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
           


            return new JsonResult(result.ToList());
        }

        public JsonResult GetMyRequestCompleteCancellation(Guid? id = null, string refNo = null, string bdNo = null,
                                   string projectNo = null, string nameOnBD = null,
                                   string requester = null, string coCode = null, string BA = null, string bdAmount = null, string status = null)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            var result = Db.Cancellation
                        .Join(Db.UserRoles,
                                  c => c.RequesterId,
                                  u => u.UserId,
                                  (c, u) => new
                                  {
                                      c.Id,
                                      refNo = c.RefNo,
                                      bdNo = c.BDNo,
                                      projectNo = c.BankDraft.ProjectNo,
                                      nameOnBd = c.NameOnBD,
                                      requester = c.Requester.FullName,
                                      coCode = c.CoCode,
                                      BA = c.BA,
                                      bdAmount = string.Format("{0:N}", c.BDAmount),
                                      status = c.Status,
                                      c.RequesterId,
                                      c.ApproverId,
                                      unit = u.Unit.Name,
                                      //c.Status,
                                      zone = u.Zone.Name,
                                      function = u.Function.Name,
                                      division = u.Division.Name,
                                  })
                        .Where(x => (x.Id == id || id == null)
                                    && (x.refNo.Contains(refNo) || refNo == null)
                                    && (x.bdNo.Contains(bdNo) || bdNo == null)
                                    && (x.projectNo.Contains(projectNo) || projectNo == null)
                                    && (x.requester.Contains(requester) || requester == null)
                                    && (x.coCode.Contains(coCode) || coCode == null)
                                    && (x.BA.Contains(BA) || BA == null)
                                    && (x.bdAmount.ToString().Contains(bdAmount) || bdAmount == null)
                                    && (x.status.Contains(status) || status == null)
                                    && (x.status == "Complete")
                                    )
                        .OrderByDescending(x => x.refNo)
                        .ToList();

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
                                      division = r.Division.Name,
                                      zone = r.Zone.Name,
                                      function = r.Function.Name,
                                      unit = r.Unit.Name,
                                  }
                              )
                          .Where(x => (x.Id == user.Id))
                          .FirstOrDefault();

            if (userRole.RoleId == "E")
            {
                result = result.Where(x => (x.RequesterId == user.Id)).ToList();
            }
            else if (userRole.RoleId == "M")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "HZ")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "SM")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "HOU")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "GM/SGM")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "TB" || userRole.RoleId == "IA" || userRole.RoleId == "BA" || userRole.RoleId == "TR")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }



            return new JsonResult(result.ToList());
        }

        public JsonResult GetMyCancellation(Guid? id = null, string refNo = null, string bdNo = null,
                                        string projectNo = null, string nameOnBD = null,
                                        string requester = null, string coCode = null, string BA = null, string bdAmount = null, string status = null)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            var result = Db.Cancellation
                        .Join(Db.UserRoles,
                                  c => c.RequesterId,
                                  u => u.UserId,
                                  (c, u) => new
                                  {
                                      c.Id,
                                      refNo = c.RefNo,
                                      bdNo = c.BDNo,
                                      projectNo = c.BankDraft.ProjectNo,
                                      nameOnBd = c.NameOnBD,
                                      requester = c.Requester.FullName,
                                      coCode = c.CoCode,
                                      ba = c.BA,
                                      bdAmount = string.Format("{0:N}", c.BDAmount),
                                      status = c.Status,
                                      c.RequesterId,
                                      c.ApproverId,
                                      unit = u.Unit.Name,
                                      //c.Status,
                                      zone = u.Zone.Name,
                                      function = u.Function.Name,
                                      division = u.Division.Name,
                                      actionNeeded = c.Status == "Submitted" ? "Approve" : c.Status == "Approved" ? "Accept" : c.Status == "Accepted" ? "Process" : c.Status == "Processed" ? "Receive" : c.Status == "Received" ? "Confirm BD" : user.Id == c.RequesterId && (c.Status == "Rejected" || c.Status == "Declined" || c.Status == "Withdrawn") ? "Resubmit": "",
                                  })
                        .Where(x => (x.Id == id || id == null)
                                    && (x.refNo.Contains(refNo) || refNo == null)
                                    && (x.bdNo.Contains(bdNo) || bdNo == null)
                                    && (x.projectNo.Contains(projectNo) || projectNo == null)
                                    && (x.requester.Contains(requester) || requester == null)
                                    && (x.coCode.Contains(coCode) || coCode == null)
                                    && (x.ba.Contains(BA) || BA == null)
                                    && (x.bdAmount.ToString().Contains(bdAmount) || bdAmount == null)
                                    && (x.status.Contains(status) || status == null)
                                    && (x.status != "Complete")
                                    )
                        .OrderByDescending(x => x.refNo)
                        .ToList();

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
                                      division = r.Division.Name,
                                      zone = r.Zone.Name,
                                      function = r.Function.Name,
                                      unit = r.Unit.Name,
                                  }
                              )
                          .Where(x => (x.Id == user.Id))
                          .FirstOrDefault();


            if (userRole.RoleId != "TB" && userRole.RoleId != "TR" && userRole.RoleId != "IA" && userRole.RoleId != "BA" && userRole.RoleId != "BP" && userRole.RoleId != "V")
            {
                result = result.Where(x =>
                  (x.RequesterId == user.Id && (x.status == "Rejected" || x.status == "Withdrawn" || x.status == "Declined"))
                  ||(x.ApproverId == user.Id && (x.status == "Submitted" || x.status == "Withdrawn"))
                  ).ToList();
            }
            else if (userRole.RoleId == "TR")
            {
                result = result.Where(x => x.status == "Processed").ToList();
            }
            else if(userRole.RoleId == "TB")
            {
                result = result.Where(x => x.status == "Approved" || x.status == "Accepted" || x.status == "Received").ToList();
            }
            else if ( userRole.RoleId == "IA" || userRole.RoleId == "BA")
            {
                result = result.Where(x => x.status != "Complete").ToList();
            }
            else if (userRole.RoleId == "BP" || userRole.RoleId == "V")
            {
                result = result.Where(x => x.division == userRole.division && x.status != "Draft" && x.status != "Complete").ToList();
            }
            //if (userRole.RoleId == "E")
            //{
            //    result = result.Where(x => x.RequesterId == user.Id && x.unit == userRole.unit).ToList();
            //}
            //else if (userRole.RoleId == "M")
            //{
            //    result = result.Where(x => x.RequesterId == user.Id || (x.status != "Draft" && x.ApproverId == user.Id) && x.unit == userRole.unit).ToList();
            //}
            //else if (userRole.RoleId == "HZ")
            //{
            //    result = result.Where(x => x.RequesterId == user.Id || ((x.status != "Draft") && x.ApproverId == user.Id) && x.zone == userRole.zone).ToList();
            //}
            //else if (userRole.RoleId == "SM")
            //{
            //    result = result.Where(x => x.RequesterId == user.Id || ((x.status != "Draft") && x.ApproverId == user.Id) && x.zone == userRole.zone).ToList();
            //}
            //else if (userRole.RoleId == "HOU" || userRole.RoleId == "GM/SGM")
            //{
            //    result = result.Where(x => (x.status != "Draft" && x.ApproverId == user.Id) || ((x.status != "Draft" && x.status != "Submitted" && x.status != "Withdrawn") && x.ApproverId == user.Id) && x.function == userRole.function).ToList();
            //}
            //else if (userRole.RoleId == "TR")
            //{
            //    result = result.Where(x => x.status == "Processed" || x.status == "Received").ToList();
            //}
            //else if (userRole.RoleId == "TB" || userRole.RoleId == "IA" || userRole.RoleId == "BA")
            //{
            //    result = result.Where(x => x.status != "Draft").ToList();
            //    //result = result.Where(x=> x.Status == "Approved" || (x.Status == "Accepted") || (x.Status == "Processed") || (x.Status == "Issued") || (x.Status == "Complete")).ToList();
            //}

            return new JsonResult(result.ToList());
        }

        public JsonResult GetMyCompleteCancellation(Guid? id = null, string refNo = null, string bdNo = null,
                                      string projectNo = null, string nameOnBD = null,
                                      string requester = null, string coCode = null, string BA = null, string bdAmount = null, string status = null)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;
            var currRole = Db.Users
                     .Join(Db.UserRoles,
                             u => u.Id,
                             r => r.UserId,
                             (u, r) => new
                             {
                                 Id = u.Id,
                                 Name = u.FullName,
                                 RoleId = r.RoleId,
                                 division = r.Division.Name,
                                 zone = r.Zone.Name,
                                 function = r.Function.Name,
                                 unit = r.Unit.Name,
                             }
                         )
                     .Where(x => (x.Id == user.Id))
                     .FirstOrDefault();

            var result = Db.Cancellation
                        .Join(Db.UserRoles,
                                  c => c.RequesterId,
                                  u => u.UserId,
                                  (c, u) => new
                                  {
                                      c.Id,
                                      refNo = c.RefNo,
                                      bdNo = c.BDNo,
                                      projectNo = c.BankDraft.ProjectNo,
                                      nameOnBd = c.NameOnBD,
                                      requester = c.Requester.FullName,
                                      coCode = c.CoCode,
                                      BA = c.BA,
                                      bdAmount = string.Format("{0:N}", c.BDAmount),
                                      status = c.Status,
                                      c.RequesterId,
                                      c.ApproverId,
                                      unit = u.Unit.Name,
                                      //c.Status,
                                      zone = u.Zone.Name,
                                      function = u.Function.Name,
                                      division = u.Division.Name,
                                      actionTaken = user.Id == c.ApproverId && c.Status == "Rejected" ? "Reject" : user.Id == c.ApproverId && c.Status != "Rejected" ? "Approve" : currRole.RoleId == "TB" && (c.Status == "Received" || c.Status == "Complete" ) ? "Confirm BD" : currRole.RoleId == "TB" && 
                                      c.Status == "Processed" ? "Process" : currRole.RoleId == "TB" && c.Status == "Declined" ? "Decline" : currRole.RoleId == "TR" ? "Receive" : user.Id == c.RequesterId && (c.Status != "Draft") ? "Submit":""
                                  })
                        .Where(x => (x.Id == id || id == null)
                                    && (x.refNo.Contains(refNo) || refNo == null)
                                    && (x.bdNo.Contains(bdNo) || bdNo == null)
                                    && (x.projectNo.Contains(projectNo) || projectNo == null)
                                    && (x.requester.Contains(requester) || requester == null)
                                    && (x.coCode.Contains(coCode) || coCode == null)
                                    && (x.BA.Contains(BA) || BA == null)
                                    && (x.bdAmount.ToString().Contains(bdAmount) || bdAmount == null)
                                    && (x.status.Contains(status) || status == null)
                                    //&& (x.status == "Complete")
                                    )
                        .OrderByDescending(x => x.refNo)
                        .ToList();

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
                                      division = r.Division.Name,
                                      zone = r.Zone.Name,
                                      function = r.Function.Name,
                                      unit = r.Unit.Name,
                                  }
                              )
                          .Where(x => (x.Id == user.Id))
                          .FirstOrDefault();

            if (userRole.RoleId != "TB" && userRole.RoleId != "TR" && userRole.RoleId != "IA" && userRole.RoleId != "BA" && userRole.RoleId != "BP" && userRole.RoleId != "V")
            {
                result = result.Where(x =>
                  (x.RequesterId == user.Id && (x.status != "Rejected" || x.status == "Draft" || x.status != "Declined" || x.status == "Withdrawn"))
                  || (x.ApproverId == user.Id && (x.status == "Approved" || x.status == "Rejected" || x.status == "Accepted" || x.status == "Processed" || x.status == "Received" || x.status == "Complete"))
                  ).ToList();
            }
            else if (userRole.RoleId == "TR")
            {
                result = result.Where(x => x.status == "Received" || x.status == "Complete").ToList();
            }
            else if (userRole.RoleId == "TB")
            {
                result = result.Where(x => x.status == "Declined" || x.status == "Processed" || x.status == "Received" || x.status == "Complete").ToList();
            }
            else if (userRole.RoleId == "IA" || userRole.RoleId == "BA")
            {
                result = result.Where(x => x.status == "Complete").ToList();
            }
            else if (userRole.RoleId == "BP" || userRole.RoleId == "V")
            {
                result = result.Where(x => x.division == userRole.division && x.status == "Complete").ToList();
            }

            return new JsonResult(result.ToList());
        }

        public JsonResult GetMyRequestRecovery(Guid? id = null, string refNo = null, string bdNo = null,
                                    string projectNo = null, string projectComDate = null, string nameOnBD = null,
                                    string requester = null, string coCode = null, string BA = null, string recoveryType = null, string bdAmount = null, string status = null)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            var result = Db.Recovery
                        .Join(Db.UserRoles,
                                  r => r.RequesterId,
                                  u => u.UserId,
                                  (r, u) => new
                                  {
                                      r.Id,
                                      refNo = r.RefNo,
                                      bdNo = r.BDNo,
                                      projectNo = r.BankDraft.ProjectNo,
                                      projectComDate = r.ProjectCompletionDate == null ? "" : r.ProjectCompletionDate.Value.ToShortDateString(),
                                      nameOnBd = r.NameOnBD,
                                      requester = r.Requester.FullName,
                                      coCode = r.CoCode,
                                      ba = r.BA,
                                      recoveryType = (r.RecoveryType == "Full") ? r.RecoveryType : (r.RecoveryType == "FirstPartial") ? r.RecoveryType + "(" + string.Format("{0:C}", r.FirstRecoveryAmount) + ")" : (r.RecoveryType == "SecondPartial") ? r.RecoveryType + "(" + string.Format("{0:C}", r.SecondRecoveryAmount) + ")" : "",
                                      //r.RecoveryType + "(" + r.FirstRecoveryAmount + ")",
                                      bdAmount = string.Format("{0:N}", r.BDAmount),
                                      status = r.Status,
                                      r.RequesterId,
                                      unit = u.Unit.Name,
                                      zone = u.Zone.Name,
                                      function = u.Function.Name,
                                      division = u.Division.Name,
                                  })
                        .Where(x => (x.Id == id || id == null)
                                    && (x.refNo.Contains(refNo) || refNo == null)
                                    && (x.bdNo.Contains(bdNo) || bdNo == null)
                                    && (x.projectNo.Contains(projectNo) || projectNo == null)
                                    && (x.projectComDate.Contains(projectComDate) || projectComDate == null)
                                    && (x.nameOnBd.Contains(nameOnBD) || nameOnBD == null)
                                    && (x.requester.Contains(requester) || requester == null)
                                    && (x.coCode.Contains(coCode) || coCode == null)
                                    && (x.ba.Contains(BA) || BA == null)
                                    && (x.recoveryType.Contains(recoveryType) || recoveryType == null)
                                    && (x.bdAmount.ToString().Contains(bdAmount) || bdAmount == null)
                                    && (x.status.Contains(status) || status == null)
                                    && (x.status != "Complete")
                                    )
                        .OrderByDescending(x => x.refNo)
                        .ToList();

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
                                      division = r.Division.Name,
                                      zone = r.Zone.Name,
                                      function = r.Function.Name,
                                      unit = r.Unit.Name,
                                  }
                              )
                          .Where(x => (x.Id == user.Id))
                          .FirstOrDefault();
            if (userRole.RoleId == "E")
            {
                result = result.Where(x => (x.RequesterId == user.Id)).ToList();
            }
            else if (userRole.RoleId == "M")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "HZ")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "SM")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "HOU")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "GM/SGM")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "TB" || userRole.RoleId == "IA" || userRole.RoleId == "BA" || userRole.RoleId == "TR")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }


            return new JsonResult(result.ToList());
        }

        public JsonResult GetMyRequestCompleteRecovery(Guid? id = null, string refNo = null, string bdNo = null,
                            string projectNo = null, string projectComDate = null, string nameOnBD = null,
                            string requester = null, string coCode = null, string BA = null, string recoveryType = null, string bdAmount = null, string status = null)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            var result = Db.Recovery
                        .Join(Db.UserRoles,
                                  r => r.RequesterId,
                                  u => u.UserId,
                                  (r, u) => new
                                  {
                                      r.Id,
                                      refNo = r.RefNo,
                                      bdNo = r.BDNo,
                                      projectNo = r.BankDraft.ProjectNo,
                                      projectComDate = r.ProjectCompletionDate == null ? "" : r.ProjectCompletionDate.Value.ToShortDateString(),
                                      nameOnBd = r.NameOnBD,
                                      requester = r.Requester.FullName,
                                      coCode = r.CoCode,
                                      ba = r.BA,
                                      recoveryType = (r.RecoveryType == "Full") ? r.RecoveryType : (r.RecoveryType == "FirstPartial") ? r.RecoveryType + "(" + string.Format("{0:C}", r.FirstRecoveryAmount) + ")" : (r.RecoveryType == "SecondPartial") ? r.RecoveryType + "(" + string.Format("{0:C}", r.SecondRecoveryAmount) + ")" : "",
                                      bdAmount = string.Format("{0:N}", r.BDAmount),
                                      status = r.Status,
                                      r.RequesterId,
                                      unit = u.Unit.Name,
                                      zone = u.Zone.Name,
                                      function = u.Function.Name,
                                      division = u.Division.Name,
                                  })
                        .Where(x => (x.Id == id || id == null)
                                    && (x.refNo.Contains(refNo) || refNo == null)
                                    && (x.bdNo.Contains(bdNo) || bdNo == null)
                                    && (x.projectNo.Contains(projectNo) || projectNo == null)
                                    && (x.projectComDate.Contains(projectComDate) || projectComDate == null)
                                    && (x.nameOnBd.Contains(nameOnBD) || nameOnBD == null)
                                    && (x.requester.Contains(requester) || requester == null)
                                    && (x.coCode.Contains(coCode) || coCode == null)
                                    && (x.ba.Contains(BA) || BA == null)
                                    && (x.recoveryType.Contains(recoveryType) || recoveryType == null)
                                    && (x.bdAmount.ToString().Contains(bdAmount) || bdAmount == null)
                                    && (x.status.Contains(status) || status == null)
                                    && (x.status == "Complete")
                                    )
                        .OrderByDescending(x => x.refNo)
                        .ToList();

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
                                      division = r.Division.Name,
                                      zone = r.Zone.Name,
                                      function = r.Function.Name,
                                      unit = r.Unit.Name,
                                  }
                              )
                          .Where(x => (x.Id == user.Id))
                          .FirstOrDefault();

            if (userRole.RoleId == "E")
            {
                result = result.Where(x => (x.RequesterId == user.Id)).ToList();
            }
            else if (userRole.RoleId == "M")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "HZ")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "SM")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "HOU")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "GM/SGM")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "TB" || userRole.RoleId == "IA" || userRole.RoleId == "BA" || userRole.RoleId == "TR")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }

            return new JsonResult(result.ToList());
        }

        public JsonResult GetMyRecovery(Guid? id = null, string refNo = null, string bdNo = null,
                                      string projectNo = null,string projectComDate = null, string nameOnBD = null,
                                      string requester = null, string coCode = null, string BA = null, string recoveryType = null, string bdAmount = null, string status = null)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            var result = Db.Recovery
                        .Join(Db.UserRoles,
                                  r => r.RequesterId,
                                  u => u.UserId,
                                  (r, u) => new
                                  {
                                      r.Id,
                                      refNo = r.RefNo,
                                      bdNo = r.BDNo,
                                      projectNo = r.BankDraft.ProjectNo,
                                      projectComDate = r.ProjectCompletionDate == null ? "" : r.ProjectCompletionDate.Value.ToShortDateString(),
                                      nameOnBd = r.NameOnBD,
                                      requester = r.Requester.FullName,
                                      coCode = r.CoCode,
                                      ba = r.BA,
                                      recoveryType = (r.RecoveryType == "Full") ? r.RecoveryType : (r.RecoveryType == "FirstPartial") ? r.RecoveryType + "(" + string.Format("{0:C}", r.FirstRecoveryAmount)  + ")" : (r.RecoveryType == "SecondPartial") ? r.RecoveryType + "(" + string.Format("{0:C}", r.SecondRecoveryAmount) + ")" :"",
                                      //r.RecoveryType + "(" + r.FirstRecoveryAmount + ")",
                                      bdAmount = string.Format("{0:N}", r.BDAmount),
                                      status = r.Status,
                                      r.RequesterId,
                                      unit = u.Unit.Name,
                                      zone = u.Zone.Name,
                                      function = u.Function.Name,
                                      division = u.Division.Name,
                                      actionNeeded = r.Status == "Submitted" ? "Accept" : r.Status == "Accepted" ? "Receive" : r.Status == "Received" ? "Confirm BD" : r.Status == "Withdrawn" ? "Resubmit" : r.Status == "PartialComplete" ? "Submit" : "",
                                  })
                        .Where(x => (x.Id == id || id == null)
                                    && (x.refNo.Contains(refNo) || refNo == null)
                                    && (x.bdNo.Contains(bdNo) || bdNo == null)
                                    && (x.projectNo.Contains(projectNo) || projectNo == null)
                                    && (x.projectComDate.Contains(projectComDate) || projectComDate == null)
                                    && (x.nameOnBd.Contains(nameOnBD) || nameOnBD == null)
                                    && (x.requester.Contains(requester) || requester == null)
                                    && (x.coCode.Contains(coCode) || coCode == null)
                                    && (x.ba.Contains(BA) || BA == null)
                                    && (x.recoveryType.Contains(recoveryType) || recoveryType == null)
                                    && (x.bdAmount.ToString().Contains(bdAmount) || bdAmount == null)
                                    && (x.status.Contains(status) || status == null)
                                    && (x.status != "Complete")
                                    )
                        .OrderByDescending(x => x.refNo)
                        .ToList();

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
                                      division = r.Division.Name,
                                      zone = r.Zone.Name,
                                      function = r.Function.Name,
                                      unit = r.Unit.Name,
                                  }
                              )
                          .Where(x => (x.Id == user.Id))
                          .FirstOrDefault();

            if (userRole.RoleId == "TR")
            {
                result = result.Where(x => x.status == "Accepted").ToList();
            }
            else if (userRole.RoleId == "TB")
            {
                result = result.Where(x => x.status == "Submitted" || x.status == "Received").ToList();
            }
            else if (userRole.RoleId == "IA" || userRole.RoleId == "BA")
            {
                result = result.Where(x => x.status != "Complete" && x.status != "PartialComplete").ToList();
            }
            else if (userRole.RoleId == "BP" || userRole.RoleId == "V")
            {
                result = result.Where(x => x.division == userRole.division && x.status != "Draft" && x.status != "Complete" && x.status != "PartialComplete").ToList();
            }
            else
            {
                result = result.Where(x => x.RequesterId == user.Id && ( x.status == "Withdrawn" || x.status == "PartialComplete" || x.status == "Declined")).ToList();
            }

            return new JsonResult(result.ToList());
        }


        public JsonResult GetMyCompleteRecovery(Guid? id = null, string refNo = null, string bdNo = null,
                              string projectNo = null, string projectComDate = null, string nameOnBD = null,
                              string requester = null, string coCode = null, string BA = null, string recoveryType = null, string bdAmount = null, string status = null)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;
            var currRole = Db.Users
                       .Join(Db.UserRoles,
                               u => u.Id,
                               r => r.UserId,
                               (u, r) => new
                               {
                                   Id = u.Id,
                                   Name = u.FullName,
                                   RoleId = r.RoleId,
                                   division = r.Division.Name,
                                   zone = r.Zone.Name,
                                   function = r.Function.Name,
                                   unit = r.Unit.Name,
                               }
                           )
                       .Where(x => (x.Id == user.Id))
                       .FirstOrDefault();

            var result = Db.Recovery
                        .Join(Db.UserRoles,
                                  r => r.RequesterId,
                                  u => u.UserId,
                                  (r, u) => new
                                  {
                                      r.Id,
                                      refNo = r.RefNo,
                                      bdNo = r.BDNo,
                                      projectNo = r.BankDraft.ProjectNo,
                                      projectComDate = r.ProjectCompletionDate == null ? "" : r.ProjectCompletionDate.Value.ToShortDateString(),
                                      nameOnBd = r.NameOnBD,
                                      requester = r.Requester.FullName,
                                      coCode = r.CoCode,
                                      ba = r.BA,
                                      recoveryType = (r.RecoveryType == "Full") ? r.RecoveryType : (r.RecoveryType == "FirstPartial") ? r.RecoveryType + "(" + string.Format("{0:C}", r.FirstRecoveryAmount) + ")" : (r.RecoveryType == "SecondPartial") ? r.RecoveryType + "(" + string.Format("{0:C}", r.SecondRecoveryAmount) + ")" : "",
                                      bdAmount = string.Format("{0:N}", r.BDAmount),
                                      status = r.Status,
                                      r.RequesterId,
                                      unit = u.Unit.Name,
                                      zone = u.Zone.Name,
                                      function = u.Function.Name,
                                      division = u.Division.Name,
                                      actionTaken = currRole.RoleId == "TR" && r.Status == "Accepted" ? "Accept" : currRole.RoleId == "TR" && r.Status == "Received" ? "Receive" : currRole.RoleId == "TB" ? "Confirm BD" : user.Id == r.RequesterId ? "Submit": ""
                                  })
                        .Where(x => (x.Id == id || id == null)
                                    && (x.refNo.Contains(refNo) || refNo == null)
                                    && (x.bdNo.Contains(bdNo) || bdNo == null)
                                    && (x.projectNo.Contains(projectNo) || projectNo == null)
                                    && (x.projectComDate.Contains(projectComDate) || projectComDate == null)
                                    && (x.nameOnBd.Contains(nameOnBD) || nameOnBD == null)
                                    && (x.requester.Contains(requester) || requester == null)
                                    && (x.coCode.Contains(coCode) || coCode == null)
                                    && (x.ba.Contains(BA) || BA == null)
                                    && (x.recoveryType.Contains(recoveryType) || recoveryType == null)
                                    && (x.bdAmount.ToString().Contains(bdAmount) || bdAmount == null)
                                    && (x.status.Contains(status) || status == null)
                                    //&& (x.status == "Complete" || x.status == "PartialComplete")
                                    )
                        .OrderByDescending(x => x.refNo)
                        .ToList();

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
                                      division = r.Division.Name,
                                      zone = r.Zone.Name,
                                      function = r.Function.Name,
                                      unit = r.Unit.Name,
                                  }
                              )
                          .Where(x => (x.Id == user.Id))
                          .FirstOrDefault();

            if (userRole.RoleId == "TR")
            {
                result = result.Where(x => x.status == "Received" || x.status == "Complete").ToList();
            }
            else if (userRole.RoleId == "TB")
            {
                result = result.Where(x => x.status == "Accepted" || x.status == "Complete" || x.status == "PartialComplete" || x.status == "Declined").ToList();
            }
            else if (userRole.RoleId == "IA" || userRole.RoleId == "BA")
            {
                result = result.Where(x => x.status == "Complete").ToList();
            }
            else if (userRole.RoleId == "BP" || userRole.RoleId == "V")
            {
                result = result.Where(x => x.division == userRole.division && x.status == "Complete").ToList();
            }
            else
            {
                result = result.Where(x => x.RequesterId == user.Id && x.status != "Withdrawn" && x.status != "Draft" && x.status != "PartialComplete" || x.status != "Declined").ToList();
            }

            return new JsonResult(result.ToList());
        }

        public JsonResult GetMyRequestLost(Guid? id = null, string refNo = null, string bdNo = null,
                                 string projectNo = null, string nameOnBD = null,
                                 string requester = null, string coCode = null, string BA = null, string bdAmount = null, string status = null)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            var result = Db.Lost
                        .Join(Db.UserRoles,
                                  l => l.RequesterId,
                                  u => u.UserId,
                                  (l, u) => new
                                  {
                                      l.Id,
                                      refNo = l.RefNo,
                                      bdNo = l.BDNo,
                                      projectNo = l.BankDraft.ProjectNo,
                                      nameOnBd = l.NameOnBD,
                                      requester = l.Requester.FullName,
                                      coCode = l.CoCode,
                                      ba = l.BA,
                                      bdAmount = string.Format("{0:N}", l.BDAmount),
                                      status = l.Status,
                                      l.RequesterId,
                                      unit = u.Unit.Name,
                                      zone = u.Zone.Name,
                                      function = u.Function.Name,
                                      division = u.Division.Name,
                                      l.ApproverId
                                  })
                        .Where(x => (x.Id == id || id == null)
                                    && (x.refNo.Contains(refNo) || refNo == null)
                                    && (x.bdNo.Contains(bdNo) || bdNo == null)
                                    && (x.projectNo.Contains(projectNo) || projectNo == null)
                                    && (x.nameOnBd.Contains(nameOnBD) || nameOnBD == null)
                                    && (x.requester.Contains(requester) || requester == null)
                                    && (x.coCode.Contains(coCode) || coCode == null)
                                    && (x.ba.Contains(BA) || BA == null)
                                    && (x.bdAmount.ToString().Contains(bdAmount) || bdAmount == null)
                                    && (x.status.Contains(status) || status == null)
                                    && (x.status != "Complete")
                                    )
                        .OrderByDescending(x => x.refNo)
                        .ToList();

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
                                      division = r.Division.Name,
                                      zone = r.Zone.Name,
                                      function = r.Function.Name,
                                      unit = r.Unit.Name,
                                  }
                              )
                          .Where(x => (x.Id == user.Id))
                          .FirstOrDefault();

            if (userRole.RoleId == "E")
            {
                result = result.Where(x => (x.RequesterId == user.Id)).ToList();
            }
            else if (userRole.RoleId == "M")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "HZ")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "SM")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "HOU")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "GM/SGM")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "TB" || userRole.RoleId == "IA" || userRole.RoleId == "BA" || userRole.RoleId == "TR")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }


            return new JsonResult(result.ToList());
        }

        public JsonResult GetMyRequestCompleteLost(Guid? id = null, string refNo = null, string bdNo = null,
                         string projectNo = null, string nameOnBD = null,
                         string requester = null, string coCode = null, string BA = null, string bdAmount = null, string status = null)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            var result = Db.Lost
                        .Join(Db.UserRoles,
                                  l => l.RequesterId,
                                  u => u.UserId,
                                  (l, u) => new
                                  {
                                      l.Id,
                                      refNo = l.RefNo,
                                      bdNo = l.BDNo,
                                      projectNo = l.BankDraft.ProjectNo,
                                      nameOnBd = l.NameOnBD,
                                      requester = l.Requester.FullName,
                                      coCode = l.CoCode,
                                      ba = l.BA,
                                      bdAmount = string.Format("{0:N}", l.BDAmount),
                                      status = l.Status,
                                      l.RequesterId,
                                      unit = u.Unit.Name,
                                      zone = u.Zone.Name,
                                      function = u.Function.Name,
                                      division = u.Division.Name,
                                      l.ApproverId

                                  })
                        .Where(x => (x.Id == id || id == null)
                                    && (x.refNo.Contains(refNo) || refNo == null)
                                    && (x.bdNo.Contains(bdNo) || bdNo == null)
                                    && (x.projectNo.Contains(projectNo) || projectNo == null)
                                    && (x.nameOnBd.Contains(nameOnBD) || nameOnBD == null)
                                    && (x.requester.Contains(requester) || requester == null)
                                    && (x.coCode.Contains(coCode) || coCode == null)
                                    && (x.ba.Contains(BA) || BA == null)
                                    && (x.bdAmount.ToString().Contains(bdAmount) || bdAmount == null)
                                    && (x.status.Contains(status) || status == null)
                                    && (x.status == "Complete")
                                    )
                        .OrderByDescending(x => x.refNo)
                        .ToList();

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
                                      division = r.Division.Name,
                                      zone = r.Zone.Name,
                                      function = r.Function.Name,
                                      unit = r.Unit.Name,
                                  }
                              )
                          .Where(x => (x.Id == user.Id))
                          .FirstOrDefault();

            if (userRole.RoleId == "E")
            {
                result = result.Where(x => (x.RequesterId == user.Id)).ToList();
            }
            else if (userRole.RoleId == "M")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "HZ")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "SM")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "HOU")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "GM/SGM")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }
            else if (userRole.RoleId == "TB" || userRole.RoleId == "IA" || userRole.RoleId == "BA" || userRole.RoleId == "TR")
            {
                result = result.Where(x => x.RequesterId == user.Id).ToList();
            }


            return new JsonResult(result.ToList());
        }

        public JsonResult GetMyLost(Guid? id = null, string refNo = null, string bdNo = null,
                                   string projectNo = null, string nameOnBD = null,
                                   string requester = null, string coCode = null, string BA = null, string bdAmount = null, string status = null)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            var result = Db.Lost
                        .Join(Db.UserRoles,
                                  l => l.RequesterId,
                                  u => u.UserId,
                                  (l, u) => new
                                  {
                                      l.Id,
                                      refNo = l.RefNo,
                                      bdNo = l.BDNo,
                                      projectNo = l.BankDraft.ProjectNo,
                                      nameOnBd = l.NameOnBD,
                                      requester = l.Requester.FullName,
                                      coCode = l.CoCode,
                                      ba = l.BA,
                                      bdAmount = string.Format("{0:N}", l.BDAmount),
                                      status = l.Status,
                                      l.RequesterId,
                                      unit = u.Unit.Name,
                                      zone = u.Zone.Name,
                                      function = u.Function.Name,
                                      division = u.Division.Name,
                                      l.ApproverId,
                                      actionNeeded = l.Status == "Draft" || l.Status == "Rejected" || l.Status == "Declined" || l.Status == "Withdrawn" ? "Resubmit" : l.Status == "Submitted"  ? "Approve" : l.Status == "Approved" ? "Accept" : l.Status == "Accepted" || l.Status == "SaveProcessed" ? "Process" : l.Status == "Processed" ? "Receive" : l.Status == "Received" ? "Confirm BD" : "",
                                  })
                        .Where(x => (x.Id == id || id == null)
                                    && (x.refNo.Contains(refNo) || refNo == null)
                                    && (x.bdNo.Contains(bdNo) || bdNo == null)
                                    && (x.projectNo.Contains(projectNo) || projectNo == null)
                                    && (x.nameOnBd.Contains(nameOnBD) || nameOnBD == null)
                                    && (x.requester.Contains(requester) || requester == null)
                                    && (x.coCode.Contains(coCode) || coCode == null)
                                    && (x.ba.Contains(BA) || BA == null)
                                    && (x.bdAmount.ToString().Contains(bdAmount) || bdAmount == null)
                                    && (x.status.Contains(status) || status == null)
                                    && (x.status != "Complete")
                                    )
                        .OrderByDescending(x => x.refNo)
                        .ToList();

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
                                      division = r.Division.Name,
                                      zone = r.Zone.Name,
                                      function = r.Function.Name,
                                      unit = r.Unit.Name,
                                  }
                              )
                          .Where(x => (x.Id == user.Id))
                          .FirstOrDefault();

            if (userRole.RoleId != "TB" && userRole.RoleId != "TR" && userRole.RoleId != "IA" && userRole.RoleId != "BA" && userRole.RoleId != "BP" && userRole.RoleId != "V")
            {
                result = result.Where(x => (x.ApproverId == user.Id && (x.status == "Submitted" || x.status == "Withdrawn"))
                || (x.RequesterId == user.Id && (x.status == "Rejected" || x.status == "Declined" || x.status == "Withdrawn"))
                ).ToList();
            }
            else if (userRole.RoleId == "TR")
            {
                result = result.Where(x => x.status == "Processed").ToList();
            }
            else if (userRole.RoleId == "TB")
            {
                result = result.Where(x => x.status == "Approved" || x.status == "Accepted" || x.status == "Received").ToList();
            }
            else if (userRole.RoleId == "IA" || userRole.RoleId == "BA")
            {
                result = result.Where(x => x.status != "Complete").ToList();
            }
            else if (userRole.RoleId == "BP" || userRole.RoleId == "V")
            {
                result = result.Where(x => x.division == userRole.division && x.status != "Draft" && x.status != "Complete").ToList();
            }

            //if (userRole.RoleId == "E")
            //{
            //    result = result.Where(x => x.RequesterId == user.Id && x.unit == userRole.unit).ToList();
            //}
            //else if (userRole.RoleId == "M")
            //{
            //    result = result.Where(x => x.RequesterId == user.Id || (x.status != "Draft" && x.ApproverId == user.Id) && x.unit == userRole.unit).ToList();
            //}
            //else if (userRole.RoleId == "HZ")
            //{
            //    result = result.Where(x => x.RequesterId == user.Id || ((x.status != "Draft") && x.ApproverId == user.Id) && x.zone == userRole.zone).ToList();
            //}
            //else if (userRole.RoleId == "SM")
            //{
            //    result = result.Where(x => x.RequesterId == user.Id || ((x.status != "Draft") && x.ApproverId == user.Id) && x.zone == userRole.zone).ToList();
            //} 
            //else if (userRole.RoleId == "GM/SGM" || userRole.RoleId == "HOU")
            //{
            //    result = result.Where(x => (x.status != "Draft" && x.status != "Submitted" && x.status != "Withdrawn") && x.ApproverId == user.Id && x.function == userRole.function).ToList();
            //}
            //else if (userRole.RoleId == "TR")
            //{
            //    result = result.Where(x => x.status == "Processed" || x.status == "Received").ToList();
            //}
            //else if (userRole.RoleId == "TB" || userRole.RoleId == "IA" || userRole.RoleId == "BA")
            //{
            //    result = result.Where(x => x.status != "Draft").ToList();
            //    //result = result.Where(x=> x.Status == "Approved" || (x.Status == "Accepted") || (x.Status == "Processed") || (x.Status == "Issued") || (x.Status == "Complete")).ToList();
            //}

            return new JsonResult(result.ToList());
        }

        public JsonResult GetMyCompleteLost(Guid? id = null, string refNo = null, string bdNo = null,
                              string projectNo = null, string nameOnBD = null,
                              string requester = null, string coCode = null, string BA = null, string bdAmount = null, string status = null)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;
            var currRole = Db.Users
                        .Join(Db.UserRoles,
                                u => u.Id,
                                r => r.UserId,
                                (u, r) => new
                                {
                                    Id = u.Id,
                                    Name = u.FullName,
                                    RoleId = r.RoleId,
                                    division = r.Division.Name,
                                    zone = r.Zone.Name,
                                    function = r.Function.Name,
                                    unit = r.Unit.Name,
                                }
                            )
                        .Where(x => (x.Id == user.Id))
                        .FirstOrDefault();

            var result = Db.Lost
                        .Join(Db.UserRoles,
                                  l => l.RequesterId,
                                  u => u.UserId,
                                  (l, u) => new
                                  {
                                      l.Id,
                                      refNo = l.RefNo,
                                      bdNo = l.BDNo,
                                      projectNo = l.BankDraft.ProjectNo,
                                      nameOnBd = l.NameOnBD,
                                      requester = l.Requester.FullName,
                                      coCode = l.CoCode,
                                      ba = l.BA,
                                      bdAmount = string.Format("{0:N}", l.BDAmount),
                                      status = l.Status,
                                      l.RequesterId,
                                      unit = u.Unit.Name,
                                      zone = u.Zone.Name,
                                      function = u.Function.Name,
                                      division = u.Division.Name,
                                      l.ApproverId,
                                      actionTaken = currRole.RoleId == "TR" ? "Receive" : currRole.RoleId == "TB" && status == "Processed" ? "Process" : currRole.RoleId == "TB" && l.Status != "Processed" ? "Confirm BD" : currRole.RoleId == "TB" && l.Status == "Declined" ? "Decline" : user.Id == l.ApproverId && l.Status != "Rejected"? "Approve" : user.Id == l.ApproverId && l.Status == "Rejected" ? "Reject" : user.Id == l.RequesterId && l.Status != "Draft" ? "Submit" : "",
                                      //actionDate = (user.Id == b.RequesterId && b.Status == "Complete") ? (b.CompletedOn.HasValue ? b.CompletedOn.Value.ToString("dd-MM-yyyy") : "") : user.Id == b.TGBSIssuerId ? (b.TGBSIssuedOn.HasValue ? b.TGBSIssuedOn.Value.ToString("dd-MM-yyyy") : "") : user.Id == b.TGBSProcesserId ? (b.TGBSProcessedOn.HasValue ? b.TGBSProcessedOn.Value.ToString("dd-MM-yyyy") : "") : user.Id == b.TGBSAcceptanceId ? (b.TGBSAcceptedOn.HasValue ? b.TGBSAcceptedOn.Value.ToString("dd-MM-yyyy") : "") : user.Id == b.ApproverId ? (b.ApprovedOn.HasValue ? b.ApprovedOn.Value.ToString("dd-MM-yyyy") : "") : user.Id == b.VerifierId ? (b.VerifiedOn.HasValue ? b.VerifiedOn.Value.ToString("dd-MM-yyyy") : "") : user.Id == b.RequesterId ? (b.SubmittedOn.HasValue ? b.SubmittedOn.Value.ToString("dd-MM-yyyy") : "") : "",

                                  })
                        .Where(x => (x.Id == id || id == null)
                                    && (x.refNo.Contains(refNo) || refNo == null)
                                    && (x.bdNo.Contains(bdNo) || bdNo == null)
                                    && (x.projectNo.Contains(projectNo) || projectNo == null)
                                    && (x.nameOnBd.Contains(nameOnBD) || nameOnBD == null)
                                    && (x.requester.Contains(requester) || requester == null)
                                    && (x.coCode.Contains(coCode) || coCode == null)
                                    && (x.ba.Contains(BA) || BA == null)
                                    && (x.bdAmount.ToString().Contains(bdAmount) || bdAmount == null)
                                    && (x.status.Contains(status) || status == null)
                                    //&& (x.status == "Complete")
                                    )
                        .OrderByDescending(x => x.refNo)
                        .ToList();

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
                                      division = r.Division.Name,
                                      zone = r.Zone.Name,
                                      function = r.Function.Name,
                                      unit = r.Unit.Name,
                                  }
                              )
                          .Where(x => (x.Id == user.Id))
                          .FirstOrDefault();

            if (userRole.RoleId != "TB" && userRole.RoleId != "TR" && userRole.RoleId != "IA" && userRole.RoleId != "BA" && userRole.RoleId != "BP" && userRole.RoleId != "V")
            {
                result = result.Where(x => (x.ApproverId == user.Id && (x.status == "Rejected" || x.status == "Approved" || x.status == "Declined" || x.status == "Accepted" || x.status == "Processed" || x.status == "Received" || x.status == "Complete"))
                 || (x.RequesterId == user.Id && (x.status != "Draft" && x.status != "Rejected" && x.status != "Withdrawn" && x.status != "Declined"))).ToList();
            }
            else if (userRole.RoleId == "TR")
            {
                result = result.Where(x => x.status == "Received" || x.status == "Complete").ToList();
            }
            else if (userRole.RoleId == "TB")
            {
                result = result.Where(x => x.status == "Processed" || x.status == "Received" || x.status == "Complete").ToList();
            }
            else if (userRole.RoleId == "IA" || userRole.RoleId == "BA")
            {
                result = result.Where(x => x.status == "Complete").ToList();
            }
            else if (userRole.RoleId == "BP" || userRole.RoleId == "V")
            {
                result = result.Where(x => x.division == userRole.division && x.status == "Complete").ToList();
            }

    
            return new JsonResult(result.ToList());
        }

        public JsonResult GetMyRequestApplication(Guid? id = null, string refNo = null, string requesterName = null, string BDNo = null, string nameOnBd = null,
                                            string projectNo = null, string amount = null, string applicationType = null,
                                            string submitDate = null, string status = null, string updatedOn = null)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            var result = Db.BankDraft
                        .Join(Db.UserRoles,
                                  b => b.RequesterId,
                                  u => u.UserId,
                                  (b, u) => new
                                  {
                                      b.Id,
                                      refNo = b.RefNo,
                                      BDNo = b.BankDrafNoIssued,
                                      projectNo = b.ProjectNo,
                                      nameOnBd = b.NameOnBD,
                                      applicationType = b.Type,
                                      submitDate = b.SubmittedOn.HasValue ? b.SubmittedOn.Value.ToShortDateString() : "",
                                      status = b.Status == "RejectedApprove" || b.Status == "RejectedVerify" ? "Rejected" : b.Status == "ToDecline" ? "Approved" : b.Status,
                                      updatedOn = b.UpdatedOn.ToString("dd-MM-yyyy"),
                                      //b.UpdatedOn.ToShortDateString(),
                                      b.CreatedById,
                                      b.VerifierId,
                                      b.ApproverId,
                                      b.SubmittedOn,
                                      b.VerifiedOn,
                                      b.ApprovedOn,
                                      b.TGBSAcceptedOn,
                                      b.RequesterId,
                                      requesterName = b.Requester.FullName,
                                      amount = b.BankDraftAmount < 0 ? "(" + string.Format("{0:N}", -b.BankDraftAmount) + ")" : string.Format("{0:N}", b.BankDraftAmount),
                                      division = u.Division.Name,
                                      function = u.Function.Name,
                                      //zone = u.Zone.Name,
                                      //unit = u.Unit.Name,
                                  })
                        .Where(x => (x.Id == id || id == null)
                                    && (x.refNo.Contains(refNo) || refNo == null)
                                    && (x.BDNo.Contains(BDNo) || BDNo == null)
                                    && (x.projectNo.Contains(projectNo) || projectNo == null)
                                    && (x.applicationType.Contains(applicationType) || applicationType == null)
                                    && (x.status == status || status == null)
                                    && (x.updatedOn.ToString() == updatedOn || updatedOn == null)
                                    && (x.submitDate.ToString() == submitDate || submitDate == null)
                                    && (x.amount.ToString().Contains(amount) || amount == null)
                                    && (x.requesterName.Contains(requesterName) || requesterName == null)
                                    && (x.nameOnBd.Contains(nameOnBd) || nameOnBd == null)
                                    && (x.status != "Complete"))
                        .OrderByDescending(x => x.refNo)
                        .ToList()
                        .Distinct();

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
                                      division = r.Division.Name,
                                      zone = r.Zone.Name,
                                      function = r.Function.Name,
                                      unit = r.Unit.Name,
                                  }
                              )
                          .Where(x => (x.Id == user.Id))
                          .FirstOrDefault();

            if (userRole.RoleId == "E")
            {
                result = result.Where(x => (x.CreatedById == user.Id)).ToList();
            }
            else if (userRole.RoleId == "M")
            {
                result = result.Where(x => x.CreatedById == user.Id).ToList();
            }
            else if (userRole.RoleId == "HZ")
            {
                result = result.Where(x => x.CreatedById == user.Id).ToList();
            }
            else if (userRole.RoleId == "SM")
            {
                result = result.Where(x => x.CreatedById == user.Id).ToList();
            }
            else if (userRole.RoleId == "HOU")
            {
                result = result.Where(x => x.CreatedById == user.Id).ToList();
            }
            else if (userRole.RoleId == "GM/SGM")
            {
                result = result.Where(x => x.CreatedById == user.Id).ToList();
            }
            else if (userRole.RoleId == "TB" || userRole.RoleId == "IA" || userRole.RoleId == "BA" || userRole.RoleId == "TR")
            {
                result = result.Where(x => x.CreatedById == user.Id).ToList();
            }
            //else if (userRole.RoleId == "TB" || userRole.RoleId == "IA" || userRole.RoleId == "BA" || userRole.RoleId == "TR")
            //{
            //    result = result.Where(x => x.Status != "Draft").ToList();
            //}
            //
            return new JsonResult(result.ToList());
        }

        public JsonResult GetMyRequestCompleteApplication(Guid? id = null, string refNo = null, string requesterName = null, string BDNo = null, string nameOnBd = null,
                                             string projectNo = null, string applicationType = null,
                                             string submitDate = null, string status = null, string updatedOn = null, string division = null, string function = null, string zone = null, string unit = null, string amount = null)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            var result = Db.BankDraft
                        .Join(Db.UserRoles,
                                  b => b.RequesterId,
                                  u => u.UserId,
                                  (b, u) => new
                                  {
                                      b.Id,
                                      refNo = b.RefNo,
                                      BDNo = b.BankDrafNoIssued,
                                      projectNo = b.ProjectNo,
                                      nameOnBd = b.NameOnBD,
                                      applicationType = b.Type,
                                      submitDate = b.SubmittedOn.HasValue ? b.SubmittedOn.Value.ToShortDateString() : "",
                                      status = b.Status == "RejectedApprove" || b.Status == "RejectedVerify" ? "Rejected" : b.Status == "ToDecline" ? "Approved" : b.Status,
                                      updatedOn = b.UpdatedOn.ToString("dd-MM-yyyy"),
                                      //b.UpdatedOn.ToShortDateString(),
                                      b.CreatedById,
                                      b.VerifierId,
                                      b.ApproverId,
                                      b.SubmittedOn,
                                      b.VerifiedOn,
                                      b.ApprovedOn,
                                      b.TGBSAcceptedOn,
                                      b.RequesterId,
                                      requesterName = b.Requester.FullName,
                                      amount = b.BankDraftAmount < 0 ? "(" + string.Format("{0:N}", -b.BankDraftAmount) + ")" : string.Format("{0:N}", b.BankDraftAmount),
                                      division = u.Division.Name,
                                      function = u.Function.Name,
                                      //zone = u.Zone.Name,
                                      //unit = u.Unit.Name,
                                  })
                        .Where(x => (x.Id == id || id == null)
                                    && (x.refNo.Contains(refNo) || refNo == null)
                                    && (x.BDNo.Contains(BDNo) || BDNo == null)
                                    && (x.projectNo.Contains(projectNo) || projectNo == null)
                                    && (x.applicationType.Contains(applicationType) || applicationType == null)
                                    && (x.status == status || status == null)
                                    && (x.updatedOn.ToString() == updatedOn || updatedOn == null)
                                    && (x.submitDate.ToString() == submitDate || submitDate == null)
                                    && (x.division.Contains(division) || division == null)
                                    && (x.function.Contains(function) || function == null)
                                    //&& (x.zone.Contains(zone) || zone == null)
                                    //&& (x.unit.Contains(unit) || unit == null)
                                    && (x.amount.ToString().Contains(amount) || amount == null)
                                    && (x.requesterName.Contains(requesterName) || requesterName == null)
                                    && (x.nameOnBd.Contains(nameOnBd) || nameOnBd == null)
                                    && (x.status == "Complete"))
                        .OrderByDescending(x => x.refNo)
                        .ToList()
                        .Distinct();

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
                                      division = r.Division.Name,
                                      zone = r.Zone.Name,
                                      function = r.Function.Name,
                                      unit = r.Unit.Name,
                                  }
                              )
                          .Where(x => (x.Id == user.Id))
                          .FirstOrDefault();

            if (userRole.RoleId == "E")
            {
                result = result.Where(x => (x.CreatedById == user.Id)).ToList();
            }
            else if (userRole.RoleId == "M")
            {
                result = result.Where(x => x.CreatedById == user.Id).ToList();
            }
            else if (userRole.RoleId == "HZ")
            {
                result = result.Where(x => x.CreatedById == user.Id).ToList();
            }
            else if (userRole.RoleId == "SM")
            {
                result = result.Where(x => x.CreatedById == user.Id).ToList();
            }
            else if (userRole.RoleId == "HOU")
            {
                result = result.Where(x => x.CreatedById == user.Id).ToList();
            }
            else if (userRole.RoleId == "GM/SGM")
            {
                result = result.Where(x => x.CreatedById == user.Id).ToList();
            }
            else if (userRole.RoleId == "TB" || userRole.RoleId == "IA" || userRole.RoleId == "BA" || userRole.RoleId == "TR")
            {
                result = result.Where(x => x.CreatedById == user.Id).ToList();
            }
            //else if (userRole.RoleId == "TB" || userRole.RoleId == "IA" || userRole.RoleId == "BA" || userRole.RoleId == "TR")
            //{
            //    result = result.Where(x => x.Status != "Draft").ToList();
            //}
            //
            return new JsonResult(result.ToList());
        }

        //public JsonResult GetMyRequestApplication(Guid? id = null, string refNo = null, string requesterName = null, string BDNo = null, 
        //                                        string projectNo = null, string amount = null, string applicationType = null,
        //                                      string submitDate = null, string status = null, string updatedOn = null)
        //{
        //    var user = _userManager.GetUserAsync(HttpContext.User).Result;

        //    var result = Db.BankDraft
        //                .Join(Db.UserRoles,
        //                          b => b.RequesterId,
        //                          u => u.UserId,
        //                          (b, u) => new
        //                          {
        //                              b.Id,
        //                              refNo = b.RefNo,
        //                              BDNo = b.BankDrafNoIssued,
        //                              //nameOnBd = b.NameOnBD,
        //                              projectNo = b.ProjectNo,
        //                              applicationType = b.Type,
        //                              submitDate = b.SubmittedOn.HasValue ? b.SubmittedOn.Value.ToShortDateString() : "",
        //                              status = b.Status == "RejectedApprove" || b.Status == "RejectedVerify" ? "Rejected" : b.Status,
        //                              updatedOn = b.UpdatedOn.ToString("dd-MM-yyyy"),
        //                              //b.UpdatedOn.ToShortDateString(),
        //                              b.CreatedById,
        //                              b.VerifierId,
        //                              b.ApproverId,
        //                              b.SubmittedOn,
        //                              b.VerifiedOn,
        //                              b.ApprovedOn,
        //                              b.TGBSAcceptedOn,
        //                              b.RequesterId,
        //                              requesterName = b.Requester.FullName,
        //                              amount = b.BankDraftAmount < 0 ? "(" + string.Format("{0:N}", -b.BankDraftAmount) + ")" : string.Format("{0:N}", b.BankDraftAmount),
        //                              division = u.Division.Name,
        //                              function = u.Function.Name,
        //                              zone = u.Zone.Name,
        //                              unit = u.Unit.Name,
        //                          })
        //                .Where(x => (x.Id == id || id == null)
        //                            && (x.refNo.Contains(refNo) || refNo == null)
        //                            && (x.BDNo.Contains(BDNo) || BDNo == null)
        //                            && (x.projectNo.Contains(projectNo) || projectNo == null)
        //                            && (x.applicationType.Contains(applicationType) || applicationType == null)
        //                            && (x.status == status || status == null)
        //                            && (x.updatedOn.ToString() == updatedOn || updatedOn == null)
        //                            && (x.submitDate.ToString() == submitDate || submitDate == null)
        //                            && (x.amount.ToString().Contains(amount) || amount == null)
        //                            && (x.requesterName.Contains(requesterName) || requesterName == null)
        //                            //&& (x.nameOnBd.Contains(nameOnBd) || nameOnBd == null)
        //                            && (x.status != "Complete"))
        //                .OrderByDescending(x => x.refNo)
        //                .ToList();

        //    //var result1 = Db.BankDraft
        //    //   .Join(Db.WangCagaran,
        //    //             b => b.Id,
        //    //             w => w.BankDraftId,
        //    //             (b, w) => new
        //    //             {
        //    //                 b.Id,
        //    //                 refNo = "",
        //    //                 BDNo = b.BankDrafNoIssued,
        //    //                 projectNo = b.ProjectNo,
        //    //                 applicationType = b.Type,
        //    //                 nameOnBd = w.NamaPemegangCagaran,
        //    //                 submitDate = b.SubmittedOn.HasValue ? b.SubmittedOn.Value.ToShortDateString() : "",
        //    //                 status = b.Status == "RejectedApprove" || b.Status == "RejectedVerify" ? "Rejected" : b.Status,
        //    //                 updatedOn = b.UpdatedOn.ToString("dd-MM-yyyy"),
        //    //                 //b.UpdatedOn.ToShortDateString(),
        //    //                 b.CreatedById,
        //    //                 b.VerifierId,
        //    //                 b.ApproverId,
        //    //                 b.SubmittedOn,
        //    //                 b.VerifiedOn,
        //    //                 b.ApprovedOn,
        //    //                 b.TGBSAcceptedOn,
        //    //                 b.RequesterId,
        //    //                 requesterName = b.Requester.FullName,
        //    //                 amount = b.BankDraftAmount < 0 ? "(" + string.Format("{0:N}", -b.BankDraftAmount) + ")" : string.Format("{0:N}", b.BankDraftAmount),
        //    //             })
        //    //   .Where(x => (x.Id == id || id == null)
        //    //               && (x.refNo.Contains(refNo) || refNo == null)
        //    //               && (x.BDNo.Contains(BDNo) || BDNo == null)
        //    //               && (x.projectNo.Contains(projectNo) || projectNo == null)
        //    //               && (x.applicationType.Contains(applicationType) || applicationType == null)
        //    //               && (x.status == status || status == null)
        //    //               && (x.updatedOn.ToString() == updatedOn || updatedOn == null)
        //    //               && (x.submitDate.ToString() == submitDate || submitDate == null)
        //    //               && (x.amount.ToString().Contains(amount) || amount == null)
        //    //               && (x.requesterName.Contains(requesterName) || requesterName == null)
        //    //               && (x.nameOnBd.Contains(nameOnBd) || nameOnBd == null)
        //    //               && (x.status != "Complete"))
        //    //   .OrderByDescending(x => x.refNo)
        //    //   .ToList();


        //    //var result2 = Db.BankDraft
        //    //   .Join(Db.WangHangus,
        //    //             b => b.Id,
        //    //             w => w.BankDraftId,
        //    //             (b, w) => new
        //    //             {
        //    //                 b.Id,
        //    //                 refNo = "",
        //    //                 BDNo = b.BankDrafNoIssued,
        //    //                 projectNo = b.ProjectNo,
        //    //                 applicationType = b.Type,
        //    //                 nameOnBd = w.VendorName,
        //    //                 submitDate = b.SubmittedOn.HasValue ? b.SubmittedOn.Value.ToShortDateString() : "",
        //    //                 status = b.Status == "RejectedApprove" || b.Status == "RejectedVerify" ? "Rejected" : b.Status,
        //    //                 updatedOn = b.UpdatedOn.ToString("dd-MM-yyyy"),
        //    //                 //b.UpdatedOn.ToShortDateString(),
        //    //                 b.CreatedById,
        //    //                 b.VerifierId,
        //    //                 b.ApproverId,
        //    //                 b.SubmittedOn,
        //    //                 b.VerifiedOn,
        //    //                 b.ApprovedOn,
        //    //                 b.TGBSAcceptedOn,
        //    //                 b.RequesterId,
        //    //                 requesterName = b.Requester.FullName,
        //    //                 amount = b.BankDraftAmount < 0 ? "(" + string.Format("{0:N}", -b.BankDraftAmount) + ")" : string.Format("{0:N}", b.BankDraftAmount),
        //    //             })
        //    //   .Where(x => (x.Id == id || id == null)
        //    //               && (x.refNo.Contains(refNo) || refNo == null)
        //    //               && (x.BDNo.Contains(BDNo) || BDNo == null)
        //    //               && (x.projectNo.Contains(projectNo) || projectNo == null)
        //    //               && (x.applicationType.Contains(applicationType) || applicationType == null)
        //    //               && (x.status == status || status == null)
        //    //               && (x.updatedOn.ToString() == updatedOn || updatedOn == null)
        //    //               && (x.submitDate.ToString() == submitDate || submitDate == null)
        //    //               && (x.amount.ToString().Contains(amount) || amount == null)
        //    //               && (x.requesterName.Contains(requesterName) || requesterName == null)
        //    //               && (x.nameOnBd.Contains(nameOnBd) || nameOnBd == null)
        //    //               && (x.status != "Complete"))
        //    //   .OrderByDescending(x => x.refNo)
        //    //   .ToList();

        //    //var result = result1.Concat(result2).OrderByDescending(x => x.refNo).ToList();

        //    var now = DateTime.Now;

        //    var userRole = Db.Users
        //                  .Join(Db.UserRoles,
        //                          u => u.Id,
        //                          r => r.UserId,
        //                          (u, r) => new
        //                          {
        //                              Id = u.Id,
        //                              Name = u.FullName,
        //                              RoleId = r.RoleId,
        //                              division = r.Division.Name,
        //                              zone = r.Zone.Name,
        //                              function = r.Function.Name,
        //                              unit = r.Unit.Name,
        //                          }
        //                      )
        //                  .Where(x => (x.Id == user.Id))
        //                  .FirstOrDefault();

        //    if (userRole.RoleId == "E")
        //    {
        //        result = result.Where(x => (x.CreatedById == user.Id)).ToList();
        //    }
        //    else if (userRole.RoleId == "M")
        //    {
        //        result = result.Where(x => x.CreatedById == user.Id).ToList();
        //    }
        //    else if (userRole.RoleId == "HZ")
        //    {
        //        result = result.Where(x => x.CreatedById == user.Id).ToList();
        //    }
        //    else if (userRole.RoleId == "SM")
        //    {
        //        result = result.Where(x => x.CreatedById == user.Id).ToList();
        //    }
        //    else if (userRole.RoleId == "HOU")
        //    {
        //        result = result.Where(x => x.CreatedById == user.Id).ToList();
        //    }
        //    else if (userRole.RoleId == "GM/SGM")
        //    {
        //        result = result.Where(x => x.CreatedById == user.Id).ToList();
        //    }
        //    else if (userRole.RoleId == "TB" || userRole.RoleId == "IA" || userRole.RoleId == "BA" || userRole.RoleId == "TR")
        //    {
        //        result = result.Where(x => x.CreatedById == user.Id).ToList();
        //    }
        //    //else if (userRole.RoleId == "TB" || userRole.RoleId == "IA" || userRole.RoleId == "BA" || userRole.RoleId == "TR")
        //    //{
        //    //    result = result.Where(x => x.Status != "Draft").ToList();
        //    //}
        //    //
        //    return new JsonResult(result.ToList());
        //}

        //public JsonResult GetMyRequestCompleteApplication(Guid? id = null, string refNo = null, string requesterName = null, string BDNo = null,
        //                                     string projectNo = null, string applicationType = null,
        //                                     string submitDate = null, string status = null, string updatedOn = null, string division = null, string function = null, string zone = null, string unit = null, string amount = null)
        //{
        //    var user = _userManager.GetUserAsync(HttpContext.User).Result;

        //    var result = Db.BankDraft
        //                .Join(Db.UserRoles,
        //                          b => b.RequesterId,
        //                          u => u.UserId,
        //                          (b, u) => new
        //                          {
        //                              b.Id,
        //                              refNo = b.RefNo,
        //                              BDNo = b.BankDrafNoIssued,
        //                              //nameOnBd = b.NameOnBD,
        //                              projectNo = b.ProjectNo,
        //                              applicationType = b.Type,
        //                              submitDate = b.SubmittedOn.HasValue ? b.SubmittedOn.Value.ToShortDateString() : "",
        //                              status = b.Status == "RejectedApprove" || b.Status == "RejectedVerify" ? "Rejected" : b.Status,
        //                              updatedOn = b.UpdatedOn.ToString("dd-MM-yyyy"),
        //                              //b.UpdatedOn.ToShortDateString(),
        //                              b.CreatedById,
        //                              b.VerifierId,
        //                              b.ApproverId,
        //                              b.SubmittedOn,
        //                              b.VerifiedOn,
        //                              b.ApprovedOn,
        //                              b.TGBSAcceptedOn,
        //                              b.RequesterId,
        //                              requesterName = b.Requester.FullName,
        //                              amount = b.BankDraftAmount < 0 ? "(" + string.Format("{0:N}", -b.BankDraftAmount) + ")" : string.Format("{0:N}", b.BankDraftAmount),
        //                              division = u.Division.Name,
        //                              function = u.Function.Name,
        //                              zone = u.Zone.Name,
        //                              unit = u.Unit.Name,
        //                          })
        //                .Where(x => (x.Id == id || id == null)
        //                            && (x.refNo.Contains(refNo) || refNo == null)
        //                            && (x.BDNo.Contains(BDNo) || BDNo == null)
        //                            && (x.projectNo.Contains(projectNo) || projectNo == null)
        //                            && (x.applicationType.Contains(applicationType) || applicationType == null)
        //                            && (x.status == status || status == null)
        //                            && (x.updatedOn.ToString() == updatedOn || updatedOn == null)
        //                            && (x.submitDate.ToString() == submitDate || submitDate == null)
        //                            && (x.division.Contains(division) || division == null)
        //                            && (x.function.Contains(function) || function == null)
        //                            && (x.zone.Contains(zone) || zone == null)
        //                            && (x.unit.Contains(unit) || unit == null)
        //                            && (x.amount.ToString().Contains(amount) || amount == null)
        //                            && (x.requesterName.Contains(requesterName) || requesterName == null)
        //                            //&& (x.nameOnBd.Contains(nameOnBd) || nameOnBd == null)
        //                            && (x.status == "Complete"))
        //                .OrderByDescending(x => x.refNo)
        //                .ToList();

        //    var now = DateTime.Now;

        //    var userRole = Db.Users
        //                  .Join(Db.UserRoles,
        //                          u => u.Id,
        //                          r => r.UserId,
        //                          (u, r) => new
        //                          {
        //                              Id = u.Id,
        //                              Name = u.FullName,
        //                              RoleId = r.RoleId,
        //                              division = r.Division.Name,
        //                              zone = r.Zone.Name,
        //                              function = r.Function.Name,
        //                              unit = r.Unit.Name,
        //                          }
        //                      )
        //                  .Where(x => (x.Id == user.Id))
        //                  .FirstOrDefault();

        //    if (userRole.RoleId == "E")
        //    {
        //        result = result.Where(x => (x.CreatedById == user.Id)).ToList();
        //    }
        //    else if (userRole.RoleId == "M")
        //    {
        //        result = result.Where(x => x.CreatedById == user.Id).ToList();
        //    }
        //    else if (userRole.RoleId == "HZ")
        //    {
        //        result = result.Where(x => x.CreatedById == user.Id).ToList();
        //    }
        //    else if (userRole.RoleId == "SM")
        //    {
        //        result = result.Where(x => x.CreatedById == user.Id).ToList();
        //    }
        //    else if (userRole.RoleId == "HOU")
        //    {
        //        result = result.Where(x => x.CreatedById == user.Id).ToList();
        //    }
        //    else if (userRole.RoleId == "GM/SGM")
        //    {
        //        result = result.Where(x => x.CreatedById == user.Id).ToList();
        //    }
        //    else if (userRole.RoleId == "TB" || userRole.RoleId == "IA" || userRole.RoleId == "BA" || userRole.RoleId == "TR")
        //    {
        //        result = result.Where(x => x.CreatedById == user.Id).ToList();
        //    }
        //    //else if (userRole.RoleId == "TB" || userRole.RoleId == "IA" || userRole.RoleId == "BA" || userRole.RoleId == "TR")
        //    //{
        //    //    result = result.Where(x => x.Status != "Draft").ToList();
        //    //}
        //    //
        //    return new JsonResult(result.ToList());
        //}

        public JsonResult GetAllMyApplication(Guid? id = null, string refNo = null, string BDNo = null, 
                                                string projectNo = null, string applicationType = null, 
                                                string submitDate = null, string status = null, string updatedOn = null, string division = null, string function = null, string zone = null, string unit = null, string amount = null)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            var result = Db.BankDraft
                        .Join(Db.UserRoles,
                                  b => b.RequesterId,
                                  u => u.UserId,
                                  (b, u) => new
                                  {
                                    b.Id,
                                    refNo = b.RefNo,
                                    BDNo = b.BankDrafNoIssued,
                                    nameOnBd = b.NameOnBD,
                                    projectNo = b.ProjectNo,
                                    applicationType = b.Type,
                                    submitDate = b.SubmittedOn.HasValue ? b.SubmittedOn.Value.ToShortDateString() : "",
                                    status = b.Status == "RejectedApprove" || b.Status == "RejectedVerify" ? "Rejected" : b.Status,
                                    updatedOn = b.UpdatedOn.ToString("dd-MM-yyyy"),
                                     //b.UpdatedOn.ToShortDateString(),
                                    b.CreatedById,
                                    b.VerifierId,
                                    b.ApproverId,
                                    b.SubmittedOn,
                                    b.VerifiedOn,
                                    b.ApprovedOn,
                                    b.CreatedOn,
                                    b.TGBSAcceptedOn,
                                    b.RequesterId,
                                    requesterName = b.Requester.FullName,
                                    amount = b.BankDraftAmount < 0 ? "(" + string.Format("{0:N}", -b.BankDraftAmount) + ")" : string.Format("{0:N}", b.BankDraftAmount),
                                      //string.Format("{0:N}", b.BankDraftAmount),
                                      division = u.Division.Name,
                                    function = u.Function.Name,
                                      zone = u.Zone.Name,
                                      unit = u.Unit.Name,
                                  })
                        .Where(x =>  (x.Id == id || id == null)
                                    && (x.refNo.Contains(refNo) || refNo == null)
                                    && (x.BDNo.Contains(BDNo) || BDNo == null)
                                    && (x.projectNo.Contains(projectNo) || projectNo == null)
                                    && (x.applicationType.Contains(applicationType) || applicationType == null)
                                    && (x.status == status || status == null)
                                    && (x.updatedOn.ToString() == updatedOn || updatedOn == null)
                                    && (x.submitDate.ToString() == submitDate || submitDate == null)
                                    && (x.division.Contains(division) || division == null)
                                    && (x.function.Contains(function) || function == null)
                                    && (x.zone.Contains(zone) || zone == null)
                                    && (x.unit.Contains(unit) || unit == null)
                                    && (x.amount.ToString().Contains(amount) || amount == null)
                                    )
                        .OrderByDescending(x=> x.CreatedOn)
                        .ToList()
                        .Distinct();

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
                                      division = r.Division.Name,
                                      zone = r.Zone.Name,
                                      function = r.Function.Name,
                                      unit = r.Unit.Name,
                                  }
                              )
                          .Where(x => (x.Id == user.Id))
                          .FirstOrDefault();

            if (userRole.RoleId == "E")
            {
                result = result.Where(x => (x.CreatedById == user.Id && x.unit == userRole.unit)).ToList();
            }
            else if (userRole.RoleId == "M")
            {
                result = result.Where(x => x.CreatedById == user.Id || (x.status != "Draft" && x.VerifierId == user.Id) && x.unit == userRole.unit).ToList();
            }
            else if (userRole.RoleId == "HZ")
            {
                result = result.Where(x => x.CreatedById == user.Id || ((x.status != "Draft" && x.status != "Submitted" && x.status != "Withdrawn") && x.ApproverId == user.Id) || (x.status != "Draft" && x.VerifierId == user.Id)  && x.zone == userRole.zone).ToList();
            }
            else if (userRole.RoleId == "SM")
            {
                result = result.Where(x => x.CreatedById == user.Id || ((x.status != "Draft" && x.status != "Submitted" && x.status != "Withdrawn") && x.ApproverId == user.Id) || (x.status != "Draft" && x.VerifierId == user.Id) && x.zone == userRole.zone).ToList();
            }
            else if (userRole.RoleId == "GM/SGM" || userRole.RoleId == "HOU")
            {
                result = result.Where(x => (x.status != "Draft" && x.status != "Submitted" && x.status != "Withdrawn") && x.ApproverId == user.Id && x.function == userRole.function).ToList();
            }
            else if (userRole.RoleId == "TB" || userRole.RoleId == "IA" || userRole.RoleId == "BA" || userRole.RoleId == "TR" )
            {
                result = result.Where(x => x.status != "Draft").ToList();
                //result = result.Where(x=> x.Status == "Approved" || (x.Status == "Accepted") || (x.Status == "Processed") || (x.Status == "Issued") || (x.Status == "Complete")).ToList();
            }
            //
            return new JsonResult(result.ToList());
        }

        public JsonResult GetMyPendingApplication(Guid? id = null, string refNo = null, string BDNo = null, string nameOnBd= null,
                                               string projectNo = null, string applicationType = null,
                                               string submitDate = null, string actionNeeded = null, string division = null, string function = null, string zone = null, string unit = null, string amount = null, string status = null)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            var result = Db.BankDraft
                .Join(Db.UserRoles,
                    b => b.RequesterId,
                    u => u.UserId,
                    (b, u) => new
                    {
                        b.Id,
                        refNo = b.RefNo,
                        BDNo = b.BankDrafNoIssued,
                        nameOnBd = b.NameOnBD,
                        projectNo = b.ProjectNo,
                        actionNeeded = b.Status == "Submitted" && b.Type == "WangHangus" ? "Approve"
                            : b.Status == "Submitted" ? "Verify"
                            : b.Status == "Verified" ? "Approve"
                            : b.Status == "Approved" ? "Accept"
                            : b.Status == "Accepted" || b.Status == "SaveProcessed" ? "Process"
                            : b.Status == "Processed" ? "Issue"
                            : b.Status == "Issue" || b.Status == "Issued" ? "Accept BD"
                            : b.Status == "SaveComplete" ? "Accept BD"
                            : b.Status == "RejectedVerify" || b.Status == "RejectedApprove" || b.Status == "Declined" ||
                              b.Status == "Withdrawn" ? "Resubmit"
                            : b.Status == "ToDecline" ? "Decline" : "",
                        applicationType = b.Type,
                        submitDate = b.SubmittedOn, // Keep as DateTime
                        status = b.Status == "RejectedApprove" || b.Status == "RejectedVerify" ? "Rejected"
                            : b.Status == "ToDecline" ? "Approved"
                            : b.Status,
                        updatedOn = b.UpdatedOn, // Keep as DateTime
                        b.CreatedById,
                        b.VerifierId,
                        b.ApproverId,
                        b.SubmittedOn,
                        b.VerifiedOn,
                        b.ApprovedOn,
                        b.TGBSAcceptedOn,
                        b.RequesterId,
                        amount = b.BankDraftAmount, // Keep as decimal
                        requesterName = b.Requester.FullName,
                        division = u.Division.Name,
                        function = u.Function.Name
                    })
                .AsEnumerable() // **Switch to in-memory processing**
                .Select(x => new
                {
                    x.Id,
                    x.refNo,
                    x.BDNo,
                    x.nameOnBd,
                    x.projectNo,
                    x.actionNeeded,
                    x.applicationType,
                    submitDate = x.submitDate.HasValue ? x.submitDate.Value.ToString("dd-MM-yyyy") : "",
                    status = x.status,
                    updatedOn = x.updatedOn.ToString("dd-MM-yyyy"),
                    x.CreatedById,
                    x.VerifierId,
                    x.ApproverId,
                    x.SubmittedOn,
                    x.VerifiedOn,
                    x.ApprovedOn,
                    x.TGBSAcceptedOn,
                    x.RequesterId,
                    amount = x.amount < 0
                        ? "(" + string.Format("{0:N}", -x.amount) + ")"
                        : string.Format("{0:N}", x.amount),
                    x.requesterName,
                    x.division,
                    x.function
                })
                .Where(x => (id == null || x.Id == id)
                            && (refNo == null || x.refNo.Contains(refNo))
                            && (status == null || x.status.Contains(status))
                            && (BDNo == null || x.BDNo.Contains(BDNo))
                            && (nameOnBd == null || x.nameOnBd.Contains(nameOnBd))
                            && (projectNo == null || x.projectNo.Contains(projectNo))
                            && (applicationType == null || x.applicationType.Contains(applicationType))
                            && (submitDate == null || x.submitDate == submitDate)
                            && (actionNeeded == null || x.actionNeeded == actionNeeded)
                            && (division == null || x.division.Contains(division))
                            && (function == null || x.function.Contains(function))
                            && (amount == null || x.amount.Contains(amount)))
                .OrderByDescending(x => x.refNo)
                .Distinct()
                .ToList();


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
                                      division = r.Division.Name,
                                      zone = r.Zone.Name,
                                      function = r.Function.Name,
                                      unit = r.Unit.Name,
                                  }
                              )
                          .Where(x => (x.Id == user.Id))
                          .FirstOrDefault();

            if (userRole.RoleId != "TB" && userRole.RoleId != "TR" && userRole.RoleId != "IA" && userRole.RoleId != "BA" && userRole.RoleId != "BP" && userRole.RoleId != "V")
            {
                result = result.Where(x =>
               (x.RequesterId == user.Id && (x.status == "Draft" || x.status == "Rejected" || x.status == "Withdrawn" || x.status == "Declined" || x.status == "Issued"))
               || (x.VerifierId == user.Id && (x.status == "Submitted"))
               || (x.ApproverId == user.Id && x.status == "Verified" && (x.applicationType == "WangCagaran" || x.applicationType == "WangCagaranHangus"))
               || (x.ApproverId == user.Id && (x.status == "Submitted") && x.applicationType == "WangHangus")
               ).ToList();
            }
            else if (userRole.RoleId == "TB")
            {
                result = result.Where(x => x.status == "Approved" || x.status == "Posted" || x.status == "Accepted" || x.status == "Processed" || x.status == "ToDecline").ToList();
            }
            else if (userRole.RoleId == "IA" || userRole.RoleId == "BA")
            {
                result = result.Where(x => x.status != "Complete").ToList();
            }
            else if (userRole.RoleId == "BP" || userRole.RoleId == "V")
            {
                result = result.Where(x => x.division == userRole.division && x.status != "Draft" && x.status != "Complete").ToList();
            }

            return new JsonResult(result.ToList());
        }


        public JsonResult GetMyTakenApplication(Guid? id = null, string refNo = null, string BDNo = null, string nameOnBd = null,
                                             string projectNo = null, string applicationType = null,
                                             string submitDate = null, string actionTaken = null, string actionDate = null, string status = null, string division = null, string function = null, string zone = null, string unit = null, string amount = null)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;


            var userRole = Db.Users
                        .Join(Db.UserRoles,
                                u => u.Id,
                                r => r.UserId,
                                (u, r) => new
                                {
                                    Id = u.Id,
                                    Name = u.FullName,
                                    RoleId = r.RoleId,
                                    division = r.Division.Name,
                                    zone = r.Zone.Name,
                                    function = r.Function.Name,
                                    unit = r.Unit.Name,
                                }
                            )
                        .Where(x => (x.Id == user.Id))
                        .FirstOrDefault();

            var result = Db.BankDraft
                        .Join(Db.UserRoles,
                                  b => b.RequesterId,
                                  u => u.UserId,
                                  (b, u) => new
                                  {
                                      b.Id,
                                      refNo = b.RefNo,
                                      BDNo = b.BankDrafNoIssued,
                                      nameOnBd = b.NameOnBD,
                                      projectNo = b.ProjectNo,
                                      actionTaken = userRole.RoleId == "TB"? "Issue" : user.Id == b.ApproverId && b.Status == "Approved" ? "Approve" : user.Id == b.ApproverId && b.Status == "RejectedApprove" ? "Reject" : user.Id == b.VerifierId && b.Status == "Verified" ? "Verify" : user.Id == b.VerifierId && b.Status == "RejectedVerify" ? "Reject" : user.Id == b.ApproverId && b.Status == "Complete" ? "Approve" : user.Id == b.VerifierId && b.Status == "Complete" ? "Verify" : user.Id == b.RequesterId && b.Status == "Submitted" ? "Submit": user.Id == b.RequesterId && b.Status == "Complete" ? "Accept BD" : "",                                  
                                      actionDate = (user.Id == b.RequesterId && b.Status == "Complete") ? (b.CompletedOn.HasValue ? b.CompletedOn.Value.ToString("dd-MM-yyyy") : "") : user.Id == b.TGBSIssuerId ? (b.TGBSIssuedOn.HasValue ? b.TGBSIssuedOn.Value.ToString("dd-MM-yyyy") : "") : user.Id == b.TGBSProcesserId ? (b.TGBSProcessedOn.HasValue ? b.TGBSProcessedOn.Value.ToString("dd-MM-yyyy") : "") : user.Id == b.TGBSAcceptanceId ? (b.TGBSAcceptedOn.HasValue ? b.TGBSAcceptedOn.Value.ToString("dd-MM-yyyy") : "") : user.Id == b.ApproverId ? (b.ApprovedOn.HasValue ? b.ApprovedOn.Value.ToString("dd-MM-yyyy") : "") : user.Id == b.VerifierId ? (b.VerifiedOn.HasValue ? b.VerifiedOn.Value.ToString("dd-MM-yyyy") : "") : user.Id == b.RequesterId ? (b.SubmittedOn.HasValue ? b.SubmittedOn.Value.ToString("dd-MM-yyyy") : "") : "",
                                      applicationType = b.Type,
                                      submitDate = b.SubmittedOn.HasValue ? b.SubmittedOn.Value.ToString("dd-MM-yyyy") : "",
                                      status = b.Status == "RejectedApprove" || b.Status == "RejectedVerify" ? "Rejected" : b.Status == "ToDecline" ? "Approved" : b.Status,
                                      status2 = b.Status,
                                      updatedOn = b.UpdatedOn.ToString("dd-MM-yyyy"),
                                      b.CreatedById,
                                      b.VerifierId,
                                      b.ApproverId,
                                      b.SubmittedOn,
                                      b.VerifiedOn,
                                      b.ApprovedOn,
                                      b.TGBSAcceptedOn,
                                      amount = b.BankDraftAmount < 0 ? "(" + string.Format("{0:N}", -b.BankDraftAmount) + ")" : string.Format("{0:N}", b.BankDraftAmount),
                                      //string.Format("{0:N}", b.BankDraftAmount),
                                      b.RequesterId,
                                      requesterName = b.Requester.FullName,
                                      division = u.Division.Name,
                                      function = u.Function.Name,
                                      //zone = u.Zone.Name,
                                      //unit = u.Unit.Name,
                                  })
                         .Where(x => (x.Id == id || id == null)
                                    && (x.refNo.Contains(refNo) || refNo == null)
                                    && (x.status.Contains(status) || status == null)
                                    && (x.BDNo.Contains(BDNo) || BDNo == null)
                                    && (x.nameOnBd.Contains(nameOnBd) || nameOnBd == null)
                                    && (x.projectNo.Contains(projectNo) || projectNo == null)
                                    && (x.applicationType.Contains(applicationType) || applicationType == null)
                                    && (x.submitDate.ToString() == submitDate || submitDate == null)
                                    && (x.actionTaken == actionTaken || actionTaken == null)
                                    && (x.actionDate.ToString() == actionDate || actionDate == null)
                                    && (x.status == status || status == null)
                                    && (x.division.Contains(division) || division == null)
                                    && (x.function.Contains(function) || function == null)
                                    //&& (x.zone.Contains(zone) || zone == null)
                                    //&& (x.unit.Contains(unit) || unit == null)
                                    && (x.amount.ToString().Contains(amount) || amount == null)
                                    )
                         .OrderByDescending(x => x.refNo)
                        .ToList()
                        .Distinct();

            var now = DateTime.Now;

            if (userRole.RoleId != "TB" && userRole.RoleId != "TR" && userRole.RoleId != "IA" && userRole.RoleId != "BA" && userRole.RoleId != "BP" && userRole.RoleId != "V") 
            {
                result = result.Where(x =>
                (x.RequesterId == user.Id && (x.status != "Draft" && x.status != "Rejected" && x.status != "Withdrawn" && x.status != "Declined"))
                || (x.VerifierId == user.Id && (x.status2 == "RejectedVerify" || x.status == "Verified" || x.status == "Approved" || x.status == "Accepted" || x.status == "Processed" || x.status == "Issued" || x.status == "Complete"))
                || (x.ApproverId == user.Id && (x.status2 == "RejectedApprove" || x.status == "Approved" || x.status == "Accepted" || x.status == "Processed" || x.status == "Issued" || x.status == "Complete"))
                ).ToList();
            }
            else if (userRole.RoleId == "TB")
            {
                result = result.Where(x => x.status == "Declined" || x.status == "Issued" || x.status == "Complete").ToList();
            }
            else if (userRole.RoleId == "IA" || userRole.RoleId == "BA")
            {
                result = result.Where(x => x.status == "Complete").ToList();
            }
            else if (userRole.RoleId == "IA" || userRole.RoleId == "BA")
            {
                result = result.Where(x => x.status == "Complete").ToList();
            }
            else if (userRole.RoleId == "BP" || userRole.RoleId == "V")
            {
                result = result.Where(x => x.division == userRole.division && x.status == "Complete").ToList();
            }


            return new JsonResult(result.ToList());
        }

    }
}