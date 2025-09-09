using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using BDA.Entities;

namespace BDA.ViewModel
{
   
    public class WangHangusViewModel
    {
        public string Id { get; set; }
        public Guid BankDraftId { get; set; }
        public string VendorType { get; set; }

        [Required]
        public string Name { get; set; }
        public string ICNo { get; set; }
        public string Email { get; set; }
        public string SSTRegNo { get; set; }
        public string BusRegNo { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Postcode { get; set; }
        //public string Negeri { get; set; }
        public string Country { get; set; }
        public string PONumber { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime Date { get; set; }
        public string CoCode { get; set; }
        public string BusinessArea { get; set; }
        public string VendorNo { get; set; }
        public string VendorName { get; set; }
        public string BankAccount { get; set; }
        public string BankCountry { get; set; }
        public string Description { get; set; }
        public string Accounts { get; set; }
        public string ErmsDocNo { get; set; }
        public decimal? Amount { get; set; }
        public decimal? TaxAmount { get; set; }
        public DateTime? PostingDate { get; set; }
        public IEnumerable<WangHangus> WangHangus { get; set; }
        public AccountingTableViewModel AccountingTableViewModel { get; set; }
        public IEnumerable<AccountingTableViewModel> AccountingTableViewModels { get; set; }
    }

}
