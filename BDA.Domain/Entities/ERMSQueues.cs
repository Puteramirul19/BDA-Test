using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BDA.Entities
{
    public class ERMSQueues: Entity<Guid>
    {
        public ERMSQueues()
        { 
            Id = Guid.NewGuid();
        }
        public virtual BankDraft BankDrafId { get; set; }
        public string RefNo { get; set; }
        public string Function { get; set; }
        public string Status { get; set; }
        public DateTime ActionDate{ get; set; }

    }
}
