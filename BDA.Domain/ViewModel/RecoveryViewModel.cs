using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using BDA.Entities;
using Microsoft.AspNetCore.Http;

namespace BDA.ViewModel
{
    public class RecoveryViewModel
    {
        public string Id { get; set; }
        public string BankDraftId { get; set; }
        public string Status { get; set; }

        //Draft & Submitted Section
        public string RefNo { get; set; }
        public string BDNo { get; set; }
        public string NameOnBD { get; set; }
        public string ProjNo { get; set; }
        public string ERMSDocNo { get; set; }
        public string CoCode { get; set; }
        public string BA { get; set; }
        public decimal? BDAmount { get; set; }
        public DateTime? ProjectCompletionDate { get; set; }
        public string BDRequesterName { get; set; }
        public string PBTEmailAddress { get; set; }
        public string RequesterId { get; set; }
        public string RequesterSubmissionComment { get; set; }
        public DateTime? DraftedOn { get; set; }
        public DateTime? SubmittedOn { get; set; }
        public DateTime? FullSubmittedOn { get; set; }
        public DateTime? WithdrewOn { get; set; }
        public string CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public string ErmsDocNo1 { get; set; }
        public string ErmsDocNo2 { get; set; }
        public string UserAction { get; set; }

        public string Comment { get; set; }

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
        //[DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = true)]
        public decimal? TotalRecoveryAmount { get; set; }
        public decimal? FirstRecoveryAmount { get; set; }
        public decimal? SecondRecoveryAmount { get; set; }
        public string RequesterFinalSubmissionComment { get; set; }
        public string PartialSubmitterId { get; set; }
        public string SubmitterId { get; set; }
        public DateTime? FinalPartialSubmissionOn { get; set; }
        public DateTime? FinalSubmissionOn { get; set; }

        //ReconAccept Section
        public string TGBSPartialAcceptanceId { get; set; }
        public string TGBSAcceptanceId { get; set; }
        public DateTime? PartialAcceptedOn { get; set; }
        public DateTime? AcceptedOn { get; set; }

        //ReconReceive Section
        public DateTime? PartialReceivedDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string TGBSPartialReceiverId { get; set; }
        public string TGBSReceiverId { get; set; }
        public DateTime? PartialReceivedOn { get; set; }
        public DateTime? ReceivedOn { get; set; }

        //ConfirmationSection
        public string TGBSConfirmationComment { get; set; }
        public DateTime? PartialCompletedOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public DateTime? PostingDate1 { get; set; }
        public DateTime? PostingDate2 { get; set; }
        public string TGBSPartialValidatorId { get; set; }
        public string TGBSValidatorId { get; set; }

        //public string Checklist { set; get; }
        //public string RecoveryLetter { set; get; }
        public string FirstParBankStatementName { set; get; }
        public string SecondParBankStatementName { set; get; }
        public string BankStatementName { set; get; }
        public string FirstPartialDocName { set; get; }
        public string SecondPartialDocName { set; get; }
        public string FullDocName { set; get; }

        public IFormFile Checklist { set; get; }
        public IFormFile RecoveryLetter { set; get; }
        public IFormFile FirstParBankStatement { set; get; }
        public IFormFile SecondParBankStatement { set; get; }
        public IFormFile BankStatement { set; get; }
        public IFormFile FirstPartialDoc { set; get; }
        public IFormFile SecondPartialDoc { set; get; }
        public IFormFile FullDoc { set; get; }
        public AttachmentViewModel ChecklistVM { get; set; }
        public AttachmentViewModel RecoveryLetterVM { get; set; }
        public AttachmentViewModel FirstParBankStatementVM { get; set; }
        public AttachmentViewModel SecondParBankStatementVM { get; set; }
        public AttachmentViewModel BankStatementVM { get; set; }
        public AttachmentViewModel FirstPartialDocVM { get; set; }
        public AttachmentViewModel SecondPartialDocVM { get; set; }
        public AttachmentViewModel FullDocVM { get; set; }

    }
}
