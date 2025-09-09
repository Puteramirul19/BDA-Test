//using System;
//using System.Collections.Generic;
//using System.Text;
//using BDA.Data;
//using BDA.Entities;
//using BDA.Identity;
//using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;
//using System.Linq;
//using BDA.ViewModel;

//namespace BDA.Services
//{
//    public class BankDraftApplicationService
//    {
//        private BdaDBContext db;
//        //private IRunningNumber runningNumber;

//        public BankDraftApplicationService(BdaDBContext db)
//        {
//            this.db = db;
//            //this.runningNumber = runningNumber;
//        }

//        public string GetRunningNo(string type)
//        {
//            RunningNo runningNo = new RunningNo();

//            var entity = db.RunningNo.Where(x => x.Name == "WangCagaran").FirstOrDefault();

//            if (type == "WangHangus")
//            {
//                entity = db.RunningNo.Where(x => x.Name == "WangHangus").FirstOrDefault(); //Id for Instruction Letter
             
//            }

//            runningNo.Code = entity.Code;
//            runningNo.RunNo = entity.RunNo;
//            string NewCode = String.Format("{0}{1:00000}", runningNo.Code, runningNo.RunNo);

//            entity.RunNo = entity.RunNo + 1;
//            db.RunningNo.Update(entity);
//            db.SaveChanges();

//            return NewCode;
//        }

//        //public async Task<WorkflowResult<BankDraft>> Create(BankDraft bankDraft)
//        //{
//        //        db.BankDraftAction.Add(new BankDraftAction
//        //        {
//        //            ActionType = ActionType.Draft,
//        //            On = now,
//        //            ById = requesterId,
//        //            BankDraft = bankDraft,
//        //            ActionRole = ActionRole.Requestor
//        //        });
         
//        //    await db.SaveChangesAsync();

//        //    result.IsSuccess = true;

//        //    ////setup scheduler to check whether to send notice to SOA
//        //    //backgroundJob.Schedule<IShutdownNoticeService>(s => s.TriggerNotice(notice.Id), notice.StartOn.AddDays(-7));

//        //    return result;
//        //}

//        public async Task<WorkflowResult<BankDraft>> Edit(Guid bankDraftId, string userId)
//        {
//            var bankDraft = await TryFindBankDraft(bankDraftId); //try find by noticed for uneditable data

//            var result = AllowStatus(bankDraft, new List<string> { Data.Status.Draft.ToString(), Data.Status.RejectedVerify.ToString(), Data.Status.RejectedApprove.ToString(), Data.Status.Declined.ToString() });
//            if (!result.IsSuccess)
//                return result;

//            var now = DateTime.Now;

//            db.BankDraft.Add(bankDraft);

//            bankDraft.Status = Data.Status.Submitted.ToString();
//            bankDraft.SubmittedOn = now;
//            bankDraft.RequesterId = userId;

//            db.BankDraftAction.Add(new BankDraftAction
//            {
//                ActionType = ActionType.Submitted,
//                On = now,
//                ById = userId,
//                BankDraft = bankDraft,
//                ActionRole = ActionRole.Requestor
//            });
//            await db.SaveChangesAsync();

//            result.IsSuccess = true;

//            ////setup scheduler to check whether to send notice to SOA
//            //backgroundJob.Schedule<IShutdownNoticeService>(s => s.TriggerNotice(notice.Id), notice.StartOn.AddDays(-7));

//            return result;
//        }

//        public async Task<WorkflowResult<BankDraft>> Submit(Guid bankDraftId, string userId)
//        {
//            var bankDraft = await TryFindBankDraft(bankDraftId);
//            var result = AllowStatus(bankDraft, new List<string> { Data.Status.Draft.ToString() });

//            // Generate ref no for wang cagaran
//            bankDraft.RefNo = "WG";//runningNumber.Next(new NumberForWangCagaran("WG"));

//            var now = DateTime.Now;

//            db.BankDraft.Add(bankDraft);

