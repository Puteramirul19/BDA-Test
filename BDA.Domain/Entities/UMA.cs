using BDA.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BDA.Entities
{
    public class UMA
    {
        [Key]
        public Guid Id { get; set; }
        public string RefNo { get; set; }
        public string RequesterId { get; set; }
        public string ApproverId { get; set; }
        public Guid BankDraftId { get; set; }
        public string BDNo { get; set; }
        public decimal? BDAmount { get; set; }
        public string Status { get; set; }
        public string InstructionLetterRefNo { get; set; }
        public string BDRequesterName { get; set; }
        public string ERMSDocNo { get; set; }
        public string CoCode { get; set; }
        public string BA { get; set; }
        public string NameOnBD { get; set; }
        public string ProjectNo { get; set; }
        public string Justification { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string Comment { get; set; }
        public DateTime? DraftedOn { get; set; }
        public DateTime? SubmittedOn { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string CreatedById { get; set; }
        public string CreatedByName { get; set; }

        [ForeignKey("RequesterId")]
        public virtual ApplicationUser Requester { get; set; }

        [ForeignKey("ApproverId")]
        public virtual ApplicationUser Approver { get; set; }

        [ForeignKey("BankDraftId")]
        public virtual BankDraft BankDraft { get; set; }
    }
}