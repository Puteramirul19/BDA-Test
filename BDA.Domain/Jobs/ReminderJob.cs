using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDA.Data;
using BDA.Entities;
using BDA.Services;
using Hangfire;
using Microsoft.Extensions.Options;

namespace BDA.Jobs
{
    public class ReminderJob
    {
        private BdaDBContext db;
        private NotificationSettings notificationSettings;
        private IBackgroundJobClient job;

        public ReminderJob(IOptions<NotificationSettings> notificationSettings,
                                    IBackgroundJobClient job, BdaDBContext db)
        {
            this.job = job;
            this.db = db;
            this.notificationSettings = notificationSettings.Value;
        }

        public EmailTemplates GetTemplate(string id)
        {
            var tpl = db.EmailTemplates.FirstOrDefault(x => x.Id == id);
        
            if (!String.IsNullOrEmpty(notificationSettings.SubjectPrefix))
                tpl.Subject = notificationSettings.SubjectPrefix + " " + tpl.Subject;

            return tpl;
        }


        public static readonly DateTime[] reminderWeekly = new[]
        {
            DateTime.Today.AddDays(-7),
            DateTime.Today.AddDays(-14),
            DateTime.Today.AddDays(-21),
            DateTime.Today.AddDays(-28),
            DateTime.Today.AddDays(-35),
            DateTime.Today.AddDays(-42),
            DateTime.Today.AddDays(-49),
            DateTime.Today.AddDays(-56),
            DateTime.Today.AddDays(-63),
            DateTime.Today.AddDays(-70),
            DateTime.Today.AddDays(-84)
        };

        public static readonly DateTime[] reminderQuarterly = new[]
        {
            DateTime.Today.AddMonths(-3),
            DateTime.Today.AddMonths(-6),
            DateTime.Today.AddMonths(-9),
            DateTime.Today.AddMonths(-12),
            DateTime.Today.AddMonths(-15),
            DateTime.Today.AddMonths(-18),
            DateTime.Today.AddMonths(-21),
            DateTime.Today.AddMonths(-24),
            DateTime.Today.AddMonths(-27),
            DateTime.Today.AddMonths(-30),
            DateTime.Today.AddMonths(-33),
            DateTime.Today.AddMonths(-36)
        };

        // Remind requester to accept BD every week after TGBS Banking issued BD to requester
        public void RemindRequesterForBDAcceptance()
        {
            //  var bdList = db.BankDraft.
            //     Where(x => x.Status == "Issued" && x.CompletedOn == null && x.TGBSIssuedOn != null &&
            //     (
            //      x.TGBSIssuedOn.Value.ToShortDateString() == DateTime.Now.AddDays(-7).ToShortDateString() ||
            //      x.TGBSIssuedOn.Value.ToShortDateString() == DateTime.Now.AddDays(-14).ToShortDateString() ||
            //      x.TGBSIssuedOn.Value.ToShortDateString() == DateTime.Now.AddDays(-21).ToShortDateString() ||
            //      x.TGBSIssuedOn.Value.ToShortDateString() == DateTime.Now.AddDays(-28).ToShortDateString() ||
            //      x.TGBSIssuedOn.Value.ToShortDateString() == DateTime.Now.AddDays(-35).ToShortDateString() ||
            //      x.TGBSIssuedOn.Value.ToShortDateString() == DateTime.Now.AddDays(-42).ToShortDateString() ||
            //      x.TGBSIssuedOn.Value.ToShortDateString() == DateTime.Now.AddDays(-49).ToShortDateString() ||
            //      x.TGBSIssuedOn.Value.ToShortDateString() == DateTime.Now.AddDays(-56).ToShortDateString() ||
            //      x.TGBSIssuedOn.Value.ToShortDateString() == DateTime.Now.AddDays(-63).ToShortDateString() ||
            //      x.TGBSIssuedOn.Value.ToShortDateString() == DateTime.Now.AddDays(-70).ToShortDateString() ||
            //      x.TGBSIssuedOn.Value.ToShortDateString() == DateTime.Now.AddDays(-84).ToShortDateString()
            //     ))
            //.ToList();
            var bdList = db.BankDraft.Where(x => x.Status == "Issued" && x.CompletedOn == null  && x.TGBSIssuedOn != null && reminderWeekly.Contains(x.TGBSIssuedOn.Value.Date)).ToList();


            if (bdList == null)
                return;

            foreach (var bd in bdList)
            {
               RemindRequesterForBDAcceptance(bd, bd.GetMessageValues());
            }

        }

