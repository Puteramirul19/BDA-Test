using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BDA.Data;
using BDA.Entities;
using BDA.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BDA.Web.Controllers
{
    [Authorize]
    public class BankDraftActionController : BaseController
    {
       
        public JsonResult GetBankDraftActionById(Guid id)
        {
            //var auditTrailList = Db.BankDraftAction
            //               .Select(x => new BankDraftActionViewModel
            //               {
            //                   BdId = x.ParentId,
            //                   ActionRole = x.ActionRole,
            //                   On = x.On.ToString("dd-MM-yyyy hh:mm tt", System.Globalization.CultureInfo.InvariantCulture),
            //                   ById = x.ById,
            //                   ActionType = x.ActionType,
            //                   Comment = x.Comment
            //               }
            //                 )
            //            .Where(x => x.BdId == id)
            //            .OrderByDescending(x=> x.On)
            //            .ToList();

            //var auditTrailList = Db.BankDraftAction
            //              .Select(x => new BankDraftActionViewModel
            //              {
            //                  BdId = x.ParentId,
            //                  ActionRole = x.ActionRole,
            //                  On = x.On.ToString("dd-MM-yyyy hh:mm tt", System.Globalization.CultureInfo.InvariantCulture),
            //                  ById = x.ById,
            //                  ActionType = x.ActionType,
            //                  Comment = x.Comment
            //              }
            //                )
            //           .Where(x => x.BdId == id)
            //           .OrderByDescending(x => x.On)
            //           .ToList();

            var auditTrailList = Db.BankDraftAction
                 .Join(Db.Users,
                                  b => b.ById,
                                  u => u.Id,
                                  (b, u) => new
                                  {
                                    BdId = b.ParentId,
                                    ActionRole = b.ActionRole,
                                    On = b.On,
                                    ById = b.ActionRole != "System" ? b.ById + " - " + u.FullName : "System",
                                    ActionType = b.ActionType,
                                    Comment = b.Comment
                                }
                           )
                      .Where(x => x.BdId == id)
                      .OrderBy(x => x.On)
                      .Select(a=> new BankDraftActionViewModel()
                      {
                          BdId = a.BdId,
                          ActionRole = a.ActionRole,
                          On = a.On.ToString("dd-MM-yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture),
                          ById = a.ById,
                          ActionType = a.ActionType,
                          Comment = a.Comment
                      })
                      .ToList();

                 var auditTrailListb = Db.BankDraftAction
                .Select ( b => new
                                 {
                                     BdId = b.ParentId,
                                     ActionRole = b.ActionRole,
                                     On = b.On,
                                     ById = b.ById,
                                     ActionType = b.ActionType,
                                     Comment = b.Comment
                                 }
                          )
                     .Where(x => x.BdId == id && x.ById == null)
                     .OrderBy(x => x.On)
                     .Select(a=> new BankDraftActionViewModel()
                     {
                         BdId = a.BdId,
                         ActionRole = a.ActionRole,
                         On = a.On.ToString("dd-MM-yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture),
                         ById = a.ById,
                         ActionType = a.ActionType,
                         Comment = a.Comment
                     })
                     .ToList();

            var list = auditTrailList.Concat(auditTrailListb).OrderBy(x => DateTime.Parse(x.On));

            return new JsonResult(list.ToList());
        }

        //public JsonResult GetCancellationActionById(Guid id)
        //{
        //    var auditTrailList = Db.BankDraftAction
        //                   .Select(x => new BankDraftActionViewModel
        //                   {
        //                       BdId = x.BankDraftId,
        //                       ActionRole = x.ActionRole,
        //                       On = x.On.ToString("dd-MM-yyyy hh:mm tt", System.Globalization.CultureInfo.InvariantCulture),
        //                       ById = x.ById,
        //                       ActionType = x.ActionType,
        //                       Comment = x.Comment
        //                   }
        //                     )
        //                .Where(x => x.BdId == id)
        //                .OrderByDescending(x => x.On)
        //                .ToList();

        //    return new JsonResult(auditTrailList.ToList());
        //}
    }
}