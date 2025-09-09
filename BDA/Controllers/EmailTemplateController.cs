using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BDA.Data;
using BDA.Entities;
using BDA.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BDA.Web.Controllers
{
    [Authorize(Roles = "Business Admin,ICT Admin,TGBS Banking")]
    public class EmailTemplateController :  BaseController
    {
        
        public IActionResult Index()
        {
            var firstEmailTemplate = Db.EmailTemplates
                          .Select(x => new EmailTemplateViewModel
                          {
                              Id = x.Id,
                              Description = x.Description,
                              Subject = x.Subject,
                              Body = x.Body
                          })
                          .FirstOrDefault();

            return View(firstEmailTemplate);
        }

        public IActionResult Index2()
        {
            var firstEmailTemplate = Db.EmailTemplates
                          .Select(x => new EmailTemplateViewModel
                          {
                              Id = x.Id,
                              Description = x.Description,
                              Subject = x.Subject,
                              Body = x.Body
                          })
                          .FirstOrDefault();

            return View(firstEmailTemplate);
        }


        [HttpGet]
        public JsonResult GetAllEmailTemplates()
        {
            var emailTemplateList = Db.EmailTemplates
                           .Select(x => new
                           {
                               id = x.Id,
                               description = x.Description,
                               subject = x.Subject,
                               body = x.Body
                           })
                        .ToList();

            return new JsonResult(emailTemplateList.ToList());
        }

        [HttpGet]
        public JsonResult GetFirstEmailTemplateById()
        {
            var emailTemplateList = Db.EmailTemplates
                           .Select(x => new EmailTemplateViewModel
                           {
                               Id = x.Id,
                               Description = x.Description,
                               Subject = x.Subject,
                               Body = x.Body
                           })
                        .FirstOrDefault();

            return new JsonResult(emailTemplateList);
        }

        [HttpGet]
        public JsonResult GetEmailTemplateById(string templateId)
        {
            var emailTemplateList = Db.EmailTemplates
                           .Select(x => new EmailTemplateViewModel
                           {
                               Id = x.Id,
                               Description = x.Description,
                               Subject = x.Subject,
                               Body = x.Body
                           })
                            .Where(x => x.Id == templateId)
                        .FirstOrDefault();

            return new JsonResult(emailTemplateList);
        }

        [HttpPost]
        public JsonResult Update(EmailTemplateViewModel model)
        {
            var entity = Db.EmailTemplates.Where(x=> x.Id == model.Id).FirstOrDefault();

            //if (ModelState.IsValid)
            //{
               if(entity != null)
                {
                    entity.Description = model.Description;
                    entity.Subject = model.Subject;
                    entity.Body = model.Body;
                 
                    Db.SetModified(entity);
                    Db.SaveChanges();
                }
               else
                {
                    return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Edit Email Template error" });
                }
            
            //}
            //else
            //{
            //    return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Edit Email Template error" });
            //}

            return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Edit Email Template Successful!" });
        }

        //[HttpPost]
        //public ActionResult Preview(EmailTemplatePreviewViewModel model)
        //{   
        //    var now = DateTime.Now;
        //    var dummy = new Dictionary<string, string>
        //    {
        //        { "Id", "SAMPLE Id" },
        //        { "InviterId", "SAMPLE InviterId" },
        //        { "InviterName", "SAMPLE InviterName" },
        //        { "InviteeId", "SAMPLE InviteeId" },
        //        { "InviteeName", "SAMPLE InviteeName" },
        //        { "Title", "SAMPLE Title" },
        //        { "BaseUrl", System.Configuration.ConfigurationManager.AppSettings["BaseUrl"]}
        //    };

        //    var parsed = ParsedTemplate.Parse(
        //        new EmailTemplate
        //        {
        //            SubjectTemplate = model.SubjectTemplate,
        //            ContentTemplate = model.ContentTemplate
        //        }, dummy);

        //    // Remove template to minimize output, we only need parsed strings
        //    model.SubjectTemplate = null;
        //    model.ContentTemplate = null;
        //    model.Subject = parsed.Subject;
        //    model.Content = parsed.Content;

        //    return Json(model);
        //}

    }
}