//            bankDraft.Status = Data.Status.Submitted.ToString();
//            bankDraft.SubmittedOn = now;
//            bankDraft.RequesterId = userId;

//            db.BankDraftAction.Add(new BankDraftAction
//            {
//                ActionType = ActionType.Submitted,
//                On = now,
//                ById = userId,
//                BankDraft = bankDraft,
//                ActionRole = ActionRole.Requestor
//            });
//            await db.SaveChangesAsync();

//            result.IsSuccess = true;

//            ////setup scheduler to check whether to send notice to SOA
//            //backgroundJob.Schedule<IShutdownNoticeService>(s => s.TriggerNotice(notice.Id), notice.StartOn.AddDays(-7));

//            return result;
//        }

//        private async Task<BankDraft> TryFindBankDraft(Guid bankDraftId)
//        {
//            var notice = await db.BankDraft
//                            .FirstOrDefaultAsync(x => x.Id == bankDraftId);
//            if (notice == null)
//                throw new IdNotFoundException<BankDraft>(bankDraftId);
//            return notice;
//        }

//        public async Task<WorkflowResult<BankDraft>> Verify(Guid bankDraftId, string userId, bool isVerified, string comment)
//        {
//            var bankDraft = await TryFindBankDraft(bankDraftId);
//            var result = AllowStatus(bankDraft, new List<string> { Data.Status.Submitted.ToString() });
//            var user = db.Users.Find(userId);
//            if (user == null)
//                throw new IdNotFoundException<ApplicationUser>(userId);

//            var now = DateTime.Now;

//            if (isVerified)
//            {
//                bankDraft.VerifierComment = comment;
//                bankDraft.Status = Data.Status.Verified.ToString();
//                bankDraft.VerifierId = userId;
//                bankDraft.VerifiedOn = now;
//            }
//            else
//            {
//                bankDraft.VerifierComment = comment;
//                bankDraft.Status = Data.Status.RejectedVerify.ToString();
//            }

//            db.SetModified(bankDraft);

//            if (isVerified)
//            {
//                db.BankDraftAction.Add(new BankDraftAction
//                {
//                    ActionType = ActionType.Verified,
//                    On = now,
//                    ById = userId,
//                    BankDraft = bankDraft,
//                    ActionRole = ActionRole.Verifier,
//                    Comment = comment
//                });
//            }
//            else
//            {
//                db.BankDraftAction.Add(new BankDraftAction
//                {
//                    ActionType = ActionType.Rejected,
//                    On = now,
//                    ById = userId,
//                    BankDraft = bankDraft,
//                    ActionRole = ActionRole.Verifier,
//                    Comment = comment
//                });
//            }
//            await db.SaveChangesAsync();

//            // Notifications
//            //backgroundJob.Enqueue<Notifier>(x => x.NotifyForReviewed(notice.Id));

//            result.IsSuccess = true;
//            return result;
//        }

//        public async Task<WorkflowResult<BankDraft>> Approve(Guid bankDraftId, string wangType, string userId, bool isApproved, string comment)
//        {
//            var bankDraft = await TryFindBankDraft(bankDraftId);
//            var result = AllowStatus(bankDraft, new List<string> { Data.Status.Verified.ToString(), Data.Status.Submitted.ToString() });

//            if (wangType == "Cagaran")
//            {
//                result = AllowStatus(bankDraft, new List<string> { Data.Status.Verified.ToString() });
//            }
//            else if(wangType == "Hangus")
//            {
//                result = AllowStatus(bankDraft, new List<string> { Data.Status.Submitted.ToString() });
//            }

//            var user = db.Users.Find(userId);
//            if (user == null)
//                throw new IdNotFoundException<ApplicationUser>(userId);

//            var now = DateTime.Now;

//            if (isApproved)
//            {
//                bankDraft.ApproverComment = comment;
//                bankDraft.Status = Data.Status.Approved.ToString();
//                bankDraft.ApprovedOn = now;
//                bankDraft.TGBSAcceptanceId = userId;
//            }
//            else
//            {
//                bankDraft.VerifierComment = comment;
//                bankDraft.Status = Data.Status.RejectedApprove.ToString();
//            }

