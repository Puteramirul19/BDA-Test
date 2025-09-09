using System;
using System.Collections.Generic;
using System.Text;

namespace BDA.ActiveDirectory
{
    public class DirectoryUser
    {
        public string StaffNo { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string OfficeNumber { get; set; }

        public string Photo { get; set; }

        /// <summary>
        /// If user is already registered
        /// </summary>
        public bool IsRegistered { get; set; }
    }
}
