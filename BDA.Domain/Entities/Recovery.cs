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
    public class Recovery : AuditableEntity
    {
        public Guid BankDraftId { get; set; }
        public virtual BankDraft BankDraft { get; set; }
        public string Status { get; set; }

        //Draft & Submitted Section
        public string RefNo { get; set; }
        public string BDNo { get; set; }
        public string ProjNo { get; set; }
        public string ERMSDocNo { get; set; }
        public string NameOnBD { get; set; }
        public string CoCode { get; set; }
        public string BA { get; set; }
        public decimal? BDAmount { get; set; }
        public DateTime? ProjectCompletionDate { get; set; }
        public string BDRequesterName { get; set; }
        public string PBTEmailAddress { get; set; }
        public virtual ApplicationUser Requester { get; set; }
        public string RequesterId { get; set; }
        public string RequesterSubmissionComment { get; set; }
        public DateTime? DraftedOn { get; set; }
        public DateTime? PartialSubmittedOn { get; set; }
        public DateTime? SubmittedOn { get; set; }
        public DateTime? WithdrewOn { get; set; }
        public string CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public string ErmsDocNo1 { get; set; }
        public string ErmsDocNo2 { get; set; }

        //Process Section
        public DateTime? SiteVisitDate { get; set; }
        public DateTime? CPCDate { get; set; }
        public string ClaimDuration { get; set; }
        public string RequesterProcessComment { get; set; }
        public string ProcesserId { get; set; }
        public DateTime? ProcessedOn { get; set; }

        //Submit Section
        public string RecoveryType { get; set; }
        public string Stage { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalRecoveryAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? FirstRecoveryAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SecondRecoveryAmount { get; set; }
        public string RequesterFinalSubmissionComment { get; set; }
        public string PartialSubmitterId { get; set; }
        public string SubmitterId { get; set; }
        public DateTime? FinalPartialSubmissionOn { get; set; }
        public DateTime? FinalSubmissionOn { get; set; }

        //ReconAccept Section
        //public string TGBSPartialAcceptanceId { get; set; }
        public string TGBSAcceptanceId { get; set; }
        //public DateTime? PartialAcceptedOn { get; set; }
        public DateTime? AcceptedOn { get; set; }
        public string TGBSPartialAcceptanceId { get; set; }
        public DateTime? PartialAcceptedOn { get; set; }

        //ReconReceive Section
        public DateTime? PartialReceivedDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string TGBSPartialReceiverId { get; set; }
        public string TGBSReceiverId { get; set; }
        public DateTime? PartialReceivedOn { get; set; }
        public DateTime? ReceivedOn { get; set; }
        public DateTime? PostingDate1 { get; set; }
        public DateTime? PostingDate2 { get; set; }

        //ConfirmationSection
        public string TGBSConfirmationComment { get; set; }
        public DateTime? PartialCompletedOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public string TGBSPartialValidatorId { get; set; }
        public string TGBSValidatorId { get; set; }

        public IDictionary<string, string> GetMessageValues()
        {
            return new Dictionary<string, string>
            {
                { "ApplicationId", (Id.ToString() == null ? "" : Id.ToString())},
                { "RefNo", (RefNo == null ? "" : RefNo)},
                { "AppRefNo", (RefNo == null ? "" : RefNo.Substring(0, RefNo.Length-2))},
                { "ApplicationType", "Recovery"},
                { "RequesterId", (RequesterId == null ? "" : RequesterId) },
                { "Amount", BDAmount == null ? "" : string.Format("{0:C}", BDAmount)  },
                { "FirstRecoveryAmount", FirstRecoveryAmount  == null ? "" : string.Format("{0:C}", FirstRecoveryAmount)  },
                { "SecondRecoveryAmount", SecondRecoveryAmount  == null ? "" : string.Format("{0:C}", SecondRecoveryAmount)  },
                { "ProjectNo", (ProjNo == null ? "" : ProjNo) },
                { "ErmsID", (ERMSDocNo == null ? "" : ERMSDocNo) },
                { "NameOnBD", (NameOnBD == null ? "" : NameOnBD)},
                { "SubmitDate", (SubmittedOn == null ? "" : SubmittedOn.Value.ToString("dd-MM-yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture)) },
            };
        }
    }

}
