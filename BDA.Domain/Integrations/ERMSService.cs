using System;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using BDA.Data;
using BDA.Entities;
using BDA.SI_VendorInvoiceCreate_Async_Out_Service;
using Microsoft.Extensions.Configuration;
using NLog;

namespace BDA.Integrations
{
    public interface IErmsService
    {
        PostingBankDraftResult PostingWangCagaran(BankDraft bd);
        PostingBankDraftResult PostingWangHangus(BankDraft bd);
        PostingBankDraftResult PostingWangCagaranHangus(BankDraft bd);
    }

    public class ErmsService : IErmsService
    {
        private static Logger logger = LogManager.GetLogger(IntegrationHelper.LOGGER_NAME);
        private BdaDBContext db;
        private EndpointAddress returnRefNoEndpoint;
        private BasicHttpBinding binding;
        private string username;
        private string password;
        IConfiguration configuration;

        public ErmsService(BdaDBContext _db, IConfiguration _configuration)
        {
            this.db = _db;
            this.configuration = _configuration;
            
            //production
            // string returnRefNoUrl = "http://erpsoa.tnb.com.my:8000/XISOAPAdapter/MessageServlet?senderParty=&senderService=BC_BDA&receiverParty=&receiverService=&interface=SI_VendorInvoiceCreate_Async_Out&interfaceNamespace=urn:tnb.com.my:BDA:po:FM:VendorInvoiceCreate:00";
            // this.username = "POP_BDA";
            // this.password = "1Qazxsdcfre!";

            //staging
            //string returnRefNoUrl = "http://erpsoapoq.tnb.com.my:8080/XISOAPAdapter/MessageServlet?senderParty=&senderService=BC_BDA&receiverParty=&receiverService=&interface=SI_VendorInvoiceCreate_Async_Out&interfaceNamespace=urn:tnb.com.my:BDA:po:FM:VendorInvoiceCreate:00";
            //this.username = "POQ_BDA";
            //this.password = "BDA@poq#2021";

            //development
            // string returnRefNoUrl = "http://erpsoapod.tnb.com.my:8000/XISOAPAdapter/MessageServlet?senderParty=&senderService=BC_BDA&receiverParty=&receiverService=&interface=SI_VendorInvoiceCreate_Async_Out&interfaceNamespace=urn:tnb.com.my:BDA:po:FM:VendorInvoiceCreate:00";
            // this.username = "POD_BDA";
            // this.password = "BDA@pod#2021";
            
            // Updated to use from appsettings.json
            if (configuration != null)
            {
                var ermsSettings = configuration.GetSection("ERMSSetting");
                string returnRefNoUrl = ermsSettings["returnRefNoUrl"];
                this.username = ermsSettings["username"];
                this.password = ermsSettings["password"];
                this.returnRefNoEndpoint = new EndpointAddress(returnRefNoUrl);
            }

            this.binding = new BasicHttpBinding();
            this.binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            this.binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
        }

