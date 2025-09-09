using System;
using System.Collections.Generic;
using System.Text;
using BDA.Entities;

namespace BDA.ViewModel
{
   
    public class InstructionLetterViewModel
    {
        public string Id { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public string ChargedBankAccount { get; set; }
        public string Remarks { get; set; }
        public string CostObject { get; set; }
        public string TaxCode { get; set; }
        public string Currency { get; set; }
        public string TaxAmount { get; set; }
        public string Amount { get; set; }
        public string BankPIC { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string State { get; set; }
        public string RinggitText { get; set; }
        public string InstructionLetterEmail { get; set; }
        public string ReferenceNo { get; set; }
        public string LetterRefNo { get; set; }
        public string LetterDate { get; set; }
        public string ValueDate { get; set; }
        public string ValueDateDay { get; set; }
        public string ApplicationType { get; set; }
        public string ProcessingType { get; set; }
        public string RujukanNo { get; set; }
        public AttachmentViewModel SignedLetterVM { get; set; }
        public InstructionLetter InstructionLetter { get; set; }
        public IEnumerable<PenerimaViewModel> Penerimas { get; set; }

    }

}
