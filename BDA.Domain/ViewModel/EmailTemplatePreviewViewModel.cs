using System;
using System.Collections.Generic;
using System.Text;

namespace BDA.ViewModel
{

    public class EmailTemplatePreviewViewModel
    {
        public string SubjectTemplate { get; set; }
        public string ContentTemplate { get; set; }

        public string Subject { get; set; }
        public string Content { get; set; }
    }
}