        public PostingBankDraftResult PostingWangCagaran(BankDraft bd)
        {
            var requesterName = db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefault();

            var wang = db.WangCagaran.Where(x => x.BankDraftId == bd.Id).FirstOrDefault();

            var defaultBA = wang.BusinessArea;

            //var coCode = db.Division.Where(x => x.Name == wang.CoCode).Select(x => x.Code).FirstOrDefault();
            if (wang.CoCode == "6001")
            {
                defaultBA = "6000"; //for all modules for 1st line
            }

            //can send 12 characters only for Reference Key 1
            if (wang.Negeri == "Negeri Sembilan")
            {
                wang.Negeri = "N. Sembilan"; 
            }

            string longtext = "";
            if (bd.NameOnBD.Length > 50)
            {
                longtext = bd.NameOnBD.Remove(0, 50);
            }

            var client = new SI_VendorInvoiceCreate_Async_OutClient(binding, returnRefNoEndpoint);   

            try
            {
                if (!String.IsNullOrEmpty(this.username))
                {
                    client.ClientCredentials.UserName.UserName = username;
                    client.ClientCredentials.UserName.Password = password;
                }

                var request = new DT_VendorInvoiceCreate
                {
                    RequestHeader = new DT_RequestHeader
                    {
                        RequestId = IntegrationHelper.GenerateRequestOrResponseId(), // 32 guid
                        ReqTransactionId = IntegrationHelper.GenerateTransactionId(), // YYMMDDHHMMSSXXX
                        ConsumerId = IntegrationHelper.CONSUMER_ID, //BDAS 
                        ReqTimestamp = DateTime.Now.ToString(), //YYYY-MM-DD HH:M:SS
                    },
                    VendorInvoiceCreateRequest = new DT_VendorInvoiceCreateVendorInvoiceCreateRequest
                    {
                        DocumentHeader = new[] {
                            new DT_VendorInvoiceCreateVendorInvoiceCreateRequestItem
                        { // Document Date DD.MM.YYYY
                            AttributeName = ERMSField.DocumentDate,
                            AttributeValue = bd.ApprovedOn.Value.ToString("dd.MM.yyyy"),
                            //AttributeValue = wang.Tarikh.Value.ToString("dd.MM.yyyy"),
                        },
                           new DT_VendorInvoiceCreateVendorInvoiceCreateRequestItem
                        { // Company Code - 4 chars
                            AttributeName = ERMSField.CompanyCode,
                            AttributeValue = wang.CoCode,
                        },
                           new DT_VendorInvoiceCreateVendorInvoiceCreateRequestItem
                        { // Reference - 16 chars - BDA Reference number
                            AttributeName = ERMSField.Reference,
                            AttributeValue = bd.RefNo,
                        },
                             new DT_VendorInvoiceCreateVendorInvoiceCreateRequestItem
                        { // Document Type - 4 chars
                            AttributeName = ERMSField.DocumentType,
                            AttributeValue = ERMSField.DefaultDocumentType,
                        },
                             new DT_VendorInvoiceCreateVendorInvoiceCreateRequestItem
                        { // Document Type - 4 chars
                            AttributeName = ERMSField.PostingDate,
                            AttributeValue = DateTime.Now.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                        },
                                 new DT_VendorInvoiceCreateVendorInvoiceCreateRequestItem
                        { // Document Type - 4 chars
                            AttributeName = ERMSField.HeaderText,
                            AttributeValue = ERMSField.HeaderTextWC,
                        },
                       },
                        VendorItems = new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem[][] {
                            new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem[]
                            {
                                  new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // Vendor No
                                    AttributeName = ERMSField.VendorNo,
                                    AttributeValue = ERMSField.DefaultWangCagaranVendoNo,
                                },
                                  new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // State
                                    AttributeName = ERMSField.State,
                                    AttributeValue = wang.Negeri,
                                },
                                   new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // Requester/Initiator
                                    AttributeName = ERMSField.Requester,
                                    AttributeValue = requesterName,
                                },
                                    new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // Business Area
                                    AttributeName = ERMSField.BusinessArea,
                                    AttributeValue = defaultBA,
                                },
                                    new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.Currency,
                                    AttributeValue = ERMSField.DefaultCurrency,
                                },
                                   new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // Amount
                                    AttributeName = ERMSField.Amount,
                                    AttributeValue = wang.Jumlah.ToString(),
                                },
                                     new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // Assignment - send Project No for WC
                                    AttributeName = ERMSField.Assignment,
                                    AttributeValue = wang.WBSProjekNo,
                                },
                                     new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // Assignment
                                    AttributeName = ERMSField.Text,
                                    AttributeValue = bd.NameOnBD,
                                },
                                     new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // Assignment
                                    AttributeName = ERMSField.LongText,
                                    AttributeValue = longtext,
                                },
                                     new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // Payment Method
                                    AttributeName = ERMSField.PaymentMethod,
                                    AttributeValue = ERMSField.DefaultPaymentMethod,
                                },

                            }

                        },
                        GLItems = new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem[][] {
                            new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem[]
                            {
                                     new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // GL Account
                                    AttributeName = ERMSField.Text,
                                    AttributeValue = bd.NameOnBD,
                                },
                                    new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // GL Account
                                    AttributeName = ERMSField.LongText,
                                    AttributeValue = longtext,
                                },
                                  new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // GL Account - WC - default to 73890
                                    AttributeName = ERMSField.GLAccount,
                                    AttributeValue = ERMSField.WangCagaranGL,
                                },
                                       new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.Currency,
                                    AttributeValue = ERMSField.DefaultCurrency,
                                },
                                    new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.Amount,
                                    AttributeValue = wang.Jumlah.ToString(),
                                },
                                     new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.TaxCode,
                                    AttributeValue = "", //ERMSField.DefaultTaxCode,
                                },
                                      new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.BusinessArea,
                                    AttributeValue = wang.BusinessArea,
                                },
                                       new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Assignment
                                    AttributeName = ERMSField.Assignment,
                                    AttributeValue = wang.WBSProjekNo,
                                },

                            }

                        },
                    }
                    
                };

                var response = client.SI_VendorInvoiceCreate_Async_OutAsync(request);
                var transactionStatus = "";
                var msg = "";
                try
                {
                    response.Wait(); // Blocks current thread until GetFooAsync task completes
                }
                catch (Exception e)
                {
                    if (e.InnerException != null)
                    {
                        //erms will send empty soap after posting -> exclude this exception
                        var emptySoapMessage = e.InnerException.Message;
                        if (emptySoapMessage.Contains("Server Error"))
                        {
                            transactionStatus = "FAIL";
                            msg = e.InnerException.Message;
                        }
                        else if (emptySoapMessage.Contains("HTTP request is unauthorized"))
                        {
                            transactionStatus = "FAIL";
                            msg = e.InnerException.Message;
                        }
                        else if (emptySoapMessage.Contains("The one-way operation"))
                        {
                            transactionStatus = "SUCCESS";
                        }
                        else
                        {
                            msg = e.InnerException.Message;

                            db.ErmsLog.Add(new ErmsLog
                            {
                                Id = new Guid(),
                                BankDraftId = bd.Id,
                                RequestId = request.RequestHeader.RequestId,
                                TransactionId = request.RequestHeader.ReqTransactionId,
                                Date = DateTime.Now,
                                Source = request.RequestHeader.ConsumerId,
                                Destination = IntegrationHelper.SOURCE_ERMS,
                                Action = IntegrationHelper.PostingAction,
                                Message = IntegrationHelper.SystemErrorMessage,
                                Status = transactionStatus,
                                Description = msg,
                            });
                            db.SaveChanges();
          
                        }
                    }
                 
                  
                }
                //logger.Trace("[UpdateNoticeRefNo] Response: " + response.Dump());

                var result = new PostingBankDraftResult
                {
                    IsSuccess = false,
                    Message = "Trying to posting wang cagaran to ERMS but failed. See description in logs for more detail.",
                    IntegrationId = request.RequestHeader.ReqTransactionId,
                };

                if (transactionStatus == "SUCCESS")
                {
                    transactionStatus = "SUCCESS";

                    result = new PostingBankDraftResult
                    {
                        IsSuccess = true,
                        Message = "Successfully send wang cagaran for posting to ERMS.",
                        IntegrationId = request.RequestHeader.ReqTransactionId,
                    };
                }
                else
                {
                    transactionStatus =  "FAIL";

                    result = new PostingBankDraftResult
                    {
                        IsSuccess = false,
                        Message = "Trying to posting wang cagaran to ERMS but failed. See description in logs for more detail.",
                    };
                }

                db.ErmsLog.Add(new ErmsLog
                {
                    Id = new Guid(),
                    BankDraftId = bd.Id,
                    RequestId = request.RequestHeader.RequestId,
                    TransactionId = request.RequestHeader.ReqTransactionId,
                    Date = DateTime.Now,
                    Source = request.RequestHeader.ConsumerId,
                    Destination = IntegrationHelper.SOURCE_ERMS,
                    Action = IntegrationHelper.PostingAction,
                    Message = result.Message,
                    Status = transactionStatus,
                    Description = msg,
                });
                db.SaveChanges();

                return result;

            }
            finally
            {
                if (client != null) client.Close();
            }
        }

        public PostingBankDraftResult PostingWangHangus(BankDraft bd)
        {
            var requesterName = db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefault();

            var wang = db.WangHangus.Where(x => x.BankDraftId == bd.Id).FirstOrDefault();

            var defaultBA = wang.BusinessArea;

            //var coCode = db.Division.Where(x => x.Name == wang.CoCode).Select(x => x.Code).FirstOrDefault();
            if (wang.CoCode == "6001")
            {
                defaultBA = "6000"; //for all modules for 1st line
            }

            //can send 12 characters only for Reference Key 1
            if (wang.Region == "Negeri Sembilan")
            {
                wang.Region = "N. Sembilan";
            }

            string longtext = "";
            if (wang.Description.Length > 50)
            {
                longtext = wang.Description.Remove(0, 50);
            }

            var accTable = db.AccountingTable.Where(x => x.WangHangusId == wang.Id).FirstOrDefault();

            var accTableList = db.AccountingTable.Where(x => x.WangHangusId == wang.Id).ToList();

            DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem[][] glItems = new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem[accTableList.Count][];

            
            int i = 0;
            foreach (var acc in accTableList)
            {
                if(acc.CONW == "C")
                {
                    glItems[i] = new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem[]
                        {
                                 new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // GL Account
                                    AttributeName = ERMSField.Text,
                                    AttributeValue = wang.Description,
                                },
                                  new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // GL Account
                                    AttributeName = ERMSField.LongText,
                                    AttributeValue = longtext,
                                },
                                  new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // GL Account - WC - default to 73890
                                    AttributeName = ERMSField.GLAccount,
                                    AttributeValue = acc.GLAccount,
                                },
                                       new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.Currency,
                                    AttributeValue = acc.Currency,
                                },
                                    new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.Amount,
                                    AttributeValue = acc.Amount.ToString(),
                                },
                                     new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.TaxCode,
                                    AttributeValue = acc.TaxCode,
                                },
                                      new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.BusinessArea,
                                    AttributeValue = wang.BusinessArea,
                                },
                                       new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Assignment
                                    AttributeName = ERMSField.CostCenter,
                                    AttributeValue = acc.CostObject,
                                },
                                      new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Assignment
                                    AttributeName = ERMSField.Assignment,
                                    AttributeValue = wang.InvoiceNumber,
                                },
                        };
                }
                else if (acc.CONW == "O")
                {
                        glItems[i] = new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem[]
                            {
                                 new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // GL Account
                                    AttributeName = ERMSField.Text,
                                    AttributeValue = wang.Description,
                                },
                                   new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // GL Account
                                    AttributeName = ERMSField.LongText,
                                    AttributeValue = longtext,
                                },
                                  new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // GL Account - WC - default to 73890
                                    AttributeName = ERMSField.GLAccount,
                                    AttributeValue = acc.GLAccount,
                                },
                                     new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.Currency,
                                    AttributeValue = acc.Currency,
                                },
                                    new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.Amount,
                                    AttributeValue = acc.Amount.ToString(),
                                },
                                     new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.TaxCode,
                                    AttributeValue = acc.TaxCode,
                                },
                                      new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.BusinessArea,
                                    AttributeValue = wang.BusinessArea,
                                },
                                       new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Assignment
                                    AttributeName = ERMSField.Order,
                                    AttributeValue = acc.CostObject,
                                },
                                        new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Assignment
                                    AttributeName = ERMSField.Assignment,
                                    AttributeValue = wang.InvoiceNumber,
                                },       
                            };
                    }
                else if (acc.CONW == "N")
                {
                    string network = acc.CostObject;
                    string activity = "";

                    if (acc.CostObject.Contains("/"))
                    {
                        network = acc.CostObject.Substring(0, acc.CostObject.IndexOf("/"));
                        activity = acc.CostObject.Substring(acc.CostObject.IndexOf("/") + 1);
                    }

                    glItems[i] = new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem[]
                        {
                                 new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // GL Account
                                    AttributeName = ERMSField.Text,
                                    AttributeValue = wang.Description,
                                },
                                   new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // GL Account
                                    AttributeName = ERMSField.LongText,
                                    AttributeValue = longtext,
                                },
                                  new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // GL Account - WC - default to 73890
                                    AttributeName = ERMSField.GLAccount,
                                    AttributeValue = acc.GLAccount,
                                },
                                     new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.Currency,
                                    AttributeValue = acc.Currency,
                                },
                                    new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.Amount,
                                    AttributeValue = acc.Amount.ToString(),
                                },
                                     new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.TaxCode,
                                    AttributeValue = acc.TaxCode,
                                },
                                      new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.BusinessArea,
                                    AttributeValue = wang.BusinessArea,
                                },
                                       new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Assignment
                                    AttributeName = ERMSField.Network,
                                    AttributeValue = network,
                                },
                                       new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Assignment
                                    AttributeName = ERMSField.Activity,
                                    AttributeValue = activity,
                                },
                                        new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Assignment
                                    AttributeName = ERMSField.Assignment,
                                    AttributeValue = wang.InvoiceNumber,
                                },
                                       
                        };
                }
                else if (acc.CONW == "W")
                {
                    glItems[i] = new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem[]
                        {
                                 new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // GL Account
                                    AttributeName = ERMSField.Text,
                                    AttributeValue = wang.Description,
                                },
                                   new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // GL Account
                                    AttributeName = ERMSField.LongText,
                                    AttributeValue = longtext,
                                },
                                  new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // GL Account - WC - default to 73890
                                    AttributeName = ERMSField.GLAccount,
                                    AttributeValue = acc.GLAccount,
                                },
                                   new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.Currency,
                                    AttributeValue = acc.Currency,
                                },
                                    new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.Amount,
                                    AttributeValue = acc.Amount.ToString(),
                                },
                                     new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.TaxCode,
                                    AttributeValue = acc.TaxCode,
                                },
                                      new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.BusinessArea,
                                    AttributeValue = wang.BusinessArea,
                                },
                                       new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Assignment
                                    AttributeName = ERMSField.WBSElement,
                                    AttributeValue = acc.CostObject,
                                },
                                        new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Assignment
                                    AttributeName = ERMSField.Assignment,
                                     AttributeValue = wang.InvoiceNumber,
                                },
                                    
                        };
                }


                i++;
            }

            var client = new SI_VendorInvoiceCreate_Async_OutClient(binding, returnRefNoEndpoint);

            try
            {
                if (!String.IsNullOrEmpty(this.username))
                {
                    client.ClientCredentials.UserName.UserName = username;
                    client.ClientCredentials.UserName.Password = password;
                }

                var request = new DT_VendorInvoiceCreate
                {
                    RequestHeader = new DT_RequestHeader
                    {
                        RequestId = IntegrationHelper.GenerateRequestOrResponseId(), // 32 guid
                        ReqTransactionId = IntegrationHelper.GenerateTransactionId(), // YYMMDDHHMMSSXXX
                        ConsumerId = IntegrationHelper.CONSUMER_ID, //BDAS 
                        ReqTimestamp = DateTime.Now.ToString(), //YYYY-MM-DD HH:M:SS
                    },
                    VendorInvoiceCreateRequest = new DT_VendorInvoiceCreateVendorInvoiceCreateRequest
                    {
                        DocumentHeader = new[] {
                            new DT_VendorInvoiceCreateVendorInvoiceCreateRequestItem
                        { // Document Date DD.MM.YYYY
                            AttributeName = ERMSField.DocumentDate,
                            AttributeValue = bd.ApprovedOn.Value.ToString("dd.MM.yyyy"),
                        },
                           new DT_VendorInvoiceCreateVendorInvoiceCreateRequestItem
                        { // Company Code - 4 chars
                            AttributeName = ERMSField.CompanyCode,
                            AttributeValue = wang.CoCode,
                        },
                           new DT_VendorInvoiceCreateVendorInvoiceCreateRequestItem
                        { // Reference - 16 chars - BDA Reference number
                            AttributeName = ERMSField.Reference,
                            AttributeValue = bd.RefNo,
                        },
                             new DT_VendorInvoiceCreateVendorInvoiceCreateRequestItem
                        { // Document Type - 4 chars
                            AttributeName = ERMSField.DocumentType,
                            AttributeValue = ERMSField.DefaultDocumentType,
                        },
                             new DT_VendorInvoiceCreateVendorInvoiceCreateRequestItem
                        { // Document Type - 4 chars
                            AttributeName = ERMSField.PostingDate,
                            AttributeValue = DateTime.Now.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                        },
                                new DT_VendorInvoiceCreateVendorInvoiceCreateRequestItem
                        { // Document Type - 4 chars
                            AttributeName = ERMSField.HeaderText,
                            AttributeValue = ERMSField.HeaderTextWH,
                        },
                       },
                        VendorItems = new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem[][] {
                            new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem[]
                            {
                                  new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // Vendor No
                                    AttributeName = ERMSField.VendorNo,
                                    AttributeValue = wang.VendorNo,
                                },
                                  new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // State
                                    AttributeName = ERMSField.State,
                                    AttributeValue = wang.Region,
                                },
                                   new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // Requester/Initiator
                                    AttributeName = ERMSField.Requester,
                                    AttributeValue = requesterName,
                                },
                                    new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // Business Area
                                    AttributeName = ERMSField.BusinessArea,
                                    AttributeValue = defaultBA,
                                },
                                    new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.Currency,
                                    AttributeValue = ERMSField.DefaultCurrency,
                                },
                                   new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // Amount
                                    AttributeName = ERMSField.Amount,
                                    AttributeValue = wang.Amount.ToString(),
                                },
                                     new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // Assignment
                                    AttributeName = ERMSField.Text,
                                    AttributeValue = wang.Description,
                                },
                                    new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // Assignment
                                    AttributeName = ERMSField.LongText,
                                    AttributeValue = longtext,
                                },
                                      new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // Assignment - send Project No for WC
                                    AttributeName = ERMSField.Assignment,
                                    AttributeValue = wang.InvoiceNumber,
                                },
                                       new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // Payment Method
                                    AttributeName = ERMSField.PaymentMethod,
                                    AttributeValue = ERMSField.DefaultPaymentMethod,
                                },
                            }

                        },
                        GLItems = glItems,
                    }

                };

                var response = client.SI_VendorInvoiceCreate_Async_OutAsync(request);
                var transactionStatus = "";
                var msg = "";
                try
                {
                    response.Wait(); // Blocks current thread until GetFooAsync task completes
                }
                catch (Exception e)
                {
                    if (e.InnerException != null)
                    {
                        //erms will send empty soap after posting -> exclude this exception
                        var emptySoapMessage = e.InnerException.Message;
                         if (emptySoapMessage.Contains("Server Error"))
                        {
                            transactionStatus = "FAIL";
                            msg = e.InnerException.Message;
                        }
                        else if (emptySoapMessage.Contains("HTTP request is unauthorized"))
                        {
                            transactionStatus = "FAIL";
                            msg = e.InnerException.Message;
                        }
                        else if (emptySoapMessage.Contains("The one-way operation"))
                        {
                            transactionStatus = "SUCCESS";
                        }
                        else
                        {
                            msg = e.InnerException.Message;

                            db.ErmsLog.Add(new ErmsLog
                            {
                                Id = new Guid(),
                                BankDraftId = bd.Id,
                                RequestId = request.RequestHeader.RequestId,
                                TransactionId = request.RequestHeader.ReqTransactionId,
                                Date = DateTime.Now,
                                Source = request.RequestHeader.ConsumerId,
                                Destination = IntegrationHelper.SOURCE_ERMS,
                                Action = IntegrationHelper.PostingAction,
                                Message = IntegrationHelper.SystemErrorMessage,
                                Status = transactionStatus,
                                Description = msg,
                            });
                            db.SaveChanges();

                        }
                    }

                }
                //logger.Trace("[UpdateNoticeRefNo] Response: " + response.Dump());

                var result = new PostingBankDraftResult
                {
                    IsSuccess = false,
                    Message = "Trying to posting wang hangus to ERMS but failed. See description in logs for more detail.",
                    IntegrationId = request.RequestHeader.ReqTransactionId,
                };

                if (transactionStatus == "SUCCESS")
                {
                    transactionStatus = "SUCCESS";

                    result = new PostingBankDraftResult
                    {
                        IsSuccess = true,
                        Message = "Successfully send wang hangus for posting to ERMS.",
                        IntegrationId = request.RequestHeader.ReqTransactionId,
                    };
                }
                else
                {
                    transactionStatus = "FAIL";

                    result = new PostingBankDraftResult
                    {
                        IsSuccess = false,
                        Message = "Trying to posting wang hangus to ERMS but failed. See description in logs for more detail.",
                    };
                }

                try
                {
                    db.ErmsLog.Add(new ErmsLog
                    {
                        Id = new Guid(),
                        BankDraftId = bd.Id,
                        RequestId = request.RequestHeader.RequestId,
                        TransactionId = request.RequestHeader.ReqTransactionId,
                        Date = DateTime.Now,
                        Source = request.RequestHeader.ConsumerId,
                        Destination = IntegrationHelper.SOURCE_ERMS,
                        Action = IntegrationHelper.PostingAction,
                        Message = result.Message,
                        Status = transactionStatus,
                        Description = msg,
                    });
                    db.SaveChanges();
                }
                catch(Exception e)
                {
                    var emptySoapMessage = e.InnerException.Message;
                }
                

                return result;

            }
            finally
            {
                if (client != null) client.Close();
            }
        }

        public PostingBankDraftResult PostingWangCagaranHangus(BankDraft bd)
        {
            var requesterName = db.Users.Where(x => x.UserName == bd.RequesterId).Select(x => x.FullName).FirstOrDefault();

            var wang = db.WangCagaranHangus.Where(x => x.BankDraftId == bd.Id).FirstOrDefault();

            var defaultBA = wang.BusinessArea;

            //var coCode = db.Division.Where(x => x.Name == wang.CoCode).Select(x => x.Code).FirstOrDefault();
            if (wang.CoCode == "6001")
            {
                defaultBA = "6000"; //for all modules for 1st line
            }

            //can send 12 characters only for Reference Key 1
            if (wang.Negeri == "Negeri Sembilan")
            {
                wang.Negeri = "N. Sembilan";
            }

            string longtext = "";
            if (bd.NameOnBD.Length > 50)
            {
                longtext = bd.NameOnBD.Remove(0, 50);
            }

            var client = new SI_VendorInvoiceCreate_Async_OutClient(binding, returnRefNoEndpoint);

            try
            {
                if (!String.IsNullOrEmpty(this.username))
                {
                    client.ClientCredentials.UserName.UserName = username;
                    client.ClientCredentials.UserName.Password = password;
                }

                var request = new DT_VendorInvoiceCreate
                {
                    RequestHeader = new DT_RequestHeader
                    {
                        RequestId = IntegrationHelper.GenerateRequestOrResponseId(), // 32 guid
                        ReqTransactionId = IntegrationHelper.GenerateTransactionId(), // YYMMDDHHMMSSXXX
                        ConsumerId = IntegrationHelper.CONSUMER_ID, //BDAS 
                        ReqTimestamp = DateTime.Now.ToString(), //YYYY-MM-DD HH:M:SS
                    },
                    VendorInvoiceCreateRequest = new DT_VendorInvoiceCreateVendorInvoiceCreateRequest
                    {
                        DocumentHeader = new[] {
                            new DT_VendorInvoiceCreateVendorInvoiceCreateRequestItem
                        { // Document Date DD.MM.YYYY
                            AttributeName = ERMSField.DocumentDate,
                            AttributeValue = bd.ApprovedOn.Value.ToString("dd.MM.yyyy"),
                            //AttributeValue = wang.Tarikh.Value.ToString("dd.MM.yyyy"),
                        },
                           new DT_VendorInvoiceCreateVendorInvoiceCreateRequestItem
                        { // Company Code - 4 chars
                            AttributeName = ERMSField.CompanyCode,
                            AttributeValue = wang.CoCode,
                        },
                           new DT_VendorInvoiceCreateVendorInvoiceCreateRequestItem
                        { // Reference - 16 chars - BDA Reference number
                            AttributeName = ERMSField.Reference,
                            AttributeValue = bd.RefNo,
                        },
                             new DT_VendorInvoiceCreateVendorInvoiceCreateRequestItem
                        { // Document Type - 4 chars
                            AttributeName = ERMSField.DocumentType,
                            AttributeValue = ERMSField.DefaultDocumentType,
                        },
                             new DT_VendorInvoiceCreateVendorInvoiceCreateRequestItem
                        { // Document Type - 4 chars
                            AttributeName = ERMSField.PostingDate,
                            AttributeValue = DateTime.Now.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                        },
                                new DT_VendorInvoiceCreateVendorInvoiceCreateRequestItem
                        { // Document Type - 4 chars
                            AttributeName = ERMSField.HeaderText,
                            AttributeValue = ERMSField.HeaderTextWCH,
                        },
                       },
                        VendorItems = new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem[][] {
                            new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem[]
                            {
                                 new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // Vendor No
                                    AttributeName = ERMSField.VendorNo,
                                    AttributeValue = ERMSField.DefaultWangCagaranVendoNo,
                                },
                                  new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // State
                                    AttributeName = ERMSField.State,
                                    AttributeValue = wang.Negeri,
                                },
                                   new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // Requester/Initiator
                                    AttributeName = ERMSField.Requester,
                                    AttributeValue = requesterName,
                                },
                                    new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // Business Area
                                    AttributeName = ERMSField.BusinessArea,
                                    AttributeValue = defaultBA,
                                },
                                    new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.Currency,
                                    AttributeValue = ERMSField.DefaultCurrency,
                                },
                                   new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // Amount
                                    AttributeName = ERMSField.Amount,
                                    AttributeValue = wang.Jumlah.ToString(),
                                },
                                     new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // Assignment
                                    AttributeName = ERMSField.Assignment,
                                    AttributeValue = wang.WBSProjekNo,
                                },
                                     new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // Assignment
                                    AttributeName = ERMSField.Text,
                                    AttributeValue = bd.NameOnBD,
                                },
                                       new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // Assignment
                                    AttributeName = ERMSField.LongText,
                                    AttributeValue = longtext,
                                },
                                        new DT_VendorInvoiceCreateVendorInvoiceCreateRequestVendorItemDataItem
                                { // Payment Method
                                    AttributeName = ERMSField.PaymentMethod,
                                    AttributeValue = ERMSField.DefaultPaymentMethod,
                                },
                            }

                        },
                        GLItems = new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem[][] {
                                //wang hangus - description
                              new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem[]
                            {
                                     new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Name On BD
                                    AttributeName = ERMSField.Text,
                                    AttributeValue = bd.NameOnBD,
                                },
                                          new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Name On BD
                                    AttributeName = ERMSField.LongText,
                                    AttributeValue = longtext,
                                },
                                  new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // GL Account - WC - default to 73890
                                    AttributeName = ERMSField.GLAccount,
                                    AttributeValue = wang.GL,
                                },
                                   new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.Currency,
                                    AttributeValue = ERMSField.DefaultCurrency,
                                },
                                    new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Amount
                                    AttributeName = ERMSField.Amount,
                                    AttributeValue = wang.RMHangus.ToString(),
                                },
                                     new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Tax Code
                                    AttributeName = ERMSField.TaxCode,
                                    AttributeValue = ERMSField.DefaultTaxCode,
                                },
                                      new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // BA
                                    AttributeName = ERMSField.BusinessArea,
                                    AttributeValue = wang.BusinessArea,
                                },
                                       new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Assignment
                                    AttributeName = ERMSField.WBSElement,
                                    AttributeValue = wang.CajKod,
                                },
                                       new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Assignment
                                    AttributeName = ERMSField.Assignment,
                                    AttributeValue = wang.CajKod,
                                },
                            },
                            //wang cagaran - description
                            new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem[]
                            {
                                     new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // GL Account
                                    AttributeName = ERMSField.Text,
                                    AttributeValue = bd.NameOnBD,
                                },
                                       new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // GL Account
                                    AttributeName = ERMSField.LongText,
                                    AttributeValue = longtext,
                                },
                                  new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // GL Account - WC - default to 73890
                                    AttributeName = ERMSField.GLAccount,
                                    AttributeValue = ERMSField.WangCagaranGL,
                                },
                                    new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.Currency,
                                    AttributeValue = ERMSField.DefaultCurrency,
                                },
                                    new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.Amount,
                                    AttributeValue = wang.RMCagaran.ToString(),
                                },
                                     new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.TaxCode,
                                    AttributeValue = "",
                                },
                                      new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Currency
                                    AttributeName = ERMSField.BusinessArea,
                                    AttributeValue = wang.BusinessArea,
                                },
                                       new DT_VendorInvoiceCreateVendorInvoiceCreateRequestGLItemDataItem
                                { // Assignment
                                    AttributeName = ERMSField.Assignment,
                                    AttributeValue = wang.CajKod,
                                },
                            },

                        },
                    }

                };

                var response = client.SI_VendorInvoiceCreate_Async_OutAsync(request);
                var transactionStatus = "";
                var msg = "";
                try
                {
                    response.Wait(); // Blocks current thread until GetFooAsync task completes
                }
                catch (Exception e)
                {
                    if (e.InnerException != null)
                    {
                        //erms will send empty soap after posting -> exclude this exception
                        var emptySoapMessage = e.InnerException.Message;
                        if (emptySoapMessage.Contains("Server Error"))
                        {
                            transactionStatus = "FAIL";
                            msg = e.InnerException.Message;
                        }
                        else if (emptySoapMessage.Contains("HTTP request is unauthorized"))
                        {
                            transactionStatus = "FAIL";
                            msg = e.InnerException.Message;
                        }
                        else if (emptySoapMessage.Contains("The one-way operation"))
                        {
                            transactionStatus = "SUCCESS";
                        }
                        else
                        {
                            msg = e.InnerException.Message;

                            db.ErmsLog.Add(new ErmsLog
                            {
                                Id = new Guid(),
                                BankDraftId = bd.Id,
                                RequestId = request.RequestHeader.RequestId,
                                TransactionId = request.RequestHeader.ReqTransactionId,
                                Date = DateTime.Now,
                                Source = request.RequestHeader.ConsumerId,
                                Destination = IntegrationHelper.SOURCE_ERMS,
                                Action = IntegrationHelper.PostingAction,
                                Message = IntegrationHelper.SystemErrorMessage,
                                Status = transactionStatus,
                                Description = msg,
                            });
                            db.SaveChanges();

                        }
                    }


                }
                //logger.Trace("[UpdateNoticeRefNo] Response: " + response.Dump());

                var result = new PostingBankDraftResult
                {
                    IsSuccess = false,
                    Message = "Trying to posting wang cagaran to ERMS but failed. See description in logs for more detail.",
                    IntegrationId = request.RequestHeader.ReqTransactionId,
                };

                if (transactionStatus == "SUCCESS")
                {
                    transactionStatus = "SUCCESS";

                    result = new PostingBankDraftResult
                    {
                        IsSuccess = true,
                        Message = "Successfully send wang cagaran for posting to ERMS.",
                        IntegrationId = request.RequestHeader.ReqTransactionId,
                    };
                }
                else
                {
                    transactionStatus = "FAIL";

                    result = new PostingBankDraftResult
                    {
                        IsSuccess = false,
                        Message = "Trying to posting wang cagaran to ERMS but failed. See description in logs for more detail.",
                    };
                }

                db.ErmsLog.Add(new ErmsLog
                {
                    Id = new Guid(),
                    BankDraftId = bd.Id,
                    RequestId = request.RequestHeader.RequestId,
                    TransactionId = request.RequestHeader.ReqTransactionId,
                    Date = DateTime.Now,
                    Source = request.RequestHeader.ConsumerId,
                    Destination = IntegrationHelper.SOURCE_ERMS,
                    Action = IntegrationHelper.PostingAction,
                    Message = result.Message,
                    Status = transactionStatus,
                    Description = msg,
                });
                db.SaveChanges();

                return result;

            }
            finally
            {
                if (client != null) client.Close();
            }
        }
    }

    public class PostingBankDraftResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string IntegrationId { get; set; }
    }

public class GLItemDataItem
{

    private string attributeNameField;

    private string attributeValueField;

    public string AttributeName
    {
        get
        {
            return this.attributeNameField;
        }
        set
        {
            this.attributeNameField = value;
        }
    }

    public string AttributeValue
    {
        get
        {
            return this.attributeValueField;
        }
        set
        {
            this.attributeValueField = value;
        }
    }
}

}
