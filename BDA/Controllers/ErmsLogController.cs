using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BDA.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace BDA.Web.Controllers
{
    public class ErmsLogController : BaseController
    {
        public IActionResult Index()
        {
            var model = new ErmsLogViewModel();
            model.ErmsLogs = Db.ErmsLog
                .ToList();
            return View(model);
        }

        public JsonResult GetAllErmsLogs(string transactionId = null, string date = null, string refNo = null,
            string source = null, string destination = null,
            string action1 = null, string status = null, string message = null)
        {
            // var ermsLogList = Db.ErmsLog
            //     .Join(Db.BankDraft,
            //         e => e.BankDraftId,
            //         b => b.Id,
            //         (e, b) => new ErmsLogViewModel
            //         {
            //             Id = e.Id,
            //             RequestId = e.RequestId,
            //             TransactionId = e.TransactionId,
            //             Date = e.Date.ToString("dd-MM-yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture),
            //             RefNo = b.RefNo,
            //             Source = e.Source,
            //             Destination = e.Destination,
            //             Action = e.Action,
            //             Message = e.Message,
            //             Status = e.Status,
            //         })
            //     .Where(x => (source != null ? x.Source.Contains(source) : source == null)
            //                 && (destination != null ? x.Destination.Contains(destination) : destination == null)
            //                 && (transactionId != null ? x.TransactionId.Contains(transactionId) : transactionId == null)
            //                 && (date != null ? x.Date.Contains(date) : date == null)
            //                 && (refNo != null ? x.RefNo.Contains(refNo) : refNo == null)
            //                 && (action1 != null ? x.Action.Contains(action1) : action1 == null)
            //                 && (status != null ? x.Status.Contains(status) : status == null)
            //                 && (message != null ? x.Message.Contains(message) : message == null))
            //     .OrderByDescending(x => x.Date)
            //     .ToList();

            var ermsLogList = Db.ErmsLog
                .Join(Db.BankDraft,
                    e => e.BankDraftId,
                    b => b.Id,
                    (e, b) => new
                    {
                        e.Id,
                        e.RequestId,
                        e.TransactionId,
                        e.Date,
                        b.RefNo,
                        e.Source,
                        e.Destination,
                        e.Action,
                        e.Message,
                        e.Status
                    }).AsEnumerable()
                .Select(x => new ErmsLogViewModel
                {
                    Id = x.Id,
                    RequestId = x.RequestId,
                    TransactionId = x.TransactionId,
                    Date = x.Date.ToString("dd-MM-yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture),
                    RefNo = x.RefNo,
                    Source = x.Source,
                    Destination = x.Destination,
                    Action = x.Action,
                    Message = x.Message,
                    Status = x.Status
                })
                .Where(x => (source == null || x.Source.Contains(source))
                            && (destination == null || x.Destination.Contains(destination))
                            && (transactionId == null || x.TransactionId.Contains(transactionId))
                            && (date != null ? x.Date.Contains(date) : date == null)
                            && (refNo == null || x.RefNo.Contains(refNo))
                            && (action1 == null || x.Action.Contains(action1))
                            && (status == null || x.Status.Contains(status))
                            && (message == null || x.Message.Contains(message)))
                .OrderByDescending(x => x.Date)
                .ToList();

            return new JsonResult(ermsLogList.ToList());
        }
    }
}