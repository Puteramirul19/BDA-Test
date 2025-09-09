using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BDA.Entities;
using BDA.Identity;
using BDA.ViewModel;
using BDA.Web.Controllers;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;


namespace BDA.Controllers
{
    [Authorize]
    public class CoverMemoController : BaseController
    {

        private readonly UserManager<ApplicationUser> _userManager;
        public CoverMemoController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        //public IActionResult Create()
        //{
           
        //    return View();
        //}

        public IActionResult Create(string RefNo = null)
        {
            ViewBag.RefNo = null;
            ViewBag.RequesterId = null;

            if (RefNo != null)
            {
                ViewBag.RefNo = RefNo;
                var result = Db.BankDraft.Where(y => y.RefNo == RefNo).FirstOrDefault(); //RefNo;
                ViewBag.RequesterId = result.RequesterId;
                /*ViewBag.ApplicationType = Data.BDType.WangCagaran.ToString();*/
                if (RefNo.Contains("WC"))
                {
                    ViewBag.ApplicationType = Data.BDType.WangCagaran.ToString();
                }
                else
                {
                    ViewBag.ApplicationType = Data.BDType.WangHangus.ToString();
                }
            }
            return View();
        }

        [HttpGet]
        public JsonResult GetMemoDetails(string refNo = null, string appType = null)
        {
            var model = new MemoViewModel();
            PenerimaViewModel bd = null;
            List<PenerimaViewModel> bdList = null;

            var bdRefNo = refNo.Split(",");

            var wangs = Db.BankDraft.Where(x => x.RefNo == "").ToList();

            foreach (var list in bdRefNo)
            {
                var wang1 = Db.BankDraft.Where(x => x.RefNo == list).FirstOrDefault();
                //var userID = Db.Users.Where(x => x.Id == wang1.VerifierId);
                //var verifierName = Db.Users.Where(x => x.Id == wang1.VerifierId).Select(x => x.FullName).FirstOrDefault();
                //var joinz = Db.Users.
                //    Join(Db.BankDraft,
                //    users => users.Id,
                //    bdraft => bd.VerifierId,
                //    (users, bdraft) => new { Users = users, BankDraft = bdraft })
                //    .Where(usersAndbdraft => usersAndbdraft.Users.Id == '1');
                wangs.Add(wang1);
                //wangs.Add(verifierName);
            }

            var approveDate = wangs.Select(x => x.ApprovedOn).FirstOrDefault();
            var requestor = wangs.Select(x => x.CreatedById).FirstOrDefault();
            var verifierUP = wangs.Select(x => x.VerifierId).FirstOrDefault();
            var verifierName = Db.Users.Where(x => x.Id == verifierUP).Select(x => x.FullName).FirstOrDefault();
            var approverUP = wangs.Select(x => x.ApproverId).FirstOrDefault();
            var approverName = Db.Users.Where(x => x.Id == approverUP).Select(x => x.FullName).FirstOrDefault();
            var requestorDetails = _userManager.Users.Where(x => x.Id == requestor).FirstOrDefault();
            if (verifierUP != null)
            {
                //model.UP = verifierUP + ", " + approverUP;
                model.UP = verifierName + ", " + approverName;
            }
            else
            {
                model.UP = approverName;
            }
            model.RequestorId = requestor;
            model.Requestor = requestorDetails.FullName;
            model.RequestorAddress = requestorDetails.FullName + "\n" + requestorDetails.Designation
                                                                    + "\n" + requestorDetails.Unit
                                                                    + "\n" + requestorDetails.Division
                                                                    + "\n" + "Tenaga Nasional Berhad";
            var l = "Surat dari {0} yang bertarikh {1} adalah dirujuk.";
            var line1 = string.Format(l, requestorDetails.Unit + ", " + requestorDetails.Division, Convert.ToDateTime(approveDate).ToString("dd MMMM yyyy"));
            model.Line1 = line1;

            var approverDetails = _userManager.Users
                                                .Join(Db.UserRoles,
                                                u => u.Id,
                                                ur => ur.UserId,
                                                (u, ur) => new
                                                {
                                                    UserId = u.Id,
                                                    UserName = u.FullName,
                                                    Unit = u.Unit,
                                                    Division = u.Division,
                                                    UserRole = ur.RoleId,
                                                    IsActive = u.IsActive,
                                                    Designation = u.Designation
                                                }
                                                )
                                                .Where(x => x.UserRole == "HBM" && x.IsActive == true).FirstOrDefault();

            //model.Approver = approverDetails.UserName;
            model.ApproverAddress = model.ApproverAddress != null ? model.ApproverAddress
                                                                : approverDetails != null ? (approverDetails.UserName
                                                                    + "\n" + approverDetails.Designation
                                                                    + "\n" + approverDetails.Unit
                                                                    + "\n" + approverDetails.Division
                                                                    + "\n" + "Tenaga Nasional Berhad") : "";

            if (appType == Data.BDType.WangCagaran.ToString())
            {
                bdList = Db.BankDraft
                                     .Join(Db.WangCagaran,
                                             b => b.Id,
                                             w => w.BankDraftId,
                                             (b, w) => new PenerimaViewModel
                                             {
                                                 CoverMemoRefNo = b.CoverMemoRefNo,
                                                 BankDraftId = b.Id.ToString(),
                                                 NamaPenerima = w.NamaPemegangCagaran,
                                                 BankDraftDate = b.BankDraftDate.ToString(),
                                                 BankDraftNo = b.BankDrafNoIssued,
                                                 ProjectNo = w.WBSProjekNo,
                                                 Jumlah = w.Jumlah
                                             }
                                         )
                                         .Where(x => x.BankDraftId == "").ToList();
            }
            else if (appType == Data.BDType.WangHangus.ToString())
            {
                bdList = Db.BankDraft
                                    .Join(Db.WangHangus,
                                            b => b.Id,
                                            w => w.BankDraftId,
                                            (b, w) => new PenerimaViewModel
                                            {
                                                CoverMemoRefNo = b.CoverMemoRefNo,
                                                BankDraftId = b.Id.ToString(),
                                                BankDraftDate = b.BankDraftDate.ToString(),
                                                BankDraftNo = b.BankDrafNoIssued,
                                                NamaPenerima = w.VendorName,
                                                ProjectNo = Db.AccountingTable
                                                                    .Where(a => a.WangHangusId == w.Id)
                                                                    .Select(s => s.CONWNo).FirstOrDefault(),
                                                Jumlah = w.AccountingTable
                                                                    .Where(a => a.WangHangusId == w.Id)
                                                                    .GroupBy(g => g.WangHangusId)
                                                                    .Select(s => s.Sum(g => Convert.ToDecimal(g.Amount)))
                                                                    .FirstOrDefault()
                                            }
                                        )
                                        .Where(x => x.BankDraftId == "").ToList();
            }
            else if (appType == Data.BDType.WangCagaranHangus.ToString())
            {
                bdList = Db.BankDraft
                                     .Join(Db.WangCagaranHangus,
                                             b => b.Id,
                                             w => w.BankDraftId,
                                             (b, w) => new PenerimaViewModel
                                             {
                                                 CoverMemoRefNo = b.CoverMemoRefNo,
                                                 BankDraftId = b.Id.ToString(),
                                                 NamaPenerima = w.NamaPemegangCagaran,
                                                 BankDraftDate = b.BankDraftDate.ToString(),
                                                 BankDraftNo = b.BankDrafNoIssued,
                                                 ProjectNo = w.WBSProjekNo,
                                                 Jumlah = w.Jumlah
                                             }
                                         )
                                         .Where(x => x.BankDraftId == "").ToList();
            }

            foreach (var wang in wangs)
            {
                if (appType == Data.BDType.WangCagaran.ToString())
                {
                    bd = Db.BankDraft
                                    .Join(Db.WangCagaran,
                                            b => b.Id,
                                            w => w.BankDraftId,
                                            (b, w) => new PenerimaViewModel
                                            {
                                                CoverMemoRefNo = b.CoverMemoRefNo,
                                                BankDraftId = b.Id.ToString(),
                                                NamaPenerima = w.NamaPemegangCagaran,
                                                BankDraftDate = b.BankDraftDate.ToString(),
                                                BankDraftNo = b.BankDrafNoIssued,
                                                ProjectNo = w.WBSProjekNo,
                                                Jumlah = w.Jumlah
                                            }
                                        )
                                        .Where(x => x.BankDraftId == wang.Id.ToString()).FirstOrDefault();
                }
                else if (appType == Data.BDType.WangHangus.ToString())
                {
                    bd = Db.BankDraft
                                    .Join(Db.WangHangus,
                                            b => b.Id,
                                            w => w.BankDraftId,
                                            (b, w) => new PenerimaViewModel
                                            {
                                                CoverMemoRefNo = b.CoverMemoRefNo,
                                                BankDraftId = b.Id.ToString(),
                                                BankDraftDate = b.BankDraftDate.ToString(),
                                                BankDraftNo = b.BankDrafNoIssued,
                                                NamaPenerima = w.VendorName,
                                                ProjectNo = Db.AccountingTable
                                                                    .Where(a => a.WangHangusId == w.Id)
                                                                    .Select(s => s.CONWNo).FirstOrDefault(),
                                                Jumlah = w.AccountingTable
                                                                    .Where(a => a.WangHangusId == w.Id)
                                                                    .GroupBy(g => g.WangHangusId)
                                                                    .Select(s => s.Sum(g => Convert.ToDecimal(g.Amount)))
                                                                    .FirstOrDefault()
                                            }
                                        )
                                        .Where(x => x.BankDraftId == wang.Id.ToString()).FirstOrDefault();

                }
                else if (appType == Data.BDType.WangCagaranHangus.ToString())
                {
                    bd = Db.BankDraft
                                    .Join(Db.WangCagaranHangus,
                                            b => b.Id,
                                            w => w.BankDraftId,
                                            (b, w) => new PenerimaViewModel
                                            {
                                                CoverMemoRefNo = b.CoverMemoRefNo,
                                                BankDraftId = b.Id.ToString(),
                                                NamaPenerima = w.NamaPemegangCagaran,
                                                BankDraftDate = b.BankDraftDate.ToString(),
                                                BankDraftNo = b.BankDrafNoIssued,
                                                ProjectNo = w.WBSProjekNo,
                                                Jumlah = w.Jumlah
                                            }
                                        )
                                        .Where(x => x.BankDraftId == wang.Id.ToString()).FirstOrDefault();
                }

                bdList.Add(bd);
            }

            //decimal amount = 0;
            //foreach (var item in bdList)
            //{
            //     amount += (decimal)amount;
            //}

            var penimaString = string.Join(",", bdList.Select(x => x.NamaPenerima));
            var total = bdList.GroupBy(x => 1).Select(x => new { Total = x.Sum(a => a.Jumlah) });

            model.FinalTotal = total.FirstOrDefault().Total.Value;
            var scnd_sk = "SGM";
            if (total != null)
            {
                if (Convert.ToDouble(model.FinalTotal) <= 10000000)
                {
                    scnd_sk = "SM";
                }
            }

            var BPDetails = _userManager.Users
                                                .Join(Db.UserRoles,
                                                u => u.Id,
                                                ur => ur.UserId,
                                                (u, ur) => new
                                                {
                                                    UserId = u.Id,
                                                    UserName = u.FullName,
                                                    Unit = u.Unit,
                                                    Division = u.Division,
                                                    UserRole = ur.RoleId,
                                                    IsActive = u.IsActive
                                                }
                                                )
                                                .Where(x => x.UserRole == "BP" && x.IsActive == true && x.Division == requestorDetails.Division).FirstOrDefault();
            var SGMDetails = _userManager.Users
                                                .Join(Db.UserRoles,
                                                u => u.Id,
                                                ur => ur.UserId,
                                                (u, ur) => new
                                                {
                                                    UserId = u.Id,
                                                    UserName = u.FullName,
                                                    Unit = u.Unit,
                                                    Division = u.Division,
                                                    UserRole = ur.RoleId,
                                                    DivisionId = ur.DivisionId,
                                                    IsActive = u.IsActive
                                                }
                                                )
                                                //.Join(Db.Division,
                                                //uur => uur.DivisionId,
                                                //d => d.Id,
                                                //(uur,d) => new
                                                //{
                                                //    UserId = uur.UserId,
                                                //    UserName = uur.UserName,
                                                //    Unit = uur.Unit,
                                                //    Division = uur.Division,
                                                //    UserRole = uur.UserRole,
                                                //    DivisionId = uur.DivisionId,
                                                //    IsActive = uur.IsActive,
                                                //    DivisionName =  d.Name
                                                //})
                                                .Where(x => x.IsActive == true
                                                        && scnd_sk == "SGM" ?
                                                                x.UserRole == scnd_sk
                                                                : (scnd_sk == "SM" ? x.UserRole == scnd_sk && x.Division == "Distribution" : x.UserRole == "HZ" && x.Division == requestorDetails.Division)).FirstOrDefault();
            var sk = BPDetails != null ? (BPDetails.UserName
                    + "\n" + BPDetails.Unit
                    + "\n" + BPDetails.Division) : "";
            sk += SGMDetails != null ? ("\n\n" + SGMDetails.UserName
                + "\n" + SGMDetails.Unit
                + "\n" + SGMDetails.Division) : "";
            model.Signiture = sk;

            var l2 = "Bersama-sama ini disertakan ({0}) keping deraf bank yang berjumlah <b>{1}</b> seperti {2}";
            var lampiran = bdList.Count() > 5 ? "di Lampiran 1." : "berikut:";
            var line2 = string.Format(l2, bdList.Count(), string.Format("{0:C}", model.FinalTotal), lampiran);
            var l3 = "Untuk makluman, TNB telah membayar wang cagaran kepada &nbsp;<b>{0}</b> sebanyak <b>{1}</b> setakat bulan <b>(SILA ISI BULAN DI SINI)</b>.";
            var line3 = string.Format(l3, "&nbsp;" + penimaString, string.Format("{0:C}", model.FinalTotal));

            if (appType == Data.BDType.WangHangus.ToString())
            {
                //Line 3 for Wang Hangus
                l3 = "Sila tuan akui penerimaannya untuk rekod jabatan kami.";
                line3 = string.Format(l3, "&nbsp;" + penimaString, string.Format("{0:C}", model.FinalTotal));
            }

            var l4 = "Pihak (tuan/puan) dikehendaki untuk memastikan cagaran di atas di <b>TUNTUT</b> semula oleh <b>PEMULA</b> dengan kadar segera dari <b>{0}</b> setelah kerja-kerja diselesaikan kerana Deraf Bank merupakan aliran tunai yang dikeluarkan oleh TNB.";
            var line4 = string.Format(l4, penimaString);
            var l5 = "Selain dari itu, pihak (tuan/puan) juga bertanggung jawab memastikan <b>PEMULA</b> membuat pembayaran deraf bank tersebut kepada <b>{0}</b>, serta mendapatkan resit asal pembayaran di atas nama <b>TENAGA NASIONAL BERHAD</b> untuk disimpan bagi tujuan tuntutan setelah kerja selesai.";
            var line5 = string.Format(l5, penimaString);
            model.Line2 = line2;
            model.Line3 = line3;
            model.Line4 = line4;
            model.Line5 = line5;

            var l6 = "Oleh itu, pihak (tuan/puan) adalah disaran merekodkan pemberian deraf bank dan merekodkan tarikh deraf bank dikembalikan kepada TNB. "
                    //+ "Pemantauan berkala perlu dilakukan dan senarai status cagaran yang masih belum dituntut perlu dimajukan kepada pemula untuk memastikan "
                    //+ "tuntutan cagaran dilakukan dengan kadar segera."
                    + "\n\n7.  Tindakan perlu diambil ke atas pemula yang gagal menuntut cagaran di atas setelah kerja-kerja selesai tanpa sebarang justifikasi. "
                    + "Kerjasama dan tindakan pihak tuan amatlah dihargai.";

            model.LineETC = l6;

            //Change on 27th June 2023 - Memo for WC
            if (appType == Data.BDType.WangCagaran.ToString())
            {
                //Line change -Remove line 3 and complusory for lineETC for Wang Cagaran
                model.Line3 = line4;
                model.Line4 = line5;
                model.Line5 = "Oleh itu, pihak (tuan/puan) adalah disaran merekodkan pemberian deraf bank dan merekodkan tarikh deraf bank dikembalikan kepada TNB."
                    + "Tindakan perlu diambil ke atas pemula yang gagal menuntut cagaran di atas setelah kerja-kerja selesai tanpa sebarang justifikasi.";
                model.LineETC = "Kerjasama dan tindakan pihak (tuan/puan) amatlah dihargai.";
            }

            //List<string> memoData = new List<string>() { model.RequestorAddress, model., monthName, totalMonth, diffMonth, percent, indicator, year };
            //var result = "";
            return new JsonResult(model);
        }

