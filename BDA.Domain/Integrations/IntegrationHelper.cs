using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BDA.Integrations
{
    public class IntegrationHelper
    {
        // Constants
        public const string PROVIDER_ID = "BDAS";
        public const string CONSUMER_ID = "BDAS";
        public const string SUCCESS = "SUCC";
        public const string FAIL = "FAIL";
        public const string LOGGER_NAME = "Integration";

        public static string SOURCE_ERMS = "ERMS";
        public static string PostingAction= "VendorInvoiceCreate";
        public static string TransactionStatusAction = "TransactionStatus";
        public static string SystemErrorMessage = "Error occured.Check in logs description.";

        public static string Trim(string str)
        {
            if (String.IsNullOrWhiteSpace(str)) return "";
            return str.Trim();
        }

        // Methods
        public static string GenerateRequestOrResponseId()
        {
            return Guid.NewGuid().ToString("n"); // GUID without dash
        }

        private static Random random = new Random();

        public static string GenerateTransactionId()
        {
            // We try our best to get a unique number here. Doa la banyak2 takde clash.
            // Sample number in example: 187268577731786
            //return DateTime.Now.ToString("yyMMddhhmmss") + random.Next(1, 999).ToString("000");

            return DateTime.Now.ToString("yyMMddhhmmss") + random.Next(1, 999).ToString("000");
        }
    }
}