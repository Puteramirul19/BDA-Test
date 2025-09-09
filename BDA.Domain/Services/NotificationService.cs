using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using BDA.Data;
using BDA.Entities;
using BDA.Identity;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Attachment = System.Net.Mail.Attachment;

namespace BDA.Services
{
    public class NotificationSettings
    {
        public NotificationSettings()
        {
            GlobalValues = new Dictionary<string, string>();
        }

        public string SubjectPrefix { get; set; }
        public Dictionary<string, string> GlobalValues { get; set; }
    }

    public class NotificationService
    {
        private readonly IBackgroundJobClient job;
        private readonly BdaDBContext db;
        private readonly IEmailSender emailSender;
        private readonly NotificationSettings notificationSettings;

        public NotificationService(IOptions<NotificationSettings> notificationSettings,
                                    IBackgroundJobClient job, BdaDBContext db,
                                    IEmailSender emailSender)
        {
            this.job = job;
            this.db = db;
            this.emailSender = emailSender;
            this.notificationSettings = notificationSettings.Value;
        }

        //private IDictionary<string, string> globalValues = new Dictionary<string, string>
        //{
        //    { "BaseUrl", "http://diaudit.com" }
        //};

        private IDictionary<string, string> PrepareValues(IDictionary<string, string> values)
        {
            var merged = new Dictionary<string, string>(notificationSettings.GlobalValues);
            values.ToList().ForEach(x => merged.Add(x.Key, x.Value));
            return merged;
        }

        private IDictionary<string, string> PrepareValues()
        {
            var merged = new Dictionary<string, string>(notificationSettings.GlobalValues);
            //values.ToList().ForEach(x => merged.Add(x.Key, x.Value));
            return merged;
        }

        private async Task<EmailTemplates> GetTemplate(string id)
        {
            var tpl = await db.EmailTemplates.FirstOrDefaultAsync(x => x.Id == id);

            if (!String.IsNullOrEmpty(notificationSettings.SubjectPrefix))
                tpl.Subject = notificationSettings.SubjectPrefix + " " + tpl.Subject;

            return tpl;
        }

        public async Task SendEmail(string receiverAddress, string receiverName, EmailTemplates template, IDictionary<string, string> values)
        {
            if (String.IsNullOrEmpty(receiverAddress) || template == null)
                return;

            //values.Add("BaseUrl2", "<a href='https://bdastaging.tnb.com.my/Edit/0c520e9b-e8a3-48e1-e359-08d793f2a463'>here</a>");

            var mergedValues = PrepareValues(values);
            mergedValues["ReceiverEmail"] = receiverAddress;
            mergedValues["ReceiverName"] = receiverName;

            var mailMsg = template.Parse(mergedValues);
            mailMsg.To.Add(new MailAddress(receiverAddress, receiverName));
            //var uniqueFileName = "5-silent-killers-cats-475212379_d30e.jpg";
            //var ext = Path.GetExtension(uniqueFileName);
            //var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/documents", uniqueFileName);
            //mailMsg.Attachments.Add(new Attachment(filePath));

            await emailSender.SendEmailAsync(mailMsg);
        }

        public async Task SendEmail2(string receiverAddress, string receiverName, EmailTemplates template, IDictionary<string, string> values, string ccList)
        {
            if (String.IsNullOrEmpty(receiverAddress) || template == null)
                return;

            //values.Add("BaseUrl2", "<a href='https://bdastaging.tnb.com.my/Edit/0c520e9b-e8a3-48e1-e359-08d793f2a463'>here</a>");

            var mergedValues = PrepareValues(values);
            mergedValues["ReceiverEmail"] = receiverAddress;
            mergedValues["ReceiverName"] = receiverName;

            var mailMsg = template.Parse(mergedValues);
            mailMsg.To.Add(new MailAddress(receiverAddress, receiverName));
            mailMsg.CC.Add(ccList);
            //var uniqueFileName = "5-silent-killers-cats-475212379_d30e.jpg";
            //var ext = Path.GetExtension(uniqueFileName);
            //var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/documents", uniqueFileName);
            //mailMsg.Attachments.Add(new Attachment(filePath));

            await emailSender.SendEmailAsync(mailMsg);
        }