        //[HttpGet]
        //public JsonResult GetMemoDetails(string refNo = null, string appType = null)
        //{
        //    var model = new MemoViewModel();
        //    PenerimaViewModel bd = null;
        //    List<PenerimaViewModel> bdList = null;

        //    var bdRefNo = refNo.Split(",");

        //    var wangs = Db.BankDraft.Where(x => x.RefNo == "").ToList();

        //    foreach (var list in bdRefNo)
        //    {
        //        var wang1 = Db.BankDraft.Where(x => x.RefNo == list).FirstOrDefault();
        //        //var userID = Db.Users.Where(x => x.Id == wang1.VerifierId);
        //        //var verifierName = Db.Users.Where(x => x.Id == wang1.VerifierId).Select(x => x.FullName).FirstOrDefault();
        //        //var joinz = Db.Users.
        //        //    Join(Db.BankDraft,
        //        //    users => users.Id,
        //        //    bdraft => bd.VerifierId,
        //        //    (users, bdraft) => new { Users = users, BankDraft = bdraft })
        //        //    .Where(usersAndbdraft => usersAndbdraft.Users.Id == '1');
        //        wangs.Add(wang1);
        //        //wangs.Add(verifierName);
        //    }

        //    var approveDate = wangs.Select(x => x.ApprovedOn).FirstOrDefault();
        //    var requestor = wangs.Select(x => x.CreatedById).FirstOrDefault();
        //    var verifierUP = wangs.Select(x => x.VerifierId).FirstOrDefault();
        //    var verifierName = Db.Users.Where(x => x.Id == verifierUP).Select(x => x.FullName).FirstOrDefault();
        //    var approverUP = wangs.Select(x => x.ApproverId).FirstOrDefault();
        //    var approverName = Db.Users.Where(x => x.Id == approverUP).Select(x => x.FullName).FirstOrDefault();
        //    var requestorDetails = _userManager.Users.Where(x => x.Id == requestor).FirstOrDefault();
        //    if(verifierUP != null)
        //    {
        //        //model.UP = verifierUP + ", " + approverUP;
        //        model.UP = verifierName + ", " + approverName;
        //    }
        //     else
        //    {
        //        model.UP = approverName;
        //    }
        //    model.RequestorId = requestor;
        //    model.Requestor = requestorDetails.FullName;
        //    model.RequestorAddress =  requestorDetails.FullName  + "\n" + requestorDetails.Designation
        //                                                            + "\n" + requestorDetails.Unit
        //                                                            + "\n" + requestorDetails.Division
        //                                                            + "\n" + "Tenaga Nasional Berhad";
        //    var l = "Surat dari {0} yang bertarikh {1} adalah dirujuk.";
        //    var line1 = string.Format(l, requestorDetails.Unit + ", " + requestorDetails.Division, Convert.ToDateTime(approveDate).ToString("dd MMMM yyyy"));
        //    model.Line1 = line1;

