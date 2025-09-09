using System;
using System.Collections.Generic;
using System.Text;
using BDA.Entities;
using Microsoft.AspNetCore.Http;

namespace BDA.ViewModel
{
    public class AttachmentViewModel
    {
        public string Id { get; set; }
        public string FileType { get; set; }
        public string FileSubType { get; set; }
        public Guid ParentId { get; set; }
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public IFormFile File { set; get; }

    }
}
