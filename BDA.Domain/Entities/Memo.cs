using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace BDA.Entities
{
    public class Memo : AuditableEntity
    {
        public string CoverRefNo { get; set; }
        public string ReferenceNo { get; set; }
        public string SendingMethod { get; set; }
        public string IssuedBDReceiverContactNo { get; set; }
        public string Status { get; set; }
        public string ApplicationType { get; set; }
        public string RequestorAddress { get; set; }
        public string Approver { get; set; }
        public string ApproverAddress { get; set; }
        public string UP { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string Line4 { get; set; }
        public string Line5 { get; set; }
        public string LineETC { get; set; }
        public string Signiture { get; set; }
        public DateTime? Date { get; set; }
        public string RujukanNo { get; set; }
        //public virtual ICollection<MemoAttachment> Attachments { get; set; }     

        public IDictionary<string, string> GetMessageValues()
        {
            return new Dictionary<string, string>
            {
                
            };
        }

    }

    //public class MemoAttachment : Entity<Guid>
    //{
    //    public MemoAttachment()
    //    {
    //        Attachment = new Attachment();
    //    }

    //    public Guid MemoId { get; set; }
    //    [JsonIgnore] public virtual Memo Memo { get; set; }
    //    public string Type { get; set; }
    //    public Attachment Attachment { get; set; }
    //}
}
