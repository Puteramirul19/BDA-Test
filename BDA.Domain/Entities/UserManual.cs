using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BDA.Entities
{
    public class UserManual
    {
        public Guid Id { get; set; }
        public string Name{ get; set; }
        public string FileName { get; set; }
        public int Sequence { get; set; }
        public string RoleAccess{ get; set; }
    }
}
