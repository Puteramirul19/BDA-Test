using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BDA.Entities
{
    public class Zone : AuditableEntity
    {
        
        public string Code { get; set; }
        public string Name { get; set; }
        public Guid FunctionId { get; set; }
        [ForeignKey("FunctionId")]
        public Function Function { get; set; }
        public ICollection<Unit> Unit { get; set; }
    }
}
