using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BDA.Entities
{
    public class Function :  AuditableEntity
    {
        //public Function()
        //{ 
        //    Id = Guid.NewGuid();
        //}

        //public Division DivisionId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Guid DivisionId { get; set; }
        //[ForeignKey("DivisionID")]
        public virtual Division Division { get; set; }
        public ICollection<Zone> Zone { get; set; }
    }
}