//            db.SetModified(bankDraft);

//            if (isApproved)
//            {
//                db.BankDraftAction.Add(new BankDraftAction
//                {
//                    ActionType = ActionType.Approved,
//                    On = now,
//                    ById = userId,
//                    BankDraft = bankDraft,
//                    ActionRole = ActionRole.Approver,
//                    Comment = comment
//                });
//            }
//            else
//            {
//                db.BankDraftAction.Add(new BankDraftAction
//                {
//                    ActionType = ActionType.Rejected,
//                    On = now,
//                    ById = userId,
//                    BankDraft = bankDraft,
//                    ActionRole = ActionRole.Approver,
//                    Comment = comment
//                });
//            }

//            await db.SaveChangesAsync();

//            // Notifications
//            //backgroundJob.Enqueue<Notifier>(x => x.NotifyForReviewed(notice.Id));

//            result.IsSuccess = true;
//            return result;
//        }

//        public async Task<WorkflowResult<BankDraft>> Accept(Guid bankDraftId, string userId, bool isAccepted, string comment)
//        {
//            var bankDraft = await TryFindBankDraft(bankDraftId);
//            var result = AllowStatus(bankDraft, new List<string> { Data.Status.Approved.ToString() } );

//            var user = db.Users.Find(userId);
//            if (user == null)
//                throw new IdNotFoundException<ApplicationUser>(userId);

//            var now = DateTime.Now;

//            if (isAccepted)
//            {
//                bankDraft.RequestorComment = comment;
//                bankDraft.Status = Data.Status.Accepted.ToString();
//                bankDraft.TGBSAcceptedOn = now;
//                bankDraft.TGBSAcceptanceId = userId;
//            }
//            else
//            {
//                bankDraft.RequestorComment = comment;
//                bankDraft.Status = Data.Status.Declined.ToString();
//            }

//            db.SetModified(bankDraft);

//            if (isAccepted)
//            {
//                db.BankDraftAction.Add(new BankDraftAction
//                {
//                    ActionType = ActionType.Accepted,
//                    On = now,
//                    ById = userId,
//                    BankDraft = bankDraft,
//                    ActionRole = ActionRole.TGBSBanking,
//                    Comment = comment
//                });
//            }
//            else
//            {
//                db.BankDraftAction.Add(new BankDraftAction
//                {
//                    ActionType = ActionType.Declined,
//                    On = now,
//                    ById = userId,
//                    BankDraft = bankDraft,
//                    ActionRole = ActionRole.TGBSBanking,
//                    Comment = comment
//                });
//            }

//            await db.SaveChangesAsync();

//            // Notifications
//            //backgroundJob.Enqueue<Notifier>(x => x.NotifyForReviewed(notice.Id));

//            result.IsSuccess = true;
//            return result;
//        }

//        public async Task<WorkflowResult<BankDraft>> Process(Guid bankDraftId, string userId, bool isProcessed)
//        {
//            var bankDraft = await TryFindBankDraft(bankDraftId);
//            var result = AllowStatus(bankDraft, new List<string> { Data.Status.Accepted.ToString(), Data.Status.SaveProcessed.ToString() });

//            var user = db.Users.Find(userId);
//            if (user == null)
//                throw new IdNotFoundException<ApplicationUser>(userId);

//            var now = DateTime.Now;

//            if (isProcessed)
//            {
//                bankDraft.Status = Data.Status.Processed.ToString();
//                bankDraft.TGBSProcessedOn = now;
//                bankDraft.TGBSProcesserId = userId;
//            }
//            else
//            {
//                bankDraft.Status = Data.Status.SaveProcessed.ToString();
//            }

//            db.SetModified(bankDraft);

//            if (isProcessed)
//            {
//                db.BankDraftAction.Add(new BankDraftAction
//                {
//                    ActionType = ActionType.SubmittedToBank,
//                    On = now,
//                    ById = userId,
//                    BankDraft = bankDraft,
//                    ActionRole = ActionRole.TGBSBanking
//                });
//            }

