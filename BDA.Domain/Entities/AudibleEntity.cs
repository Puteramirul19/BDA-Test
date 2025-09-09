using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BDA.Entities
{
    public abstract class AuditableEntity
    {
        private Guid _id;

        [Key]
        [Required]
        [ScaffoldColumn(false)]
        public Guid Id
        {
            get
            {
                return _id;// = Guid.NewGuid();
            }
            set
            {
                _id = value;
            }
        }

        [ScaffoldColumn(false)]
        public DateTime CreatedOn { get; set; }

        [ScaffoldColumn(false)]
        public DateTime UpdatedOn { get; set; }

        public bool isActive { get; set; }

        protected AuditableEntity()
        {
            var now = DateTime.Now;
            CreatedOn  = new DateTime(now.Ticks / 100000 * 100000, now.Kind);
            UpdatedOn = new DateTime(now.Ticks / 100000 * 100000, now.Kind);
            isActive = true;
        }
    }
}