        //    var approverDetails = _userManager.Users
        //                                        .Join(Db.UserRoles,
        //                                        u => u.Id,
        //                                        ur => ur.UserId,
        //                                        (u, ur) => new
        //                                        {
        //                                            UserId = u.Id,
        //                                            UserName = u.FullName,
        //                                            Unit = u.Unit,
        //                                            Division = u.Division,
        //                                            UserRole = ur.RoleId,
        //                                            IsActive = u.IsActive,
        //                                            Designation = u.Designation
        //                                        }
        //                                        )
        //                                        .Where(x => x.UserRole == "HBM" && x.IsActive == true).FirstOrDefault();

        //    //model.Approver = approverDetails.UserName;
        //    model.ApproverAddress = model.ApproverAddress != null ? model.ApproverAddress
        //                                                        : approverDetails != null ? (approverDetails.UserName
        //                                                            + "\n" + approverDetails.Designation
        //                                                            + "\n" + approverDetails.Unit
        //                                                            + "\n" + approverDetails.Division
        //                                                            + "\n" + "Tenaga Nasional Berhad") : "";

        //    if (appType == Data.BDType.WangCagaran.ToString())
        //    {
        //        bdList = Db.BankDraft
        //                             .Join(Db.WangCagaran,
        //                                     b => b.Id,
        //                                     w => w.BankDraftId,
        //                                     (b, w) => new PenerimaViewModel
        //                                     {
        //                                         CoverMemoRefNo = b.CoverMemoRefNo,
        //                                         BankDraftId = b.Id.ToString(),
        //                                         NamaPenerima = w.NamaPemegangCagaran,
        //                                         BankDraftDate = b.BankDraftDate.ToString(),
        //                                         BankDraftNo = b.BankDrafNoIssued,
        //                                         ProjectNo = w.WBSProjekNo,
        //                                         Jumlah = w.Jumlah
        //                                     }
        //                                 )
        //                                 .Where(x => x.BankDraftId == "").ToList();
        //    }
        //    else if (appType == Data.BDType.WangHangus.ToString())
        //    {
        //        bdList = Db.BankDraft
        //                            .Join(Db.WangHangus,
        //                                    b => b.Id,
        //                                    w => w.BankDraftId,
        //                                    (b, w) => new PenerimaViewModel
        //                                    {
        //                                        CoverMemoRefNo = b.CoverMemoRefNo,
        //                                        BankDraftId = b.Id.ToString(),
        //                                        BankDraftDate = b.BankDraftDate.ToString(),
        //                                        BankDraftNo = b.BankDrafNoIssued,
        //                                        NamaPenerima = w.VendorName,
        //                                        ProjectNo = Db.AccountingTable
        //                                                            .Where(a => a.WangHangusId == w.Id)
        //                                                            .Select(s => s.CONWNo).FirstOrDefault(),
        //                                        Jumlah = w.AccountingTable
        //                                                            .Where(a => a.WangHangusId == w.Id)
        //                                                            .GroupBy(g => g.WangHangusId)
        //                                                            .Select(s => s.Sum(g => Convert.ToDecimal(g.Amount)))
        //                                                            .FirstOrDefault()
        //                                    }
        //                                )
        //                                .Where(x => x.BankDraftId == "").ToList();
        //    }

        //    foreach (var wang in wangs)
        //    {
        //        if (appType == Data.BDType.WangCagaran.ToString())
        //        {
        //            bd = Db.BankDraft
        //                            .Join(Db.WangCagaran,
        //                                    b => b.Id,
        //                                    w => w.BankDraftId,
        //                                    (b, w) => new PenerimaViewModel
        //                                    {
        //                                        CoverMemoRefNo = b.CoverMemoRefNo,
        //                                        BankDraftId = b.Id.ToString(),
        //                                        NamaPenerima = w.NamaPemegangCagaran,
        //                                        BankDraftDate = b.BankDraftDate.ToString(),
        //                                        BankDraftNo = b.BankDrafNoIssued,
        //                                        ProjectNo = w.WBSProjekNo,
        //                                        Jumlah = w.Jumlah
        //                                    }
        //                                )
        //                                .Where(x => x.BankDraftId == wang.Id.ToString()).FirstOrDefault();
        //        }
        //        else if (appType == Data.BDType.WangHangus.ToString())
        //        {
        //            bd = Db.BankDraft
        //                            .Join(Db.WangHangus,
        //                                    b => b.Id,
        //                                    w => w.BankDraftId,
        //                                    (b, w) => new PenerimaViewModel
        //                                    {
        //                                        CoverMemoRefNo = b.CoverMemoRefNo,
        //                                        BankDraftId = b.Id.ToString(),
        //                                        BankDraftDate = b.BankDraftDate.ToString(),
        //                                        BankDraftNo = b.BankDrafNoIssued,
        //                                        NamaPenerima = w.VendorName,
        //                                        ProjectNo = Db.AccountingTable
        //                                                            .Where(a => a.WangHangusId == w.Id)
        //                                                            .Select(s => s.CONWNo).FirstOrDefault(),
        //                                        Jumlah = w.AccountingTable
        //                                                            .Where(a => a.WangHangusId == w.Id)
        //                                                            .GroupBy(g => g.WangHangusId)
        //                                                            .Select(s => s.Sum(g => Convert.ToDecimal(g.Amount)))
        //                                                            .FirstOrDefault()
        //                                    }
        //                                )
        //                                .Where(x => x.BankDraftId == wang.Id.ToString()).FirstOrDefault();

        //        }

        //        bdList.Add(bd);
        //    }

        //    //decimal amount = 0;
        //    //foreach (var item in bdList)
        //    //{
        //    //     amount += (decimal)amount;
        //    //}

        //    var penimaString = string.Join(",", bdList.Select(x => x.NamaPenerima));
        //    var total = bdList.GroupBy(x => 1).Select(x => new { Total = x.Sum(a => a.Jumlah) });

        //    model.FinalTotal = total.FirstOrDefault().Total.Value;
        //    var scnd_sk = "SGM";
        //    if (total != null)
        //    {
        //        if (Convert.ToDouble(model.FinalTotal) <= 10000000)
        //        {
        //            scnd_sk = "SM";
        //        }
        //    }

        //    var BPDetails = _userManager.Users
        //                                        .Join(Db.UserRoles,
        //                                        u => u.Id,
        //                                        ur => ur.UserId,
        //                                        (u, ur) => new
        //                                        {
        //                                            UserId = u.Id,
        //                                            UserName = u.FullName,
        //                                            Unit = u.Unit,
        //                                            Division = u.Division,
        //                                            UserRole = ur.RoleId,
        //                                            IsActive = u.IsActive
        //                                        }
        //                                        )
        //                                        .Where(x => x.UserRole == "BP" && x.IsActive == true && x.Division == requestorDetails.Division).FirstOrDefault();
        //    var SGMDetails = _userManager.Users
        //                                        .Join(Db.UserRoles,
        //                                        u => u.Id,
        //                                        ur => ur.UserId,
        //                                        (u, ur) => new
        //                                        {
        //                                            UserId = u.Id,
        //                                            UserName = u.FullName,
        //                                            Unit = u.Unit,
        //                                            Division = u.Division,
        //                                            UserRole = ur.RoleId,
        //                                            DivisionId = ur.DivisionId,
        //                                            IsActive = u.IsActive
        //                                        }
        //                                        )
        //                                        //.Join(Db.Division,
        //                                        //uur => uur.DivisionId,
        //                                        //d => d.Id,
        //                                        //(uur,d) => new
        //                                        //{
        //                                        //    UserId = uur.UserId,
        //                                        //    UserName = uur.UserName,
        //                                        //    Unit = uur.Unit,
        //                                        //    Division = uur.Division,
        //                                        //    UserRole = uur.UserRole,
        //                                        //    DivisionId = uur.DivisionId,
        //                                        //    IsActive = uur.IsActive,
        //                                        //    DivisionName =  d.Name
        //                                        //})
        //                                        .Where(x => x.IsActive == true
        //                                                && scnd_sk == "SGM" ?
        //                                                        x.UserRole == scnd_sk
        //                                                        : (scnd_sk == "SM" ? x.UserRole == scnd_sk && x.Division == "Distribution" : x.UserRole == "HZ" && x.Division == requestorDetails.Division)).FirstOrDefault();
        //    var sk = BPDetails != null ? (BPDetails.UserName
        //            + "\n" + BPDetails.Unit
        //            + "\n" + BPDetails.Division) : "";
        //    sk += SGMDetails != null ? ("\n\n" + SGMDetails.UserName
        //        + "\n" + SGMDetails.Unit
        //        + "\n" + SGMDetails.Division) : "";
        //    model.Signiture = sk;

