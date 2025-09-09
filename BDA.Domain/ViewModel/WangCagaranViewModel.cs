using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using BDA.Entities;

namespace BDA.ViewModel
{
   
    public class WangCagaranViewModel
    {
        public string Id { get; set; }
        public Guid BankDraftId { get; set; }
        public string ErmsDocNo { get; set; }
        public string Pemula { get; set; }
        public DateTime? Tarikh { get; set; }
        public string Alamat1 { get; set; }
        public string Alamat2 { get; set; }
        public string Bandar { get; set; }
        public string Poskod { get; set; }
        public string Negeri { get; set; }
        public string KeteranganKerja { get; set; }
        public bool JKRInvolved { get; set; }
        public string UserRole { get; set; }
        public string JKRType { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Jumlah { get; set; }
        public string CajKod { get; set; }
        public string NamaPemegangCagaran { get; set; }
        public string WBSProjekNo { get; set; }
        public string GL { get; set; }
        public string CoCode { get; set; }
        public string CostCenter { get; set; }
        public string Assignment { get; set; }
        public string BusinessArea { get; set; }
        public string PK { get; set; }
        public string Type { get; set; }
        public DateTime? PostingDate { get; set; }
        public IEnumerable<WangCagaran> WangCagarans { get; set; }

    }

}
