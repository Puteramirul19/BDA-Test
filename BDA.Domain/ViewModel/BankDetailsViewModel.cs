using System;
using System.Collections.Generic;
using System.Text;
using BDA.Entities;

namespace BDA.ViewModel
{
    public class BankDetailsViewModel
    {

        public string Id { get; set; }
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
        public bool IsActive { get; set; }
        public IEnumerable<BankDetails> BankDetails { get; set; }

    }
}
