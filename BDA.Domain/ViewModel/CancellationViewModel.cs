using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using BDA.Entities;
using Microsoft.AspNetCore.Http;

namespace BDA.ViewModel
{
    public class CancellationViewModel
    {
        public string Id { get; set; }
        public string BankDraftId { get; set; }
        public string Status { get; set; }
        public string RefNo { get; set; }
        public string BDNo { get; set; }
        public string BDRequesterName { get; set; }
        public string ProjectNo { get; set; }
        public string ERMSDocNo { get; set; }
        public string CoCode { get; set; }
        public string BA { get; set; }
        public string NameOnBD { get; set; }
        public decimal? BDAmount { get; set; }
        public string Comment { get; set; }

        //Draft & Submitted Section
        public string RequesterId { get; set; }
        public string ReasonCancel { get; set; }
        public string OthersRemark { get; set; }
        public DateTime? DraftedOn { get; set; }
        public DateTime? SubmittedOn { get; set; }
        public DateTime? WithdrewOn { get; set; }
        public string CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public string UserAction { get; set; }

        //Approve Section
        public string ApproverId { get; set; }
        public DateTime? ApprovedOn { get; set; }

        //Accept Section
        public string TGBSAcceptanceId { get; set; }
        public DateTime? TGBSAcceptedOn { get; set; }

        //TGBSProcess Section
        public string InstructionLetterRefNo { get; set; }
        public string TGBSProcesserId { get; set; }
        public string BankProcessType { get; set; }
        public DateTime? TGBSProcessedOn { get; set; }

        //ReconReceive Section
        public DateTime? ReceivedDate { get; set; }
        //public double BankDraftAmount { get; set; } 
        public string ReceivedById { get; set; }
        public DateTime? ReceivedOn { get; set; }

        //ConfirmationSection
        public DateTime? CompletedOn { get; set; }

        public string ScannedBankDraftName { set; get; }
        public string ScannedMemoName { set; get; }
        public string BankStatementName { set; get; }
        public IFormFile ScannedBankDraft { set; get; }
        public IFormFile ScannedMemo { set; get; } 
        public IFormFile BankStatement { set; get; }
        public IFormFile SignedLetter { set; get; }
        public IFormFile UMAPinForm { set; get; }
        public IFormFile ScannedLetter { set; get; }
        public AttachmentViewModel ScannedBankDraftVM { get; set; }
        public AttachmentViewModel ScannedMemoVM { get; set; }
        public AttachmentViewModel BankStatementVM { get; set; }
        public AttachmentViewModel SignedLetterVM { get; set; }
        public AttachmentViewModel UMAPinFormVM { get; set; }
        public AttachmentViewModel ScannedLetterVM { get; set; }
        public InstructionLetterViewModel InstructionLetterViewModel { get; set; }
    }
}
