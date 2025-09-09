using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using BDA.Data;
using BDA.Entities;
using BDA.Integrations;
using BDA.WebService;
using Hangfire;
using Hangfire.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NLog;


//using Snms.Settings;

namespace BDA.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CreateNotice" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select CreateNotice.svc or CreateNotice.svc.cs at the Solution Explorer and start debugging.


    public class TransactionStatus : SI_TransactionStatus_Sync_In
    {
        private BdaDBContext db;
        private IBackgroundJobClient Job;
        private static Logger logger = LogManager.GetLogger(IntegrationHelper.LOGGER_NAME);

        public TransactionStatus(BdaDBContext db, IBackgroundJobClient Job)
        {

            this.db = db;
            this.Job = Job;
            //httpContextAccessor.HttpContext.RequestServices.GetService<BdaDBContext>();
        }

        public SI_TransactionStatus_Sync_InResponse SI_TransactionStatus_Sync_InAsync(SI_TransactionStatus_Sync_InRequest request)
        {
            DT_TransactionStatusRequest transaction = request.MT_TransactionStatusRequest;
            DT_RequestHeader header = transaction.RequestHeader;
            DT_TransactionStatusRequestTRANSACTION_STATUS status = transaction.TRANSACTION_STATUS;
            DT_TransactionStatusRequestItem[] output = transaction.TRANSACTION_OUTPUT;
            DT_TransactionStatusRequestTRANSACTION_STATUSMESSAGE[] message = status.MESSAGE;

            var result = "";
            var actionType = "";
            var actionRole = "";
            var byId = "";
            var msg = "";
            bool baChange = false;
            var newBA = "";
            var currBA = "";

            var bd = db.BankDraft.Where(x => x.IntegrationId == status.CONSUMER_REF_ID).FirstOrDefault();
            if(bd == null)
            {
                return new SI_TransactionStatus_Sync_InResponse
                {
                    MT_TransactionStatusResponse = new DT_TransactionStatusResponse
                    {
                        ResponseHeader = new DT_ResponseHeader
                        {
                            ResponseId = IntegrationHelper.GenerateRequestOrResponseId(),
                            ResTransactionId = header.ReqTransactionId,
                            ProviderId = IntegrationHelper.PROVIDER_ID,
                            ResTimestamp = DateTime.Now.ToString(),
                            ResStatus = IntegrationHelper.FAIL,
                            MsgCode = "IM-0003",
                            MsgDesc = "Bank draft with given CONSUMER_REF_ID does not exist.",
                        }

                    },

                };
            }
            else if(bd.Status != "Posted")
            {
                return new SI_TransactionStatus_Sync_InResponse
                {
                    MT_TransactionStatusResponse = new DT_TransactionStatusResponse
                    {
                        ResponseHeader = new DT_ResponseHeader
                        {
                            ResponseId = IntegrationHelper.GenerateRequestOrResponseId(),
                            ResTransactionId = header.ReqTransactionId,
                            ProviderId = IntegrationHelper.PROVIDER_ID,
                            ResTimestamp = DateTime.Now.ToString(),
                            ResStatus = IntegrationHelper.FAIL,
                            MsgCode = "IM-0004",
                            MsgDesc = "Bank draft with given CONSUMER_REF_ID not available for posting anymore.",
                        }

                    },

                };
            }
            else if (bd.Status == "Posted")
            {
                
                try
                {
                    if (status.STATUS == "SUCC")
                {

                        //change BA to new BA corrected by ERMS
                        foreach (var m in message)
                        {
                           // msg = m.MESSAGE_DESCRIPTION + ".";
                            if (m.MESSAGE_DESCRIPTION.Contains("Business area") == true && m.MESSAGE_DESCRIPTION.Contains("changed to") == true)
                            {
                                baChange = true;
                                newBA = m.MESSAGE_DESCRIPTION.Substring(m.MESSAGE_DESCRIPTION.Length - 4); 
                            }
                        }

                        if (bd.Type == "WangCagaran")
                    {
                        var wc = db.WangCagaran.Where(x => x.BankDraftId == bd.Id).FirstOrDefault();
                        foreach (var o in output)
                        {
                            if (o.OUTPUT_NAME == "Document No")
                            {
                                wc.ErmsDocNo = o.OUTPUT_VALUE.Substring(0, o.OUTPUT_VALUE.IndexOf('/'));
                            }
                            else if (o.OUTPUT_NAME == "Posting Date")
                            {
                                DateTime date = DateTime.ParseExact(o.OUTPUT_VALUE, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                                wc.PostingDate = date;//Convert.ToDateTime(o.OUTPUT_VALUE);
                            }
                        }

                        //change BA to new BA corrected by ERMS
                        if (baChange == true)
                        {
                                currBA = wc.BusinessArea;
                                wc.BusinessArea = newBA;
                        }

                        db.SetModified(wc);
                        db.SaveChanges();
                    }
                    else if (bd.Type == "WangHangus")
                    {
                        var wh = db.WangHangus.Where(x => x.BankDraftId == bd.Id).FirstOrDefault();
                        foreach (var o in output)
                        {
                            if (o.OUTPUT_NAME == "Document No")
                            {
                                wh.ErmsDocNo = o.OUTPUT_VALUE.Substring(0, o.OUTPUT_VALUE.IndexOf('/'));
                            }
                            else if (o.OUTPUT_NAME == "Posting Date")
                            {
                                    DateTime date = DateTime.ParseExact(o.OUTPUT_VALUE, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                                    wh.PostingDate = date; // Convert.ToDateTime(o.OUTPUT_VALUE);
                            }
                        }

                            //change BA to new BA corrected by ERMS
                            if (baChange == true)
                            {
                                currBA = wh.BusinessArea;
                                wh.BusinessArea = newBA;
                            }

                        db.SetModified(wh);
                        db.SaveChanges();
                    }
                    else if (bd.Type == "WangCagaranHangus")
                    {
                        var wch = db.WangCagaranHangus.Where(x => x.BankDraftId == bd.Id).FirstOrDefault();
                        foreach (var o in output)
                        {
                            if (o.OUTPUT_NAME == "Document No")
                            {
                                wch.ErmsDocNo = o.OUTPUT_VALUE.Substring(0, o.OUTPUT_VALUE.IndexOf('/'));
                            }
                            else if (o.OUTPUT_NAME == "Posting Date")
                            {
                                    DateTime date = DateTime.ParseExact(o.OUTPUT_VALUE, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                                    wch.PostingDate = date; // Convert.ToDateTime(o.OUTPUT_VALUE);
                            }
                        }

                            //change BA to new BA corrected by ERMS
                            if (baChange == true)
                            {
                                currBA = wch.BusinessArea;
                                wch.BusinessArea = newBA;
                            }


                        db.SetModified(wch);
                        db.SaveChanges();
                    }

                    result = "SUCCESSFUL";
                    actionType = Data.ActionType.Accepted.ToString();
                    actionRole = Data.ActionRole.TGBSBanking.ToString();
                    byId = bd.TGBSAcceptanceId;

                    foreach (var m in message)
                    {
                        msg = m.MESSAGE_DESCRIPTION + ".";
                           
                    }

                    bd.Status = "Accepted";
                    db.SetModified(bd);
                    db.SaveChanges();

                    //Send email for acceptance
                    Job.Enqueue<Services.NotificationService>(x => x.NotifyRequesterForTGBSAcceptance(bd.Id));

                }
                else if (status.STATUS == "FAIL")
                {
                    result = "FAIL";
                    actionType = Data.ActionType.PostedFail.ToString();
                    actionRole = "ERMS";
                    byId = null;

                    foreach (var m in message)
                    {
                        msg += m.MESSAGE_DESCRIPTION + ".\n";
                    }

                        //bd.Status = "Approved"; //change bankdraft status to decline for failed posting application
                    bd.Status = "ToDecline";
                    db.SetModified(bd);
                    db.SaveChanges();

                }

                }
                catch (Exception ex)
                {
                    var p = ex.Message;
                    var exceptionMessage = ex.InnerException.Message;

                    //save into ERMS Log
                    ErmsLog log = new ErmsLog();
                    log.Id = new Guid();
                    log.BankDraftId = bd.Id;
                    log.RequestId = header.RequestId;
                    log.TransactionId = header.ReqTransactionId;
                    log.Date = DateTime.Now;
                    log.Source = IntegrationHelper.SOURCE_ERMS;
                    log.Destination = IntegrationHelper.PROVIDER_ID;
                    log.Action = IntegrationHelper.TransactionStatusAction;
                    log.Status = "FAIL";
                    log.Message = IntegrationHelper.SystemErrorMessage;
                    log.Description = exceptionMessage;

                    db.ErmsLog.Add(log);
                    db.SaveChanges();

                    //return successfully to ERMS - Unable to get data from ERMS 
                    return new SI_TransactionStatus_Sync_InResponse
                    {
                        MT_TransactionStatusResponse = new DT_TransactionStatusResponse
                        {
                            ResponseHeader = new DT_ResponseHeader
                            {
                                ResponseId = IntegrationHelper.GenerateRequestOrResponseId(),
                                ResTransactionId = header.ReqTransactionId,
                                ProviderId = IntegrationHelper.PROVIDER_ID,
                                ResTimestamp = DateTime.Now.ToString(),
                                ResStatus = IntegrationHelper.FAIL,
                                MsgCode = "IM-0002",
                                MsgDesc = "Message does not received successfully",
                            }

                        },

                    };
                }
            }

            // save into BankDraftAction
            BankDraftAction ba = new BankDraftAction();
            ba.ActionType = actionType;
            ba.On = DateTime.Now;
            ba.ById = byId;
            ba.ParentId = bd.Id;
            ba.ActionRole = actionRole;
            ba.Comment = msg;

            db.BankDraftAction.Add(ba);
            db.SaveChanges();

            //change BA to new BA corrected by ERMS to be saved in logs
            if (baChange == true)
            {
                BankDraftAction ba1 = new BankDraftAction();
                ba1.ActionType = "Update";
                ba1.On = DateTime.Now;
                ba1.ById = byId;
                ba1.ParentId = bd.Id;
                ba1.ActionRole = "System"; ;
                ba1.Comment = "Business area has been updated from " + currBA + " to " + newBA + ".";

                db.BankDraftAction.Add(ba1);
                db.SaveChanges();
            }

            //save into ErmsLog
            ErmsLog el = new ErmsLog();
            el.Id = new Guid();
            el.BankDraftId = bd.Id;
            el.RequestId = header.RequestId;
            el.TransactionId = header.ReqTransactionId;
            el.Date = DateTime.Now;
            el.Source = IntegrationHelper.SOURCE_ERMS;
            el.Destination = IntegrationHelper.PROVIDER_ID;
            el.Action = IntegrationHelper.TransactionStatusAction;
            el.Status = result;
            el.Message = msg;

            db.ErmsLog.Add(el);
            db.SaveChanges();

            //return successfully to ERMS - Unable to get data from ERMS 
            return new SI_TransactionStatus_Sync_InResponse
            {
                MT_TransactionStatusResponse = new DT_TransactionStatusResponse
                {
                    ResponseHeader = new DT_ResponseHeader
                    {
                        ResponseId = IntegrationHelper.GenerateRequestOrResponseId(),
                        ResTransactionId = header.ReqTransactionId,
                        ProviderId = IntegrationHelper.PROVIDER_ID,
                        ResTimestamp = DateTime.Now.ToString(),
                        ResStatus = IntegrationHelper.SUCCESS,
                        MsgCode = "IM-0001",
                        MsgDesc = "Message received successfully",
                    }

                },

            };


        }


    }
}
