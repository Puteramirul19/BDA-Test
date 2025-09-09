using System;
using System.Collections.Generic;
using System.Text;

namespace BDA.Entities
{
    public class BankDetails : AuditableEntity
    {
        public string Type { get; set; }
        public string BankName { get; set; }
        public string BankPIC { get; set; }
        public string BankPICPosition { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Street { get; set; }
        public string State { get; set; }
        public string AccountNo1 { get; set; }
        public string AccountNo2 { get; set; }
        public string AccountNo3 { get; set; }
        public string ChargeAccountNo { get; set; }
        public string Email { get; set; }
    }
}
