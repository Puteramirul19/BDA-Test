    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BDA.Entities
{
    public class MonthlyAmount : AuditableEntity
    {
        //Based on Posting Date - Acceptance Date
        public int Month { get; set; }
        public int Year { get; set; }
        public string CoCode { get; set; }
        public decimal? sumAmount{ get; set; }
        public decimal? diffAmount { get; set; }
        public decimal? percentageAmount { get; set; }
        public decimal? sumRecovery { get; set; }
        public decimal? diffAmountRecovery { get; set; }
        public decimal? percentageAmountRecovery { get; set; }

    }
}
