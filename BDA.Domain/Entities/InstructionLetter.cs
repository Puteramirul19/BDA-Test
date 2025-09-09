using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace BDA.Entities
{
    public class InstructionLetter : AuditableEntity//Entity<Guid>
    {
        public InstructionLetter()
        { 
            //Id = Guid.NewGuid();
            //Attachments = new List<InstructionLetterAttachment>();
        }
        //public Guid BankDraftId { get; set; }
        //public virtual BankDraft BankDraft { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public string ChargedBankAccount { get; set; }
        public string Remarks { get; set; }
        //public string CostObject { get; set; }
        //public string TaxCode { get; set; }
        //public string Currency { get; set; }
        //public string TaxAmount { get; set; }
        //public string Amount { get; set; }
        public string BankPIC { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string State { get; set; }
        public string RinggitText { get; set; }
        public string InstructionLetterEmail { get; set; }
        public string ReferenceNo { get; set; }
        public string ApplicationType { get; set; }
        public DateTime? LetterDate { get; set; }
        public DateTime? ValueDate { get; set; }
        public string LetterRefNo { get; set; }
        public string Status { get; set; }
        public string ProcessingType { get; set; }
        public string RujukanNo { get; set; }
        //public virtual ICollection<InstructionLetterAttachment> Attachments { get; set; }

        public IDictionary<string, string> GetMessageValues()
        {
            return new Dictionary<string, string>
            {
                { "BankPIC", (BankPIC == null ? "" : BankPIC)},
                { "BankName", (BankName == null ? "" : BankName)},
                //{ "RequesterId", (RequesterId == null ? "" : RequesterId) },
                //{ "VerifierId", (VerifierId == null ? "" : VerifierId) },
                //{ "ApproverId", (ApproverId == null ? "" : ApproverId) },
                //{ "ProjectNo", (ProjectNo == null ? "" : ProjectNo) },
                //{ "Amount", BankDraftAmount.ToString() },
                //{ "VerifierComment", (VerifierComment == null ? "" : VerifierComment) },
                //{ "ApproverComment", (ApproverComment == null ? "" : ApproverComment) },
                //{ "TGBSComment", (TGBSAcceptedComment == null ? "" : TGBSAcceptedComment) },
                //{ "SendMethod", (SendMethod == "SelfCollect" ? "ready to collect" : "tracking postage") },
            };
        }
    }

    //public class InstructionLetterAttachment : Entity<Guid>
    //{
    //    public InstructionLetterAttachment()
    //    {
    //        Attachment = new Attachment();
    //    }

    //    public Guid InstructionLetterId { get; set; }
    //    [JsonIgnore] public virtual InstructionLetter InstructionLetter { get; set; }
    //    public string Type { get; set; }
    //    public Attachment Attachment { get; set; }
    //}
}
