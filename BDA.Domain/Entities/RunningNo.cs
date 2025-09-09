using System;
using System.Collections.Generic;
using System.Text;

namespace BDA.Entities
{
    public class RunningNo : AuditableEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int RunNo { get; set; }
    }
}
