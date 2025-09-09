using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BDA.Entities
{
    public class Division : AuditableEntity
    {
        //public Division()
        //{ 
        //    Id = Guid.NewGuid();
        //}
        public string Code { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Function> Function { get; set; }
        public int LOAType { get; set; }
}
}
