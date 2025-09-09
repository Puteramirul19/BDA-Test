using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BDA.Entities
{
    public class EmailQueues : Entity<Guid>
    {
        public EmailQueues()
        { 
            Id = Guid.NewGuid();
        }
       
        public string Subject { get; set; }
        public string ToAddress { get; set; }
        public string ToName { get; set; }
        public string Content { get; set; }
        public string Attachments { get; set; }
        public DateTime ScheduledOn { get; set; }
        public DateTime ProcessedOn { get; set; }
        public DateTime SentOn { get; set; }
        public DateTime ObjectId { get; set; }
    }

}
