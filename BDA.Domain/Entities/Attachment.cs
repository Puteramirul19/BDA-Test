using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BDA.Entities
{
        public class Attachment : Entity<Guid>
       {
            public Attachment()
            {
                Id = Guid.NewGuid(); 
            }

            public string FileType { get; set; }
            public string FileSubType { get; set; }
            public Guid ParentId { get; set; }
            public string FileId { get; set; }
            public string FileName { get; set; }
            public string FileExtension { get; set; }
            public string Title { get; set; }
    }

    //public enum AttachmentType
    //{
    //    BankDraft,
    //    InstructionLetter,
    //    Memo
    //}

    //public enum AttachmentSubType
    //{
    //    WCSuratKelulusan,
    //    WCUMAP,
    //    Receipt
    //}
}
