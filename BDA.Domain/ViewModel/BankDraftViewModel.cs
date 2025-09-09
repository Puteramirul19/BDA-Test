using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using BDA.Entities;
using Microsoft.AspNetCore.Http;

namespace BDA.ViewModel
{
    public class BankDraftViewModel
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }

        //Draft & Submitted Section
        public string RefNo { get; set; }
        public string RequesterId { get; set; }
        public DateTime? DraftedOn { get; set; }
        public DateTime? SubmittedOn { get; set; }
        public string CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public string UserAction { get; set; }

        public string Comment { get; set; }

        //Verify Section
        public string VerifierComment { get; set; }
        public string VerifierId { get; set; }
        public DateTime? VerifiedOn { get; set; }

        //Approve Section
        public string ApproverComment { get; set; }
        public string ApproverId { get; set; }
        //public virtual ApplicationUser Approver { get; set; }
        public DateTime? ApprovedOn { get; set; }

        //Accept Section
        public string RequestorComment { get; set; }
        public string TGBSAcceptanceId { get; set; }
        public DateTime? TGBSAcceptedOn { get; set; }

        //TGBSProcess Section
        //public string InstructionLetterEmail { get; set; }
        //public DateTime? ValueDate { get; set; }
        public string InstructionLetterRefNo { get; set; }
        public string TGBSProcesserId { get; set; }
        public DateTime? TGBSProcessedOn { get; set; }

        //TGBSIssue Section
        public string BankDrafNoIssued { get; set; }
        public string NameOnBD { get; set; }
        public string SendMethod { get; set; }
        public string PostageNo { get; set; }
        public string ReceiverContactNo { get; set; }
        public string CoverMemoRefNo { get; set; }
        public DateTime? BankDraftDate { get; set; }
        public string TGBSIssuerId { get; set; }
        public DateTime? TGBSIssuedOn { get; set; }

        //Complete Section
        public string RequesterComment { get; set; }
        public DateTime? ReceiveBankDraftDate { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime? CompletedOn { get; set; }
        public string FinalApplication { get; set; }

        public string WCSuratKelulusanName { set; get; }
        public string WCUMAPName { set; get; }
        public string WHFileName { set; get; }
        public IFormFile WCSuratKelulusan { set; get; }
        public IFormFile WCUMAP { set; get; }
        public IFormFile WHFile { set; get; }
        public IFormFile SignedLetter { set; get; }
        public IFormFile SignedMemo { set; get; }
        public IFormFile Evidence { set; get; }
        public AttachmentViewModel WCSuratKelulusanVM { get; set; }
        public AttachmentViewModel WCUMAPVM { get; set; }
        public AttachmentViewModel WHFileVM { get; set; }
        public AttachmentViewModel SignedLetterVM { get; set; }
        public AttachmentViewModel SignedMemoVM { get; set; }
        public AttachmentViewModel EvidenceVM { get; set; }
        public WangCagaranViewModel WangCagaranViewModel { get; set; }
        public WangHangusViewModel WangHangusViewModel { get; set; }
        public WangCagaranHangusViewModel WangCagaranHangusViewModel { get; set; }
        public InstructionLetterViewModel InstructionLetterViewModel { get; set; }
        public MemoViewModel MemoViewModel { get; set; }
        public IEnumerable<BankDraft> BankDrafts { get; set; }
    }
}
