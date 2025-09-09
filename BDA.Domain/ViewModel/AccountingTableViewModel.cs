using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using BDA.Entities;

namespace BDA.ViewModel
{
   
    public class AccountingTableViewModel
    {
        public string Id { get; set; }
        public Guid WangHangusId { get; set; }
        public string drCr { get; set; }
        public string glAccount { get; set; }
        public string conw { get; set; }
        public string cownNo { get; set; }
        public string costObject { get; set; }
        public string taxCode { get; set; }
        public string currency { get; set; }
        public string taxAmount { get; set; }
        public string amount { get; set; }
        //public IEnumerable<AccountingTable> AccountingTables { get; set; }

    }

}
