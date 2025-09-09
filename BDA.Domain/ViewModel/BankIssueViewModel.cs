using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using BDA.Entities;

namespace BDA.ViewModel
{
   
    public class BankIssueViewModel
    {
        public string BDReferenceNo { get; set; }
        public string CoverMemoRefNo { get; set; }
        public int SendMethod { get; set; }
        public string PostageNo { get; set; }
        public string ContactNo { get; set; }
        public string BankDrafNoIssued { get; set; }

    }

}
