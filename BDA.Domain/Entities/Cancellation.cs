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
    public class Cancellation : AuditableEntity
    {
        public Guid BankDraftId { get; set; }
        public virtual BankDraft BankDraft { get; set; }
        public string Status { get; set; }
        public string RefNo { get; set; }
        public string BDNo { get; set; }
        public string BDRequesterName { get; set; }
        public string ProjectNo { get; set; }
        public string ERMSDocNo { get; set; }
        public string CoCode { get; set; }
        public string BA { get; set; }
        public string NameOnBD { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? BDAmount { get; set; }

        //Draft & Submitted Section
        public string RequesterId { get; set; }
        public string ReasonCancel{ get; set; }
        public string OthersRemark { get; set; }
        //public string RequesterSubmissionComment { get; set; }
        public virtual ApplicationUser Requester { get; set; }
        public DateTime? DraftedOn { get; set; }
        public DateTime? SubmittedOn { get; set; }
        public DateTime? WithdrewOn { get; set; }
        public string CreatedById { get; set; }
        public string CreatedByName { get; set; }

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
        public string BankProcessType { get; set; }
        public virtual ApplicationUser TGBSProcesser { get; set; }
        public DateTime? TGBSProcessedOn { get; set; }

        //ReconReceive Section
        public DateTime? ReceivedDate { get; set; }
        //public double BankDraftAmount { get; set; } 
        public string TGBSReceiverId { get; set; }
        public virtual ApplicationUser ReconReceiver { get; set; }
        public DateTime? ReceivedOn { get; set; }

        //ConfirmationSection
        public string TGBSConfirmationComment { get; set; }
        public DateTime? CompletedOn { get; set; }
        public string TGBSValidatorId { get; set; }

        public IDictionary<string, string> GetMessageValues()
        {
            return new Dictionary<string, string>
            {
                { "ApplicationId", (Id.ToString() == null ? "" : Id.ToString())},
                { "RefNo", (RefNo == null ? "" : RefNo)},
                { "AppRefNo", (RefNo == null ? "" : RefNo.Substring(0, RefNo.Length-2))},
                { "ApplicationType", "Cancellation"},
                { "Amount", BDAmount == null ? "" : string.Format("{0:C}", BDAmount)  },
                { "RequesterId", (RequesterId == null ? "" : RequesterId) },
                { "ApproverId", (ApproverId == null ? "" : ApproverId) },
                { "ProjectNo", (ProjectNo == null ? "" : ProjectNo) },
                { "ErmsID", (ERMSDocNo == null ? "" : ERMSDocNo) },
                { "NameOnBD", (NameOnBD == null ? "" : NameOnBD)},
                { "ApproverComment", (ApproverComment == null ? "" : ApproverComment) },
                { "TGBSComment", (TGBSAcceptedComment == null ? "" : TGBSAcceptedComment) },
                { "SubmitDate", (SubmittedOn == null ? "" : SubmittedOn.Value.ToString("dd-MM-yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture)) },
                //{ "ValueDate", (SubmittedOn == null ? "" : SubmittedOn.Value.ToString("dd-MM-yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture)) },
            };
        }
    }

}
