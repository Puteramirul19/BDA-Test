using System;
using System.Collections.Generic;
using System.Text;
using BDA.Identity;

namespace BDA.Entities
{
    public class PasswordHistory
    {
        public string Id { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string Password { get; set; }
        public DateTime? PasswordCreateDate { get; set; }
        public string Password2 { get; set; }
        public DateTime? Password2CreateDate { get; set; }
        public string Password3 { get; set; }
        public DateTime? Password3CreateDate { get; set; }
        public string Password4 { get; set; }
        public DateTime? Password4CreateDate { get; set; }
        public string Password5 { get; set; }
        public DateTime? Password5CreateDate { get; set; }

    }
}
