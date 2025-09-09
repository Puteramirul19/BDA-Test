using System;
using System.Collections.Generic;
using System.Text;
using BDA.Entities;

namespace BDA.ViewModel
{
   
    public class StateMonthlyAmountViewModel
    {
        //Based on Posting Date - Acceptance Date
        public int Month { get; set; }
        public int Year { get; set; }
        public Guid StateId { get; set; }

        //Total number of bank draft issued - WangCagaran only as only WG can be recovered
        public int BDNoIssued { get; set; }
        public decimal? Amount { get; set; }

        //Total number of Bank Draft recovered, grouped by state
        public int BDNoRecovered { get; set; }
        public decimal? RecoveryAmount { get; set; }

        //Amount of Bank Draft issued that has not yet been recovered
        public int BDNoOutstanding { get; set; }
        public decimal? OutstandingAmount { get; set; }
        public IEnumerable<StateMonthlyProfileViewModel> SMAs { get; set; }
    }

}
