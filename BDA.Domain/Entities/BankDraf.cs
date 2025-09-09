using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using BDA.Data;
using BDA.Identity;
using Newtonsoft.Json;

namespace BDA.Entities
{
    public class BankDraft : AuditableEntity
    {
        //public BankDraft()
        //{
        //    Id = Guid.NewGuid();
        //    Attachments = new List<BankDraftAttachment>();
        //}

        public string Status { get; set; }
        public string Type { get; set; }
        public string ProjectNo { get; set; }

        //Draft & Submitted Section
        public string RefNo { get; set; }
        public string RequesterId { get; set; }
        public string RequesterSubmissionComment { get; set; }
        public virtual ApplicationUser Requester { get; set; }
        public DateTime? DraftedOn { get; set; }
        public DateTime? SubmittedOn { get; set; }
        public DateTime? WithdrewOn { get; set; }
        public string CreatedById { get; set; }
        public string CreatedByName { get; set; }

        //Verify Section
        public string VerifierComment { get; set; }
        public string VerifierId { get; set; }
        public virtual ApplicationUser Verifier { get; set; }
        public DateTime? VerifiedOn { get; set; }

        //Approve Section
        public string ApproverComment { get; set; }
        public string ApproverId { get; set; }
        public virtual ApplicationUser Approver { get; set; }
        public DateTime? ApprovedOn { get; set; }

        //Accept Section
        public string TGBSAcceptedComment { get; set; }
        public string TGBSAcceptanceId { get; set; }
        public virtual ApplicationUser TGBSAcceptance { get; set; }
        public DateTime? TGBSAcceptedOn { get; set; }

        //TGBSProcess Section
        public string InstructionLetterRefNo { get; set; }
        public string TGBSProcesserId { get; set; }
        public virtual ApplicationUser TGBSProcesser { get; set; }
        public DateTime? TGBSProcessedOn { get; set; }

        //TGBSIssue Section
        public string BankDrafNoIssued { get; set; }
        public string NameOnBD { get; set; }
        public string SendMethod { get; set; }
        public string PostageNo { get; set; }
        public string ReceiverContactNo { get; set; }
        public string CoverMemoRefNo { get; set; }
        public DateTime? BankDraftDate { get; set; }
        //public double BankDraftAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? BankDraftAmount { get; set; }
        public string TGBSIssuerId { get; set; }
        public virtual ApplicationUser TGBSIssuer { get; set; }
        public DateTime? TGBSIssuedOn { get; set; }

        //Complete Section
        public string RequesterComment { get; set; }
        public DateTime? ReceiveBankDraftDate{ get; set; }
        public string ReceiptNo{ get; set; }
        public DateTime? CompletedOn { get; set; }
        public string FinalApplication { get; set; }
        public string IntegrationId { get; set; }

        public IDictionary<string, string> GetMessageValues()
        {
            return new Dictionary<string, string>
            {
                { "ApplicationId", (Id.ToString() == null ? "" : Id.ToString())},
                { "RefNo", (RefNo == null ? "" : RefNo)},
                { "ApplicationType", (Type == null ? "" : Type)},
                { "RequesterId", (RequesterId == null ? "" : RequesterId) },
                { "VerifierId", (VerifierId == null ? "" : VerifierId) },
                { "ApproverId", (ApproverId == null ? "" : ApproverId) },
                { "ProjectNo", (ProjectNo == null ? "" : ProjectNo) },
                { "Amount", (BankDraftAmount == null ? "" : string.Format("{0:C}", BankDraftAmount)) },
                { "VerifierComment", (VerifierComment == null ? "" : VerifierComment) },
                { "ApproverComment", (ApproverComment == null ? "" : ApproverComment) },
                { "TGBSComment", (TGBSAcceptedComment == null ? "" : TGBSAcceptedComment) },
                { "SendMethod", (SendMethod == "SelfCollect" ? "ready to collect" : "tracking postage") },
                { "SubmitDate", (SubmittedOn == null ? "" : SubmittedOn.Value.ToString("dd-MM-yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture)) },
            };
        }
        //public virtual ICollection<BankDraftAttachment> Attachments { get; set; }
        //public virtual ICollection<ERMSQueues> ERMSQueues { get; set; }
    }

    public class BankDraftAttachment : Entity<Guid>
    {
        public BankDraftAttachment()
        {
            Attachment = new Attachment();
        }

        public Guid BankDraftId { get; set; }
        [JsonIgnore] public virtual BankDraft BankDraft { get; set; }
        public BDAttachmentType Type { get; set; }
        public Attachment Attachment { get; set; }
    }

    //public enum Status
    //{
    //    Draft,
    //    Submitted,
    //    RejectedVerify,
    //    Verified,
    //    RejectedApprove,
    //    Approved,
    //    Declined,
    //    Accepted,
    //    SaveProcessed,
    //    Processed,
    //    SaveIssued,
    //    Issued,
    //    SaveComplete,
    //    Complete

    //}


    public enum SendingMethod
    {
        SelfCollect,
        ByPos

    }
    //7 Stages (Wang Hangus) Draft -> Submitted -> Approved -> TGBS Accepted -> TGBS Processed -> IssuedByBank -> Complete
    //8 Stages (Wang Cagar) Draft -> Submitted -> Verified -> Approved -> TGBS Accepted -> TGBS Processed -> IssuedByBank -> Complete
}