//            await db.SaveChangesAsync();

//            // Notifications
//            //backgroundJob.Enqueue<Notifier>(x => x.NotifyForReviewed(notice.Id));

//            result.IsSuccess = true;
//            return result;
//        }

//        public async Task<WorkflowResult<BankDraft>> Issue(Guid bankDraftId, string userId, bool isIssued)
//        {
//            var bankDraft = await TryFindBankDraft(bankDraftId);
//            var result = AllowStatus(bankDraft, new List<string> { Data.Status.Processed.ToString(), Data.Status.SaveIssued.ToString() });

//            var user = db.Users.Find(userId);
//            if (user == null)
//                throw new IdNotFoundException<ApplicationUser>(userId);

//            var now = DateTime.Now;

//            if (isIssued)
//            {
//                bankDraft.Status = Data.Status.Issued.ToString();
//                bankDraft.TGBSIssuerId = userId;
//                bankDraft.TGBSIssuedOn = now;
//            }
//            else
//            {
//                bankDraft.Status = Data.Status.SaveIssued.ToString();
//            }

//            db.SetModified(bankDraft);

//            if (isIssued)
//            {
//                db.BankDraftAction.Add(new BankDraftAction
//                {
//                    ActionType = ActionType.BankDraftIssued,
//                    On = now,
//                    ById = userId,
//                    BankDraft = bankDraft,
//                    ActionRole = ActionRole.TGBSBanking
//                });
//            }

//            await db.SaveChangesAsync();

//            // Notifications
//            //backgroundJob.Enqueue<Notifier>(x => x.NotifyForReviewed(notice.Id));

//            result.IsSuccess = true;
//            return result;
//        }

//        public async Task<WorkflowResult<BankDraft>> Complete(Guid bankDraftId, string userId, bool isCompleted, string comment)
//        {
//            var bankDraft = await TryFindBankDraft(bankDraftId);
//            var result = AllowStatus(bankDraft, new List<string> { Data.Status.Processed.ToString(), Data.Status.SaveComplete.ToString() });

//            var user = db.Users.Find(userId);
//            if (user == null)
//                throw new IdNotFoundException<ApplicationUser>(userId);

//            var now = DateTime.Now;

//            if (isCompleted)
//            {
//                bankDraft.RequestorComment = comment;
//                bankDraft.Status = Data.Status.Complete.ToString();
//                bankDraft.CompletedOn = now;
//            }
//            else
//            {
//                bankDraft.RequestorComment = comment;
//                bankDraft.Status = Data.Status.SaveComplete.ToString();
//            }

//            db.SetModified(bankDraft);

//            if (isCompleted)
//            {
//                db.BankDraftAction.Add(new BankDraftAction
//                {
//                    ActionType = ActionType.Complete,
//                    On = now,
//                    ById = userId,
//                    BankDraft = bankDraft,
//                    ActionRole = ActionRole.Requestor,
//                    Comment = comment
//                });
//            }

//            await db.SaveChangesAsync();

//            // Notifications
//            //backgroundJob.Enqueue<Notifier>(x => x.NotifyForReviewed(notice.Id));

//            result.IsSuccess = true;
//            return result;
//        }

//        private WorkflowResult<BankDraft> AllowStatus(BankDraft bankDraft, List<string> statuses)
//        {
//            var result = new WorkflowResult<BankDraft>
//            {
//                Entity = bankDraft
//            };
            
//            if (statuses.Contains(bankDraft.Status))
//                result.IsSuccess = true;
//            else
//            {
//                result.IsSuccess = false;
//                result.Message = "This action requires BankDraft to be in status " + String.Join(" or ", statuses);
//            }

//            return result;
//        }

//        public class WorkflowResult<T>
//        {
//            public bool IsSuccess { get; set; }
//            public string Message { get; set; }
//            public T Entity { get; set; }
//        }
//    }
//}