        //    var l2 = "Bersama-sama ini disertakan ({0}) keping deraf bank yang berjumlah <b>{1}</b> seperti {2}";
        //    var lampiran = bdList.Count() > 5 ? "di Lampiran 1." : "berikut:";
        //    var line2 = string.Format(l2, bdList.Count(), string.Format("{0:C}", model.FinalTotal), lampiran);
        //    var l3 = "Untuk makluman, TNB telah membayar wang cagaran kepada &nbsp;<b>{0}</b> sebanyak <b>{1}</b> setakat bulan <b>(SILA ISI BULAN DI SINI)</b>.";
        //    var line3 = string.Format(l3, "&nbsp;" + penimaString, string.Format("{0:C}", model.FinalTotal));

        //    if( appType == Data.BDType.WangHangus.ToString() )
        //    {
        //        //Line 3 for Wang Hangus
        //        l3 = "Sila tuan akui penerimaannya untuk rekod jabatan kami.";
        //        line3 = string.Format(l3, "&nbsp;" + penimaString, string.Format("{0:C}", model.FinalTotal));
        //    }

        //    var l4 = "Pihak (tuan/puan) dikehendaki untuk memastikan cagaran di atas di <b>TUNTUT</b> semula oleh <b>PEMULA</b> dengan kadar segera dari <b>{0}</b> setelah kerja-kerja diselesaikan kerana Deraf Bank merupakan aliran tunai yang dikeluarkan oleh TNB.";
        //    var line4 = string.Format(l4, penimaString);
        //    var l5 = "Selain dari itu, pihak (tuan/puan) juga bertanggung jawab memastikan <b>PEMULA</b> membuat pembayaran deraf bank tersebut kepada <b>{0}</b>, serta mendapatkan resit asal pembayaran di atas nama <b>TENAGA NASIONAL BERHAD</b> untuk disimpan bagi tujuan tuntutan setelah kerja selesai.";
        //    var line5 = string.Format(l5, penimaString);
        //    model.Line2 = line2;
        //    model.Line3 = line3;
        //    model.Line4 = line4;
        //    model.Line5 = line5;

        //    var l6 = "Oleh itu, pihak (tuan/puan) adalah disaran merekodkan pemberian deraf bank dan merekodkan tarikh deraf bank dikembalikan kepada TNB. "
        //            + "Pemantauan berkala perlu dilakukan dan senarai status cagaran yang masih belum dituntut perlu dimajukan kepada pemula untuk memastikan "
        //            + "tuntutan cagaran dilakukan dengan kadar segera."
        //            + "\n\n7.  Tindakan perlu diambil ke atas pemula yang gagal menuntut cagaran di atas setelah kerja-kerja selesai tanpa sebarang justifikasi. "
        //            + "Kerjasama dan tindakan pihak tuan amatlah dihargai.";

        //    model.LineETC = l6;

        //    //List<string> memoData = new List<string>() { model.RequestorAddress, model., monthName, totalMonth, diffMonth, percent, indicator, year };
        //    //var result = "";
        //    return new JsonResult(model);
        //}

        [HttpGet]
        public IActionResult Edit(string Id = null, string CoverMemoRefNo = null)
        {
            var model = new MemoViewModel();
            IEnumerable<PenerimaViewModel> bd = null;

            Guid gId = (Id == null ? Guid.NewGuid() : Guid.Parse(Id));
            var item = Db.Memo.Where(x => (Id == null ? x.CoverRefNo == CoverMemoRefNo : x.Id == gId)).FirstOrDefault();

            model.Id = item.Id.ToString();
            model.Date = item.Date;
            model.ReferenceNo = item.ReferenceNo;
            model.CoverRefNo = item.CoverRefNo;
            model.ApplicationType = item.ApplicationType;
            model.RujukanNo = item.RujukanNo;
            
            var wangs = Db.BankDraft.Where(x => x.CoverMemoRefNo == model.CoverRefNo).ToList();

            var approveDate = wangs.Select(x => x.ApprovedOn).FirstOrDefault();
            var requestor = wangs.Select(x => x.CreatedById).FirstOrDefault();
            var verifierUP = wangs.Select(x => x.VerifierId).FirstOrDefault();
            var approverUP = wangs.Select(x => x.ApproverId).FirstOrDefault();
            var requestorDetails = _userManager.Users.Where(x => x.Id == requestor).FirstOrDefault();
            if (verifierUP != null)
            {
                model.UP = item.UP != null ? item.UP : verifierUP + ", " + approverUP;
            }
            else
            {
                model.UP = item.UP != null ? item.UP : approverUP;
            }
            model.RequestorId = requestor;
            model.Requestor = requestorDetails.FullName;
            model.RequestorAddress = item.RequestorAddress != null ? item.RequestorAddress
                                                                    : requestorDetails.FullName 
                                                                    + "\n" + requestorDetails.Designation
                                                                    + "\n" + requestorDetails.Unit
                                                                    + "\n" + requestorDetails.Division
                                                                    + "\n" + "Tenaga Nasional Berhad";
            var l = "Surat dari {0} yang bertarikh {1} adalah dirujuk.";
            var line1 = string.Format(l, requestorDetails.Unit + ", " + requestorDetails.Division, Convert.ToDateTime(approveDate).ToString("dd MMMM yyyy"));
            model.Line1 = item.Line1 != null ? item.Line1 : line1;

            var approverDetails = _userManager.Users
                                                .Join(Db.UserRoles,
                                                u => u.Id,
                                                ur => ur.UserId,
                                                (u, ur) => new
                                                {
                                                    UserId = u.Id,
                                                    UserName = u.FullName,
                                                    Unit = u.Unit,
                                                    Division = u.Division,
                                                    UserRole = ur.RoleId,
                                                    IsActive = u.IsActive,
                                                    Designation = u.Designation
                                                }
                                                )
                                                .Where(x=>x.UserRole == "HBM" && x.IsActive == true).FirstOrDefault();

            //model.Approver = approverDetails.UserName;
            model.ApproverAddress = item.ApproverAddress!= null? item.ApproverAddress 
                                                                : approverDetails != null ? (approverDetails.UserName
                                                                    + "\n" + approverDetails.Designation
                                                                    + "\n" + approverDetails.Division) : "";

            foreach (var wang in wangs)
            {
                if (model.ApplicationType == Data.BDType.WangCagaran.ToString())
                {
                    bd = Db.BankDraft
                                    .Join(Db.WangCagaran,
                                            b => b.Id,
                                            w => w.BankDraftId,
                                            (b, w) => new PenerimaViewModel
                                            {
                                                CoverMemoRefNo = b.CoverMemoRefNo,
                                                BankDraftId = b.Id.ToString(),
                                                NamaPenerima = w.NamaPemegangCagaran,
                                                BankDraftDate = b.BankDraftDate.ToString(),
                                                BankDraftNo = b.BankDrafNoIssued,
                                                ProjectNo = w.WBSProjekNo,
                                                Jumlah = w.Jumlah
                                            }
                                        )
                                        .Where(x => x.BankDraftId == wang.Id.ToString()).ToList();
                }
                else if(model.ApplicationType == Data.BDType.WangHangus.ToString())
                {
                    bd = Db.BankDraft
                                    .Join(Db.WangHangus,
                                            b => b.Id,
                                            w => w.BankDraftId,
                                            (b, w) => new PenerimaViewModel
                                            {
                                                CoverMemoRefNo = b.CoverMemoRefNo,
                                                BankDraftId = b.Id.ToString(),
                                                BankDraftDate = b.BankDraftDate.ToString(),
                                                BankDraftNo = b.BankDrafNoIssued,
                                                NamaPenerima = w.VendorName,
                                                ProjectNo = Db.AccountingTable
                                                                    .Where(a => a.WangHangusId == w.Id)
                                                                    .Select(s => s.CONWNo).FirstOrDefault(),
                                                Jumlah = w.AccountingTable
                                                                    .Where(a => a.WangHangusId == w.Id)
                                                                    .GroupBy(g => g.WangHangusId)
                                                                    .Select(s => s.Sum(g => Convert.ToDecimal(g.Amount)))
                                                                    .FirstOrDefault()
                                            }
                                        )
                                        .Where(x => x.BankDraftId == wang.Id.ToString()).ToList();

                }

            }
            var penimaString = string.Join(",", bd.Select(x => x.NamaPenerima));
            var total = bd.GroupBy(x => x.BankDraftId).Select(x => new { Total = x.Sum(a => a.Jumlah) }).FirstOrDefault();
            model.FinalTotal = total.Total.Value;
            var scnd_sk = "SGM";
            if(total != null)
            {
                if (Convert.ToDouble(model.FinalTotal) <= 10000000)
                {
                    scnd_sk = "SM";
                }
            }
            
            var BPDetails = _userManager.Users
                                                .Join(Db.UserRoles,
                                                u => u.Id,
                                                ur => ur.UserId,
                                                (u, ur) => new
                                                {
                                                    UserId = u.Id,
                                                    UserName = u.FullName,
                                                    Unit = u.Unit,
                                                    Division = u.Division,
                                                    UserRole = ur.RoleId,
                                                    IsActive = u.IsActive
                                                }
                                                )
                                                .Where(x => x.UserRole == "BP" && x.IsActive == true && x.Division == requestorDetails.Division).FirstOrDefault();
            var SGMDetails = _userManager.Users
                                                .Join(Db.UserRoles,
                                                u => u.Id,
                                                ur => ur.UserId,
                                                (u, ur) => new
                                                {
                                                    UserId = u.Id,
                                                    UserName = u.FullName,
                                                    Unit = u.Unit,
                                                    Division = u.Division,
                                                    UserRole = ur.RoleId,
                                                    DivisionId = ur.DivisionId,
                                                    IsActive = u.IsActive
                                                }
                                                )
                                                //.Join(Db.Division,
                                                //uur => uur.DivisionId,
                                                //d => d.Id,
                                                //(uur,d) => new
                                                //{
                                                //    UserId = uur.UserId,
                                                //    UserName = uur.UserName,
                                                //    Unit = uur.Unit,
                                                //    Division = uur.Division,
                                                //    UserRole = uur.UserRole,
                                                //    DivisionId = uur.DivisionId,
                                                //    IsActive = uur.IsActive,
                                                //    DivisionName =  d.Name
                                                //})
                                                .Where(x => x.IsActive == true 
                                                        && scnd_sk == "SGM" ? 
                                                                x.UserRole == scnd_sk 
                                                                : (scnd_sk == "SM" ? x.UserRole == scnd_sk && x.Division == "Distribution" : x.UserRole == "HZ" && x.Division == requestorDetails.Division)).FirstOrDefault();
            var sk = BPDetails != null ? (BPDetails.UserName
                    + "\n" + BPDetails.Unit
                    + "\n" + BPDetails.Division) : "";
                sk += SGMDetails != null ? ("\n\n" + SGMDetails.UserName
                    + "\n" + SGMDetails.Unit
                    + "\n" + SGMDetails.Division) : "";
            model.Signiture = item.Signiture != null ? item.Signiture : ""; //sk

            var l2 = "Bersama-sama ini disertakan ({0}) keping deraf bank yang berjumlah <b>{1}</b> seperti {2}";
            var lampiran = bd.Count() > 5 ? "di Lampiran 1." : "berikut:";
            var line2 = string.Format(l2, bd.Count(), string.Format("{0:C}", model.FinalTotal), lampiran);
            //var l3 = "Untuk makluman, TNB telah membayar wang cagaran kepada <b>{0}</b> sebanyak <b>{1}</b> setakat bulan <b>(SILA ISI BULAN DI SINI)</b>.";
            //var line3 = string.Format(l3, "&nbsp;" + penimaString, string.Format("{0:C}", model.FinalTotal), Convert.ToDateTime(item.CreatedOn).ToString("MMM yyyy"));
            var l4 = "Pihak (tuan/puan) dikehendaki untuk memastikan cagaran di atas di <b>TUNTUT</b> semula oleh <b>PEMULA</b> dengan kadar segera dari <b>{0}</b> setelah kerja-kerja diselesaikan kerana Deraf Bank merupakan aliran tunai yang dikeluarkan oleh TNB.";
            var line4 = string.Format(l4, penimaString);
            var l5 = "Selain dari itu, pihak tuan juga bertanggung jawab memastikan <b>PEMULA</b> membuat pembayaran deraf bank tersebut kepada <b>{0}</b>, serta mendapatkan resit asal pembayaran untuk disimpan bagi tujuan tuntutan setelah keraja selesai.";
            var line5 = string.Format(l5, penimaString);
            model.Line2 = item.Line2 != null ? item.Line2 : line2;
            //model.Line3 = item.Line3 != null ? item.Line3 : line3;
            model.Line4 = item.Line4 != null ? item.Line4 : line4;
            model.Line5 = item.Line5 != null ? item.Line5 : line5;

            var l6 = "Oleh itu, pihak (tuan/puan) adalah disaran merekodkan pemberian deraf bank dan merekodkan tarikh deraf bank dikembalikan kepada TNB."
                    + "Pemantauan berkala perlu dilakukan dan senarai status cagaran yang masih belum dituntut perlu dimajukan kepada pemula untuk memmastikan"
                    + "tuntutan cagaran dilakukan dengan kadar segera."
                    + "\n\nKerjasama dan tindakan pihak (tuan/puan) amatlah dihargai.";

                    //+ "\n\n7.  Tindakan perlu diambil ke atas pemula yang gagal menuntut cagaran di atas setelah kerja-kerja selesai tanpa sebarang justifikasi."
                    //+ "Kerjasama dan tindakan pihak (tuan/puan) amatlah dihargai.";

            model.LineETC = item.LineETC != null ? item.LineETC : l6;

            return View(model);
        }

