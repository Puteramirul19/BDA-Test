using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using BDA.Entities;

namespace BDA.ViewModel
{
   
    public class ErmsLogViewModel
    {
        public Guid Id { get; set; }
        public Guid BankDraftId { get; set; }
        public string RequestId { get; set; }
        public string RefNo { get; set; }
        public string TransactionId { get; set; }
        public string Date { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Action { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public IEnumerable<ErmsLog> ErmsLogs { get; set; }

    }

}
