using System;
using System.Collections.Generic;
using System.Text;

namespace BDA.Data
{
    public class Dictionary
    {
    }

    public enum Status
    {
        Draft,
        Created,
        Submitted,
        Withdrawn,
        RejectedVerify,
        Verified,
        RejectedApprove,
        Approved,
        Declined,
        Accepted,
        SaveProcessed,
        Processed,
        SaveIssued,
        Issued,
        SaveComplete,
        Complete,
        Processing,
        Issuing,
        Received,
        PartialComplete,
        Rejected,
        ToDecline

    }

    public enum VendorType
    {
       OneTime,
       Registered
    }

    public enum AppType
    {
        Application,
        Cancellation,
        Recovery,
        Lost
    }

    public enum RefundType
    {
        Partial,
        Full
    }

    public enum BDType
    {
        WangCagaran,
        WangHangus,
        WangCagaranHangus
    }

    public enum AttachmentType
    {
        BankDraft,
        InstructionLetter,
        Memo,
        Evidence,
        Cancellation,
        Recovery,
        Lost
    }
    public enum BDAttachmentType
    {
        WHDocument,
        WCDocument,
        WCSuratKelulusan,
        WCUMAP,
        Receipt,
        ScannedBankDraft,
        ScannedMemo,
        UMAPinForm,
        ScannedLetter,
        SignedLetter,
        FirstPartialBankStatement,
        SecondPartialBankStatement,
        BankStatement,
        Checklist,
        RecoveryLetter,
        FirstPartial,
        SecondPartial,
        FullPartial,
        PoliceReport,
        PBTDoc,
        IndemningForm,
        OthersDoc
    }

    public enum SendingMethod
    {
        SelfCollect,
        ByPos

    }

    public enum ActionType
    {
        Draft,
        Created,
        Submitted,
        Withdrawn,
        Rejected,
        Verified,
        Approved,
        Declined,
        Accepted,
        SubmittedToBank,
        BankDraftIssued,
        PartialComplete,
        Complete,
        Received,
        Processed,
        Posted,
        PostedFail,
        Update
    }

    public enum ActionRole
    {
        Requester,
        Verifier,
        Approver,
        TGBSBanking,
        TGBSReconciliation,
        ERMS,
        System
    }

    public enum ProcessingType
    {
        Normal,
        Bulk
    }

    public class ERMSField
    {
        private ERMSField(string value) { Value = value; }
        public string Value { get; private set; }

        //header 
        public static string DocumentDate { get { return "Document Date"; } }
        public static string CompanyCode { get { return "Company Code"; } }
        public static string PostingDate { get { return "Posting Date"; } }
        public static string DocumentType { get { return "Document Type"; } }
        public static string Reference { get { return "Reference"; } }
        public static string HeaderText { get { return "Header Text"; } }

        //vendor data
        public static string VendorNo { get { return "Vendor No"; } }
        public static string State { get { return "Reference Key 1"; } }
        public static string Requester { get { return "Reference Key 3"; } }
        public static string BusinessArea { get { return "Business Area"; } }
        public static string Currency { get { return "Currency"; } }
        public static string Amount { get { return "Amount"; } }
        public static string Assignment { get { return "Assignment"; } }
        public static string Text { get { return "Text"; } }
        public static string LongText { get { return "Long Text"; } }
        public static string PaymentMethod  { get { return "Payment Method"; } }

        //gl account data
        public static string GLAccount { get { return "GL Account"; } }
        public static string TaxCode { get { return "Tax Code"; } }
        //public static string TaxAmount { get { return "Tax Amount"; } }
        public static string CostCenter { get { return "Cost Center"; } }
        public static string Order { get { return "Order"; } }
        public static string OperationNo { get { return "Operation No"; } }
        public static string Network { get { return "Network"; } }
        public static string Activity { get { return "Activity"; } }
        public static string WBSElement { get { return "WBS Element"; } }

        //gl account
        public static string WangCagaranGL { get { return "73890"; } } //73890
        public static string DefaultWangCagaranVendoNo { get { return "4004506"; } }//4004506
        public static string DefaultDocumentType { get { return "KR"; } }
        public static string DefaultTaxCode { get { return "PZ"; } }
        public static string DefaultCurrency { get { return "MYR"; } }
        public static string HeaderTextWC { get { return "BAY WANG CAGARAN"; } }
        public static string HeaderTextWH { get { return "BAY WANG HANGUS"; } }
        public static string HeaderTextWCH { get { return "BAY WANG CAGARAN"; } }
        public static string DefaultPaymentMethod { get { return "T"; } } //Payment Method
    }


}
