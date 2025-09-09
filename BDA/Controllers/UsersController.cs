using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BDA.Data;
using BDA.Entities;
using BDA.Identity;
using BDA.ViewModel;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using JqueryDataTables.ServerSide.AspNetCoreWeb.Binders;
using JqueryDataTables.ServerSide.AspNetCoreWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BDA.Web.Controllers
{
    [Authorize(Roles = "Business Admin,ICT Admin,TGBS Banking")]
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult AddUser()
        {
            return View();
        }

        public IActionResult Test()
        {
            return View();
        }

        public IActionResult EditUser2()
        {
            return View();
        }
        public IActionResult ChangePassword()
        {
            return View();
        }

        public IActionResult ManageUser()
        {
            //var userList2 = Db.Users
            //  //.Where(x => x.IsRemoved == false)
            //  .ToList();

            var userList = Db.Users
        .Include("Roles")
      .Where(x => x.IsRemoved == false)
      .ToList();

            //var userList3 = userList2.Except(userList);

            //foreach (var value in userList3)
            //{
            //    var userList5 = Db.Users
            //          .Select(u => new UserChange
            //          {
            //              id = u.Id,
            //              fullName = u.FullName,
            //              roleId = "",
            //              roles = "",
            //              isActive = u.IsActive,
            //              division = "",
            //              function = "",
            //              zone = "",
            //              unit = "",
            //          })
            //          .Where(x => x.id == x.id).FirstOrDefault();

            //    userList.Add(userList5);

            //}

            //var userList4 = userList.Concat(userList3);

            return View(userList);
        }

        public class UserChange
        {
            public string id;
            public string fullName;
            public string roleId;
            public string roles;
            public string roleShort;
            public bool isActive;
            public string division;
            public string function;
            public string zone;
            public string unit;
            public bool isRemoved;
        };
        
        [HttpGet]
        public JsonResult GetAllUser(string id = null, string name = null, string roles = null, bool? status = null, string division = null, string function = null, string zone = null, string unit = null)
        {

            var userList = Db.Users
                   .Join(Db.UserRoles,
                    u => u.Id,
                    ur => ur.UserId,
                    (u, ur) => new { u = u, ur = ur })
                   .Join(Db.Roles,
                   ur => ur.ur.RoleId,
                   r => r.Id,
                   (ur, r) => new UserChange
                   {
                       id = ur.u.Id,
                       fullName = ur.u.FullName,
                       roleId = ur.ur.Id.ToString(),
                       roles = r.Name,
                       roleShort = ur.ur.RoleId,
                       isActive = ur.u.IsActive,
                       division = ur.ur.Division.Name,
                       function = ur.ur.Function.Name,
                       zone = ur.ur.Zone.Name,
                       unit = ur.ur.Unit.Name,
                       isRemoved = ur.u.IsRemoved
                   })
                   .Where(x =>
                      (x.id.Contains(id) || id == null)
                      && (x.fullName.Contains(name) || name == null)
                      && (x.roles.Contains(roles) || roles == null)
                      && (x.isActive == status || status == null)
                      && (x.division.Contains(division) || division == null)
                      && (x.function.Contains(function) || function == null)
                      && (x.zone.Contains(zone) || zone == null)
                      && (x.unit.Contains(unit) || unit == null)
                      //&& (x.isRemoved == false)
                      )
            .ToList();

            var result2 = userList.GroupBy(x => new { x.id, x.roleShort })
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();

            string area = "";

            foreach (var item in result2)
            {
                var result3 = userList.Where(x => x.id == item.id).ToList();
                var result4 = userList.Where(x => x.id == item.id).FirstOrDefault();

                if (item.roleShort == "M")
                {
                    area = "";
                    foreach (var UN in result3)
                    {
                        area += UN.unit + ", ";
                    }

                    area = area.Remove(area.Trim().Length - 1);

                    result4.unit = area;

                    userList.RemoveAll(x => x.id == item.id);

                    userList.Add(result4);
                }
                else if (item.roleShort == "SM" || item.roleShort == "HZ")
                {
                    area = "";
                    foreach (var UN in result3)
                    {
                        area += UN.zone + ", ";
                    }

                    area = area.Remove(area.Trim().Length - 1);

                    result3.FirstOrDefault().zone = area;

                    userList.RemoveAll(x => x.id == item.id);

                    userList.Add(result3.FirstOrDefault());
                }
                else if (item.roleShort == "HOU" || item.roleShort == "GM/SGM")
                {
                    area = "";
                    foreach (var UN in result3)
                    {
                        area += UN.function + ", ";
                    }

                    area = area.Remove(area.Trim().Length - 1);

                    result3.FirstOrDefault().function = area;

                    userList.RemoveAll(x => x.id == item.id);

                    userList.Add(result3.FirstOrDefault());
                }
            }

            var userList2 = Db.Users
            .GroupJoin(Db.UserRoles,
                    u => u.Id,
                    ur => ur.UserId,
               (u, ur) => new { Staff = u, UserRole = ur.FirstOrDefault() })
           .Where(x => x.UserRole == null
            && (x.Staff.Id.Contains(id) || id == null)
                      && (x.Staff.FullName.Contains(name) || name == null)
                      && (x.Staff.IsActive == status || status == null)
           )
           .ToList();

            foreach (var value in userList2)
            {
                var userList5 = Db.Users
                      .Select(u => new UserChange
                      {
                          id = u.Id,
                          fullName = u.FullName,
                          roleId = "",
                          roles = "",
                          isActive = u.IsActive,
                          division = "",
                          function = "",
                          zone = "",
                          unit = "",
                      })
                      .Where(x => x.id == value.Staff.Id).FirstOrDefault();

                userList.Add(userList5);

            }

            return new JsonResult(userList.ToList());
        }

        //public JsonResult GetAllUser(string id = null, string name = null, string roles = null, bool? status = null, string division = null, string function = null, string zone = null, string unit = null)
        //{

        //    var userList2 = Db.Users
        //            .Select(u => new
        //            {
        //                id = u.Id,
        //                fullName = u.FullName,
        //                roleId = "",
        //                Roles = "",
        //                isActive = u.IsActive,
        //                division = "",
        //                function = "",
        //                zone = "",
        //                unit = "",
        //            })
        //            .Where(x =>
        //               (x.id.Contains(id) || id == null)
        //               && (x.fullName.Contains(name) || name == null)
        //               && (x.Roles.Contains(roles) || roles == null)
        //               && (x.isActive == status || status == null)
        //               && (x.division.Contains(division) || division == null)
        //               && (x.function.Contains(function) || function == null)
        //               && (x.zone.Contains(zone) || zone == null)
        //               && (x.unit.Contains(unit) || unit == null)
        //               )
        //        .ToList();

        //    var userList = Db.Users
        //           .Join(Db.UserRoles,
        //            u => u.Id,
        //            ur => ur.UserId,
        //            (u, ur) => new { u = u, ur = ur })
        //           .Join(Db.Roles,
        //           ur => ur.ur.RoleId,
        //           r => r.Id,
        //           (ur, r) => new
        //           {
        //               id = ur.u.Id,
        //               fullName = ur.u.FullName,
        //               roleId = ur.ur.Id.ToString(),
        //               Roles = r.Name,
        //               isActive = ur.u.IsActive,
        //               division = ur.ur.Division.Name,
        //               function = ur.ur.Function.Name,
        //               zone = ur.ur.Zone.Name,
        //               unit = ur.ur.Unit.Name,
        //           })
        //           .Where(x =>
        //              (x.id.Contains(id) || id == null)
        //              && (x.fullName.Contains(name) || name == null)
        //              && (x.Roles.Contains(roles) || roles == null)
        //              && (x.isActive == status || status == null)
        //              && (x.division.Contains(division) || division == null)
        //              && (x.function.Contains(function) || function == null)
        //              && (x.zone.Contains(zone) || zone == null)
        //              && (x.unit.Contains(unit) || unit == null)
        //              )
        //    .ToList();

        //    var userList3 = userList.Concat(userList2).ToList();

        //    var userList4 = userList3.GroupBy(c => c.id)
        //         .ToDictionary(group => group.Key, group => group.Count())
        //         .Where(group => group.Value == 2);

        //    foreach (var value in userList4)
        //    {
        //        var userList5 = Db.Users
        //             .Select(u => new
        //             {
        //                 id = u.Id,
        //                 fullName = u.FullName,
        //                 roleId = "",
        //                 Roles = "",
        //                 isActive = u.IsActive,
        //                 division = "",
        //                 function = "",
        //                 zone = "",
        //                 unit = "",
        //             })
        //             .Where(x => x.id == value.Key).FirstOrDefault();

        //        userList2.Remove(userList5);

        //    }
        //    //var userList5 = userList2.Where(x=>x.id == userList4.))

        //    //var groupedlist = list.GroupBy(c => c.Col1)
        //    //          .Select((key, c) => new { Value = key, Count = c.Count() });

        //    return new JsonResult(userList.Concat(userList2).ToList());
        //}

        [HttpPost]
        public JsonResult Create(UserViewModel user)
        {

            //var currentUser = GetApplicationUser();

            var existing = Db.Users.FirstOrDefault(x => x.UserName == user.UserName);
            if (existing != null)
                return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "User with name '" + user.FullName + "' already existed." });

            if (ModelState.IsValid)
            { 
                var authen = user.AuthenticationMethod;
                AuthenticationMethod authType = AuthenticationMethod.ActiveDirectory;

                if (authen == "Internal")
                {
                    authType = AuthenticationMethod.InternalDatabase;
                }

                var model = new ApplicationUser
                {
                    Id = user.UserName,
                    UserName = user.UserName,
                    NormalizedUserName = user.UserName.ToUpper(),
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    OfficeNo = user.OfficeNo,
                    Email = user.Email,
                    Designation = user.Designation,
                    NormalizedEmail = user.Email.ToUpper(),
                    IsActive = true,
                    Division = user.DivisionName,
                    Unit = user.UnitName,
                    AuthenticationMethod = authType,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    IsRemoved= false,
                };

                if (authen == "Internal")
                {
                    model.PasswordHash = _userManager.PasswordHasher.HashPassword(model,user.Password);
                    model.PasswordExpiredOn = DateTime.Now.AddMonths(3);
                    model.PasswordExpiredOn = DateTime.Now.AddMonths(4);
                }

                Db.Users.Add(model);
                Db.SaveChanges();

                if (authen == "Internal")
                {
                    var user_password = new PasswordHistory
                    {
                        Id = user.UserName,
                        Password = model.PasswordHash,
                        PasswordCreateDate = DateTime.Now
                    };

                    Db.PasswordHistory.Add(user_password);
                }

                Db.SaveChanges();
                user.Id = model.Id;
            }

            return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Add New User Successful!" });
        }

        [HttpGet]
        public ViewResult EditUser(string userName)
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
                          DivisionName = x.Division,
                          UnitName = x.Unit,
                          Designation = x.Designation,
                          IsActive = x.IsActive,
                          UserRoles = x.Roles,
                          AuthenticationMethod = x.AuthenticationMethod.ToString() == "InternalDatabase"? "Internal" : "Active Directory",
                          IsRemoved = x.IsRemoved,
                      })
                      .FirstOrDefault(x => x.UserName == userName);

            ViewBag.UserRole = Db.UserRoles
                .Include(x => x.Role)
                .Include(x => x.Division)
                .Include(x => x.Function)
                .Include(x => x.Zone)
                .Include(x => x.Unit)
                .Where(x => x.UserId == userName).ToList();

            return View(entity);
        }

        [HttpPost]
        public JsonResult Update(UserViewModel user)
        {
            var entity = Db.Users.Find(user.UserName);

                entity.Id = user.UserName;
                entity.UserName = user.UserName;
                entity.FullName = user.FullName;
                entity.PhoneNumber = user.PhoneNumber;
                entity.OfficeNo = user.OfficeNo;
                entity.Designation = user.Designation;
                entity.Email = user.Email;
                entity.IsActive = user.IsActive;
                entity.Division = user.DivisionName;
                entity.Unit = user.UnitName;
                entity.IsRemoved = user.IsRemoved;

                Db.SetModified(entity);
                Db.SaveChanges();
            

            return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Edit User Successful!" });
        }

        //[HttpPost]
        //public JsonResult Remove(UserViewModel user)
        //{
        //    var entity = Db.Users.Find(user.UserName);

        //    entity.IsRemoved = user.IsDeleted;
          
        //    Db.SetModified(entity);
        //    Db.SaveChanges();

        //    return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Remove User Successful!" });
        //}

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
                 return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Username : '" + model.UserName + "'; not found" });

            if(model.Password == model.Repassword)
            { 
 
                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.Password);
                    user.ResetPassword = false;
                    user.PasswordExpiredOn = DateTime.Now.AddMonths(3);
                    user.AccessFailedCount = 0;

                    var psswdHistory = Db.PasswordHistory.Where(x=> x.Id == user.UserName).FirstOrDefault();

                    if(psswdHistory ==  null)
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
                        if (_userManager.PasswordHasher.VerifyHashedPassword(user, psswdHistory.Password, model.Password) != PasswordVerificationResult.Failed)
                        {
                            return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Please dont use last 5 previous password to change password!" });
                        }
                        else if(psswdHistory.Password2 != null && _userManager.PasswordHasher.VerifyHashedPassword(user, psswdHistory.Password2, model.Password) != PasswordVerificationResult.Failed)
                        {
                                return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Please dont use last 5 previous password to change password!" });
                            
                        }
                        else if (psswdHistory.Password3 != null && _userManager.PasswordHasher.VerifyHashedPassword(user, psswdHistory.Password3, model.Password) != PasswordVerificationResult.Failed)
                        {
                                return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Please dont use last 5 previous password to change password!" });
                            
                        }
                        else if (psswdHistory.Password4 != null && _userManager.PasswordHasher.VerifyHashedPassword(user, psswdHistory.Password4, model.Password) != PasswordVerificationResult.Failed)
                        {
                            return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Please dont use last 5 previous password to change password!" });
                            
                        }
                        else if (psswdHistory.Password5 != null && _userManager.PasswordHasher.VerifyHashedPassword(user, psswdHistory.Password5, model.Password) != PasswordVerificationResult.Failed)
                        {
                                return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Please dont use last 5 previous password to change password!" });
                            
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
                        else if (psswdHistory.Password4 == null && psswdHistory.Password2 != null && psswdHistory.Password3 != null)
                        {
                            psswdHistory.Password4 = user.PasswordHash;
                            psswdHistory.Password4CreateDate = DateTime.Now;
                        }
                        else if (psswdHistory.Password5 == null && psswdHistory.Password2 != null && psswdHistory.Password3 != null && psswdHistory.Password4 != null)
                        {
                            psswdHistory.Password5 = user.PasswordHash;
                            psswdHistory.Password5CreateDate = DateTime.Now;
                        }
                        else
                        {
                            if (psswdHistory.PasswordCreateDate < psswdHistory.Password2CreateDate && psswdHistory.PasswordCreateDate < psswdHistory.Password3CreateDate && psswdHistory.PasswordCreateDate < psswdHistory.Password4CreateDate && psswdHistory.PasswordCreateDate < psswdHistory.Password5CreateDate)
                            {
                                psswdHistory.Password = user.PasswordHash;
                                psswdHistory.PasswordCreateDate = DateTime.Now;
                            }
                            else if (psswdHistory.Password2CreateDate < psswdHistory.PasswordCreateDate && psswdHistory.Password2CreateDate < psswdHistory.Password3CreateDate && psswdHistory.Password2CreateDate < psswdHistory.Password4CreateDate && psswdHistory.Password2CreateDate < psswdHistory.Password5CreateDate)
                            {
                                psswdHistory.Password2 = user.PasswordHash;
                                psswdHistory.Password2CreateDate = DateTime.Now;
                            }
                            else if (psswdHistory.Password3CreateDate < psswdHistory.PasswordCreateDate && psswdHistory.Password3CreateDate < psswdHistory.Password2CreateDate && psswdHistory.Password3CreateDate < psswdHistory.Password4CreateDate && psswdHistory.Password3CreateDate < psswdHistory.Password5CreateDate)
                            {
                                psswdHistory.Password3 = user.PasswordHash;
                                psswdHistory.Password3CreateDate = DateTime.Now;
                            }
                            else if (psswdHistory.Password4CreateDate < psswdHistory.PasswordCreateDate && psswdHistory.Password4CreateDate < psswdHistory.Password2CreateDate && psswdHistory.Password4CreateDate < psswdHistory.Password3CreateDate && psswdHistory.Password4CreateDate < psswdHistory.Password5CreateDate)
                            {
                                psswdHistory.Password4 = user.PasswordHash;
                                psswdHistory.Password4CreateDate = DateTime.Now;
                            }
                            else if (psswdHistory.Password5CreateDate < psswdHistory.PasswordCreateDate && psswdHistory.Password5CreateDate < psswdHistory.Password2CreateDate && psswdHistory.Password5CreateDate < psswdHistory.Password3CreateDate && psswdHistory.Password5CreateDate < psswdHistory.Password4CreateDate)
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
                return Json(new { response = StatusCode(StatusCodes.Status403Forbidden), message = "Password not matching" });
            }
           
            Db.SetModified(user);
            Db.SaveChanges();

            return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Reset Password Successful!" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await Db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            Db.Users.Remove(user);
            await Db.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public JsonResult CreateUserRole(UserViewModel userRole)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { response = StatusCode(StatusCodes.Status406NotAcceptable), message = "Role '" + userRole.Role + "' is not valid." });
            }

            var existing = Db.UserRoles
                           .Where(x => x.UserId == userRole.UserName)
                           .Where(x => x.DivisionId == userRole.DivisionId)
                           .Where(x => x.FunctionId == userRole.FunctionId)
                           .Where(x => x.ZoneId == userRole.ZoneId)
                           .Where(x => x.UnitId == userRole.UnitId)
                           .FirstOrDefault();

            if (existing != null)
                return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Role name already existed." });


            var entity = new ApplicationUserRole
            {
                UserId = userRole.UserName,
                RoleId = userRole.RoleId,
                DivisionId = userRole.DivisionId,
                FunctionId = userRole.FunctionId,
                ZoneId = userRole.ZoneId,
                UnitId = userRole.UnitId,
                Id = Guid.NewGuid()
            };

            Db.UserRoles.Add(entity);
            Db.SaveChanges();

            return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Add New Role Successful!" });
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

        [HttpGet]
        public JsonResult GetFirstUserRoleByStaffId(string id)
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
                                       RoleShort = r.RoleId,
                                       Division = r.Division.Name == null ? "" : r.Division.Name,
                                       Function = r.Function.Name == null ? "" : r.Function.Name,
                                       Zone = r.Zone.Name == null ? "" : r.Zone.Name,
                                       Unit = r.Unit.Name == null ? "" : r.Unit.Name,
                                       DivId = r.Division.Id == Guid.Empty ? "" : r.Division.Id.ToString(),
                                       FuncId = r.Function.Id == Guid.Empty ? "" :  r.Function.Id.ToString(),
                                       ZoneId = r.Zone.Id == Guid.Empty ? "": r.Zone.Id.ToString(),
                                       UnitId = r.Unit.Id == Guid.Empty ? "" : r.Unit.Id.ToString(),
                                   }
                               )
                           .Where(x => x.UserId == id)
                           .FirstOrDefault();

            return new JsonResult(result);
        }

        [HttpDelete]
        public JsonResult DeleteUserRole(string id)
        {
            var role = Db.UserRoles.Where(x => x.Id == Guid.Parse(id)).FirstOrDefault();
            if (role == null)
            {
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Role Not Found" });
            }

            Db.UserRoles.Remove(role);
            Db.SaveChanges();

            return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Role Deleted Successfully!" });
        }

        public IActionResult AddUser2()
        {
            return View();
        }

        public IActionResult EditUser()
        {
            return View();
        }

        

    }
}