using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BDA.Entities
{
    public class Unit : AuditableEntity
    {

        public string Code { get; set; }
        public string Name { get; set; }
        public Guid ZoneId { get; set; }
        [ForeignKey("ZoneId")]
        public Zone Zone { get; set; }
    }
}
