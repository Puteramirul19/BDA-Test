using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BDA.ViewModel
{
    public class UMAViewModel
    {
        public string Id { get; set; }
        public string RefNo { get; set; }
        public string RequesterId { get; set; }
        public string ApproverId { get; set; }
        public string BankDraftId { get; set; }
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

        public AttachmentViewModel ScannedPoliceReportVM { get; set; }
        public AttachmentViewModel ScannedPBTDocVM { get; set; }
        public AttachmentViewModel SignedLetterVM { get; set; }
        public AttachmentViewModel SignedIndemningFormVM { get; set; }
        public AttachmentViewModel BankStatementVM { get; set; }
    }
}