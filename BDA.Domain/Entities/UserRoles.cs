using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BDA.Entities
{
    public class UserRoles
    {
        private Guid _id;
        [Key]
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
        
        public string RoleId { get; set; }
        public string UserId { get; set; }
        public Guid? DivisionId { get; set; }
        public Guid? FunctionId { get; set; }
        public Guid? ZoneId { get; set; }
        public Guid? UnitId { get; set; }

    }
}
