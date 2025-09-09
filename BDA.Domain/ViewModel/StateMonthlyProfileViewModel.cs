using System;
using System.Collections.Generic;
using System.Text;
using BDA.Entities;

namespace BDA.ViewModel
{
   
    public class StateMonthlyProfileViewModel
    {
        //Based on Posting Date - Acceptance Date
        public string month { get; set; }
        public string year { get; set; }
        public string monthName { get; set; }
        public string state { get; set; }

        //Total number of bank draft issued - WangCagaran only as only WG can be recovered
        public int noOfBDIssued { get; set; }
        public string amount { get; set; }
        public decimal? amountNum { get; set; }
        public int noOfRecovery { get; set; }
        public string amountRev { get; set; }
        public decimal? amountRevNum { get; set; }
        public int noOfOutstanding { get; set; }
        public string outstandingAmount { get; set; }

    }

}
