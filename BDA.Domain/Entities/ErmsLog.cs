using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDA.Identity;

namespace BDA.Entities
{
    public class ErmsLog
    {
        public Guid Id { get; set; }
        public Guid BankDraftId { get; set; }
        public virtual BankDraft BankDraft { get; set; }
        public string RequestId { get; set; }
        public string TransactionId { get; set; }
        public DateTime Date { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Action { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }

    }

   
}