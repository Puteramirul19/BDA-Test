using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BDA.Entities
{
    public class AccountingTable : Entity<Guid>
    {
        public AccountingTable()
        { 
            Id = Guid.NewGuid();
        }

        public Guid WangHangusId { get; set; }
        public virtual WangHangus WangHangus { get; set; }
        public string DrCr { get; set; }
        public string GLAccount { get; set; }
        public string CONW { get; set; }
        public string CONWNo { get; set; }
        public string CostObject { get; set; }
        public string TaxCode { get; set; }
        public string Currency { get; set; }
        public string TaxAmount { get; set; }
        public string Amount { get; set; }
      
    }
}
