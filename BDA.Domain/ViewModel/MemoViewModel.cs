using System;
using System.Collections.Generic;
using System.Text;
using BDA.Entities;

namespace BDA.ViewModel
{
   
    public class MemoViewModel
    {
        public string Id { get; set; }
        public string CoverRefNo { get; set; }
        public string ReferenceNo { get; set; }
        public string SendingMethod { get; set; }
        public string IssuedBDReceiverContactNo { get; set; }
        public string Status { get; set; }
        public string ApplicationType { get; set; }
        public string RequestorId { get; set; }
        public string Requestor { get; set; }
        public string RequestorAddress { get; set; }
        public string Approver { get; set; }
        public string ApproverAddress { get; set; }
        public string UP { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string Line4 { get; set; }
        public string Line5 { get; set; }
        public string LineETC { get; set; }
        public string Signiture { get; set; }
        public decimal FinalTotal { get; set; }
        public DateTime? Date { get; set; }
        public string MemoDate { get; set; }
        public string RujukanNo { get; set; }
        public List<String> RefereceNos { get; set; }
        public AttachmentViewModel SignedMemoVM { get; set; }
        public IEnumerable<Memo> Memos { get; set; }
        public IEnumerable<PenerimaViewModel> Penerimas { get; set; }

    }

}