        public async Task SendEmail(string receiverAddress, string receiverName, EmailTemplates template, IDictionary<string, string> values, string fileName)
        {
            if (String.IsNullOrEmpty(receiverAddress) || template == null)
                return;

            var mergedValues = PrepareValues(values);
            mergedValues["ReceiverEmail"] = receiverAddress;
            mergedValues["ReceiverName"] = receiverName;

            var mailMsg = template.Parse(mergedValues);
            mailMsg.To.Add(new MailAddress(receiverAddress, receiverName));
            await Task.Delay(180000);
            var uniqueFileName = fileName;
            var ext = Path.GetExtension(uniqueFileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/documents", uniqueFileName);
            mailMsg.Attachments.Add(new Attachment(filePath));
            await emailSender.SendEmailAsync(mailMsg);

        }

        public async Task SendEmail(string receiverAddress, string receiverName, EmailTemplates template, IDictionary<string, string> values, string fileName, string ccList)
        {
            if (String.IsNullOrEmpty(receiverAddress) || template == null)
                return;

            var mergedValues = PrepareValues(values);
            mergedValues["ReceiverEmail"] = receiverAddress;
            mergedValues["ReceiverName"] = receiverName;

            var mailMsg = template.Parse(mergedValues);
            mailMsg.To.Add(new MailAddress(receiverAddress, receiverName));
            mailMsg.CC.Add(ccList);
            await Task.Delay(180000);
            var uniqueFileName = fileName;
            var ext = Path.GetExtension(uniqueFileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/documents", uniqueFileName);
            mailMsg.Attachments.Add(new Attachment(filePath));
            await emailSender.SendEmailAsync(mailMsg);

        }

        public async Task SendEmail(string receiverAddress, string receiverName, EmailTemplates template)
        {
            if (String.IsNullOrEmpty(receiverAddress) || template == null)
                return;

            var mergedValues = PrepareValues(); ;
            mergedValues["ReceiverEmail"] = receiverAddress;
            mergedValues["ReceiverName"] = receiverName;

            var mailMsg = template.Parse(mergedValues);
            mailMsg.To.Add(new MailAddress(receiverAddress, receiverName));

            await emailSender.SendEmailAsync(mailMsg);
        }

        //Application WC & WH modules
        public async Task NotifyVerifierForVerification(Guid bdId)
        {
            var bd = await db.BankDraft.FirstOrDefaultAsync(x => x.Id == bdId);
            if (bd == null)
                return;

            await NotifyVerifierForVerification(bd, bd.GetMessageValues());

        }

        public async Task NotifyVerifierForVerification(BankDraft bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("B-1E01 Application Verification to Verifier");
            var verifier = db.Users.Where(x => x.UserName == bd.VerifierId).FirstOrDefault();

            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            var verifierName = await db.Users.Where(x => x.UserName == bd.VerifierId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (verifierName == null)
                return;

            var approverName = await db.Users.Where(x => x.UserName == bd.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            values.Add("VerifierName", verifierName);
            values.Add("ApproverName", approverName);

            job.Enqueue<NotificationService>(x => x.SendEmail(verifier.Email, verifier.FullName, tpl, values));

        }


        public async Task NotifyApproverForApproval(Guid bdId)
        {
            var bd = await db.BankDraft.FirstOrDefaultAsync(x => x.Id == bdId);
            if (bd == null)
                return;


            await NotifyApproverForApproval(bd,bd.GetMessageValues());

        }

        public async Task NotifyApproverForApproval(BankDraft bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("B-1E02 Application Approval to Approver");
            var approver = db.Users.Where(x => x.UserName == bd.ApproverId).FirstOrDefault();


            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            if(bd.Type == "WangCagaran")
            {
                var verifierName = await db.Users.Where(x => x.UserName == bd.VerifierId).Select(x => x.FullName).FirstOrDefaultAsync();
                if (verifierName == null)
                    return;

                values.Add("VerifierName", verifierName);
            }

            var approverName = await db.Users.Where(x => x.UserName == bd.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            values.Add("ApproverName", approverName);

            job.Enqueue<NotificationService>(x => x.SendEmail(approver.Email, approver.FullName, tpl, values));

        }

        public async Task NotifyApproverForWithdrawn(Guid bdId)
        {
            var bd = await db.BankDraft.FirstOrDefaultAsync(x => x.Id == bdId);
            if (bd == null)
                return;


            await NotifyApproverForWithdrawn(bd, bd.GetMessageValues());

        }

        public async Task NotifyApproverForWithdrawn(BankDraft bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("B-1E11 Application Withdrawal to Verifier/Approver");

            var approver = db.Users.Where(x => x.UserName == bd.ApproverId).FirstOrDefault();
            var verifier = db.Users.Where(x => x.UserName == bd.VerifierId).FirstOrDefault();

            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            var approverName = await db.Users.Where(x => x.UserName == bd.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            if (bd.Type == "WangCagaran")
            {
                var verifierName = await db.Users.Where(x => x.UserName == bd.VerifierId).Select(x => x.FullName).FirstOrDefaultAsync();
                if (verifierName == null)
                    return;

                values.Add("VerifierName", verifierName);
            }
            else
            {
                values.Add("VerifierName", approverName);
            }

            values.Add("RequesterName", requesterName);
            values.Add("ApproverName", approverName);

            if (bd.Type == "WangHangus")
            {
                job.Enqueue<NotificationService>(x => x.SendEmail(approver.Email, approver.FullName, tpl, values));
            }
            else if (bd.Type == "WangCagaran")
            {
                job.Enqueue<NotificationService>(x => x.SendEmail(verifier.Email, verifier.FullName, tpl, values));
            }
            

        }

        public void RemindRequesterForBDAcceptance(object p)
        {
            throw new NotImplementedException();
        }

        public async Task NotifyTGBSBankingForAcceptance(Guid bdId)
        {
            var bd = await db.BankDraft.FirstOrDefaultAsync(x => x.Id == bdId);
            if (bd == null)
                return;

            await NotifyTGBSBankingForAcceptance(bd, bd.GetMessageValues());

        }

        public async Task NotifyTGBSBankingForAcceptance(BankDraft bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("B-1E03 Application Acceptance to TGBS Banking");

            var tgbsBanking = db.Users
                       .Join(db.UserRoles,
                               u => u.Id,
                               r => r.UserId,
                               (u, r) => new
                               {
                                   id = u.Id,
                                   fullName = u.FullName,
                                   email = u.Email,
                                   roleId = r.RoleId,
                                   active = u.IsActive
                               }
                           )
                       .Where(x => ((x.roleId == "TB" || x.roleId == "BA") && x.active == true))
                       .ToList();

            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;


            if (bd.Type == "WangCagaran")
            {
                var verifierName = await db.Users.Where(x => x.UserName == bd.VerifierId).Select(x => x.FullName).FirstOrDefaultAsync();
                if (verifierName == null)
                    return;

                values.Add("VerifierName", verifierName);
            }


            var approverName = await db.Users.Where(x => x.UserName == bd.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            //values.Add("VerifierName", verifierName);
            values.Add("ApproverName", approverName);

            foreach (var user in tgbsBanking)
            {
                job.Enqueue<NotificationService>(x => x.SendEmail(user.email, user.fullName, tpl, values));
            }

        }

        public async Task NotifyRequesterForTGBSAcceptance(Guid bdId)
        {
            var bd = await db.BankDraft.FirstOrDefaultAsync(x => x.Id == bdId);
            if (bd == null)
                return;

            await NotifyRequesterForTGBSAcceptance(bd, bd.GetMessageValues());

        }

        public async Task NotifyRequesterForTGBSAcceptance(BankDraft bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("B-1E04 Application Process to Requester (As Acknowledgement)");
            var requester = db.Users.Where(x => x.UserName == bd.RequesterId).FirstOrDefault();

            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;


            if (bd.Type == "WangCagaran")
            {
                var verifierName = await db.Users.Where(x => x.UserName == bd.VerifierId).Select(x => x.FullName).FirstOrDefaultAsync();
                if (verifierName == null)
                    return;

                values.Add("VerifierName", verifierName);
            }


            var approverName = await db.Users.Where(x => x.UserName == bd.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            //values.Add("VerifierName", verifierName);
            values.Add("ApproverName", approverName);

            job.Enqueue<NotificationService>(x => x.SendEmail(requester.Email, requester.FullName, tpl, values));

        }

        public async Task NotifyBankForProcessing(Guid letterId)
        {
            var ilr = await db.InstructionLetter.FirstOrDefaultAsync(x => x.Id == letterId);
            if (ilr == null)
                return;
            else
            {
                var bdRefNo = ilr.ReferenceNo.Split(",");
                var bd = await db.BankDraft.Where(x => x.RefNo == bdRefNo[0]).FirstOrDefaultAsync();

                await NotifyBankForProcessing(ilr, bd.GetMessageValues());
            }
        }

        public async Task NotifyBankForProcessing(InstructionLetter ilr, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("B-1E05 Application with Instruction Letter to Bank");
            var bankEmail = ilr.InstructionLetterEmail;
            var bankName = ilr.BankName;
            var bdRefNo = ilr.ReferenceNo.Split(",");

            var tgbsBanking = db.Users
                       .Join(db.UserRoles,
                               u => u.Id,
                               r => r.UserId,
                               (u, r) => new
                               {
                                   id = u.Id,
                                   fullName = u.FullName,
                                   email = u.Email,
                                   roleId = r.RoleId,
                                   active = u.IsActive
                               }
                           )
                       .Where(x => ((x.roleId == "TB" || x.roleId == "BA") && x.active == true))
                       .ToList();

            var ccList = "";

            foreach (var user in tgbsBanking)
            {
                ccList += user.email + ",";
            }
            ccList = ccList.Remove(ccList.Trim().Length - 1);

            int count = 0;
            var bdList = "";
            foreach(var refNo in bdRefNo)
            {
                count++;
                bdList += count + ". " + refNo + "\n";
            }
            values.Add("BDListForLetter", bdList);
            values.Add("BankPIC", ilr.BankPIC);
            values.Add("BankName", bankName);

            var bd = await db.BankDraft.Where(x => x.RefNo == bdRefNo[0]).FirstOrDefaultAsync();

            var attachment = await db.Attachment.Where(x => x.ParentId == bd.Id && x.FileType == Data.AttachmentType.InstructionLetter.ToString()).FirstOrDefaultAsync();

            job.Enqueue<NotificationService>(x => x.SendEmail(bankEmail, bankName, tpl, values, attachment.FileName, ccList));

        }

        public async Task NotifyRequesterForBDAcceptance(Guid bdId)
        {
            var bd = await db.BankDraft.FirstOrDefaultAsync(x => x.Id == bdId);
            if (bd == null)
                return;

            await NotifyRequesterForBDAcceptance(bd, bd.GetMessageValues());

        }

        public async Task NotifyRequesterForBDAcceptance(BankDraft bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("B-1E06 Application with Covering Memo to Requester");
            var requester = db.Users.Where(x => x.UserName == bd.RequesterId).FirstOrDefault();

            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;


            if (bd.Type == "WangCagaran")
            {
                var verifierName = await db.Users.Where(x => x.UserName == bd.VerifierId).Select(x => x.FullName).FirstOrDefaultAsync();
                if (verifierName == null)
                    return;

                values.Add("VerifierName", verifierName);
            }


            var approverName = await db.Users.Where(x => x.UserName == bd.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            //values.Add("VerifierName", verifierName);
            values.Add("ApproverName", approverName);

            //send Memo Letter to requester
            var attachment = await db.Attachment.Where(x => x.ParentId == bd.Id && x.FileType == "Memo").FirstOrDefaultAsync();

            job.Enqueue<NotificationService>(x => x.SendEmail(requester.Email, requester.FullName, tpl, values, attachment.FileName));

        }

        public async Task NotifyRequesterForBDAcceptanceFromBulkMemo(Guid memoId)
        {
            var memo = await db.Memo.FirstOrDefaultAsync(x => x.Id == memoId);
            if (memo == null)
                return;
            else
            {
               // var bdRefNo = memo.ReferenceNo.Split(",");
                //var bd = await db.BankDraft.Where(x => x.RefNo == bdRefNo[0]).FirstOrDefaultAsync();

                await NotifyRequesterForBDAcceptanceFromBulkMemo(memo);
            }

            //await NotifyRequesterForBDAcceptanceFromBulkMemo(memo, memo.GetMessageValues());

        }

        public async Task NotifyRequesterForBDAcceptanceFromBulkMemo(Memo memo)
        {
            var tpl = await GetTemplate("B-1E06 Application with Covering Memo to Requester");

            var bdRefNo = memo.ReferenceNo.Split(",");

            foreach(var refNo in bdRefNo)
            {
               
                var bd = await db.BankDraft.Where(x => x.RefNo == refNo).FirstOrDefaultAsync();
                IDictionary<string, string> values = null;
                values = bd.GetMessageValues();
                var requester = db.Users.Where(x => x.UserName == bd.RequesterId).FirstOrDefault();

                var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
                if (requesterName == null)
                    return;


                if (bd.Type == "WangCagaran")
                {
                    var verifierName = await db.Users.Where(x => x.UserName == bd.VerifierId).Select(x => x.FullName).FirstOrDefaultAsync();
                    if (verifierName == null)
                        return;

                    values.Add("VerifierName", verifierName);
                }


                var approverName = await db.Users.Where(x => x.UserName == bd.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
                if (approverName == null)
                    return;

                values.Add("RequesterName", requesterName);
                values.Add("ApproverName", approverName);

                //send Memo Letter to requester
                var attachment = await db.Attachment.Where(x => x.ParentId == bd.Id && x.FileType == "Memo").FirstOrDefaultAsync();

                job.Enqueue<NotificationService>(x => x.SendEmail(requester.Email, requester.FullName, tpl, values, attachment.FileName));
            }
         
        }

        public async Task NotifyRequesterForVerification(Guid bdId)
        {
            var bd = await db.BankDraft.FirstOrDefaultAsync(x => x.Id == bdId);
            if (bd == null)
                return;

            await NotifyRequesterForVerification(bd, bd.GetMessageValues());

        }

        public async Task NotifyRequesterForVerification(BankDraft bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("B-1E12 Application Verification to Requester  (As Acknowledgement)");
            var requester = db.Users.Where(x => x.UserName == bd.RequesterId).FirstOrDefault();

            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;


            if (bd.Type == "WangCagaran")
            {
                var verifierName = await db.Users.Where(x => x.UserName == bd.VerifierId).Select(x => x.FullName).FirstOrDefaultAsync();
                if (verifierName == null)
                    return;

                values.Add("VerifierName", verifierName);
            }

            var approverName = await db.Users.Where(x => x.UserName == bd.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            //values.Add("VerifierName", verifierName);
            values.Add("ApproverName", approverName);

            job.Enqueue<NotificationService>(x => x.SendEmail(requester.Email, requester.FullName, tpl, values));

        }

        public async Task NotifyRequesterForApproval(Guid bdId)
        {
            var bd = await db.BankDraft.FirstOrDefaultAsync(x => x.Id == bdId);
            if (bd == null)
                return;

            await NotifyRequesterForApproval(bd, bd.GetMessageValues());

        }

        public async Task NotifyRequesterForApproval(BankDraft bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("B-1E13 Application Approval to Requester  (As Acknowledgement)");
            var requester = db.Users.Where(x => x.UserName == bd.RequesterId).FirstOrDefault();

            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;


            if (bd.Type == "WangCagaran")
            {
                var verifierName = await db.Users.Where(x => x.UserName == bd.VerifierId).Select(x => x.FullName).FirstOrDefaultAsync();
                if (verifierName == null)
                    return;

                values.Add("VerifierName", verifierName);
            }


            var approverName = await db.Users.Where(x => x.UserName == bd.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            //values.Add("VerifierName", verifierName);
            values.Add("ApproverName", approverName);

            job.Enqueue<NotificationService>(x => x.SendEmail(requester.Email, requester.FullName, tpl, values));

        }


        public async Task NotifyRequesterForVerificationRejection(Guid bdId)
        {
            var bd = await db.BankDraft.FirstOrDefaultAsync(x => x.Id == bdId);
            if (bd == null)
                return;

            await NotifyRequesterForVerificationRejection(bd, bd.GetMessageValues());

        }

        public async Task NotifyRequesterForVerificationRejection(BankDraft bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("B-1E08 Application Verification Rejection to Requester");
            var requester = db.Users.Where(x => x.UserName == bd.RequesterId).FirstOrDefault();

            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;


            if (bd.Type == "WangCagaran")
            {
                var verifierName = await db.Users.Where(x => x.UserName == bd.VerifierId).Select(x => x.FullName).FirstOrDefaultAsync();
                if (verifierName == null)
                    return;

                values.Add("VerifierName", verifierName);
            }


            var approverName = await db.Users.Where(x => x.UserName == bd.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            //values.Add("VerifierName", verifierName);
            values.Add("ApproverName", approverName);

            job.Enqueue<NotificationService>(x => x.SendEmail(requester.Email, requester.FullName, tpl, values));

        }

        public async Task NotifyRequesterForApprovalRejection(Guid bdId)
        {
            var bd = await db.BankDraft.FirstOrDefaultAsync(x => x.Id == bdId);
            if (bd == null)
                return;

            await NotifyRequesterForApprovalRejection(bd, bd.GetMessageValues());

        }

        public async Task NotifyRequesterForApprovalRejection(BankDraft bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("B-1E09 Application Approval Rejection to Requester");
            var requester = db.Users.Where(x => x.UserName == bd.RequesterId).FirstOrDefault();

            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;


            if (bd.Type == "WangCagaran")
            {
                var verifierName = await db.Users.Where(x => x.UserName == bd.VerifierId).Select(x => x.FullName).FirstOrDefaultAsync();
                if (verifierName == null)
                    return;

                values.Add("VerifierName", verifierName);
            }


            var approverName = await db.Users.Where(x => x.UserName == bd.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            //values.Add("VerifierName", verifierName);
            values.Add("ApproverName", approverName);

            job.Enqueue<NotificationService>(x => x.SendEmail(requester.Email, requester.FullName, tpl, values));

        }
       

        public async Task NotifyRequesterForAcceptanceRejection(Guid bdId)
        {
            var bd = await db.BankDraft.FirstOrDefaultAsync(x => x.Id == bdId);
            if (bd == null)
                return;

            await NotifyRequesterForAcceptanceRejection(bd, bd.GetMessageValues());

        }

        public async Task NotifyRequesterForAcceptanceRejection(BankDraft bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("B-1E10 Application Acceptance Rejection to Requester");
            var requester = db.Users.Where(x => x.UserName == bd.RequesterId).FirstOrDefault();

            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;


            if (bd.Type == "WangCagaran")
            {
                var verifierName = await db.Users.Where(x => x.UserName == bd.VerifierId).Select(x => x.FullName).FirstOrDefaultAsync();
                if (verifierName == null)
                    return;

                values.Add("VerifierName", verifierName);
            }


            var approverName = await db.Users.Where(x => x.UserName == bd.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            //values.Add("VerifierName", verifierName);
            values.Add("ApproverName", approverName);

            job.Enqueue<NotificationService>(x => x.SendEmail(requester.Email, requester.FullName, tpl, values));

        }

        public async Task NotifyCancellationApproverForApproval(Guid bdId)
        {
            var c = await db.Cancellation.FirstOrDefaultAsync(x => x.Id == bdId);
            if (c == null)
                return;


            await NotifyCancellationApproverForApproval(c, c.GetMessageValues());

        }

        public async Task NotifyCancellationApproverForApproval(Cancellation bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("C-3E01 Cancellation Approval to Approver");
            var approver = db.Users.Where(x => x.UserName == bd.ApproverId).FirstOrDefault();


            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            var approverName = await db.Users.Where(x => x.UserName == bd.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            values.Add("ApproverName", approverName);

            job.Enqueue<NotificationService>(x => x.SendEmail(approver.Email, approver.FullName, tpl, values));
            
        }

        public async Task NotifyCancellationRequesterForApprovalRejection(Guid bdId)
        {
            var c = await db.Cancellation.FirstOrDefaultAsync(x => x.Id == bdId);
            if (c == null)
                return;

            await NotifyCancellationRequesterForApprovalRejection(c, c.GetMessageValues());

        }

        public async Task NotifyCancellationRequesterForApprovalRejection(Cancellation c, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("C-3E02 Cancellation Rejection to Requester");
            var requester = db.Users.Where(x => x.UserName == c.RequesterId).FirstOrDefault();

            var requesterName = await db.Users.Where(x => x.UserName == c.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;


            var approverName = await db.Users.Where(x => x.UserName == c.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            values.Add("ApproverName", approverName);

            job.Enqueue<NotificationService>(x => x.SendEmail(requester.Email, requester.FullName, tpl, values));

        }

        public async Task NotifyCancellationApproverForWithdrawn(Guid bdId)
        {
            var c = await db.Cancellation.FirstOrDefaultAsync(x => x.Id == bdId);
            if (c == null)
                return;


            await NotifyCancellationApproverForWithdrawn(c, c.GetMessageValues());

        }

        public async Task NotifyCancellationApproverForWithdrawn(Cancellation bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("C-3E07 Cancellation Withdrawal to Approver");
            var approver = db.Users.Where(x => x.UserName == bd.ApproverId).FirstOrDefault();


            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            var approverName = await db.Users.Where(x => x.UserName == bd.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            values.Add("ApproverName", approverName);

            job.Enqueue<NotificationService>(x => x.SendEmail(approver.Email, approver.FullName, tpl, values));

        }

        public async Task NotifyCancellationTGBSBankingForAcceptance(Guid bdId)
        {
            var c = await db.Cancellation.FirstOrDefaultAsync(x => x.Id == bdId);
            if (c == null)
                return;

            await NotifyCancellationTGBSBankingForAcceptance(c, c.GetMessageValues());

        }

        public async Task NotifyCancellationTGBSBankingForAcceptance(Cancellation bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("C-3E03 Cancellation Acceptance to TGBS Banking");

            var tgbsBanking = db.Users
                       .Join(db.UserRoles,
                               u => u.Id,
                               r => r.UserId,
                               (u, r) => new
                               {
                                   id = u.Id,
                                   fullName = u.FullName,
                                   email = u.Email,
                                   roleId = r.RoleId,
                                   active = u.IsActive
                               }
                           )
                       .Where(x => ((x.roleId == "TB" || x.roleId == "BA") && x.active == true))
                       .ToList();

            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            var approverName = await db.Users.Where(x => x.UserName == bd.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            values.Add("ApproverName", approverName);

            foreach (var user in tgbsBanking)
            {
                job.Enqueue<NotificationService>(x => x.SendEmail(user.email, user.fullName, tpl, values));
            }

        }

        public async Task NotifyCancellationRequesterForAcceptanceRejection(Guid bdId)
        {
            var c = await db.Cancellation.FirstOrDefaultAsync(x => x.Id == bdId);
            if (c == null)
                return;

            await NotifyCancellationRequesterForAcceptanceRejection(c, c.GetMessageValues());

        }

        public async Task NotifyCancellationRequesterForAcceptanceRejection(Cancellation bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("C-3E04 Cancellation Acceptance Rejection to Requester");

            var requester = db.Users.Where(x => x.UserName == bd.RequesterId).FirstOrDefault();

            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            var approverName = await db.Users.Where(x => x.UserName == bd.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            values.Add("ApproverName", approverName);

            job.Enqueue<NotificationService>(x => x.SendEmail(requester.Email, requester.FullName, tpl, values));

        }

        public async Task NotifyCancellationTGBSReconForReceive(Guid bdId)
        {
            var c = await db.Cancellation.FirstOrDefaultAsync(x => x.Id == bdId);
            if (c == null)
                return;

            await NotifyCancellationTGBSReconForReceive(c, c.GetMessageValues());

        }

        public async Task NotifyCancellationTGBSReconForReceive(Cancellation c, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("C-3E05 Cancellation Receive to TGBS Reconcilliation");

            var tgbsRecon = db.Users
                       .Join(db.UserRoles,
                               u => u.Id,
                               r => r.UserId,
                               (u, r) => new
                               {
                                   id = u.Id,
                                   fullName = u.FullName,
                                   email = u.Email,
                                   roleId = r.RoleId,
                                   active = u.IsActive
                               }
                           )
                       .Where(x => (x.roleId == "TR" && x.active==true))
                       .ToList();

            var insLetter = await db.InstructionLetter.Where(x => x.ReferenceNo.Contains(c.BDNo) && x.ApplicationType == "Cancellation").FirstOrDefaultAsync();
            if (insLetter == null)
                return;

            var requesterName = await db.Users.Where(x => x.UserName == c.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            var approverName = await db.Users.Where(x => x.UserName == c.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("ValueDate", insLetter.ValueDate.Value.ToString("dd-MM-yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture));
            values.Add("RequesterName", requesterName);
            values.Add("ApproverName", approverName);

            foreach (var user in tgbsRecon)
            {
                job.Enqueue<NotificationService>(x => x.SendEmail(user.email, user.fullName, tpl, values));
            }

        }

        public async Task NotifyCancellationTGBSBankingForConfirmation(Guid bdId)
        {
            var c = await db.Cancellation.FirstOrDefaultAsync(x => x.Id == bdId);
            if (c == null)
                return;

            await NotifyCancellationTGBSBankingForConfirmation(c, c.GetMessageValues());

        }

        public async Task NotifyCancellationTGBSBankingForConfirmation(Cancellation bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("C-3E06 Cancellation Confirmation to TGBS Banking");

            var tgbsBanking = db.Users
                       .Join(db.UserRoles,
                               u => u.Id,
                               r => r.UserId,
                               (u, r) => new
                               {
                                   id = u.Id,
                                   fullName = u.FullName,
                                   email = u.Email,
                                   roleId = r.RoleId,
                                   active = u.IsActive
                               }
                           )
                       .Where(x => ((x.roleId == "TB" || x.roleId == "BA") && x.active == true))
                       .ToList();

            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            var approverName = await db.Users.Where(x => x.UserName == bd.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            values.Add("ApproverName", approverName);

            foreach (var user in tgbsBanking)
            {
                job.Enqueue<NotificationService>(x => x.SendEmail(user.email, user.fullName, tpl, values));
            }

        }

        public async Task NotifyCancellationRequesterForApproval(Guid bdId)
        {
            var c = await db.Cancellation.FirstOrDefaultAsync(x => x.Id == bdId);
            if (c == null)
                return;

            await NotifyCancellationRequesterForApproval(c, c.GetMessageValues());

        }

        public async Task NotifyCancellationRequesterForApproval(Cancellation c, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("C-3E08 Cancellation Approval to Requester (As Acknowledgement)");
            var requester = db.Users.Where(x => x.UserName == c.RequesterId).FirstOrDefault();

            var requesterName = await db.Users.Where(x => x.UserName == c.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            var approverName = await db.Users.Where(x => x.UserName == c.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            values.Add("ApproverName", approverName);

            job.Enqueue<NotificationService>(x => x.SendEmail(requester.Email, requester.FullName, tpl, values));

        }

        public async Task NotifyCancellationRequesterForProcess(Guid bdId)
        {
            var c = await db.Cancellation.FirstOrDefaultAsync(x => x.Id == bdId);
            if (c == null)
                return;

            await NotifyCancellationRequesterForProcess(c, c.GetMessageValues());

        }

        public async Task NotifyCancellationRequesterForProcess(Cancellation c, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("C-3E09 Cancellation Process to Requester (As Acknowledgement)");
            var requester = db.Users.Where(x => x.UserName == c.RequesterId).FirstOrDefault();

            var requesterName = await db.Users.Where(x => x.UserName == c.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            var approverName = await db.Users.Where(x => x.UserName == c.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            values.Add("ApproverName", approverName);

            job.Enqueue<NotificationService>(x => x.SendEmail(requester.Email, requester.FullName, tpl, values));

        }

        public async Task NotifyCancellationRequesterForComplete(Guid bdId)
        {
            var c = await db.Cancellation.FirstOrDefaultAsync(x => x.Id == bdId);
            if (c == null)
                return;

            await NotifyCancellationRequesterForComplete(c, c.GetMessageValues());

        }

        public async Task NotifyCancellationRequesterForComplete(Cancellation c, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("C-3E10 Cancellation Complete to Requester (As Acknowledgement)");
            var requester = db.Users.Where(x => x.UserName == c.RequesterId).FirstOrDefault();

            var requesterName = await db.Users.Where(x => x.UserName == c.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            var approverName = await db.Users.Where(x => x.UserName == c.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            values.Add("ApproverName", approverName);

            job.Enqueue<NotificationService>(x => x.SendEmail(requester.Email, requester.FullName, tpl, values));

        }

        public async Task NotifyBankForProcessingForCancellationForMaybank(Guid letterId, string bankType)
        {
            var ilr = await db.InstructionLetter.FirstOrDefaultAsync(x => x.Id == letterId);
            if (ilr == null)
                return;
            else
            {
                var bdRefNo = ilr.ReferenceNo.Split(",");
                var canc = await db.Cancellation.Where(x => x.BDNo == bdRefNo[0]).FirstOrDefaultAsync();

                await NotifyBankForProcessingForCancellationForMaybank(ilr, canc.GetMessageValues(), bankType);
            }
        }

        public async Task NotifyBankForProcessingForCancellationForMaybank(InstructionLetter ilr, IDictionary<string, string> values, string bankType)
        {
            var bankEmail = ilr.InstructionLetterEmail;
            var bankName = ilr.BankName;
            var bdRefNo = ilr.ReferenceNo.Split(",");

            var tpl = await GetTemplate("C-3E12 Cancellation with Instruction Letter to Bank (< 1 Year)");
            var bd = await db.Cancellation.Where(x => x.BDNo == bdRefNo[0]).FirstOrDefaultAsync();
            var attachment = await db.Attachment.Where(x => x.ParentId == bd.Id && x.FileType == Data.AttachmentType.InstructionLetter.ToString() && x.FileSubType == Data.BDAttachmentType.SignedLetter.ToString()).FirstOrDefaultAsync();

            var tgbsBanking = db.Users
                       .Join(db.UserRoles,
                               u => u.Id,
                               r => r.UserId,
                               (u, r) => new
                               {
                                   id = u.Id,
                                   fullName = u.FullName,
                                   email = u.Email,
                                   roleId = r.RoleId,
                                   active = u.IsActive
                               }
                           )
                       .Where(x => ((x.roleId == "TB" || x.roleId == "BA") && x.active == true))
                       .ToList();

            var ccList = "";

            foreach (var user in tgbsBanking)
            {
                ccList += user.email + ",";
            }
            ccList = ccList.Remove(ccList.Trim().Length - 1);

            int count = 0;
            var bdList = "";
            foreach (var refNo in bdRefNo)
            {
                count++;
                bdList += count + ". " + refNo + "\n";
            }
            values.Add("BDListForLetter", bdList);
            values.Add("BankPIC", ilr.BankPIC);
            values.Add("BankName", bankName);

            job.Enqueue<NotificationService>(x => x.SendEmail(bankEmail, bankName, tpl, values, attachment.FileName, ccList));

        }

        public async Task NotifyBankForProcessingForCancellationForUMA(Guid bdId, string bankType)
        {
            var c = await db.Cancellation.FirstOrDefaultAsync(x => x.Id == bdId);
            if (c == null)
                return;
            await NotifyBankForProcessingForCancellationForUMA(c, c.GetMessageValues(), bankType);

        }

        public async Task NotifyBankForProcessingForCancellationForUMA(Cancellation c, IDictionary<string, string> values, string bankType)
        {
            var bank = await db.BankDetails.FirstOrDefaultAsync(x => x.Type == "Cancellation" && x.isActive == true);

            var bankEmail = bank.Email;
            var bankName = bank.BankName;

            var tpl = await GetTemplate("C-3E11 Cancellation with Instruction Letter to Bank (> 1 Year)");
            var attachment = await db.Attachment.Where(x => x.ParentId == c.Id && x.FileType == Data.AttachmentType.Cancellation.ToString() && x.FileSubType == Data.BDAttachmentType.ScannedLetter.ToString()).FirstOrDefaultAsync();


            var tgbsBanking = db.Users
                       .Join(db.UserRoles,
                               u => u.Id,
                               r => r.UserId,
                               (u, r) => new
                               {
                                   id = u.Id,
                                   fullName = u.FullName,
                                   email = u.Email,
                                   roleId = r.RoleId,
                                   active = u.IsActive
                               }
                           )
                       .Where(x => ((x.roleId == "TB" || x.roleId == "BA") && x.active == true))
                       .ToList();

            var ccList = "";

            foreach (var user in tgbsBanking)
            {
                ccList += user.email + ",";
            }
            ccList = ccList.Remove(ccList.Trim().Length - 1);

            values.Add("BDListForLetter", c.RefNo);
            values.Add("BankPIC", bank.BankPIC);
            values.Add("BankName", bankName);

            job.Enqueue<NotificationService>(x => x.SendEmail(bankEmail, bankName, tpl, values, attachment.FileName, ccList));

        }

        public async Task NotifyRecoveryAgensiKerajaanForCreation(Guid bdId)
        {
            var r = await db.Recovery.FirstOrDefaultAsync(x => x.Id == bdId);
            if (r == null)
                return;

            await NotifyRecoveryAgensiKerajaanForCreation(r, r.GetMessageValues(), r.PBTEmailAddress);

        }

        public async Task NotifyRecoveryAgensiKerajaanForCreation(Recovery c, IDictionary<string, string> values, string pbtEmailAddress)
        {
            var tpl = await GetTemplate("R-2E01 Recovery Initiation");
            var pbtEmail = pbtEmailAddress;
            var pbtName = "Contoh Kerajaan";

            var requester = await db.Users.Where(x => x.UserName == c.RequesterId).FirstOrDefaultAsync();
            if (requester == null)
                return;

            //var PName = await db.Users.Where(x => x.UserName == c.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            //if (requesterName == null)
            //    return;

            values.Add("RequesterName", requester.FullName);

            var attachment = await db.Attachment.Where(x => x.ParentId == c.Id && x.FileType == "Recovery" && x.FileSubType == "RecoveryLetter").FirstOrDefaultAsync();

            job.Enqueue<NotificationService>(x => x.SendEmail(pbtEmail, c.NameOnBD, tpl, values, attachment.FileName, requester.Email));

        }

        public async Task NotifyRecoveryTGBSBankingForAcceptFullSubmission(Guid bdId)
        {
            var r = await db.Recovery.FirstOrDefaultAsync(x => x.Id == bdId);
            if (r == null)
                return;

            await NotifyRecoveryTGBSBankingForAcceptFullSubmission(r, r.GetMessageValues());

        }

        public async Task NotifyRecoveryTGBSBankingForAcceptFullSubmission(Recovery bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("R-2E17 Recovery Acceptance Full Recovery Amount To AP Banking");

            var tgbsBank = db.Users
                       .Join(db.UserRoles,
                               u => u.Id,
                               r => r.UserId,
                               (u, r) => new
                               {
                                   id = u.Id,
                                   fullName = u.FullName,
                                   email = u.Email,
                                   roleId = r.RoleId,
                                   active = u.IsActive
                               }
                           )
                       .Where(x => ((x.roleId == "TB" || x.roleId == "BA") && x.active == true))
                       .ToList();

            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            values.Add("RequesterName", requesterName);

            foreach (var user in tgbsBank)
            {
                job.Enqueue<NotificationService>(x => x.SendEmail(user.email, user.fullName, tpl, values));
            }

        }

        public async Task NotifyRecoveryTGBSBankingForAcceptFirstPartial(Guid bdId)
        {
            var r = await db.Recovery.FirstOrDefaultAsync(x => x.Id == bdId);
            if (r == null)
                return;

            await NotifyRecoveryTGBSBankingForAcceptFirstPartial(r, r.GetMessageValues());

        }

        public async Task NotifyRecoveryTGBSBankingForAcceptFirstPartial(Recovery bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("R-2E18 Recovery Acceptance First Partial Amount To AP Banking");

            var tgbsBank = db.Users
                       .Join(db.UserRoles,
                               u => u.Id,
                               r => r.UserId,
                               (u, r) => new
                               {
                                   id = u.Id,
                                   fullName = u.FullName,
                                   email = u.Email,
                                   roleId = r.RoleId,
                                   active = u.IsActive
                               }
                           )
                       .Where(x => ((x.roleId == "TB" || x.roleId == "BA") && x.active == true))
                       .ToList();

            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            values.Add("RequesterName", requesterName);

            foreach (var user in tgbsBank)
            {
                job.Enqueue<NotificationService>(x => x.SendEmail(user.email, user.fullName, tpl, values));
            }

        }

        public async Task NotifyRecoveryTGBSBankingForAcceptSecondPartial(Guid bdId)
        {
            var r = await db.Recovery.FirstOrDefaultAsync(x => x.Id == bdId);
            if (r == null)
                return;

            await NotifyRecoveryTGBSBankingForAcceptSecondPartial(r, r.GetMessageValues());

        }

        public async Task NotifyRecoveryTGBSBankingForAcceptSecondPartial(Recovery bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("R-2E19 Recovery Acceptance Second Partial Amount To AP Banking");

            var tgbsBank = db.Users
                       .Join(db.UserRoles,
                               u => u.Id,
                               r => r.UserId,
                               (u, r) => new
                               {
                                   id = u.Id,
                                   fullName = u.FullName,
                                   email = u.Email,
                                   roleId = r.RoleId,
                                   active = u.IsActive
                               }
                           )
                       .Where(x => ((x.roleId == "TB" || x.roleId == "BA") && x.active == true))
                       .ToList();

            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            values.Add("RequesterName", requesterName);

            foreach (var user in tgbsBank)
            {
                job.Enqueue<NotificationService>(x => x.SendEmail(user.email, user.fullName, tpl, values));
            }

        }

        public async Task NotifyRecoveryTGBSReconForReceiveFullSubmission(Guid bdId)
        {
            var r = await db.Recovery.FirstOrDefaultAsync(x => x.Id == bdId);
            if (r == null)
                return;

            await NotifyRecoveryTGBSReconForReceiveFullSubmission(r, r.GetMessageValues());

        }

        public async Task NotifyRecoveryTGBSReconForReceiveFullSubmission(Recovery bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("R-2E05 Recovery Full Recovery Amount Receive To AP Recon");

            var tgbsRecon = db.Users
                       .Join(db.UserRoles,
                               u => u.Id,
                               r => r.UserId,
                               (u, r) => new
                               {
                                   id = u.Id,
                                   fullName = u.FullName,
                                   email = u.Email,
                                   roleId = r.RoleId,
                                   active = u.IsActive
                               }
                           )
                       .Where(x => (x.roleId == "TR" && x.active == true))
                       .ToList();

            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            values.Add("RequesterName", requesterName);

            foreach (var user in tgbsRecon)
            {
                job.Enqueue<NotificationService>(x => x.SendEmail(user.email, user.fullName, tpl, values));
            }

        }

        public async Task NotifyRecoveryTGBSBankingForConfirmFullSubmission(Guid bdId)
        {
            var r = await db.Recovery.FirstOrDefaultAsync(x => x.Id == bdId);
            if (r == null)
                return;

            await NotifyRecoveryTGBSBankingForConfirmFullSubmission(r, r.GetMessageValues());

        }

        public async Task NotifyRecoveryTGBSBankingForConfirmFullSubmission(Recovery bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("R-2E06 Recovery Submission Full Recovery Amount To AP Banking");

            var tgbsRecon = db.Users
                       .Join(db.UserRoles,
                               u => u.Id,
                               r => r.UserId,
                               (u, r) => new
                               {
                                   id = u.Id,
                                   fullName = u.FullName,
                                   email = u.Email,
                                   roleId = r.RoleId,
                                   active = u.IsActive
                               }
                           )
                       .Where(x => ((x.roleId == "TB" || x.roleId == "BA") && x.active == true))
                       .ToList();

            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            values.Add("RequesterName", requesterName);

            foreach (var user in tgbsRecon)
            {
                job.Enqueue<NotificationService>(x => x.SendEmail(user.email, user.fullName, tpl, values));
            }

        }

        public async Task NotifyRecoveryTGBSReconForReceiveFirstPartial(Guid bdId)
        {
            var r = await db.Recovery.FirstOrDefaultAsync(x => x.Id == bdId);
            if (r == null)
                return;

            await NotifyRecoveryTGBSReconForReceiveFirstPartial(r, r.GetMessageValues());

        }

        public async Task NotifyRecoveryTGBSReconForReceiveFirstPartial(Recovery bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("R-2E08 Recovery First Partial Receive To AP Recon");

            var tgbsRecon = db.Users
                       .Join(db.UserRoles,
                               u => u.Id,
                               r => r.UserId,
                               (u, r) => new
                               {
                                   id = u.Id,
                                   fullName = u.FullName,
                                   email = u.Email,
                                   roleId = r.RoleId,
                                   active = u.IsActive
                               }
                           )
                       .Where(x => (x.roleId == "TR" && x.active == true))
                       .ToList();

            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            values.Add("RequesterName", requesterName);

            foreach (var user in tgbsRecon)
            {
                job.Enqueue<NotificationService>(x => x.SendEmail(user.email, user.fullName, tpl, values));
            }

        }

        //public async Task NotifyRecoveryTGBSReconForReceiveFirstPartial(Guid bdId)
        //{
        //    var r = await db.Recovery.FirstOrDefaultAsync(x => x.Id == bdId);
        //    if (r == null)
        //        return;

        //    await NotifyRecoveryTGBSReconForReceiveFirstPartial(r, r.GetMessageValues());

        //}

        //public async Task NotifyRecoveryTGBSReconForReceiveFirstPartial(Recovery bd, IDictionary<string, string> values)
        //{
        //    var tpl = await GetTemplate("R-2E09 Recovery 1st Partial Submission To AP Banking");

        //    var tgbsRecon = db.Users
        //               .Join(db.UserRoles,
        //                       u => u.Id,
        //                       r => r.UserId,
        //                       (u, r) => new
        //                       {
        //                           id = u.Id,
        //                           fullName = u.FullName,
        //                           email = u.Email,
        //                           roleId = r.RoleId,
        //                           active = u.IsActive
        //                       }
        //                   )
        //               .Where(x => (x.roleId == "TB" && x.active == true))
        //               .ToList();

        //    var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
        //    if (requesterName == null)
        //        return;

        //    values.Add("RequesterName", requesterName);

        //    foreach (var user in tgbsRecon)
        //    {
        //        job.Enqueue<NotificationService>(x => x.SendEmail(user.email, user.fullName, tpl, values));
        //    }

        //}

        public async Task NotifyRecoveryTGBSReconForReceiveSecondPartial(Guid bdId)
        {
            var r = await db.Recovery.FirstOrDefaultAsync(x => x.Id == bdId);
            if (r == null)
                return;

            await NotifyRecoveryTGBSReconForReceiveSecondPartial(r, r.GetMessageValues());

        }

        public async Task NotifyRecoveryTGBSReconForReceiveSecondPartial(Recovery bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("R-2E12 Recovery Second Partial Receive To AP Recon");

            var tgbsRecon = db.Users
                       .Join(db.UserRoles,
                               u => u.Id,
                               r => r.UserId,
                               (u, r) => new
                               {
                                   id = u.Id,
                                   fullName = u.FullName,
                                   email = u.Email,
                                   roleId = r.RoleId,
                                   active = u.IsActive
                               }
                           )
                       .Where(x => (x.roleId == "TR" && x.active == true))
                       .ToList();

            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            values.Add("RequesterName", requesterName);

            foreach (var user in tgbsRecon)
            {
                job.Enqueue<NotificationService>(x => x.SendEmail(user.email, user.fullName, tpl, values));
            }

        }

        public async Task NotifyRecoveryTGBSBankingForConfirmFirstPartial(Guid bdId)
        {
            var r = await db.Recovery.FirstOrDefaultAsync(x => x.Id == bdId);
            if (r == null)
                return;

            await NotifyRecoveryTGBSBankingForConfirmFirstPartial(r, r.GetMessageValues());

        }

        public async Task NotifyRecoveryTGBSBankingForConfirmFirstPartial(Recovery bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("R-2E09 Recovery 1st Partial Submission To AP Banking");

            var tgbsRecon = db.Users
                       .Join(db.UserRoles,
                               u => u.Id,
                               r => r.UserId,
                               (u, r) => new
                               {
                                   id = u.Id,
                                   fullName = u.FullName,
                                   email = u.Email,
                                   roleId = r.RoleId,
                                   active = u.IsActive
                               }
                           )
                       .Where(x => ((x.roleId == "TB" || x.roleId == "BA") && x.active == true))
                       .ToList();

            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            values.Add("RequesterName", requesterName);

            foreach (var user in tgbsRecon)
            {
                job.Enqueue<NotificationService>(x => x.SendEmail(user.email, user.fullName, tpl, values));
            }

        }

        public async Task NotifyRecoveryTGBSBankingForConfirmSecondPartial(Guid bdId)
        {
            var r = await db.Recovery.FirstOrDefaultAsync(x => x.Id == bdId);
            if (r == null)
                return;

            await NotifyRecoveryTGBSBankingForConfirmSecondPartial(r, r.GetMessageValues());

        }

        public async Task NotifyRecoveryTGBSBankingForConfirmSecondPartial(Recovery bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("R-2E13 Recovery 2nd Partial Submission To AP Banking");

            var tgbsRecon = db.Users
                       .Join(db.UserRoles,
                               u => u.Id,
                               r => r.UserId,
                               (u, r) => new
                               {
                                   id = u.Id,
                                   fullName = u.FullName,
                                   email = u.Email,
                                   roleId = r.RoleId,
                                   active = u.IsActive
                               }
                           )
                       .Where(x => ((x.roleId == "TB" || x.roleId == "BA") && x.active == true))
                       .ToList();

            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            values.Add("RequesterName", requesterName);

            foreach (var user in tgbsRecon)
            {
                job.Enqueue<NotificationService>(x => x.SendEmail(user.email, user.fullName, tpl, values));
            }

        }

        public async Task NotifyRecoveryTGBSBankingForConfirmation(Guid bdId)
        {
            var r = await db.Recovery.FirstOrDefaultAsync(x => x.Id == bdId);
            if (r == null)
                return;

            await NotifyRecoveryTGBSBankingForConfirmation(r, r.GetMessageValues());

        }

        public async Task NotifyRecoveryTGBSBankingForConfirmation(Recovery r, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("R-2E06 Recovery Confirmation");

            var tgbsBanking = db.Users
                       .Join(db.UserRoles,
                               u => u.Id,
                               t => t.UserId,
                               (u, t) => new
                               {
                                   id = u.Id,
                                   fullName = u.FullName,
                                   email = u.Email,
                                   roleId = t.RoleId,
                                   active = u.IsActive
                               }
                           )
                       .Where(x => ((x.roleId == "TB" || x.roleId == "BA") && x.active == true))
                       .ToList();

            var requesterName = await db.Users.Where(x => x.UserName == r.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            values.Add("RequesterName", requesterName);

            foreach (var user in tgbsBanking)
            {
                job.Enqueue<NotificationService>(x => x.SendEmail(user.email, user.fullName, tpl, values));
            }

        }

        public async Task NotifyRecoveryRequesterForAcknowledgement(Guid bdId)
        {
            var r = await db.Recovery.FirstOrDefaultAsync(x => x.Id == bdId);
            if (r == null)
                return;

            await NotifyRecoveryRequesterForAcknowledgement(r, r.GetMessageValues());

        }

        public async Task NotifyRecoveryRequesterForAcknowledgement(Recovery r, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("R-2E10 Recovery Acknowledgement First Partial Complete");


            var requester = db.Users.Where(x => x.UserName == r.RequesterId).FirstOrDefault();
            var requesterName = await db.Users.Where(x => x.UserName == r.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            values.Add("RequesterName", requesterName);

            
                job.Enqueue<NotificationService>(x => x.SendEmail(requester.Email, requester.FullName, tpl, values));
            

        }

        public async Task NotifyRecoveryRequesterForAcceptance(Guid bdId)
        {
            var r = await db.Recovery.FirstOrDefaultAsync(x => x.Id == bdId);
            if (r == null)
                return;

            await NotifyRecoveryRequesterForAcceptance(r, r.GetMessageValues());

        }

        public async Task NotifyRecoveryRequesterForAcceptance(Recovery r, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("R-2E20 Recovery Acceptance To Requester (As Acknowledgement)");


            var requester = db.Users.Where(x => x.UserName == r.RequesterId).FirstOrDefault();
            var requesterName = await db.Users.Where(x => x.UserName == r.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            values.Add("RequesterName", requesterName);


            job.Enqueue<NotificationService>(x => x.SendEmail(requester.Email, requester.FullName, tpl, values));


        }

        public async Task NotifyRecoveryRequesterForComplete(Guid bdId)
        {
            var r = await db.Recovery.FirstOrDefaultAsync(x => x.Id == bdId);
            if (r == null)
                return;

            await NotifyRecoveryRequesterForComplete(r, r.GetMessageValues());

        }

        public async Task NotifyRecoveryRequesterForComplete(Recovery r, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("R-2E14 Recovery Completed (Partial)");

            if (r.RecoveryType == "Full")
            {
                tpl = await GetTemplate("R-2E16 Recovery Completed (Full)");
            }


            var requester = db.Users.Where(x => x.UserName == r.RequesterId).FirstOrDefault();
            var requesterName = await db.Users.Where(x => x.UserName == r.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            values.Add("RequesterName", requesterName);


            job.Enqueue<NotificationService>(x => x.SendEmail(requester.Email, requester.FullName, tpl, values));


        }

        public async Task NotifyRequesterForRecoveryDecline(Guid recId)
        {
            var rec = await db.Recovery.FirstOrDefaultAsync(x => x.Id == recId);
            if (rec == null)
                return;

            await NotifyRequesterForRecoveryDecline(rec, rec.GetMessageValues());

        }

        public async Task NotifyRequesterForRecoveryDecline(Recovery rec, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("R-2E15 Recovery Declined to Requester");
            var requester = db.Users.Where(x => x.UserName == rec.RequesterId).FirstOrDefault();

            //var requesterName = await db.Users.Where(x => x.UserName == rec.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            //if (requesterName == null)
            //    return;

            values.Add("RequesterName", requester.FullName);

            job.Enqueue<NotificationService>(x => x.SendEmail2(requester.Email, requester.FullName, tpl, values, rec.PBTEmailAddress));

        }

        public async Task NotifyLostApproverForApproval(Guid bdId)
        {
            var c = await db.Lost.FirstOrDefaultAsync(x => x.Id == bdId);
            if (c == null)
                return;


            await NotifyLostApproverForApproval(c, c.GetMessageValues());

        }

        public async Task NotifyLostApproverForApproval(Lost bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("L-4E01 Lost Submission");
            var approver = db.Users.Where(x => x.UserName == bd.ApproverId).FirstOrDefault();


            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            var approverName = await db.Users.Where(x => x.UserName == bd.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            values.Add("ApproverName", approverName);

            job.Enqueue<NotificationService>(x => x.SendEmail(approver.Email, approver.FullName, tpl, values));

        }

        public async Task NotifyLostApproverForWithdrawn(Guid bdId)
        {
            var c = await db.Lost.FirstOrDefaultAsync(x => x.Id == bdId);
            if (c == null)
                return;


            await NotifyLostApproverForWithdrawn(c, c.GetMessageValues());

        }

        public async Task NotifyLostApproverForWithdrawn(Lost bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("L-4E05 Lost Withdrawn");
            var approver = db.Users.Where(x => x.UserName == bd.ApproverId).FirstOrDefault();


            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            var approverName = await db.Users.Where(x => x.UserName == bd.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            values.Add("ApproverName", approverName);

            job.Enqueue<NotificationService>(x => x.SendEmail(approver.Email, approver.FullName, tpl, values));

        }

        public async Task NotifyLostTGBSBankingForAcceptance(Guid bdId)
        {
            var c = await db.Lost.FirstOrDefaultAsync(x => x.Id == bdId);
            if (c == null)
                return;

            await NotifyLostTGBSBankingForAcceptance(c, c.GetMessageValues());

        }

        public async Task NotifyLostTGBSBankingForAcceptance(Lost bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("L-4E02 Lost Approved");

            var tgbsBanking = db.Users
                       .Join(db.UserRoles,
                               u => u.Id,
                               r => r.UserId,
                               (u, r) => new
                               {
                                   id = u.Id,
                                   fullName = u.FullName,
                                   email = u.Email,
                                   roleId = r.RoleId,
                                   active = u.IsActive
                               }
                           )
                       .Where(x => ((x.roleId == "TB" || x.roleId == "BA") && x.active == true))
                       .ToList();

            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            var approverName = await db.Users.Where(x => x.UserName == bd.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            values.Add("ApproverName", approverName);

            foreach (var user in tgbsBanking)
            {
                job.Enqueue<NotificationService>(x => x.SendEmail(user.email, user.fullName, tpl, values));
            }

        }

        public async Task NotifyLostRequesterForApprovalRejection(Guid bdId)
        {
            var c = await db.Lost.FirstOrDefaultAsync(x => x.Id == bdId);
            if (c == null)
                return;

            await NotifyLostRequesterForApprovalRejection(c, c.GetMessageValues());

        }

        public async Task NotifyLostRequesterForApprovalRejection(Lost c, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("L-4E03 Lost Rejected");
            var requester = db.Users.Where(x => x.UserName == c.RequesterId).FirstOrDefault();

            var requesterName = await db.Users.Where(x => x.UserName == c.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;


            var approverName = await db.Users.Where(x => x.UserName == c.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            values.Add("ApproverName", approverName);

            job.Enqueue<NotificationService>(x => x.SendEmail(requester.Email, requester.FullName, tpl, values));

        }

        public async Task NotifyTGBSBankingForBDConfirmation(Guid bdId)
        {
            var bd = await db.Lost.FirstOrDefaultAsync(x => x.Id == bdId);
            if (bd == null)
                return;

            await NotifyTGBSBankingForBDConfirmation(bd, bd.GetMessageValues());

        }

        public async Task NotifyTGBSBankingForBDConfirmation(Lost bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("L-4E04 Lost Completed");
            var requester = db.Users.Where(x => x.UserName == bd.RequesterId).FirstOrDefault();

            var tgbsBanking = db.Users
                        .Join(db.UserRoles,
                                u => u.Id,
                                r => r.UserId,
                                (u, r) => new
                                {
                                    id = u.Id,
                                    fullName = u.FullName,
                                    email = u.Email,
                                    roleId = r.RoleId,
                                    active = u.IsActive
                                }
                            )
                        .Where(x => (x.roleId == "TR" && x.active == true))
                        .ToList();

            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            var approverName = await db.Users.Where(x => x.UserName == bd.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            values.Add("ApproverName", approverName);

            foreach (var user in tgbsBanking)
            {
                job.Enqueue<NotificationService>(x => x.SendEmail(user.email, user.fullName, tpl, values));
            }

        }

        public async Task NotifyLostRequesterForBDCompletion(Guid bdId)
        {
            var l = await db.Lost.FirstOrDefaultAsync(x => x.Id == bdId);
            if (l == null)
                return;

            await NotifyLostRequesterForBDCompletion(l, l.GetMessageValues());

        }

        public async Task NotifyLostRequesterForBDCompletion(Lost bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("L-4E09 Lost Complete");
        
            var requester = db.Users.Where(x => x.UserName == bd.RequesterId).FirstOrDefault();

            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            var approverName = await db.Users.Where(x => x.UserName == bd.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            values.Add("ApproverName", approverName);

            job.Enqueue<NotificationService>(x => x.SendEmail(requester.Email, requester.FullName, tpl, values));

        }

        public async Task NotifyLostRequesterForAcceptanceRejection(Guid bdId)
        {
            var l = await db.Lost.FirstOrDefaultAsync(x => x.Id == bdId);
            if (l == null)
                return;

            await NotifyLostRequesterForAcceptanceRejection(l, l.GetMessageValues());

        }

        public async Task NotifyLostRequesterForAcceptanceRejection(Lost bd, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("L-4E06 Lost Decline");

            var requester = db.Users.Where(x => x.UserName == bd.RequesterId).FirstOrDefault();

            var requesterName = await db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            var approverName = await db.Users.Where(x => x.UserName == bd.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            values.Add("ApproverName", approverName);

            job.Enqueue<NotificationService>(x => x.SendEmail(requester.Email, requester.FullName, tpl, values));

        }

        public async Task NotifyLostRequesterForApproval(Guid bdId)
        {
            var lost = await db.Lost.FirstOrDefaultAsync(x => x.Id == bdId);
            if (lost == null)
                return;

            await NotifyLostRequesterForApproval(lost, lost.GetMessageValues());

        }

        public async Task NotifyLostRequesterForApproval(Lost lost, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("L-4E07 Lost Approval to Requester (As Acknowledgement)");
            var requester = db.Users.Where(x => x.UserName == lost.RequesterId).FirstOrDefault();

            var requesterName = await db.Users.Where(x => x.UserName == lost.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            var approverName = await db.Users.Where(x => x.UserName == lost.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            //values.Add("VerifierName", verifierName);
            values.Add("ApproverName", approverName);

            job.Enqueue<NotificationService>(x => x.SendEmail(requester.Email, requester.FullName, tpl, values));

        }

        public async Task NotifyLostRequesterForAcceptance(Guid bdId)
        {
            var lost = await db.Lost.FirstOrDefaultAsync(x => x.Id == bdId);
            if (lost == null)
                return;

            await NotifyLostRequesterForAcceptance(lost, lost.GetMessageValues());

        }

        public async Task NotifyLostRequesterForAcceptance(Lost lost, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("L-4E08 Lost Acceptance to Requester (As Acknowledgement)");
            var requester = db.Users.Where(x => x.UserName == lost.RequesterId).FirstOrDefault();

            var requesterName = await db.Users.Where(x => x.UserName == lost.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            var approverName = await db.Users.Where(x => x.UserName == lost.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            //values.Add("VerifierName", verifierName);
            values.Add("ApproverName", approverName);

            job.Enqueue<NotificationService>(x => x.SendEmail(requester.Email, requester.FullName, tpl, values));

        }

        public async Task NotifyBankForProcessingForLost(Guid letterId)
        {
            var ilr = await db.InstructionLetter.FirstOrDefaultAsync(x => x.Id == letterId);
            if (ilr == null)
                return;
            else
            {
                var bdRefNo = ilr.ReferenceNo.Split(",");
                var lost = await db.Lost.Where(x => x.BDNo == bdRefNo[0]).FirstOrDefaultAsync();

                await NotifyBankForProcessingForLost(ilr, lost.GetMessageValues());
            }
        }

        public async Task NotifyBankForProcessingForLost(InstructionLetter ilr, IDictionary<string, string> values)
        {
            var bankEmail = ilr.InstructionLetterEmail;
            var bankName = ilr.BankName;
            var bdRefNo = ilr.ReferenceNo.Split(",");

            var tpl = await GetTemplate("L-4E19 Lost with Instruction Letter to Bank");
            var bd = await db.Lost.Where(x => x.BDNo == bdRefNo[0]).FirstOrDefaultAsync();
            var attachment = await db.Attachment.Where(x => x.ParentId == bd.Id && x.FileType == Data.AttachmentType.InstructionLetter.ToString() && x.FileSubType == Data.BDAttachmentType.SignedLetter.ToString()).FirstOrDefaultAsync();

            var tgbsBanking = db.Users
                       .Join(db.UserRoles,
                               u => u.Id,
                               r => r.UserId,
                               (u, r) => new
                               {
                                   id = u.Id,
                                   fullName = u.FullName,
                                   email = u.Email,
                                   roleId = r.RoleId,
                                   active = u.IsActive
                               }
                           )
                       .Where(x => ((x.roleId == "TB" || x.roleId == "BA") && x.active == true))
                       .ToList();

            var ccList = "";

            foreach (var user in tgbsBanking)
            {
                ccList += user.email + ",";
            }
            ccList = ccList.Remove(ccList.Trim().Length - 1);

            int count = 0;
            var bdList = "";
            foreach (var refNo in bdRefNo)
            {
                count++;
                bdList += count + ". " + refNo + "\n";
            }
            values.Add("BDListForLetter", bdList);
            values.Add("BankPIC", ilr.BankPIC);
            values.Add("BankName", bankName);

            job.Enqueue<NotificationService>(x => x.SendEmail(bankEmail, bankName, tpl, values, attachment.FileName, ccList));

        }

        public async Task NotifyLostTGBSReconForReceive(Guid bdId)
        {
            var c = await db.Lost.FirstOrDefaultAsync(x => x.Id == bdId);
            if (c == null)
                return;

            await NotifyLostTGBSReconForReceive(c, c.GetMessageValues());

        }

        public async Task NotifyLostTGBSReconForReceive(Lost c, IDictionary<string, string> values)
        {
            var tpl = await GetTemplate("L-4E10 Lost Receive to TGBS Reconcilliation");

            var tgbsRecon = db.Users
                       .Join(db.UserRoles,
                               u => u.Id,
                               r => r.UserId,
                               (u, r) => new
                               {
                                   id = u.Id,
                                   fullName = u.FullName,
                                   email = u.Email,
                                   roleId = r.RoleId,
                                   active = u.IsActive
                               }
                           )
                       .Where(x => (x.roleId == "TR" && x.active == true))
                       .ToList();

            var requesterName = await db.Users.Where(x => x.UserName == c.RequesterId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (requesterName == null)
                return;

            var approverName = await db.Users.Where(x => x.UserName == c.ApproverId).Select(x => x.FullName).FirstOrDefaultAsync();
            if (approverName == null)
                return;

            values.Add("RequesterName", requesterName);
            values.Add("ApproverName", approverName);

            foreach (var user in tgbsRecon)
            {
                job.Enqueue<NotificationService>(x => x.SendEmail(user.email, user.fullName, tpl, values));
            }

        }

        //Forget Password

        public async Task NotifyPasswordResetEmail(ApplicationUser user, string tempPassword)
        {
            IDictionary<string, string> values = user.GetMessageValues();

            var tpl = await GetTemplate("Z-1E01 Send Temporary Password for Forget Password");
            values.Add("TemporaryPassword", tempPassword);
      
            job.Enqueue<NotificationService>(x => x.SendEmail(user.Email, user.FullName, tpl, values));
        }


    }
}
