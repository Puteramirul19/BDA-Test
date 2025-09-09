using System;
using System.Linq;
using System.Threading.Tasks;
using BDA.ActiveDirectory;
using BDA.Entities;
using BDA.Identity;
using BDA.ViewModel;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BDA.Web.Controllers
{
    //[Authorize]
    public class AccountController : BaseController
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IAuthenticator authenticator;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IDirectoryService directoryService;
        //public const string SessionId = "_Name";


        public AccountController(IAuthenticator authenticator, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IDirectoryService directoryService)
        {
            this.authenticator = authenticator;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.directoryService = directoryService;
        }

        public IActionResult Login()
        {
            if (User.Identity.Name != null)
                return LocalRedirect("/Home/Dashboard");
            else
                return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginModel login)
        {

            var appUser = await Db.Users.FirstOrDefaultAsync(x => x.UserName == login.UserName);

            if (appUser == null)
            {
                ViewBag.ErrorMessage = "Log masuk tidak sah.";
                ModelState.AddModelError(string.Empty, "Log masuk tidak sah.");
                return View();
            }

            TimeSpan varTime = DateTime.Now - appUser.LastLogin;
            double fractionalMinutes = varTime.TotalMinutes;
            int wholeMinutes = (int)fractionalMinutes;

            if (appUser.SessionCount == 2 && wholeMinutes < 30)
            {
                ViewBag.ErrorMessage = "Limit log masuk untuk akaun yang sama pada masa yang sama telah melebihi 2 kali.";
                ModelState.AddModelError(string.Empty, "Limit log masuk untuk akaun yang sama pada masa yang sama telah melebihi 2 kali.");
                return View();
            }

            if(appUser.LockoutEnd > DateTime.Now)
            {
                ViewBag.ErrorMessage = "Akaun anda sudah dikunci. Sila tunggu 15 minit.";
                ModelState.AddModelError(string.Empty, "Akaun anda sudah dikunci. Sila tunggu 15 minit.");
                return View();
            }


            var result = await authenticator.SignInAsync(appUser, login.Password, false);

            if (result.Succeeded)
            {
                if (appUser.AuthenticationMethod.ToString() == "InternalDatabase" && appUser.PasswordExpiredOn < DateTime.Now)
                {
                    ViewBag.ErrorMessage = "Password sudah luput. Sila kemaskini password anda.";
                    ModelState.AddModelError(string.Empty, "Password sudah luput. Sila kemaskini password anda.");
                    return View();
                }

                string sessionId = Guid.NewGuid().ToString();
                //HttpContext.Session.SetString(SessionId, sessionId.ToString());
                appUser.LastLogin = DateTime.Now;
                appUser.AccessFailedCount = 0;

                if (wholeMinutes >= 30)
                {
                    appUser.SessionCount = 1;
                }
                else
                {
                    appUser.SessionCount = appUser.SessionCount + 1;
                }

                await userManager.UpdateAsync(appUser);

                if(appUser.ResetPassword == false)
                {
                    return Redirect("../Home/Dashboard");
                }
                else
                {
                    return Redirect("../Home/ChangePassword?userName=" + appUser.UserName);
                }

            }
            if (result.IsNotAllowed)
            {
                ViewBag.ErrorMessage = "Pengguna tidak aktif.";
                ModelState.AddModelError(string.Empty, "Pengguna tidak aktif.");
                return View();
            }
            if (result.IsLockedOut)
            {
                //logger.LogWarning("User account locked out.");
                //return RedirectToPage("./Lockout");
                ViewBag.ErrorMessage = "Akaun anda sudah dikunci. Sila tunggu 15 minit.";
                ModelState.AddModelError(string.Empty, "Akaun anda sudah dikunci. Sila tunggu 15 minit.");
                return View();
            }
            else
            {
                if (wholeMinutes > 30)
                {
                    appUser.AccessFailedCount = 0;
                }

                appUser.AccessFailedCount = appUser.AccessFailedCount + 1;
               
                if ((appUser.AuthenticationMethod.ToString() == "InternalDatabase" && appUser.AccessFailedCount > 2) || (appUser.AuthenticationMethod.ToString() == "ActiveDirectory" && appUser.AccessFailedCount > 4))
                {
                    appUser.LockoutEnd = DateTime.Now.AddMinutes(15);
                }
                await userManager.UpdateAsync(appUser);

                if(appUser.AuthenticationMethod.ToString() == "InternalDatabase")
                {
                    if(appUser.AccessFailedCount >= 3)
                    {
                        ViewBag.ErrorMessage = "Akaun anda sudah dikunci. Sila tunggu 15 minit.";
                        ModelState.AddModelError(string.Empty, "Akaun anda sudah dikunci. Sila tunggu 15 minit.");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Log masuk tidak sah. Pastikan katalaluan anda betul. Anda telah log masuk " + appUser.AccessFailedCount + " daripada 3 kali percubaan sebelum dikunci.";
                        ModelState.AddModelError(string.Empty, "Log masuk tidak sah. Pastikan katalaluan anda betul. Anda telah log masuk " + appUser.AccessFailedCount + " daripada 3 kali sebelum dikunci.");
                    }
                
                }
                else
                {
                    if (appUser.AccessFailedCount >= 5)
                    {
                        ViewBag.ErrorMessage = "Akaun anda sudah dikunci. Sila tunggu 15 minit.";
                        ModelState.AddModelError(string.Empty, "Akaun anda sudah dikunci. Sila tunggu 15 minit.");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Log masuk tidak sah. Pastikan katalaluan anda betul. Anda telah log masuk " + appUser.AccessFailedCount + " daripada 5 kali percubaan sebelum dikunci.";
                        ModelState.AddModelError(string.Empty, "Log masuk tidak sah. Pastikan katalaluan anda betul.");
                    }
                  
                }
               
                return View();
            }
            //}
            //return View();
        }

        [HttpPost]
        public async Task<RedirectResult> LogOut()
        {
            //var sessionId = HttpContext.Session.GetString(SessionId);
            var appUser = await Db.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);

            appUser.SessionCount = appUser.SessionCount - 1;
            await userManager.UpdateAsync(appUser);
            await signInManager.SignOutAsync();

            //_logger.LogInformation("User logged out.");
            //return LocalRedirect(Url.Content("~/"));
            //return View("Login");
            return Redirect("~/Account/Login");
        }


        [HttpPost]
        public async Task<JsonResult> LogOut2()
        {
            //var userName = HttpContext.Session.GetString(SessionName);
            //var sessionId = HttpContext.Session.GetString(SessionId);
            var appUser = await Db.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);

            appUser.SessionCount = appUser.SessionCount - 1;

            await userManager.UpdateAsync(appUser);
            await signInManager.SignOutAsync();

            //_logger.LogInformation("User logged out.");
            //return LocalRedirect(Url.Content("~/"));
            //return View("Login");
            return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Logout success" });
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }


        [HttpPost]
        public JsonResult ForgetPassword(LoginModel model)
        {
            var user = Db.Users.FirstOrDefault(x => x.UserName == model.UserName && x.Email == model.Email);

            if (user == null)
                return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Username " + model.UserName + " with email " + model.Email + " is not found. Please use correct username & email!" });

            if (user.AuthenticationMethod == AuthenticationMethod.ActiveDirectory)
            {
                return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "For TNB user, please use email password to login!" });
            }
            else
            {
                //Create temporary password for external user
                var chars = "abcdefghijklmnopqrstuvwxyz@#$&ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var random = new Random();
                var tempPassword = new string(
                    Enumerable.Repeat(chars, 12)
                              .Select(s => s[random.Next(s.Length)])
                              .ToArray());

                //Store temp password in db 
                user.PasswordHash = userManager.PasswordHasher.HashPassword(user, tempPassword);
                user.ResetPassword = true;
                user.LockoutEnd = null;
                user.AccessFailedCount = 0;

                Db.SetModified(user);
                Db.SaveChanges();

                //MRIS - Temporary Password email send for Forget Password (Internal User)
                Job.Enqueue<Services.NotificationService>(x => x.NotifyPasswordResetEmail(user, tempPassword));

                return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Reset Password Successful!" });
            }
        }

        [HttpPost]
        public JsonResult ChangePassword(LoginModel model)
        {

            var user = Db.Users.Find(model.UserName);
            if (user == null)
                return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Username : '" + model.UserName + "'; not found" });

            if (model.NewPassword == model.Repassword)
            {
               
                    user.PasswordHash = userManager.PasswordHasher.HashPassword(user, model.NewPassword);
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
                    if (userManager.PasswordHasher.VerifyHashedPassword(user, psswdHistory.Password, model.NewPassword) != PasswordVerificationResult.Failed)
                    {
                        return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Please dont use last 5 previous password to change password!" });
                    }
                    else if (psswdHistory.Password2 != null && userManager.PasswordHasher.VerifyHashedPassword(user, psswdHistory.Password2, model.NewPassword) != PasswordVerificationResult.Failed)
                    {
                        return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Please dont use last 5 previous password to change password!" });

                    }
                    else if (psswdHistory.Password3 != null && userManager.PasswordHasher.VerifyHashedPassword(user, psswdHistory.Password3, model.NewPassword) != PasswordVerificationResult.Failed)
                    {
                        return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Please dont use last 5 previous password to change password!" });

                    }
                    else if (psswdHistory.Password4 != null && userManager.PasswordHasher.VerifyHashedPassword(user, psswdHistory.Password4, model.NewPassword) != PasswordVerificationResult.Failed)
                    {
                        return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Please dont use last 5 previous password to change password!" });

                    }
                    else if (psswdHistory.Password5 != null && userManager.PasswordHasher.VerifyHashedPassword(user, psswdHistory.Password5, model.NewPassword) != PasswordVerificationResult.Failed)
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
    }
}