        public void RemindRequesterForBDAcceptance(BankDraft bd, IDictionary<string, string> values)
        {
            var tpl = GetTemplate("B-1E07 Application Awaiting Receipt to Requester (Reminder)");
            var requester = db.Users.Where(x => x.UserName == bd.RequesterId).FirstOrDefault();

            var requesterName = db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefault();
            if (requesterName == null)
                return;


            if (bd.Type == "WangCagaran")
            {
                var verifierName = db.Users.Where(x => x.UserName == bd.VerifierId).Select(x => x.FullName).FirstOrDefault();
                if (verifierName == null)
                    return;

                values.Add("VerifierName", verifierName);
            }


            var approverName = db.Users.Where(x => x.UserName == bd.ApproverId).Select(x => x.FullName).FirstOrDefault();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            values.Add("ApproverName", approverName);

            job.Enqueue<NotificationService>(x => x.SendEmail(requester.Email, requester.FullName, tpl, values));

        }


        // Remind requester to submit First Partial every 3 months once it is in Processed status
        public void RemindRequesterForFirstPartialRecoverySubmission()
        {
            //var rList_old = db.Recovery.Where(x => x.RecoveryType == "FirstPartial" && x.Status == "Processed" && x.PartialSubmittedOn == null && x.ProcessedOn != null &&
            //                                   (
            //                                       x.ProcessedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-3).ToShortDateString() ||
            //                                       x.ProcessedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-6).ToShortDateString() ||
            //                                       x.ProcessedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-9).ToShortDateString() ||
            //                                       x.ProcessedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-12).ToShortDateString() ||
            //                                       x.ProcessedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-15).ToShortDateString() ||
            //                                       x.ProcessedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-18).ToShortDateString() ||
            //                                       x.ProcessedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-21).ToShortDateString() ||
            //                                       x.ProcessedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-24).ToShortDateString() ||
            //                                       x.ProcessedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-27).ToShortDateString() ||
            //                                       x.ProcessedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-30).ToShortDateString() ||
            //                                       x.ProcessedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-33).ToShortDateString() ||
            //                                       x.ProcessedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-36).ToShortDateString() 
            //                                   )).ToList();
            //var rList = db.Recovery.Where(x => x.RecoveryType == "FirstPartial" && x.Status == "Processed" && x.PartialSubmittedOn == null && x.ProcessedOn != null &&
            //(
            //   x.ProcessedOn.Value.Date == DateTime.Now.AddMonths(-3).Date ||
            //   x.ProcessedOn.Value.Date == DateTime.Now.AddMonths(-6).Date ||
            //   x.ProcessedOn.Value.Date == DateTime.Now.AddMonths(-9).Date ||
            //   x.ProcessedOn.Value.Date == DateTime.Now.AddMonths(-12).Date ||
            //   x.ProcessedOn.Value.Date == DateTime.Now.AddMonths(-15).Date ||
            //   x.ProcessedOn.Value.Date == DateTime.Now.AddMonths(-18).Date ||
            //   x.ProcessedOn.Value.Date == DateTime.Now.AddMonths(-21).Date ||
            //   x.ProcessedOn.Value.Date == DateTime.Now.AddMonths(-24).Date ||
            //   x.ProcessedOn.Value.Date == DateTime.Now.AddMonths(-27).Date ||
            //   x.ProcessedOn.Value.Date == DateTime.Now.AddMonths(-30).Date ||
            //   x.ProcessedOn.Value.Date == DateTime.Now.AddMonths(-33).Date ||
            //   x.ProcessedOn.Value.Date == DateTime.Now.AddMonths(-36).Date 
            //)).ToList();

            var rList = db.Recovery.Where(x => x.RecoveryType == "FirstPartial" && x.Status == "Processed" && x.PartialSubmittedOn == null && x.ProcessedOn != null 
            && reminderQuarterly.Contains(x.ProcessedOn.Value.Date)).ToList();

            if (rList == null)
                return;

            foreach (var r in rList)
            {
                RemindRequesterForFirstPartialRecoverySubmission(r, r.GetMessageValues());
            }

        }

