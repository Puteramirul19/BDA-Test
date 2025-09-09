using System;
using System.Collections.Generic;
using System.Text;

namespace BDA.ViewModel
{
   
    public class PenerimaViewModel
    {
        //InstructionLetter
        public string BankDraftId { get; set; }
        //public string RefNo { get; set; }
        public string InstructionLetterRefNo { get; set; }
        public string NamaPenerima { get; set; }
        public string Tempat { get; set; }
        public decimal? Jumlah { get; set; }

        //CoverMemo
        public string ProjectNo { get; set; }
        public string CoverMemoRefNo { get; set; }
        public string BankDraftDate { get; set; }
        public string BankDraftNo { get; set; }
        public string ApplicationRefNo { get; set; } /*added by Hanif 15102020*/
    }

}