        [HttpPost]
        public JsonResult Edit(MemoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = Db.Memo.Find(Guid.Parse(model.Id));
                if (entity != null)
                {

                    try
                    {
                        entity.Date = model.Date;
                        entity.ReferenceNo = string.Join(", ", model.RefereceNos);
                        entity.Status = Data.Status.Issuing.ToString();
                        entity.UpdatedOn = DateTime.Now;
                        entity.RequestorAddress = model.RequestorAddress;
                        entity.ApproverAddress = model.ApproverAddress;
                        entity.UP = model.UP;
                        entity.Line1 = model.Line1;
                        entity.Line2 = model.Line2;
                        entity.Line3 = model.Line3;
                        entity.Line4 = model.Line4;
                        entity.Line5 = model.Line5;
                        entity.LineETC = model.LineETC;
                        entity.Signiture = model.Signiture;
                        entity.RujukanNo = model.RujukanNo;

                        Db.SetModified(entity);
                        Db.SaveChanges();

                        ResetBDCoverMemo(entity.CoverRefNo, entity.ReferenceNo);
                        return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Update Cover Memo Successful!", coverRefNo = entity.CoverRefNo, coverId = entity.Id });
                    }
                    catch (Exception e)
                    {
                        return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });

                    }
                }
                else
                {

                    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Error! Cover Memo Invalid!" });
                }
            }
            else
            {
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Error! Please try again later." });
            }
        }
        [HttpPost]
        public JsonResult Create(MemoViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var dt = model.Date.HasValue ? model.Date.Value.ToString() : DateTime.Now.ToString();
                    var entity = new Memo
                    {
                        Date = Convert.ToDateTime(dt), //DateTime.ParseExact(dt, "dd/MM/yyyy", null),
                        ReferenceNo = string.Join(", ", model.ReferenceNo),
                        CoverRefNo = GetRunningNo(),
                        ApplicationType = model.ReferenceNo.Contains("WC") ? "WangCagaran" : "WangHangus",
                        Status = Data.Status.Issuing.ToString(),
                        RequestorAddress = model.RequestorAddress,
                        ApproverAddress = model.ApproverAddress,
                        UP = model.UP,
                        Line1 = model.Line1,
                        Line2 = model.Line2,
                        Line3 = model.Line3,
                        Line4 = model.Line4,
                        Line5 = model.Line5,
                        LineETC = model.LineETC,
                        Signiture = model.Signiture,
                        RujukanNo = model.RujukanNo,
                    };
                    //entity.Date = DateTime.ParseExact(model.Date.Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    Db.Memo.Add(entity);
                    Db.SaveChanges();

                    ResetBDCoverMemo(entity.CoverRefNo, entity.ReferenceNo);
                    return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Add New Cover Memo Successful!", coverRefNo = entity.CoverRefNo, coverId = entity.Id });
                }
                catch (Exception e)
                {
                    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });
                } 
            }
            else
            {
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Error! Please try again later." });
            }
        }

        public JsonResult SetBDCoverMemo(string CoverMemoRefNo = null, string bdRefNo = null)
        {
            if (CoverMemoRefNo == null)
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Cover Memo Number is Invalid!" });

            if (bdRefNo == null)
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Invalid Bank Draft Id!" });

            var entity = Db.Memo.Where(x => x.CoverRefNo == CoverMemoRefNo).FirstOrDefault();

            if (entity != null)
            {
                if (entity.ReferenceNo != null)
                {
                    var listRefNo = entity.ReferenceNo.Split(", ").ToList();
                    if (!listRefNo.Contains(bdRefNo))
                        listRefNo.Add(bdRefNo);
                    entity.ReferenceNo = string.Join(", ", listRefNo);
                }
                else
                {
                    entity.ReferenceNo = bdRefNo;
                }
                Db.SetModified(entity);
                Db.SaveChanges();
                return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Cover Memo Number is Valid!" });
            }
            else
            {
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Cover Memo Number is Invalid!" });

            }

        }
        public void ResetBDCoverMemo(string MemoRefNo = null, string strBDRefNo = null)
        {
            //remove current letterrefno in bd
            var memos = Db.BankDraft.Where(x => x.CoverMemoRefNo == MemoRefNo).ToList();
            foreach (var bank in memos)
            {
                bank.CoverMemoRefNo = null;
                Db.SetModified(bank);
                Db.SaveChanges();
            }

            //reset bd after remove
            var items = strBDRefNo.Split(",").ToList();
            foreach (var item in items)
            {
                var bd = Db.BankDraft.Where(x => x.RefNo == item).FirstOrDefault();
                if(bd != null)
                {
                    bd.CoverMemoRefNo = MemoRefNo;
                    Db.SetModified(bd);
                    Db.SaveChanges();

                }
                
            }

        }
        public IActionResult SubmitToRequestor(string Id)
        {
            var model = new MemoViewModel();
            var item = Db.Memo.Where(x => x.Id == Guid.Parse(Id)).FirstOrDefault();

            model.Id = item.Id.ToString();
            model.ReferenceNo = item.ReferenceNo;
            model.CoverRefNo = item.CoverRefNo;

            return View(model);
        }
        [HttpPost]
        public JsonResult SubmitMemo(MemoViewModel model, IFormFile SignedLetter)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var file = Db.Attachment.Where(x => x.ParentId == Guid.Parse(model.Id) && x.FileType == Data.AttachmentType.Memo.ToString()).FirstOrDefault();
                    if (SignedLetter == null && file == null)
                    {
                        return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Signed Document Required!" });
                    }

                    var user = _userManager.GetUserAsync(HttpContext.User).Result;
                    var items = model.ReferenceNo.Split(",").ToList();
                    foreach (var item in items)
                    {
                        SaveBankIssueByRefNo(item, model.CoverRefNo, user.Id, SignedLetter);
                    }

                    var entity = Db.Memo.Where(x => x.CoverRefNo == model.CoverRefNo).FirstOrDefault();
                    entity.Status = Data.Status.Issued.ToString();
                    entity.UpdatedOn = DateTime.Now;
                    Db.SetModified(entity);
                    Db.SaveChanges();

                    Job.Enqueue<Services.NotificationService>(x => x.NotifyRequesterForBDAcceptanceFromBulkMemo(entity.Id));

                    return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Issued Successful!" });
                }
                catch (Exception e)
                {
                    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });

                }
            }
            else
            {
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Error! Please try again later." });
            }
        }
        public void UploadFile(IFormFile file, Guid parentId, string fileType, string fileSubType = null)
        {
            BankDraftController bdc = new BankDraftController(_userManager);
            var uniqueFileName = bdc.GetUniqueFileName(file.FileName);
            var ext = Path.GetExtension(uniqueFileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/documents", uniqueFileName);
            file.CopyTo(new FileStream(filePath, FileMode.Create));

            //to do : Save uniqueFileName  to your db table 
            Attachment attachement = new Attachment();
            attachement.FileType = fileType;
            attachement.FileSubType = fileSubType;
            attachement.ParentId = parentId;
            attachement.FileName = uniqueFileName;
            attachement.FileExtension = ext;
            attachement.CreatedOn = DateTime.Now;
            attachement.UpdatedOn = DateTime.Now;
            attachement.IsActive = true;

            Db.Attachment.Add(attachement);
            Db.SaveChanges();
        }
        public void SaveBankIssueByRefNo(string RefNo, string CoverRefNo, string UserId, IFormFile SignedLetter)
        {
            var entity = Db.BankDraft.Where(x => x.RefNo == RefNo).FirstOrDefault();
            entity.CoverMemoRefNo = CoverRefNo;
            entity.UpdatedOn = DateTime.Now;
            //entity.TGBSProcessedOn = DateTime.Now;
            //entity.TGBSProcesserId = UserId;
            entity.TGBSIssuedOn = DateTime.Now;
            entity.TGBSIssuerId = UserId;
            entity.Status = Data.Status.Issued.ToString();
            Db.SetModified(entity);
            Db.SaveChanges();


            var _BankDraftId = entity.Id.ToString();
            UploadFile(SignedLetter, Guid.Parse(_BankDraftId), Data.AttachmentType.Memo.ToString());


            Db.BankDraftAction.Add(new BankDraftAction
            {
                ActionType = Data.ActionType.BankDraftIssued.ToString(),
                On = DateTime.Now,
                ById = UserId,
                ParentId = entity.Id,
                ActionRole = Data.ActionRole.TGBSBanking.ToString(),
            });
            Db.SaveChanges();
        }
        public JsonResult RemoveBDCoverMemo(string CoverMemoRefNo = null, string bdRefNo = null)
        {
            if (CoverMemoRefNo == null)
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Cover Memo Number is Invalid!" });

            if (bdRefNo == null)
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Invalid Bank Draft Id!" });

            var entity = Db.Memo.Where(x => x.CoverRefNo == CoverMemoRefNo).FirstOrDefault();

            if (entity != null)
            {
                if (entity.ReferenceNo != null)
                {
                    var listRefNo = entity.ReferenceNo.Split(", ").ToList();
                    if (listRefNo.Contains(bdRefNo))
                        listRefNo.Remove(bdRefNo);
                    entity.ReferenceNo = string.Join(", ", listRefNo);
                }
                else
                {
                    entity.ReferenceNo = String.Empty;
                }
                Db.SetModified(entity);
                Db.SaveChanges();
                return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft removed from Cover Memo Successful!" });
            }
            else
            {
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Cover Memo Number is Invalid!" });

            }

        }
        public string GetRunningNo()
        {
            RunningNo runningNo = new RunningNo();

            var entity = Db.RunningNo.Where(x => x.Name == "CoverMemo").FirstOrDefault(); //Id for Instruction Letter
            runningNo.Code = entity.Code;
            runningNo.RunNo = entity.RunNo;
            string NewCode = String.Format("{0}{1:00000}", runningNo.Code, runningNo.RunNo);

            entity.RunNo = entity.RunNo + 1;
            Db.RunningNo.Update(entity);
            Db.SaveChanges();

            return NewCode;
        }

        public JsonResult GetAllWCRequester()
        {
         
            var result = Db.BankDraft
                 .Join(Db.Users,
                                  b => b.RequesterId,
                                  u => u.Id,
                                  (b, u) => new
                                  {
                                      refNo = b.RefNo,
                                      Status = b.Status,
                                      ApplicationType = b.Type,
                                      CoverMemoRefNo = b.CoverMemoRefNo,
                                      b.SendMethod,
                                      b.BankDrafNoIssued,
                                      Id = u.Id,
                                      Name = u.FullName
                                  })
                .Where(x => x.Status == Data.Status.Processed.ToString()
                && (x.SendMethod != null || x.BankDrafNoIssued != null)
                && x.CoverMemoRefNo == null
                && x.ApplicationType == Data.BDType.WangCagaran.ToString()).ToList(); //RefNo;

            var result2 = result.Select(x => new { x.Id, x.Name }).Distinct();

            return new JsonResult(result2.ToList());
        }

        public JsonResult GetAllWHRequester()
        {
            var result = Db.BankDraft
                 .Join(Db.Users,
                                  b => b.RequesterId,
                                  u => u.Id,
                                  (b, u) => new
                                  {
                                      refNo = b.RefNo,
                                      Status = b.Status,
                                      ApplicationType = b.Type,
                                      CoverMemoRefNo = b.CoverMemoRefNo,
                                      b.SendMethod,
                                      b.BankDrafNoIssued,
                                      Id = u.Id,
                                      Name = u.FullName
                                  })
                .Where(x => x.Status == Data.Status.Processed.ToString()
                && (x.SendMethod != null || x.BankDrafNoIssued != null)
                && x.CoverMemoRefNo == null
                && x.ApplicationType == Data.BDType.WangHangus.ToString()).ToList(); //RefNo;

            var result2 = result.Select(x => new { x.Id, x.Name }).Distinct();

            return new JsonResult(result2.ToList());
        }

        public JsonResult GetAllWCReferenceNoForBulk(string requesterId)
        {
            var result = Db.BankDraft.Select(x => new { Id = x.RefNo, Name = x.RefNo, Status = x.Status, ApplicationType = x.Type, CoverMemoRefNo = x.CoverMemoRefNo, x.RequesterId, x.SendMethod, x.BankDrafNoIssued })
                .Where(x => x.Status == Data.Status.Processed.ToString()
                && x.CoverMemoRefNo == null
                && x.RequesterId == requesterId
               && (!string.IsNullOrEmpty(x.SendMethod))
                && (!string.IsNullOrEmpty(x.BankDrafNoIssued))
                && x.ApplicationType == Data.BDType.WangCagaran.ToString()).ToList(); //RefNo;

            return new JsonResult(result.ToList());
        }
        public JsonResult GetAllWHReferenceNoForBulk(string requesterId)
        {
            var result = Db.BankDraft.Select(x => new { Id = x.RefNo, Name = x.RefNo, Status = x.Status, ApplicationType = x.Type, CoverMemoRefNo = x.CoverMemoRefNo, x.RequesterId, x.SendMethod, x.BankDrafNoIssued })
                .Where(x => x.Status == Data.Status.Processed.ToString()
                && x.CoverMemoRefNo == null
                && x.RequesterId == requesterId
                && (!string.IsNullOrEmpty(x.SendMethod))
                && (!string.IsNullOrEmpty(x.BankDrafNoIssued))
                && x.ApplicationType == Data.BDType.WangHangus.ToString()).ToList(); //RefNo;

            return new JsonResult(result.ToList());
        }

        public JsonResult GetAllWHReferenceNo()
        {
            var result = Db.BankDraft.Select(x => new { Id = x.RefNo, Name = x.RefNo, Status = x.Status, ApplicationType = x.Type, CoverMemoRefNo = x.CoverMemoRefNo, x.SendMethod, x.BankDrafNoIssued })
                .Where(x => x.Status == Data.Status.Processed.ToString()
                //&& (x.SendMethod != null || x.BankDrafNoIssued != null)
                && x.CoverMemoRefNo == null
                && x.ApplicationType == Data.BDType.WangHangus.ToString()).ToList(); //RefNo;

            return new JsonResult(result.ToList());
        }

        public JsonResult GetAllWCReferenceNo()
        {
            var result = Db.BankDraft.Select(x => new { Id = x.RefNo, Name = x.RefNo, Status = x.Status, ApplicationType = x.Type, CoverMemoRefNo = x.CoverMemoRefNo, x.SendMethod, x.BankDrafNoIssued })
                .Where(x => x.Status == Data.Status.Processed.ToString()
                //&& (x.SendMethod != null || x.BankDrafNoIssued != null)
                && x.CoverMemoRefNo == null
                && x.ApplicationType == Data.BDType.WangCagaran.ToString()).ToList(); //RefNo;

            return new JsonResult(result.ToList());
        }

        public JsonResult GetAllCoverMemo(string _CoverRefNo = null, string _ReferenceNo = null, string _Status = null)
        {
            var result1 = Db.Memo
                     .Join(Db.BankDraft,
                      m => m.CoverRefNo,
                      b => b.CoverMemoRefNo,
                      (m, b) => new { mm = m, bd = b })
                     .Join(Db.Attachment,
                     b => b.bd.Id,
                     a => a.ParentId,
                     (b, a) => new
                     {
                         id = b.mm.Id,
                         coverRefNo = b.mm.CoverRefNo,
                         referenceNo = b.mm.ReferenceNo,
                         status = b.mm.Status,
                         createdOn = b.mm.CreatedOn,
                         signedMemo = a.FileName,
                         fileType = a.FileType
                     })
                    .Where(x =>
                        (x.coverRefNo.Contains(_CoverRefNo) || _CoverRefNo == null)
                        && (x.referenceNo.Contains(_ReferenceNo) || _ReferenceNo == null)
                        && (x.status != "Processed")
                        && (x.status == _Status || _Status == null)
                        && (x.referenceNo != "")
                        && (x.status == "Issued" && x.fileType == "Memo")
                        )
                     .ToList();

            var result2 = Db.Memo
                .Select(x => new {
                    id = x.Id,
                    coverRefNo = x.CoverRefNo,
                    referenceNo = x.ReferenceNo,
                    status = x.Status,
                    createdOn = x.CreatedOn,
                    signedMemo = "",
                    fileType = ""
                })
                .Where(x =>
                        (x.coverRefNo.Contains(_CoverRefNo) || _CoverRefNo == null)
                        && (x.referenceNo.Contains(_ReferenceNo) || _ReferenceNo == null)
                        && (x.status != "Processed")
                        && (x.status == _Status || _Status == null)
                        && (x.referenceNo != "")
                        && (x.status == "Issuing")
                        )
              .ToList();

            var result3 = result1.Concat(result2).OrderByDescending(x => x.createdOn);

            var result = result3
             .GroupBy(r => new { r.coverRefNo })
             .Select(group => new
             {
                 fee = group.Key,
                 id = group.Select(r => r.id).FirstOrDefault(),
                 coverRefNo = group.Select(r => r.coverRefNo).FirstOrDefault(),
                 referenceNo = group.Select(r => r.referenceNo).FirstOrDefault(),
                 status = group.Select(r => r.status).FirstOrDefault(),
                 createdOn = group.Select(r => r.createdOn).FirstOrDefault(),
                 signedMemo = group.Select(r => r.signedMemo).FirstOrDefault(),
                 fileType = group.Select(r => r.fileType).FirstOrDefault(),
             })
             .OrderByDescending(x => x.createdOn);

            return new JsonResult(result.ToList());
        }


        public JsonResult GetCoverMemo(string refNo)
        {
            var result = Db.Memo.Where(x => x.ReferenceNo == refNo).FirstOrDefault();

            return new JsonResult(result);
        }

        public JsonResult GetCoverMemoForBulk(string refNo)
        {
            var result = Db.Memo.Where(x => x.ReferenceNo.Contains(refNo)).FirstOrDefault();

            return new JsonResult(result);
        }

        public IActionResult Details()
        {
            return View();
        }
        public IActionResult BulkBankDraft()
        {
            return View();
        }

        public IActionResult BulkBankDraft2()
        {
            return View();
        }

        public IActionResult Memo(string CoverMemoRefNo = null)
        {

            if (CoverMemoRefNo != null)
            {
                var model = new MemoViewModel();
                IEnumerable<PenerimaViewModel> bd = null;
                var item = Db.Memo.Where(x => x.CoverRefNo == CoverMemoRefNo).FirstOrDefault();
                if (item != null)
                {

                    model.Id = item.Id.ToString();
                    model.MemoDate = Convert.ToDateTime(item.Date).ToString("dd MMMM yyyy");
                    model.ReferenceNo = item.ReferenceNo;
                    model.CoverRefNo = item.CoverRefNo;
                    model.ApplicationType = item.ApplicationType;
                    model.RujukanNo = item.RujukanNo;

                    var wangs = Db.BankDraft.Where(x => x.CoverMemoRefNo == model.CoverRefNo).ToList();

                    var approveDate = wangs.Select(x => x.ApprovedOn).FirstOrDefault();
                    var requestor = wangs.Select(x => x.RequesterId).FirstOrDefault();
                    var requestorDetails = _userManager.Users.Where(x => x.Id == requestor).FirstOrDefault();
                    model.Requestor = requestorDetails.FullName;
                    model.Approver = item.Approver;
                    model.UP = item.UP;
                    model.Line1 = item.Line1 != null ?  item.Line1.Replace("\n", "<br>") : "";
                    model.ApproverAddress = item.ApproverAddress != null ? item.ApproverAddress.Replace("\n", "<br>") : "";
                    model.RequestorAddress = item.RequestorAddress != null ? item.RequestorAddress.Replace("\n", "<br>") : "";
                    model.Signiture = item.Signiture != null ?  item.Signiture.Replace("\n","<br>") : "";

                    foreach (var wang in wangs)
                    {
                        if (model.ApplicationType == Data.BDType.WangCagaran.ToString())
                        {
                            bd = (bd ?? Enumerable.Empty<PenerimaViewModel>())
                                        .Concat(Db.BankDraft
                                            .Join(Db.WangCagaran,
                                                    b => b.Id,
                                                    w => w.BankDraftId,
                                                    (b, w) => new PenerimaViewModel
                                                    {
                                                        CoverMemoRefNo = b.CoverMemoRefNo,
                                                        BankDraftId = b.Id.ToString(),
                                                        NamaPenerima = w.NamaPemegangCagaran,
                                                        BankDraftDate = b.BankDraftDate.ToString(),
                                                        BankDraftNo = b.BankDrafNoIssued,
                                                        ProjectNo = w.WBSProjekNo,
                                                        Jumlah = w.Jumlah
                                                    }
                                                )
                                                .Where(x => x.BankDraftId == wang.Id.ToString()).ToList());
                        }
                        else if (model.ApplicationType == Data.BDType.WangHangus.ToString())
                        {
                            bd = (bd ?? Enumerable.Empty<PenerimaViewModel>())
                                        .Concat(Db.BankDraft
                                            .Join(Db.WangHangus,
                                                    b => b.Id,
                                                    w => w.BankDraftId,
                                                    (b, w) => new PenerimaViewModel
                                                    {
                                                        CoverMemoRefNo = b.CoverMemoRefNo,
                                                        BankDraftId = b.Id.ToString(),
                                                        BankDraftDate = b.BankDraftDate.ToString(),
                                                        BankDraftNo = b.BankDrafNoIssued,
                                                        NamaPenerima = w.VendorName,
                                                        ApplicationRefNo = b.RefNo,
                                                        ProjectNo = b.ProjectNo,
                                                        //Db.AccountingTable
                                                        //                    .Where(a => a.WangHangusId == w.Id)
                                                        //                    .Select(s => s.CONWNo).FirstOrDefault(),
                                                        Jumlah = w.Amount
                                                        
                                                        //AccountingTable
                                                        //                    .Where(a => a.WangHangusId == w.Id)
                                                        //                    .GroupBy(g => g.WangHangusId)
                                                        //                    .Select(s => s.Sum(g => Convert.ToDecimal(g.Amount)))
                                                        //                    .FirstOrDefault()
                                                    }
                                                )
                                                .Where(x => x.BankDraftId == wang.Id.ToString()).ToList());

                        }
                    }
                    model.Line2 = item.Line2 != null ?  item.Line2.Replace("\n", "<br>") : "";
                    model.Line3 = item.Line3 != null ? item.Line3.Replace("\n", "<br>") : "";
                    model.Line4 = item.Line4 != null ? item.Line4.Replace("\n", "<br>") : "";
                    model.Line5 = item.Line5 != null ? item.Line5.Replace("\n", "<br>") : "";

                    model.Penerimas = bd;
                    model.LineETC = item.LineETC != null ? item.LineETC.Replace("\n","<br>") : "";
                    var total = bd.GroupBy(x => x.BankDraftId).Select(x => new { Total = x.Sum(a => a.Jumlah) }).FirstOrDefault();
                    model.FinalTotal = total.Total.Value;

                    return new ViewAsPdf("Memo", model)
                    {
                        PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 },
                    };
                }
            }
            return new ViewAsPdf("Memo")
            {
                PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 },
            };
        }

        //[HttpGet]
        //public IActionResult Edit(string Id = null, string CoverMemoRefNo = null)
        //{
        //    var model = new MemoViewModel();
        //    IEnumerable<PenerimaViewModel> bd = null;

        //    Guid gId = (Id == null ? Guid.NewGuid() : Guid.Parse(Id));
        //    var item = Db.Memo.Where(x => (Id == null ? x.CoverRefNo == CoverMemoRefNo : x.Id == gId)).FirstOrDefault();

        //    model.Id = item.Id.ToString();
        //    model.Date = item.Date;
        //    model.ReferenceNo = item.ReferenceNo;
        //    model.CoverRefNo = item.CoverRefNo;
        //    model.ApplicationType = item.ApplicationType;

        //    var wangs = Db.BankDraft.Where(x => x.CoverMemoRefNo == model.CoverRefNo).ToList();

        //    var approveDate = wangs.Select(x => x.ApprovedOn).FirstOrDefault();
        //    var requestor = wangs.Select(x => x.CreatedById).FirstOrDefault();
        //    var requestorDetails = _userManager.Users.Where(x => x.Id == requestor).FirstOrDefault();
        //    model.RequestorId = requestor;
        //    model.Requestor = requestorDetails.FullName;
        //    model.RequestorAddress = item.RequestorAddress != null ? item.RequestorAddress
        //                                                            : requestorDetails.FullName
        //                                                            + "\n" + requestorDetails.Designation
        //                                                            + "\n" + requestorDetails.Unit
        //                                                            + "\n" + requestorDetails.Division
        //                                                            + "\n" + "Tenaga Nasional Berhad";
        //    var l = "Surat dari {0} yang bertarikh {1} adalah dirujuk.";
        //    var line1 = string.Format(l, requestorDetails.Unit + ", " + requestorDetails.Division, Convert.ToDateTime(approveDate).ToString("dd MMMM yyyy"));
        //    model.Line1 = item.Line1 != null ? item.Line1 : line1;

        //    var approverDetails = _userManager.Users
        //                                        .Join(Db.UserRoles,
        //                                        u => u.Id,
        //                                        ur => ur.UserId,
        //                                        (u, ur) => new
        //                                        {
        //                                            UserId = u.Id,
        //                                            UserName = u.FullName,
        //                                            Unit = u.Unit,
        //                                            Division = u.Division,
        //                                            UserRole = ur.RoleId,
        //                                            IsActive = u.IsActive
        //                                        }
        //                                        )
        //                                        .Where(x => x.UserRole == "HBM" && x.IsActive == true).FirstOrDefault();

        //    //model.Approver = approverDetails.UserName;
        //    model.ApproverAddress = model.ApproverAddress != null ? model.ApproverAddress
        //                                                        : approverDetails != null ? (approverDetails.UserName
        //                                                            + "\n" + approverDetails.Unit
        //                                                            + "\n" + approverDetails.Division) : "";

        //    foreach (var wang in wangs)
        //    {
        //        if (model.ApplicationType == Data.BDType.WangCagaran.ToString())
        //        {
        //            bd = Db.BankDraft
        //                            .Join(Db.WangCagaran,
        //                                    b => b.Id,
        //                                    w => w.BankDraftId,
        //                                    (b, w) => new PenerimaViewModel
        //                                    {
        //                                        CoverMemoRefNo = b.CoverMemoRefNo,
        //                                        BankDraftId = b.Id.ToString(),
        //                                        NamaPenerima = w.NamaPemegangCagaran,
        //                                        BankDraftDate = b.BankDraftDate.ToString(),
        //                                        BankDraftNo = b.BankDrafNoIssued,
        //                                        ProjectNo = w.WBSProjekNo,
        //                                        Jumlah = w.Jumlah
        //                                    }
        //                                )
        //                                .Where(x => x.BankDraftId == wang.Id.ToString()).ToList();
        //        }
        //        else if (model.ApplicationType == Data.BDType.WangHangus.ToString())
        //        {
        //            bd = Db.BankDraft
        //                            .Join(Db.WangHangus,
        //                                    b => b.Id,
        //                                    w => w.BankDraftId,
        //                                    (b, w) => new PenerimaViewModel
        //                                    {
        //                                        CoverMemoRefNo = b.CoverMemoRefNo,
        //                                        BankDraftId = b.Id.ToString(),
        //                                        BankDraftDate = b.BankDraftDate.ToString(),
        //                                        BankDraftNo = b.BankDrafNoIssued,
        //                                        NamaPenerima = w.VendorName,
        //                                        ProjectNo = Db.AccountingTable
        //                                                            .Where(a => a.WangHangusId == w.Id)
        //                                                            .Select(s => s.CONWNo).FirstOrDefault(),
        //                                        Jumlah = w.AccountingTable
        //                                                            .Where(a => a.WangHangusId == w.Id)
        //                                                            .GroupBy(g => g.WangHangusId)
        //                                                            .Select(s => s.Sum(g => Convert.ToDecimal(g.Amount)))
        //                                                            .FirstOrDefault()
        //                                    }
        //                                )
        //                                .Where(x => x.BankDraftId == wang.Id.ToString()).ToList();

        //        }

        //    }
        //    var penimaString = string.Join(",", bd.Select(x => x.NamaPenerima));
        //    var total = bd.GroupBy(x => x.BankDraftId).Select(x => new { Total = x.Sum(a => a.Jumlah) }).FirstOrDefault();
        //    model.FinalTotal = total.Total.Value;
        //    var scnd_sk = "SGM";
        //    if (total != null)
        //    {
        //        if (Convert.ToDouble(model.FinalTotal) <= 10000000)
        //        {
        //            scnd_sk = "SM";
        //        }
        //    }

        //    var BPDetails = _userManager.Users
        //                                        .Join(Db.UserRoles,
        //                                        u => u.Id,
        //                                        ur => ur.UserId,
        //                                        (u, ur) => new
        //                                        {
        //                                            UserId = u.Id,
        //                                            UserName = u.FullName,
        //                                            Unit = u.Unit,
        //                                            Division = u.Division,
        //                                            UserRole = ur.RoleId,
        //                                            IsActive = u.IsActive
        //                                        }
        //                                        )
        //                                        .Where(x => x.UserRole == "BP" && x.IsActive == true && x.Division == requestorDetails.Division).FirstOrDefault();
        //    var SGMDetails = _userManager.Users
        //                                        .Join(Db.UserRoles,
        //                                        u => u.Id,
        //                                        ur => ur.UserId,
        //                                        (u, ur) => new
        //                                        {
        //                                            UserId = u.Id,
        //                                            UserName = u.FullName,
        //                                            Unit = u.Unit,
        //                                            Division = u.Division,
        //                                            UserRole = ur.RoleId,
        //                                            DivisionId = ur.DivisionId,
        //                                            IsActive = u.IsActive
        //                                        }
        //                                        )
        //                                        //.Join(Db.Division,
        //                                        //uur => uur.DivisionId,
        //                                        //d => d.Id,
        //                                        //(uur,d) => new
        //                                        //{
        //                                        //    UserId = uur.UserId,
        //                                        //    UserName = uur.UserName,
        //                                        //    Unit = uur.Unit,
        //                                        //    Division = uur.Division,
        //                                        //    UserRole = uur.UserRole,
        //                                        //    DivisionId = uur.DivisionId,
        //                                        //    IsActive = uur.IsActive,
        //                                        //    DivisionName =  d.Name
        //                                        //})
        //                                        .Where(x => x.IsActive == true
        //                                                && scnd_sk == "SGM" ?
        //                                                        x.UserRole == scnd_sk
        //                                                        : (scnd_sk == "SM" ? x.UserRole == scnd_sk && x.Division == "Distribution" : x.UserRole == "HZ" && x.Division == requestorDetails.Division)).FirstOrDefault();
        //    var sk = BPDetails != null ? (BPDetails.UserName
        //            + "\n" + BPDetails.Unit
        //            + "\n" + BPDetails.Division) : "";
        //    sk += SGMDetails != null ? ("\n\n" + SGMDetails.UserName
        //        + "\n" + SGMDetails.Unit
        //        + "\n" + SGMDetails.Division) : "";
        //    model.Signiture = model.Signiture != null ? model.Signiture : sk;

        //    var l2 = "Bersama-sama ini disertakan {0} keping deraf bank yang berjumlah <b>{1}</b> seperti {2}";
        //    var lampiran = bd.Count() > 5 ? "di Lampiran 1." : "berikut:";
        //    var line2 = string.Format(l2, bd.Count(), string.Format("{0:C}", model.FinalTotal), lampiran);
        //    var l3 = "Untuk makluman, TNB telah membayar wang cagaran kepada <b> {0}</b> sebanyak <b>{1}</b> setakat bulan <b>(SILA ISI BULAN DI SINI)</b>";
        //    var line3 = string.Format(l3, penimaString, string.Format("{0:C}", model.FinalTotal), Convert.ToDateTime(item.CreatedOn).ToString("MMM yyyy"));
        //    var l4 = "Pihak tuan dikehendaki untuk memastikan cagaran di atas di <b>TUNTUT</b> semula oleh <b>PEMULA</b> dengan kadar segera dari <b>{0}</b> setelah kerja-kerja diselesaikan kerana Deraf Bank merupakan aliran tunai yang dikeluarkan oleh TNB.";
        //    var line4 = string.Format(l4, penimaString);
        //    var l5 = "Selain dari itu, pihak tuan juga bertanggung jawab memastikan <b>PEMULA</b> membuat pembayaran deraf bank tersebut kepada <b>{0}</b>, serta mendapatkan resit asal pembayaran untuk disimpan bagi tujuan tuntutan setelah keraja selesai.";
        //    var line5 = string.Format(l5, penimaString);
        //    model.Line2 = item.Line2 != null ? item.Line2 : line2;
        //    model.Line3 = item.Line3 != null ? item.Line3 : line3;
        //    model.Line4 = item.Line4 != null ? item.Line4 : line4;
        //    model.Line5 = item.Line5 != null ? item.Line5 : line5;

        //    var l6 = "Oleh itu, pihak tuan adalah disaran merekodkan pemberian deraf bank dan merekodkan tarikh deraf bank dikembalikan kepada TNB."
        //            + "Pemantauan berkala perlu dilakukan dan senarai status cagaran yang masih belum dituntut perlu dimajukan kepada pemula untuk memmastikan"
        //            + "tuntutan cagaran dilakukan dengan kadar segera."
        //            + "\n\n7.  Tindakan perlu diambil ke atas pemula yang gagal menuntut cagaran di atas setelah kerja-kerja selesai tanpa sebarang justifikasi."
        //            + "Kerjasama dan tindakan pihak tuan amatlah dihargai.";

        //    model.LineETC = item.LineETC != null ? item.LineETC : l6;

        //    return View(model);
        //}

    }
}