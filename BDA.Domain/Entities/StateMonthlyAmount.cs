using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BDA.Entities
{
    public class StateMonthlyAmount : AuditableEntity
    {
        //Based on Posting Date - Acceptance Date
        public int Month { get; set; }
        public int Year { get; set; }
        public string CoCode { get; set; }
        public Guid StateId { get; set; }

        //Total number of bank draft issued - WangCagaran only as only WG can be recovered
        public int BDNoIssued { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Amount { get; set; }

        //Total number of Bank Draft recovered, grouped by state
        public int BDNoRecovered{ get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? RecoveryAmount { get; set; }

        //Amount of Bank Draft issued that has not yet been recovered
        public int BDNoOutstanding { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? OutstandingAmount { get; set; }

    }
}