        public void RemindRequesterForFirstPartialRecoverySubmission(Recovery r, IDictionary<string, string> values)
        {
            var tpl = GetTemplate("R-2E07 Recovery Reminder 1st Partial");
            var requester = db.Users.Where(x => x.UserName == r.RequesterId).FirstOrDefault();

            var requesterName = db.Users.Where(x => x.UserName == r.RequesterId).Select(x => x.FullName).FirstOrDefault();
            if (requesterName == null)
                return;

            values.Add("RequesterName", requesterName);

            job.Enqueue<NotificationService>(x => x.SendEmail(requester.Email, requester.FullName, tpl, values));

        }

        // Remind requester to submit Second Partial every 1 months once it is in PartialComplete status
        public void RemindRequesterForSecondPartialRecoverySubmission()
        {
            //var rList_old = db.Recovery.Where(x => (x.RecoveryType == "SecondPartial" && x.Status == "PartialComplete" && x.FinalSubmissionOn == null && x.PartialCompletedOn != null &&
            //(
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-1).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-2).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-3).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-4).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-5).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-6).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-7).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-8).ToShortDateString() || 
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-9).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-10).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-11).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-12).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-13).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-14).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-15).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-16).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-17).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-18).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-19).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-20).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-21).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-22).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-23).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-24).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-25).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-26).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-27).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-28).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-29).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-30).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-31).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-32).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-33).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-34).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-35).ToShortDateString() ||
            //   x.PartialCompletedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-36).ToShortDateString() 
            //)
            //)).ToList();
            
            //var rList = db.Recovery.Where(x => (x.RecoveryType == "SecondPartial" && x.Status == "PartialComplete" && x.FinalSubmissionOn == null && x.PartialCompletedOn != null &&
            //(
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-1).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-2).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-3).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-4).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-5).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-6).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-7).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-8).Date || 
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-9).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-10).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-11).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-12).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-13).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-14).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-15).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-16).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-17).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-18).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-19).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-20).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-21).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-22).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-23).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-24).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-25).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-26).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-27).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-28).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-29).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-30).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-31).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-32).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-33).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-34).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-35).Date ||
            //   x.PartialCompletedOn.Value.Date == DateTime.Now.AddMonths(-36).Date 
            //)
            //)).ToList();

            var range = Enumerable.Range(1, 36).Select(i => DateTime.Today.AddMonths(-i)).ToArray();
            var rList = db.Recovery.Where(x => (x.RecoveryType == "SecondPartial" && x.Status == "PartialComplete" && x.FinalSubmissionOn == null && x.PartialCompletedOn != null 
            && range.Contains(x.PartialCompletedOn.Value.Date))).ToList();

            if (rList == null)
                return;

            foreach (var r in rList)
            {
                RemindRequesterForSecondPartialRecoverySubmission(r, r.GetMessageValues());
            }

        }

        public void RemindRequesterForSecondPartialRecoverySubmission(Recovery r, IDictionary<string, string> values)
        {
            var tpl = GetTemplate("R-2E11 Recovery Reminder 2nd Partial");
            var requester = db.Users.Where(x => x.UserName == r.RequesterId).FirstOrDefault();

            var requesterName = db.Users.Where(x => x.UserName == r.RequesterId).Select(x => x.FullName).FirstOrDefault();
            if (requesterName == null)
                return;

            values.Add("RequesterName", requesterName);

            job.Enqueue<NotificationService>(x => x.SendEmail(requester.Email, requester.FullName, tpl, values));

        }

        // Remind requester to key in CPC Date every 3 month
        public void RemindRequesterForRecoveryProcess()
        {
            //var rList_old = db.Recovery.Where(x => x.Status == "Created" && x.ProcessedOn == null && x.CreatedOn != null &&
            //                                   (
            //                                       x.CreatedOn.ToShortDateString() == DateTime.Now.AddMonths(-3).ToShortDateString() ||
            //                                       x.CreatedOn.ToShortDateString() == DateTime.Now.AddMonths(-6).ToShortDateString() ||
            //                                       x.CreatedOn.ToShortDateString() == DateTime.Now.AddMonths(-9).ToShortDateString() ||
            //                                       x.CreatedOn.ToShortDateString() == DateTime.Now.AddMonths(-12).ToShortDateString() ||
            //                                       x.CreatedOn.ToShortDateString() == DateTime.Now.AddMonths(-15).ToShortDateString() ||
            //                                       x.CreatedOn.ToShortDateString() == DateTime.Now.AddMonths(-18).ToShortDateString() ||
            //                                       x.CreatedOn.ToShortDateString() == DateTime.Now.AddMonths(-21).ToShortDateString() ||
            //                                       x.CreatedOn.ToShortDateString() == DateTime.Now.AddMonths(-24).ToShortDateString() ||
            //                                       x.CreatedOn.ToShortDateString() == DateTime.Now.AddMonths(-27).ToShortDateString() ||
            //                                       x.CreatedOn.ToShortDateString() == DateTime.Now.AddMonths(-30).ToShortDateString() ||
            //                                       x.CreatedOn.ToShortDateString() == DateTime.Now.AddMonths(-33).ToShortDateString() ||
            //                                       x.CreatedOn.ToShortDateString() == DateTime.Now.AddMonths(-36).ToShortDateString()
            //                                   )
            //).ToList();

            //var rList = db.Recovery.Where(x => x.Status == "Created" && x.ProcessedOn == null && x.CreatedOn != null &&
            //(
            //    x.CreatedOn.Date == DateTime.Now.AddMonths(-3).Date||
            //    x.CreatedOn.Date == DateTime.Now.AddMonths(-6).Date ||
            //    x.CreatedOn.Date == DateTime.Now.AddMonths(-9).Date ||
            //    x.CreatedOn.Date == DateTime.Now.AddMonths(-12).Date ||
            //    x.CreatedOn.Date == DateTime.Now.AddMonths(-15).Date ||
            //    x.CreatedOn.Date == DateTime.Now.AddMonths(-18).Date ||
            //    x.CreatedOn.Date == DateTime.Now.AddMonths(-21).Date ||
            //    x.CreatedOn.Date == DateTime.Now.AddMonths(-24).Date ||
            //    x.CreatedOn.Date == DateTime.Now.AddMonths(-27).Date ||
            //    x.CreatedOn.Date == DateTime.Now.AddMonths(-30).Date ||
            //    x.CreatedOn.Date == DateTime.Now.AddMonths(-33).Date ||
            //    x.CreatedOn.Date == DateTime.Now.AddMonths(-36).Date
            //)
            //).ToList();
            var rList = db.Recovery.Where(x => x.Status == "Created" && x.ProcessedOn == null && x.CreatedOn != null 
            && reminderQuarterly.Contains(x.CreatedOn.Date)).ToList();

            if (rList == null)
                return;

            foreach (var r in rList)
            {
                RemindRequesterForRecoveryProcess(r, r.GetMessageValues());
            }

        }

        public void RemindRequesterForRecoveryProcess(Recovery r, IDictionary<string, string> values)
        {
            var tpl = GetTemplate("R-2E02 Recovery Reminder To Key In CPC Date");
            var requester = db.Users.Where(x => x.UserName == r.RequesterId).FirstOrDefault();

            var requesterName = db.Users.Where(x => x.UserName == r.RequesterId).Select(x => x.FullName).FirstOrDefault();
            if (requesterName == null)
                return;

            values.Add("RequesterName", requesterName);

            job.Enqueue<NotificationService>(x => x.SendEmail(requester.Email, requester.FullName, tpl, values));

        }

        // Remind requester to submit Full Submission every 3 months once it is in Processed status
        public void RemindRequesterForFullRecoverySubmission()
        {
            //var rList_old = db.Recovery.Where(x => x.RecoveryType == "Full" && x.Status == "Processed" && x.FinalSubmissionOn == null && x.ProcessedOn != null &&
            //                                   (
            //                                       x.ProcessedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-3).ToShortDateString() ||
            //                                       x.ProcessedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-6).ToShortDateString() ||
            //                                       x.ProcessedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-9).ToShortDateString() ||
            //                                       x.ProcessedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-12).ToShortDateString() ||
            //                                       x.ProcessedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-15).ToShortDateString() ||
            //                                       x.ProcessedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-18).ToShortDateString() ||
            //                                       x.ProcessedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-21).ToShortDateString() ||
            //                                       x.ProcessedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-24).ToShortDateString() ||
            //                                       x.ProcessedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-27).ToShortDateString() ||
            //                                       x.ProcessedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-30).ToShortDateString() ||
            //                                       x.ProcessedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-33).ToShortDateString() ||
            //                                       x.ProcessedOn.Value.ToShortDateString() == DateTime.Now.AddMonths(-36).ToShortDateString() 
            //                                   )).ToList();

            //var rList = db.Recovery.Where(x => x.RecoveryType == "Full" && x.Status == "Processed" && x.FinalSubmissionOn == null && x.ProcessedOn != null &&
            //   (
            //        x.ProcessedOn.Value.Date == DateTime.Now.AddMonths(-3).Date ||
            //        x.ProcessedOn.Value.Date == DateTime.Now.AddMonths(-6).Date ||
            //        x.ProcessedOn.Value.Date == DateTime.Now.AddMonths(-9).Date ||
            //        x.ProcessedOn.Value.Date == DateTime.Now.AddMonths(-12).Date ||
            //        x.ProcessedOn.Value.Date == DateTime.Now.AddMonths(-15).Date ||
            //        x.ProcessedOn.Value.Date == DateTime.Now.AddMonths(-18).Date ||
            //        x.ProcessedOn.Value.Date == DateTime.Now.AddMonths(-21).Date ||
            //        x.ProcessedOn.Value.Date == DateTime.Now.AddMonths(-24).Date ||
            //        x.ProcessedOn.Value.Date == DateTime.Now.AddMonths(-27).Date ||
            //        x.ProcessedOn.Value.Date == DateTime.Now.AddMonths(-30).Date ||
            //        x.ProcessedOn.Value.Date == DateTime.Now.AddMonths(-33).Date ||
            //        x.ProcessedOn.Value.Date == DateTime.Now.AddMonths(-36).Date 
            //)).ToList();

            var rList = db.Recovery.Where(x => x.RecoveryType == "Full" && x.Status == "Processed" && x.FinalSubmissionOn == null && x.ProcessedOn != null 
            && reminderQuarterly.Contains(x.ProcessedOn.Value.Date)).ToList();
             

            if (rList == null)
                return;

            foreach (var r in rList)
            {
                RemindRequesterForFullRecoverySubmission(r, r.GetMessageValues());
            }

        }

        public void RemindRequesterForFullRecoverySubmission(Recovery r, IDictionary<string, string> values)
        {
            var tpl = GetTemplate("R-2E04 Recovery Reminder Full Submission");
            var requester = db.Users.Where(x => x.UserName == r.RequesterId).FirstOrDefault();

            var requesterName = db.Users.Where(x => x.UserName == r.RequesterId).Select(x => x.FullName).FirstOrDefault();
            if (requesterName == null)
                return;

            values.Add("RequesterName", requesterName);

            job.Enqueue<NotificationService>(x => x.SendEmail(requester.Email, requester.FullName, tpl, values));

        }

    }
}
