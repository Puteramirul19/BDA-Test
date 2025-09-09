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
    public class InstructionLetterController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public InstructionLetterController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Cancellation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create(string RefNo = null)
        {
            ViewBag.RefNo = RefNo;
            //if(RefNo == null)
            //{
            //    ViewBag.ProcessingType = "Bulk";
            //}
            //else
            //{
            //    ViewBag.ProcessingType = "Normal";
            //}
            return View();
        }

        public IActionResult CreateCancellation(string RefNo = null)
        {
            var result = Db.Cancellation.Where(y => y.RefNo == RefNo && y.Status == "Accepted").FirstOrDefault(); //RefNo;

            ViewBag.BDNo = "";
            if (result != null)
            {
                ViewBag.BDNo = result.BDNo;
            }
             
            return View();
        }

        public IActionResult CreateLost(string RefNo = null)
        {
            var result = Db.Lost.Where(y => y.RefNo == RefNo && y.Status == "Accepted").FirstOrDefault(); //RefNo;

            ViewBag.BDNo = "";
            if (result != null)
            {
                ViewBag.BDNo = result.BDNo;
            }

            return View();
        }

        [HttpGet]
        public IActionResult Edit(string Id = null, string LetterRefNo = null)
        {
            var model = new InstructionLetterViewModel();
            Guid gId = (Id == null ? Guid.NewGuid() : Guid.Parse(Id));
            var item = Db.InstructionLetter.Where(x => (Id == null ? x.LetterRefNo == LetterRefNo : x.Id == gId)).FirstOrDefault();

            model.Id = item.Id.ToString();
            model.LetterDate = item.LetterDate.ToString();
            model.BankName = item.BankName;
            model.BankAccount = item.BankAccount;
            model.ChargedBankAccount = item.ChargedBankAccount;
            model.Remarks = item.Remarks;
            model.ReferenceNo = item.ReferenceNo;
            model.LetterRefNo = item.LetterRefNo;

            model.InstructionLetterEmail = item.InstructionLetterEmail;
            model.AddressLine1 = item.AddressLine1;
            model.AddressLine2 = item.AddressLine2;
            model.AddressLine3 = item.AddressLine3;
            model.State = item.State;
            model.BankAccount = item.BankAccount;
            model.BankName = item.BankName;
            model.BankPIC = item.BankPIC;
            model.ChargedBankAccount = item.ChargedBankAccount;
            model.RujukanNo = item.RujukanNo;

            if (item.LetterDate != null)
                model.LetterDate = Convert.ToDateTime(item.LetterDate).ToString("dd/MM/yyyy");
            if (item.ValueDate != null)
            {
                model.ValueDate = Convert.ToDateTime(item.ValueDate).ToString("dd/MM/yyyy");
            }
            model.RinggitText = item.RinggitText.ToLower();
            model.Remarks = item.Remarks;
            var wcwh = GetWCWHPenerima(item.LetterRefNo);
            //var jumlah = wcwh.Sum(x => x.Jumlah).Value.ToString();
            //ViewBag.Jumlah = (Convert.ToDecimal(jumlah)).ToString("#,##0.00");

            return View(model);
        }

        [HttpGet]
        public IActionResult EditCancellation(string Id = null, string LetterRefNo = null)
        {

            var model = new InstructionLetterViewModel();
            Guid gId = (Id == null ? Guid.NewGuid() : Guid.Parse(Id));
            var item = Db.InstructionLetter.Where(x => (Id == null ? x.LetterRefNo == LetterRefNo : x.Id == gId && x.ApplicationType == "Cancellation")).FirstOrDefault();

            model.Id = item.Id.ToString();
            model.LetterDate = item.LetterDate.ToString();
            model.BankName = item.BankName;
            model.BankAccount = item.BankAccount;
            model.ChargedBankAccount = item.ChargedBankAccount;
            model.Remarks = item.Remarks;
            model.ReferenceNo = item.ReferenceNo;
            model.LetterRefNo = item.LetterRefNo;

            model.InstructionLetterEmail = item.InstructionLetterEmail;
            model.AddressLine1 = item.AddressLine1;
            model.AddressLine2 = item.AddressLine2;
            model.AddressLine3 = item.AddressLine3;
            model.State = item.State;
            model.BankAccount = item.BankAccount;
            model.BankName = item.BankName;
            model.BankPIC = item.BankPIC;
            model.ChargedBankAccount = item.ChargedBankAccount;
            model.RujukanNo = item.RujukanNo;

            if (item.LetterDate != null)
                model.LetterDate = Convert.ToDateTime(item.LetterDate).ToString("dd/MM/yyyy");
            if (item.ValueDate != null)
            {
                model.ValueDate = Convert.ToDateTime(item.ValueDate).ToString("dd/MM/yyyy");
            }
            //model.RinggitText = item.RinggitText;
            model.Remarks = item.Remarks;
            var wcwh = GetCancellationPenerima(item.LetterRefNo); 
            //var jumlah = wcwh.Sum(x => x.Jumlah).Value.ToString();
            //ViewBag.Jumlah = (Convert.ToDecimal(jumlah)).ToString("#,##0.00");

            return View(model);
        }

        [HttpGet]
        public IActionResult EditLost(string Id = null, string LetterRefNo = null)
        {
            var result = Db.Lost.Where(y => y.InstructionLetterRefNo == LetterRefNo).FirstOrDefault(); //RefNo;
            ViewBag.BDNo = result.BDNo;

            var model = new InstructionLetterViewModel();
            Guid gId = (Id == null ? Guid.NewGuid() : Guid.Parse(Id));
            var item = Db.InstructionLetter.Where(x => (Id == null ? x.LetterRefNo == LetterRefNo : x.Id == gId && x.ApplicationType == "Lost")).FirstOrDefault();

            model.Id = item.Id.ToString();
            model.LetterDate = item.LetterDate.ToString();
            model.BankName = item.BankName;
            model.BankAccount = item.BankAccount;
            model.ChargedBankAccount = item.ChargedBankAccount;
            model.Remarks = item.Remarks;
            model.ReferenceNo = item.ReferenceNo;
            model.LetterRefNo = item.LetterRefNo;

            model.InstructionLetterEmail = item.InstructionLetterEmail;
            model.AddressLine1 = item.AddressLine1;
            model.AddressLine2 = item.AddressLine2;
            model.AddressLine3 = item.AddressLine3;
            model.State = item.State;
            model.BankAccount = item.BankAccount;
            model.BankName = item.BankName;
            model.BankPIC = item.BankPIC;
            model.ChargedBankAccount = item.ChargedBankAccount;
            if (item.LetterDate != null)
                model.LetterDate = Convert.ToDateTime(item.LetterDate).ToString("dd/MM/yyyy");
            if (item.ValueDate != null)
            {
                model.ValueDate = Convert.ToDateTime(item.ValueDate).ToString("dd/MM/yyyy");
            }
            //model.RinggitText = item.RinggitText;
            model.Remarks = item.Remarks;
            var wcwh = GetLostPenerima(item.LetterRefNo);
            //var jumlah = wcwh.Sum(x => x.Jumlah).Value.ToString();
            //ViewBag.Jumlah = (Convert.ToDecimal(jumlah)).ToString("#,##0.00");

            return View(model);
        }

        public IActionResult SubmitToBank(string Id)
        {
            var model = new InstructionLetterViewModel();
            var item = Db.InstructionLetter.Where(x => x.Id == Guid.Parse(Id)).FirstOrDefault();

            model.Id = item.Id.ToString();
            model.LetterDate = item.LetterDate.ToString();
            model.BankName = item.BankName;
            model.BankAccount = item.BankAccount;
            model.ChargedBankAccount = item.ChargedBankAccount;
            model.Remarks = item.Remarks;
            model.ReferenceNo = item.ReferenceNo;
            model.LetterRefNo = item.LetterRefNo;

            return View(model);
        }

        [HttpPost]
        public JsonResult Edit(InstructionLetterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = Db.InstructionLetter.Find(Guid.Parse(model.Id));
                if (entity != null)
                {

                    try
                    {
                        entity.LetterDate = DateTime.ParseExact(model.LetterDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);//Convert.ToDateTime(model.LetterDate);
                        entity.BankName = model.BankName;
                        entity.BankAccount = model.BankAccount;
                        entity.AddressLine1 = model.AddressLine1;
                        entity.AddressLine2 = model.AddressLine2;
                        entity.AddressLine3 = model.AddressLine3;
                        entity.State = model.State;
                        entity.ValueDate = DateTime.ParseExact(model.ValueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);//Convert.ToDateTime(model.ValueDate);
                        entity.BankPIC = model.BankPIC;
                        entity.RinggitText = model.RinggitText.ToLower();
                        entity.BankName = model.BankName;
                        entity.BankAccount = model.BankAccount;
                        entity.InstructionLetterEmail = model.InstructionLetterEmail;
                        entity.ChargedBankAccount = model.ChargedBankAccount;
                        entity.Remarks = model.Remarks;
                        entity.ReferenceNo = string.Join(", ",model.ReferenceNo);
                        entity.Status = Data.Status.Processing.ToString();
                        entity.UpdatedOn = DateTime.Now;
                        entity.RujukanNo = model.RujukanNo;

                        Db.SetModified(entity);
                        Db.SaveChanges();

                        ResetBDInstructionLetter(entity.LetterRefNo, entity.ReferenceNo, model.ValueDate);
                        return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Update Instruction Letter Successful!", letterRefNo = entity.LetterRefNo, letterId = entity.Id });
                    }
                    catch (Exception e)
                    {
                        return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });

                    }
                }
                else
                {

                    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Error! Instruction Letter Invalid!" });
                }
            }
            else
            {
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Error! Please try again later." });
            }
        }

        [HttpPost]
        public JsonResult EditCancellation(InstructionLetterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = Db.InstructionLetter.Find(Guid.Parse(model.Id));
                if (entity != null)
                {

                    try
                    {
                        entity.LetterDate = DateTime.ParseExact(model.LetterDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);//Convert.ToDateTime(model.LetterDate);
                        entity.BankName = model.BankName;
                        entity.BankAccount = model.BankAccount;
                        entity.AddressLine1 = model.AddressLine1;
                        entity.AddressLine2 = model.AddressLine2;
                        entity.AddressLine3 = model.AddressLine3;
                        entity.State = model.State;
                        entity.ValueDate = DateTime.ParseExact(model.ValueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);//Convert.ToDateTime(model.ValueDate);
                        entity.BankPIC = model.BankPIC;
                        entity.BankName = model.BankName;
                        entity.BankAccount = model.BankAccount;
                        entity.InstructionLetterEmail = model.InstructionLetterEmail;
                        entity.ChargedBankAccount = model.ChargedBankAccount;
                        entity.Remarks = model.Remarks;
                        entity.ReferenceNo = string.Join(", ", model.ReferenceNo);
                        entity.Status = Data.Status.Processing.ToString();
                        entity.UpdatedOn = DateTime.Now;
                        entity.RujukanNo = model.RujukanNo;

                        Db.SetModified(entity);
                        Db.SaveChanges();

                        ResetCancellationInstructionLetter(entity.LetterRefNo, entity.ReferenceNo);
                        return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Update Instruction Letter Successful!", letterRefNo = entity.LetterRefNo, letterId = entity.Id });
                    }
                    catch (Exception e)
                    {
                        return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });

                    }
                }
                else
                {

                    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Error! Instruction Letter Invalid!" });
                }
            }
            else
            {
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Error! Please try again later." });
            }
        }

        [HttpPost]
        public JsonResult EditLost(InstructionLetterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = Db.InstructionLetter.Find(Guid.Parse(model.Id));
                if (entity != null)
                {

                    try
                    {
                        entity.LetterDate = DateTime.ParseExact(model.LetterDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);//Convert.ToDateTime(model.LetterDate);
                        entity.BankName = model.BankName;
                        entity.BankAccount = model.BankAccount;
                        entity.AddressLine1 = model.AddressLine1;
                        entity.AddressLine2 = model.AddressLine2;
                        entity.AddressLine3 = model.AddressLine3;
                        entity.State = model.State;
                        entity.ValueDate = DateTime.ParseExact(model.ValueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);//Convert.ToDateTime(model.ValueDate);
                        entity.BankPIC = model.BankPIC;
                        entity.BankName = model.BankName;
                        entity.BankAccount = model.BankAccount;
                        entity.InstructionLetterEmail = model.InstructionLetterEmail;
                        entity.ChargedBankAccount = model.ChargedBankAccount;
                        entity.Remarks = model.Remarks;
                        entity.ReferenceNo = string.Join(", ", model.ReferenceNo);
                        entity.Status = Data.Status.Processing.ToString();
                        entity.UpdatedOn = DateTime.Now;

                        Db.SetModified(entity);
                        Db.SaveChanges();

                        ResetLostInstructionLetter(entity.LetterRefNo, entity.ReferenceNo);
                        return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Update Instruction Letter Successful!", letterRefNo = entity.LetterRefNo, letterId = entity.Id });
                    }
                    catch (Exception e)
                    {
                        return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });

                    }
                }
                else
                {

                    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Error! Instruction Letter Invalid!" });
                }
            }
            else
            {
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Error! Please try again later." });
            }
        }

        public IActionResult Details()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Letter(string LetterRefNo = null)
        {
            if(LetterRefNo != null)
            {
                var letter = Db.InstructionLetter.Where(x => x.LetterRefNo == LetterRefNo && x.ApplicationType == "Application").FirstOrDefault();
                if (letter != null)
                {
                    var model = new InstructionLetterViewModel();
                    model.AddressLine1 = letter.AddressLine1;
                    model.AddressLine2 = letter.AddressLine2;
                    model.AddressLine3 = letter.AddressLine3;
                    model.State = letter.State;
                    model.BankAccount = letter.BankAccount;
                    model.BankName = letter.BankName;
                    model.BankPIC = letter.BankPIC;
                    model.ChargedBankAccount = letter.ChargedBankAccount;

                    if(letter.RujukanNo != null)
                    {
                        model.RujukanNo = letter.RujukanNo;
                    }
                    else
                    {
                        model.RujukanNo = "TNB/TGBS/SD/FS/3/15/13 TF BD";
                    }
             
                    if(letter.LetterDate != null)
                        model.LetterDate = Convert.ToDateTime(letter.LetterDate).ToString("dd MMMM yyyy");
                    if (letter.ValueDate != null)
                    {
                        //model.ValueDate = Convert.ToDateTime(letter.ValueDate).ToShortDateString();
                        //model.ValueDateDay = strHari(Convert.ToDateTime(letter.ValueDate).ToString("dddd")).ToUpper();
                        model.ValueDate = Convert.ToDateTime(letter.ValueDate).ToString("dd.MM.yyyy");
                        model.ValueDateDay = Convert.ToDateTime(letter.ValueDate).ToString("dddd").ToUpper();
                    }
                    model.RinggitText = letter.RinggitText.ToLower();
                    model.Remarks = letter.Remarks == null ? "" : letter.Remarks.ToUpper();
                    model.Penerimas = GetWCWHPenerima(LetterRefNo);

                    return new ViewAsPdf("Letter", model)
                    {
                        PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 },
                    };
                }
            }

            return new ViewAsPdf("Letter")
            {
                PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 },
            };
        }

        [HttpGet]
        public IActionResult CancellationLetter(string LetterRefNo = null)
        {
            if (LetterRefNo != null)
            {
                var letter = Db.InstructionLetter.Where(x => x.LetterRefNo == LetterRefNo && x.ApplicationType == "Cancellation").FirstOrDefault();
                if (letter != null)
                {
                    var model = new InstructionLetterViewModel();
                    model.AddressLine1 = letter.AddressLine1;
                    model.AddressLine2 = letter.AddressLine2;
                    model.AddressLine3 = letter.AddressLine3;
                    model.State = letter.State;
                    model.BankAccount = letter.BankAccount;
                    model.BankName = letter.BankName;
                    model.BankPIC = letter.BankPIC;
                    model.ChargedBankAccount = letter.ChargedBankAccount;

                    if (letter.RujukanNo != null)
                    {
                        model.RujukanNo = letter.RujukanNo;
                    }
                    else
                    {
                        model.RujukanNo = "TNB/TGBS/SD/FS/3/15/13 TF BD";
                    }

                    if (letter.LetterDate != null)
                        model.LetterDate = Convert.ToDateTime(letter.LetterDate).ToString("dd MMMM yyyy");
                    if (letter.ValueDate != null)
                    {
                        model.ValueDate = Convert.ToDateTime(letter.ValueDate).ToString("dd.MM.yyyy");
                        model.ValueDateDay = Convert.ToDateTime(letter.ValueDate).ToString("dddd").ToUpper();
                            //strHari(Convert.ToDateTime(letter.ValueDate).ToString("dddd")).ToUpper();
                    }
                    //model.RinggitText = letter.RinggitText;
                    model.Remarks = letter.Remarks == null ? "" : letter.Remarks.ToUpper();
                    model.Penerimas = GetCancellationPenerima(LetterRefNo);

                    return new ViewAsPdf("CancellationLetter", model)
                    {
                        PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 },
                    };
                }
            }

            return new ViewAsPdf("CancellationLetter")
            {
                PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 },
            };
        }

        [HttpGet]
        public IActionResult LostLetter(string LetterRefNo = null)
        {
            if (LetterRefNo != null)
            {
                var letter = Db.InstructionLetter.Where(x => x.LetterRefNo == LetterRefNo && x.ApplicationType == "Lost").FirstOrDefault();
                if (letter != null)
                {
                    var model = new InstructionLetterViewModel();
                    model.AddressLine1 = letter.AddressLine1;
                    model.AddressLine2 = letter.AddressLine2;
                    model.AddressLine3 = letter.AddressLine3;
                    model.State = letter.State;
                    model.BankAccount = letter.BankAccount;
                    model.BankName = letter.BankName;
                    model.BankPIC = letter.BankPIC;
                    model.ChargedBankAccount = letter.ChargedBankAccount;
                    if (letter.LetterDate != null)
                        model.LetterDate = Convert.ToDateTime(letter.LetterDate).ToString("dd MMMM yyyy");
                    if (letter.ValueDate != null)
                    {
                        model.ValueDate = Convert.ToDateTime(letter.ValueDate).ToString("dd.MM.yyyy");
                            //.ToShortDateString();
                        model.ValueDateDay = Convert.ToDateTime(letter.ValueDate).ToString("dddd").ToUpper();
                        //strHari(Convert.ToDateTime(letter.ValueDate).ToString("dddd")).ToUpper();
                    }
                    //model.RinggitText = letter.RinggitText;
                    model.Remarks = letter.Remarks;
                    model.Penerimas = GetLostPenerima(LetterRefNo);

                    return new ViewAsPdf("LostLetter", model)
                    {
                        PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 },
                    };
                }
            }

            return new ViewAsPdf("LostLetter")
            {
                PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 },
            };
        }

        private string strBulan(string strValue)
        {
            if (strValue == "" || strValue == null) strValue = DateTime.Now.ToString("dd MMMM yyyy ");

            string[] Month = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            string[] Bulan = { "Januari", "Februari", "Mac", "April", "Mei", "Jun", "Julai", "Ogos", "September", "Oktober", "November", "Disember" };

            string strReplaced = "";
            string m = "";

            foreach (string x in Month)
            {
                if (strValue.Contains(x))
                {
                    m = x;
                }
            }

            int i;
            for (i = 0; i < Month.Length; i++)
            {
                if (m == Month[i])
                {
                    return strValue.Replace(Month[i], Bulan[i]);
                }

            }
            return strReplaced;
        }
        private string strHari(string strValue)
        {
            string[] Days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            string[] Hari = { "Isnin", "Selasa", "Rabu", "Khamis", "Jumaat", "Sabtu", "Ahad" };
            string strReplaced = "";
            string d = "";

            foreach (string x in Days)
            {
                if (strValue.Contains(x))
                {
                    d = x;
                }
            }


            int i;
            for (i = 0; i < Days.Length; i++)
            {
                if (d == Days[i])
                    return strValue.Replace(Days[i], Hari[i]);
            }


            return strReplaced;
        }
        private IEnumerable<PenerimaViewModel> GetWCWHPenerima(string LetterRefNo = null, string RefNo = null)
        {
            IEnumerable<PenerimaViewModel> wc = null;
            IEnumerable<PenerimaViewModel> wh = null;
            IEnumerable<PenerimaViewModel> wcwh = null;
            if(RefNo == null)
            {
                var wangs = Db.BankDraft.Where(x => x.InstructionLetterRefNo == LetterRefNo).ToList();
                foreach (var wang in wangs)
                {
                    if (wang.Type == Data.BDType.WangCagaran.ToString())
                    {
                        wc = (wc ?? Enumerable.Empty<PenerimaViewModel>())
                                            .Concat(Db.BankDraft
                                            .Join(Db.WangCagaran,
                                                  b => b.Id,
                                                  w => w.BankDraftId,
                                                  (b, w) => new PenerimaViewModel
                                                  {
                                                      InstructionLetterRefNo = b.InstructionLetterRefNo,
                                                      BankDraftId = b.Id.ToString(),
                                                      NamaPenerima = w.NamaPemegangCagaran.ToUpper(),
                                                      Tempat = w.Negeri.ToUpper(),
                                                      Jumlah = w.Jumlah
                                                  }
                                              )
                                              .Where(x => x.BankDraftId == wang.Id.ToString()).ToList());
                    }
                    else if (wang.Type == Data.BDType.WangHangus.ToString())
                    {
                        wh = (wh ?? Enumerable.Empty<PenerimaViewModel>())
                                            .Concat(Db.BankDraft
                                            .Join(Db.WangHangus,
                                                  b => b.Id,
                                                  w => w.BankDraftId,
                                                  (b, w) => new PenerimaViewModel
                                                  {
                                                      InstructionLetterRefNo = b.InstructionLetterRefNo,
                                                      BankDraftId = b.Id.ToString(),
                                                      NamaPenerima = w.VendorName.ToUpper(),
                                                      Tempat = w.Region.ToUpper(),
                                                      //Db.State.Where(y => y.Code == w.Region)
                                                                //.Select(s => s.Name).FirstOrDefault().ToUpper(),
                                                      Jumlah = w.AccountingTable
                                                                .Where(a => a.WangHangusId == w.Id)
                                                                .GroupBy(g => g.WangHangusId)
                                                                .Select(s => s.Sum(g => Convert.ToDecimal(g.Amount)))
                                                                .FirstOrDefault()
                                                  }
                                              )
                                              .Where(x => x.BankDraftId == wang.Id.ToString()).ToList());

                    }
                    wcwh = ((wc ?? Enumerable.Empty<PenerimaViewModel>()).Concat(wh ?? Enumerable.Empty<PenerimaViewModel>()));
                }
                return wcwh;
            }
            else
            {
                var bdRefNo = RefNo.Split(",");
                foreach(var bd in bdRefNo)
                {
                    var wang = Db.BankDraft.Where(x => x.RefNo == bd).FirstOrDefault();
                    if (wang.Type == Data.BDType.WangCagaran.ToString())
                    {
                        wc = (wc ?? Enumerable.Empty<PenerimaViewModel>())
                                            .Concat(Db.BankDraft
                                            .Join(Db.WangCagaran,
                                                  b => b.Id,
                                                  w => w.BankDraftId,
                                                  (b, w) => new PenerimaViewModel
                                                  {
                                                      InstructionLetterRefNo = b.InstructionLetterRefNo,
                                                      BankDraftId = b.Id.ToString(),
                                                      NamaPenerima = w.NamaPemegangCagaran.ToUpper(),
                                                      Tempat = w.Negeri.ToUpper(),
                                                      Jumlah = w.Jumlah
                                                  }
                                              )
                                              .Where(x => x.BankDraftId == wang.Id.ToString()).ToList());
                    }
                    else if (wang.Type == Data.BDType.WangHangus.ToString())
                    {
                        wh = (wh ?? Enumerable.Empty<PenerimaViewModel>())
                                            .Concat(Db.BankDraft
                                            .Join(Db.WangHangus,
                                                  b => b.Id,
                                                  w => w.BankDraftId,
                                                  (b, w) => new PenerimaViewModel
                                                  {
                                                      InstructionLetterRefNo = b.InstructionLetterRefNo,
                                                      BankDraftId = b.Id.ToString(),
                                                      NamaPenerima = w.VendorName.ToUpper(),
                                                      Tempat = w.Region.ToUpper(),
                                                      //Db.State.Where(y => y.Code == w.Region)
                                                      //          .Select(s => s.Name).FirstOrDefault().ToUpper(),
                                                      Jumlah = w.AccountingTable
                                                                .Where(a => a.WangHangusId == w.Id)
                                                                .GroupBy(g => g.WangHangusId)
                                                                .Select(s => s.Sum(g => Convert.ToDecimal(g.Amount)))
                                                                .FirstOrDefault()
                                                  }
                                              )
                                              .Where(x => x.BankDraftId == wang.Id.ToString()).ToList());

                    }
                }
                wcwh = ((wc ?? Enumerable.Empty<PenerimaViewModel>()).Concat(wh ?? Enumerable.Empty<PenerimaViewModel>()));
                return wcwh;
            }
        }

        private IEnumerable<PenerimaViewModel> GetCancellationPenerima(string LetterRefNo = null, string RefNo = null)
        {
            IEnumerable<PenerimaViewModel> cla = null;
            IEnumerable<PenerimaViewModel> cla1 = null;
            IEnumerable<PenerimaViewModel> cla2 = null;

            if (RefNo == null)
            {
                var cancs = Db.Cancellation.Where(x => x.InstructionLetterRefNo == LetterRefNo).ToList();

                foreach (var canc in cancs)
                {
                    cla1 = (cla1 ?? Enumerable.Empty<PenerimaViewModel>())
                        .Concat(Db.Cancellation
                        .Join(Db.BankDraft,
                        c => c.BankDraftId,
                        b => b.Id,
                        (c, b) => new { canc = c, bd = b })
                       .Join(Db.WangCagaran,
                        cx => cx.bd.Id,
                        w => w.BankDraftId,
                        (cx, w) => new PenerimaViewModel
                         {
                                InstructionLetterRefNo = cx.canc.InstructionLetterRefNo,
                                BankDraftNo = cx.canc.BDNo,
                                BankDraftId = cx.canc.Id.ToString(),
                                NamaPenerima = cx.canc.NameOnBD.ToUpper(),
                                Tempat = w.Negeri,
                                Jumlah = cx.canc.BankDraft.BankDraftAmount
                         })
                      .Where(c => c.BankDraftId == canc.Id.ToString())
                      .ToList());

                    cla2 = (cla2 ?? Enumerable.Empty<PenerimaViewModel>())
                        .Concat(Db.Cancellation
                        .Join(Db.BankDraft,
                        c => c.BankDraftId,
                        b => b.Id,
                        (c, b) => new { canc = c, bd = b })
                       .Join(Db.WangHangus,
                        cx => cx.bd.Id,
                        w => w.BankDraftId,
                        (cx, w) => new PenerimaViewModel
                        {
                            InstructionLetterRefNo = cx.canc.InstructionLetterRefNo,
                            BankDraftNo = cx.canc.BDNo,
                            BankDraftId = cx.canc.Id.ToString(),
                            NamaPenerima = cx.canc.NameOnBD.ToUpper(),
                            Tempat = w.Region,
                            Jumlah = cx.canc.BankDraft.BankDraftAmount
                        })
                      .Where(c => c.BankDraftId == canc.Id.ToString())
                      .ToList());

                    //cla = (cla ?? Enumerable.Empty<PenerimaViewModel>())
                    //   .Concat(Db.Cancellation
                    //    .Join(Db.BusinessArea,
                    //         c => c.BA,
                    //         ba => ba.Name,
                    //         (c, ba) => new { c = c, ba = ba })
                    //         .Join(Db.State,
                    //         ba => ba.ba.StateId,
                    //         s => s.Id,
                    //         (ba, s) => new PenerimaViewModel
                    //         {
                    //             InstructionLetterRefNo = ba.c.InstructionLetterRefNo,
                    //             BankDraftNo = ba.c.BDNo,
                    //             BankDraftId = ba.c.Id.ToString(),
                    //             NamaPenerima = ba.c.NameOnBD.ToUpper(),
                    //             Tempat = s.Name,
                    //             Jumlah = ba.c.BankDraft.BankDraftAmount
                    //         })
                    // .Where(c => c.BankDraftId == canc.Id.ToString())
                    // .ToList());


                    //cla = (cla ?? Enumerable.Empty<PenerimaViewModel>())
                    //    .Concat(Db.Cancellation

                    //     .Select(c => new PenerimaViewModel
                    //     {
                    //                InstructionLetterRefNo = c.InstructionLetterRefNo,
                    //                BankDraftId = c.Id.ToString(),
                    //                NamaPenerima = c.NameOnBD.ToUpper(),
                    //                Tempat = "",
                    //                Jumlah = c.BankDraft.BankDraftAmount
                    //   })
                    //  .Where(c => c.InstructionLetterRefNo == LetterRefNo)
                    //  .ToList());
                }

            }
            else
            {
                var cancs = Db.Cancellation.Where(x => x.RefNo == RefNo).FirstOrDefault();

                cla1 = (cla1 ?? Enumerable.Empty<PenerimaViewModel>())
                      .Concat(Db.Cancellation
                        .Join(Db.BankDraft,
                        c => c.BankDraftId,
                        b => b.Id,
                        (c, b) => new { canc = c, bd = b })
                       .Join(Db.WangCagaran,
                        cx => cx.bd.Id,
                        w => w.BankDraftId,
                       (cx, w) => new PenerimaViewModel 
                       {
                                  InstructionLetterRefNo = cx.canc.InstructionLetterRefNo,
                                  BankDraftNo = cx.canc.BDNo,
                                  BankDraftId = cx.canc.Id.ToString(),
                                  NamaPenerima = cx.canc.NameOnBD.ToUpper(),
                                  Tempat = w.Negeri,
                                  Jumlah = cx.canc.BankDraft.BankDraftAmount
                              })
                      .Where(c => c.BankDraftId == cancs.Id.ToString())
                      .ToList());

                cla2 = (cla2 ?? Enumerable.Empty<PenerimaViewModel>())
                     .Concat(Db.Cancellation
                       .Join(Db.BankDraft,
                       c => c.BankDraftId,
                       b => b.Id,
                       (c, b) => new { canc = c, bd = b })
                      .Join(Db.WangHangus,
                       cx => cx.bd.Id,
                       w => w.BankDraftId,
                      (cx, w) => new PenerimaViewModel
                      {
                          InstructionLetterRefNo = cx.canc.InstructionLetterRefNo,
                          BankDraftNo = cx.canc.BDNo,
                          BankDraftId = cx.canc.Id.ToString(),
                          NamaPenerima = cx.canc.NameOnBD.ToUpper(),
                          Tempat = w.Region,
                          Jumlah = cx.canc.BankDraft.BankDraftAmount
                      })
                     .Where(c => c.BankDraftId == cancs.Id.ToString())
                     .ToList());

            }

            cla = cla1.Concat(cla2);
            return cla;
        }

        private IEnumerable<PenerimaViewModel> GetLostPenerima(string LetterRefNo = null, string RefNo = null)
        {
            IEnumerable<PenerimaViewModel> cla = null;

            if (RefNo == null)
            {
                var losts = Db.Lost.Where(x => x.InstructionLetterRefNo == LetterRefNo).ToList();

                foreach (var lost in losts)
                {
                    cla = (cla ?? Enumerable.Empty<PenerimaViewModel>())
                        .Concat(Db.Lost
                        .Join(Db.BankDraft,
                        l => l.BankDraftId,
                        b => b.Id,
                        (l, b) => new { lost = l, bd = b })
                       .Join(Db.WangCagaran,
                        lx => lx.bd.Id,
                        w => w.BankDraftId,
                       (lx, w) => new { l2 = lx, w2 = w })
                       .Join(Db.State,
                        l3 => l3.w2.Negeri,
                        s => s.Name,
                        (l3, s) => new PenerimaViewModel
                        {
                                  InstructionLetterRefNo = l3.l2.lost.InstructionLetterRefNo,
                                  BankDraftNo = l3.l2.lost.BDNo,
                                  BankDraftId = l3.l2.lost.Id.ToString(),
                                  NamaPenerima = l3.l2.lost.NameOnBD.ToUpper(),
                                  Tempat = s.Name,
                                  Jumlah = l3.l2.lost.BankDraft.BankDraftAmount
                              })
                      .Where(c => c.BankDraftId == lost.Id.ToString())
                      .ToList());

                }

            }
            else
            {
                var losts = Db.Lost.Where(x => x.RefNo == RefNo).FirstOrDefault();

                cla = (cla ?? Enumerable.Empty<PenerimaViewModel>())
                      .Concat(Db.Lost
                        .Join(Db.BankDraft,
                        l => l.BankDraftId,
                        b => b.Id,
                        (l, b) => new { lost = l, bd = b })
                       .Join(Db.WangCagaran,
                        lx => lx.bd.Id,
                        w => w.BankDraftId,
                       (lx, w) => new { l2 = lx, w2 = w })
                       .Join(Db.State,
                        l3 => l3.w2.Negeri,
                        s => s.Name,
                        (l3, s) => new PenerimaViewModel
                        {
                                  InstructionLetterRefNo = l3.l2.lost.InstructionLetterRefNo,
                                  BankDraftNo = l3.l2.lost.BDNo,
                                  BankDraftId = l3.l2.lost.Id.ToString(),
                                  NamaPenerima = l3.l2.lost.NameOnBD.ToUpper(),
                                  Tempat = s.Name,
                                  Jumlah = l3.l2.lost.BankDraft.BankDraftAmount
                              })
                      .Where(c => c.BankDraftId == losts.Id.ToString())
                      .ToList());

            }
            return cla;
        }

        public string GetRunningNo(string type)
        {
            RunningNo runningNo = new RunningNo();

            if (type == "Application")
            {
                var entity = Db.RunningNo.Where(x => x.Name == "InstructionLetter").FirstOrDefault(); //Id for Instruction Letter
                runningNo.Code = entity.Code;
                runningNo.RunNo = entity.RunNo;
                string NewCode = String.Format("{0}{1:00000}", runningNo.Code, runningNo.RunNo);

                entity.RunNo = entity.RunNo + 1;
                Db.RunningNo.Update(entity);
                Db.SaveChanges();

                return NewCode;
            }
            else if(type == "Cancellation")
            {
                var entity = Db.RunningNo.Where(x => x.Name == "InstructionLetterCancellation").FirstOrDefault(); //Id for Instruction Letter
                runningNo.Code = entity.Code;
                runningNo.RunNo = entity.RunNo;
                string NewCode = String.Format("{0}{1:00000}", runningNo.Code, runningNo.RunNo, "/C");

                entity.RunNo = entity.RunNo + 1;
                Db.RunningNo.Update(entity);
                Db.SaveChanges();

                return NewCode;
            }
            else 
            {
                var entity = Db.RunningNo.Where(x => x.Name == "InstructionLetterLost").FirstOrDefault(); //Id for Instruction Letter
                runningNo.Code = entity.Code;
                runningNo.RunNo = entity.RunNo;
                string NewCode = String.Format("{0}{1:00000}", runningNo.Code, runningNo.RunNo, "/L");

                entity.RunNo = entity.RunNo + 1;
                Db.RunningNo.Update(entity);
                Db.SaveChanges();

                return NewCode;
            }
           
        }

        [HttpPost]
        public JsonResult SubmitLetter(InstructionLetterViewModel model, IFormFile SignedLetter)
        {
            //if (ModelState.IsValid)
            //{
                try
                {
                    var file = Db.Attachment.Where(x => x.ParentId == Guid.Parse(model.Id) && x.FileType == Data.AttachmentType.InstructionLetter.ToString()).FirstOrDefault();
                    if (SignedLetter == null && file == null)
                    {
                        return Json(new { response = StatusCode(StatusCodes.Status204NoContent), message = "Signed Document Required!" });
                    }

                    var user = _userManager.GetUserAsync(HttpContext.User).Result;
                    var items = model.ReferenceNo.Split(",").ToList();
                    var appType = model.LetterRefNo.Contains("/C") ? "Cancellation" : "Application";
                    foreach (var item in items)
                    {
                        SaveBankDetailsByRefNo(item,model.LetterRefNo,user.Id, SignedLetter,appType);
                    }

                    var entity = Db.InstructionLetter.Where(x => x.LetterRefNo == model.LetterRefNo).FirstOrDefault();
                    entity.Status = Data.Status.Processed.ToString();
                    entity.UpdatedOn = DateTime.Now;
                    Db.SetModified(entity);
                    Db.SaveChanges();

                Job.Enqueue<Services.NotificationService>(x => x.NotifyBankForProcessing(entity.Id));

                return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft Processed Successful!" });

            }
                catch (Exception e)
                {
                    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });

                }
            //}
            //else
            //{
            //    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Error! Please try again later." });
            //}
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
        public void SaveBankDetailsByRefNo(string RefNo, string InstructionLetterRefNo, string UserId, IFormFile SignedLetter, string ApplicationType)
        {
            var _BankDraftId = "";

            if (ApplicationType == "Cancellation")
            {
                var entity = Db.Cancellation.Where(x => x.BDNo == RefNo).FirstOrDefault();
                entity.InstructionLetterRefNo = InstructionLetterRefNo;
                entity.UpdatedOn = DateTime.Now;
                entity.TGBSProcessedOn = DateTime.Now;
                entity.TGBSProcesserId = UserId;
                entity.Status = Data.Status.Processed.ToString();
             
                Db.SetModified(entity);
                Db.SaveChanges();
                _BankDraftId = entity.Id.ToString();

                UploadFile(SignedLetter, Guid.Parse(_BankDraftId), Data.AttachmentType.InstructionLetter.ToString(), Data.BDAttachmentType.SignedLetter.ToString());
            }
            else if (ApplicationType == "Lost")
            {
                var entity = Db.Lost.Where(x => x.BDNo == RefNo).FirstOrDefault();
                entity.InstructionLetterRefNo = InstructionLetterRefNo;
                entity.UpdatedOn = DateTime.Now;
                entity.TGBSProcessedOn = DateTime.Now;
                entity.TGBSProcesserId = UserId;
                entity.Status = Data.Status.Processed.ToString();
               
                Db.SetModified(entity);
                Db.SaveChanges();

                _BankDraftId = entity.Id.ToString();
                UploadFile(SignedLetter, Guid.Parse(_BankDraftId), Data.AttachmentType.InstructionLetter.ToString(), Data.BDAttachmentType.SignedLetter.ToString());
            }
           else
            {
                var entity = Db.BankDraft.Where(x => x.RefNo == RefNo).FirstOrDefault();
                entity.InstructionLetterRefNo = InstructionLetterRefNo;
                entity.UpdatedOn = DateTime.Now;
                entity.TGBSProcessedOn = DateTime.Now;
                entity.TGBSProcesserId = UserId;
                entity.Status = Data.Status.Processed.ToString();
                Db.SetModified(entity);
                Db.SaveChanges();

                _BankDraftId = entity.Id.ToString();
                UploadFile(SignedLetter, Guid.Parse(_BankDraftId), Data.AttachmentType.InstructionLetter.ToString());
            }
          
            Db.BankDraftAction.Add(new BankDraftAction
            {
                ApplicationType = ApplicationType,
                ActionType = Data.ActionType.SubmittedToBank.ToString(),
                On = DateTime.Now,
                ById = UserId,
                ParentId = Guid.Parse(_BankDraftId),
                ActionRole = Data.ActionRole.TGBSBanking.ToString(),
            });
            Db.SaveChanges();
        }

        public JsonResult GetAllInstructionLetter(string _LetterRefNo = null, string _ReferenceNo = null, string _Status = null)
        {
            var result1 = Db.InstructionLetter
                       .Join(Db.BankDraft,
                        i => i.LetterRefNo,
                        b => b.InstructionLetterRefNo,
                        (i, b) => new { il = i, bd = b })
                       .Join(Db.Attachment,
                       b => b.bd.Id,
                       a => a.ParentId,
                       (b, a) => new
                       {
                           id = b.il.Id,
                           letterRefNo = b.il.LetterRefNo,
                           referenceNo = b.il.ReferenceNo,
                           status = b.il.Status,
                           createdOn = b.il.CreatedOn,
                           applicationType = b.il.ApplicationType,
                           signedLetter = a.FileName,
                           fileType = a.FileType
                       })
                       .Where(x =>
                        (x.letterRefNo.Contains(_LetterRefNo) || _LetterRefNo == null)
                        && (x.referenceNo.Contains(_ReferenceNo) || _ReferenceNo == null)
                        && (x.status == _Status || _Status == null)
                        && (x.referenceNo != "")
                        && (x.applicationType == "Application" && x.status == "Processed" && x.fileType == "InstructionLetter")
                        )
                       //.OrderByDescending(x => x.createdOn)
                       .ToList();

            var result2 = Db.InstructionLetter
                .Select(x => new {
                    id = x.Id,
                    letterRefNo = x.LetterRefNo,
                    referenceNo = x.ReferenceNo,
                    status = x.Status,
                    createdOn = x.CreatedOn,
                    applicationType = x.ApplicationType,
                    signedLetter = "",
                    fileType = ""
                })
                .Where(x =>
                        (x.letterRefNo.Contains(_LetterRefNo) || _LetterRefNo == null)
                        && (x.referenceNo.Contains(_ReferenceNo) || _ReferenceNo == null)
                        && (x.status == _Status || _Status == null)
                        && (x.referenceNo != "")
                        && (x.applicationType == "Application" && x.status == "Processing")
                        )
                 //.OrderByDescending(x => x.CreatedOn)
              .ToList();

            var result3 = result1.Concat(result2).OrderByDescending(x => x.createdOn);

            var result = result3
             .GroupBy(r => new { r.letterRefNo})
             .Select(group => new
             {
                 fee = group.Key,
                 id = group.Select(r => r.id).FirstOrDefault(),
                 letterRefNo = group.Select(r => r.letterRefNo).FirstOrDefault(),
                 referenceNo = group.Select(r => r.referenceNo).FirstOrDefault(),
                 status = group.Select(r => r.status).FirstOrDefault(),
                 createdOn = group.Select(r => r.createdOn).FirstOrDefault(),
                 applicationType = group.Select(r => r.applicationType).FirstOrDefault(),
                 signedLetter = group.Select(r => r.signedLetter).FirstOrDefault(),
                 fileType = group.Select(r => r.fileType).FirstOrDefault(),
             })
             .OrderByDescending(x => x.createdOn);


            return new JsonResult(result.ToList());
        }

        public JsonResult GetAllInstructionLetterForCancellation(string _LetterRefNo = null, string _ReferenceNo = null, string _Status = null)
        {
            var result1 = Db.InstructionLetter
                       .Join(Db.Cancellation,
                        i => i.LetterRefNo,
                        c => c.InstructionLetterRefNo,
                        (i, c) => new { il = i, cn = c })
                       .Join(Db.Attachment,
                       c => c.cn.Id,
                       a => a.ParentId,
                       (c, a) => new
                       {
                           id = c.il.Id,
                           letterRefNo = c.il.LetterRefNo,
                           referenceNo = c.il.ReferenceNo,
                           status = c.il.Status,
                           createdOn = c.il.CreatedOn,
                           applicationType = c.il.ApplicationType,
                           signedLetter = a.FileName,
                           fileType = a.FileSubType
                       })
                       .Where(x =>
                        (x.letterRefNo.Contains(_LetterRefNo) || _LetterRefNo == null)
                        && (x.referenceNo.Contains(_ReferenceNo) || _ReferenceNo == null)
                        && (x.status == _Status || _Status == null)
                        && (x.referenceNo != "")
                        && (x.applicationType == "Cancellation" && x.status == "Processed" && x.fileType == "SignedLetter")
                        )
                       .ToList();

            var result2 = Db.InstructionLetter
              .Select(x => new {
                  id = x.Id,
                  letterRefNo = x.LetterRefNo,
                  referenceNo = x.ReferenceNo,
                  status = x.Status,
                  createdOn = x.CreatedOn,
                  applicationType = x.ApplicationType,
                  signedLetter = "",
                  fileType = ""
              })
              .Where(x =>
                      (x.letterRefNo.Contains(_LetterRefNo) || _LetterRefNo == null)
                      && (x.referenceNo.Contains(_ReferenceNo) || _ReferenceNo == null)
                      && (x.status == _Status || _Status == null)
                      && (x.referenceNo != "")
                      && (x.applicationType == "Cancellation" && x.status == "Processing")
                      )
            .ToList();

            var result3 = result1.Concat(result2).OrderByDescending(x => x.createdOn);
            var result = result3
              .GroupBy(r => new { r.letterRefNo })
              .Select(group => new
              {
                  fee = group.Key,
                  id = group.Select(r => r.id).FirstOrDefault(),
                  letterRefNo = group.Select(r => r.letterRefNo).FirstOrDefault(),
                  referenceNo = group.Select(r => r.referenceNo).FirstOrDefault(),
                  status = group.Select(r => r.status).FirstOrDefault(),
                  createdOn = group.Select(r => r.createdOn).FirstOrDefault(),
                  applicationType = group.Select(r => r.applicationType).FirstOrDefault(),
                  signedLetter = group.Select(r => r.signedLetter).FirstOrDefault(),
                  fileType = group.Select(r => r.fileType).FirstOrDefault(),
              })
              .OrderByDescending(x => x.createdOn);

            return new JsonResult(result.ToList());
        }

        [HttpPost]
        public JsonResult Create(InstructionLetterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    List<string> refNoList = model.ReferenceNo.Split(',').ToList();

                    foreach(var refNo in refNoList)
                    {
                        var letters = Db.InstructionLetter.Where(x => x.ReferenceNo == refNo && x.ApplicationType == "Application").FirstOrDefault();
                        if (letters != null)
                        {
                            return Json(new { response = StatusCode(StatusCodes.Status403Forbidden), message = "Letter " + model.LetterRefNo + " for " + refNo + " already created by another user!" });
                        }
                    }

                        var entity = new InstructionLetter
                        {
                            LetterDate = DateTime.ParseExact(model.LetterDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),//Convert.ToDateTime(model.LetterDate),
                            BankName = model.BankName,
                            BankAccount = model.BankAccount,
                            AddressLine1 = model.AddressLine1,
                            AddressLine2 = model.AddressLine2,
                            AddressLine3 = model.AddressLine3,
                            State = model.State,
                            ValueDate = DateTime.ParseExact(model.ValueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),// Convert.ToDateTime(model.ValueDate),
                            BankPIC = model.BankPIC,
                            RinggitText = model.RinggitText.ToLower(),
                            InstructionLetterEmail = model.InstructionLetterEmail,
                            ChargedBankAccount = model.ChargedBankAccount,
                            Remarks = model.Remarks,
                            ReferenceNo = string.Join(", ", model.ReferenceNo),
                            LetterRefNo = GetRunningNo("Application"),
                            ApplicationType = "Application",
                            ProcessingType = model.ReferenceNo.Contains(',') ? "Bulk" : "Single",
                            Status = Data.Status.Processing.ToString(),
                            RujukanNo = model.RujukanNo
                        };

                        Db.InstructionLetter.Add(entity);
                        Db.SaveChanges();

                        ResetBDInstructionLetter(entity.LetterRefNo, entity.ReferenceNo, model.ValueDate);

                    return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Add New Instruction Letter Successful!", letterRefNo = entity.LetterRefNo, letterId = entity.Id });
                    
                }
                catch(Exception e)
                {
                    //return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });
                    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Please fill all mandatory fields!" });

                }
            }
            else
            {
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Error! Please try again later." });
            }
        }

        [HttpPost]
        public JsonResult CreateCancellation(InstructionLetterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    List<string> refNoList = model.ReferenceNo.Split(',').ToList();

                    foreach (var refNo in refNoList)
                    {
                        var letters = Db.InstructionLetter.Where(x => x.ReferenceNo == refNo && x.ApplicationType == "Cancellation").FirstOrDefault();
                        if (letters != null)
                        {
                            return Json(new { response = StatusCode(StatusCodes.Status403Forbidden), message = "Letter " + model.LetterRefNo + " for " + refNo + " already created by another user!" });
                        }
                    }

                    var entity = new InstructionLetter
                        {
                            LetterDate = DateTime.ParseExact(model.LetterDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),//Convert.ToDateTime(model.LetterDate),
                            BankName = model.BankName,
                            BankAccount = model.BankAccount,
                            AddressLine1 = model.AddressLine1,
                            AddressLine2 = model.AddressLine2,
                            AddressLine3 = model.AddressLine3,
                            State = model.State,
                            ValueDate = DateTime.ParseExact(model.ValueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),// Convert.ToDateTime(model.ValueDate),
                            BankPIC = model.BankPIC,
                            //RinggitText = model.RinggitText,
                            InstructionLetterEmail = model.InstructionLetterEmail,
                            ChargedBankAccount = model.ChargedBankAccount,
                            Remarks = model.Remarks,
                            ReferenceNo = string.Join(", ", model.ReferenceNo),
                            ProcessingType = model.ReferenceNo.Contains(',') ? "Bulk" : "Single",
                            LetterRefNo = GetRunningNo("Cancellation") + "/C",
                            ApplicationType = "Cancellation",
                            Status = Data.Status.Processing.ToString(),
                            RujukanNo = model.RujukanNo,
                    };

                        Db.InstructionLetter.Add(entity);
                        Db.SaveChanges();

                        ResetCancellationInstructionLetter(entity.LetterRefNo, entity.ReferenceNo);
                        return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Add New Instruction Letter Successful!", letterRefNo = entity.LetterRefNo, letterId = entity.Id });
                    
                }
                catch (Exception e)
                {
                    //return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });
                    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Please fill all mandatory fields!" });

                }
            }
            else
            {
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Error! Please try again later." });
            }
        }

        [HttpPost]
        public JsonResult CreateLost(InstructionLetterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    List<string> refNoList = model.ReferenceNo.Split(',').ToList();

                    foreach (var refNo in refNoList)
                    {
                        var letters = Db.InstructionLetter.Where(x => x.ReferenceNo == refNo && x.ApplicationType == "Lost").FirstOrDefault();
                        if (letters != null)
                        {
                            return Json(new { response = StatusCode(StatusCodes.Status403Forbidden), message = "Letter " + model.LetterRefNo + " for " + refNo + " already created by another user!" });
                        }
                    }

                    var entity = new InstructionLetter
                        {
                            LetterDate = DateTime.ParseExact(model.LetterDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),//Convert.ToDateTime(model.LetterDate),
                            BankName = model.BankName,
                            BankAccount = model.BankAccount,
                            AddressLine1 = model.AddressLine1,
                            AddressLine2 = model.AddressLine2,
                            AddressLine3 = model.AddressLine3,
                            State = model.State,
                            ValueDate = DateTime.ParseExact(model.ValueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),// Convert.ToDateTime(model.ValueDate),
                            BankPIC = model.BankPIC,
                            //RinggitText = model.RinggitText,
                            InstructionLetterEmail = model.InstructionLetterEmail,
                            ChargedBankAccount = model.ChargedBankAccount,
                            Remarks = model.Remarks,
                            ReferenceNo = string.Join(", ", model.ReferenceNo),
                            ProcessingType = model.ReferenceNo.Contains(',') ? "Bulk" : "Single",
                            LetterRefNo = GetRunningNo("Lost") + "/L",
                            ApplicationType = "Lost",
                            Status = Data.Status.Processing.ToString()
                        };

                        Db.InstructionLetter.Add(entity);
                        Db.SaveChanges();

                        ResetLostInstructionLetter(entity.LetterRefNo, entity.ReferenceNo);
                        return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Add New Instruction Letter Successful!", letterRefNo = entity.LetterRefNo, letterId = entity.Id });
                   
                }
                catch (Exception e)
                {
                    //return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = e.Message });
                    return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Please fill all mandatory fields!" });

                }
            }
            else
            {
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Error! Please try again later." });
            }
        }



        public void ResetBDInstructionLetter(string LetterRefNo = null, string strBDRefNo = null,string valueDate = null)
        {
            //remove current letterrefno in bd
            var letters = Db.BankDraft.Where(x => x.InstructionLetterRefNo == LetterRefNo).ToList();
            foreach(var bank in letters)
            {
                bank.InstructionLetterRefNo = null;
                Db.SetModified(bank);
                Db.SaveChanges();
            }

            //reset bd after remove
            var items = strBDRefNo.Split(",").ToList();
            foreach(var item in items)
            {
                var bd = Db.BankDraft.Where(x => x.RefNo == item).FirstOrDefault();
                bd.InstructionLetterRefNo = LetterRefNo;
                bd.BankDraftDate = DateTime.ParseExact(valueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                Db.SetModified(bd);
                Db.SaveChanges();
            }
                
        }

        public void ResetCancellationInstructionLetter(string LetterRefNo = null, string strBDRefNo = null)
        {
            //remove current letterrefno in bd
            var letters = Db.Cancellation.Where(x => x.InstructionLetterRefNo == LetterRefNo).ToList();
            foreach (var bank in letters)
            {
                bank.InstructionLetterRefNo = null;
                Db.SetModified(bank);
                Db.SaveChanges();
            }

            //reset bd after remove
            var items = strBDRefNo.Split(",").ToList();
            foreach (var item in items)
            {
                var cancel = Db.Cancellation.Where(x => x.BDNo == item).FirstOrDefault();
                cancel.InstructionLetterRefNo = LetterRefNo;
                Db.SetModified(cancel);
                Db.SaveChanges();
            }

        }

        public void ResetLostInstructionLetter(string LetterRefNo = null, string strBDRefNo = null)
        {
            //remove current letterrefno in bd
            var letters = Db.Lost.Where(x => x.InstructionLetterRefNo == LetterRefNo).ToList();
            foreach (var bank in letters)
            {
                bank.InstructionLetterRefNo = null;
                Db.SetModified(bank);
                Db.SaveChanges();
            }

            //reset bd after remove
            var items = strBDRefNo.Split(",").ToList();
            foreach (var item in items)
            {
                var lost = Db.Lost.Where(x => x.BDNo == item).FirstOrDefault();
                lost.InstructionLetterRefNo = LetterRefNo;
                Db.SetModified(lost);
                Db.SaveChanges();
            }

        }

        public JsonResult SetBDInstructionLetter(string LetterRefNo = null, string bdRefNo = null)
        {
            if(LetterRefNo == null)
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Instruction Letter Number is Invalid!" });

            if (bdRefNo == null)
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Invalid Bank Draft Id!" });

            var entity = Db.InstructionLetter.Where(x => x.LetterRefNo == LetterRefNo).FirstOrDefault();

            if(entity != null)
            {
                if(entity.ReferenceNo != null)
                {
                    var listRefNo = entity.ReferenceNo.Split(", ").ToList();
                    if (!listRefNo.Contains(bdRefNo))
                         listRefNo.Add(bdRefNo);
                    entity.ReferenceNo = string.Join(", ",listRefNo);
                }
                else
                {
                    entity.ReferenceNo = bdRefNo;
                }
                Db.SetModified(entity);
                Db.SaveChanges();

                var bd = Db.BankDraft.Where(x => x.RefNo == bdRefNo).FirstOrDefault();
                bd.InstructionLetterRefNo = LetterRefNo;
                Db.SetModified(bd);
                Db.SaveChanges();

                return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Instruction Letter Number is Valid!" });
            }
            else
            {
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Instruction Letter Number is Invalid!" });

            }

        }

        public JsonResult SetCancellationInstructionLetter(string LetterRefNo = null, string bdRefNo = null)
        {
            if (LetterRefNo == null)
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Instruction Letter Number is Invalid!" });

            if (bdRefNo == null)
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Invalid Bank Draft Id!" });

            var entity = Db.InstructionLetter.Where(x => x.LetterRefNo == LetterRefNo && x.ApplicationType == "Cancellation").FirstOrDefault();

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

                var bd = Db.Cancellation.Where(x => x.RefNo == bdRefNo).FirstOrDefault();
                bd.InstructionLetterRefNo = LetterRefNo;
                Db.SetModified(bd);
                Db.SaveChanges();

                return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Instruction Letter Number is Valid!" });
            }
            else
            {
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Instruction Letter Number is Invalid!" });

            }

        }

        public JsonResult SetLostInstructionLetter(string LetterRefNo = null, string bdRefNo = null)
        {
            if (LetterRefNo == null)
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Instruction Letter Number is Invalid!" });

            if (bdRefNo == null)
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Invalid Bank Draft Id!" });

            var entity = Db.InstructionLetter.Where(x => x.LetterRefNo == LetterRefNo && x.ApplicationType == "Lost").FirstOrDefault();

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

                var bd = Db.Lost.Where(x => x.RefNo == bdRefNo).FirstOrDefault();
                bd.InstructionLetterRefNo = LetterRefNo;
                Db.SetModified(bd);
                Db.SaveChanges();

                return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Instruction Letter Number is Valid!" });
            }
            else
            {
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Instruction Letter Number is Invalid!" });

            }

        }

        public JsonResult RemoveBDInstructionLetter(string LetterRefNo = null, string bdRefNo = null)
        {
            if (LetterRefNo == null)
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Instruction Letter Number is Invalid!" });

            if (bdRefNo == null)
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Invalid Bank Draft Id!" });

            var entity = Db.InstructionLetter.Where(x => x.LetterRefNo == LetterRefNo).FirstOrDefault();

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

                var bd = Db.BankDraft.Where(x => x.RefNo == bdRefNo).FirstOrDefault();
                bd.InstructionLetterRefNo = null;
                Db.SetModified(bd);
                Db.SaveChanges();

                return Json(new { response = StatusCode(StatusCodes.Status200OK), message = "Bank Draft removed from Instruction Letter Successful!" });
            }
            else
            {
                return Json(new { response = StatusCode(StatusCodes.Status500InternalServerError), message = "Instruction Letter Number is Invalid!" });

            }

        }
        public JsonResult GetAllBankAccount()
        {
            var bankAcc = Db.BankDetails.Where(x => x.isActive == true && x.Type == "Application").FirstOrDefault();
          
            List<Object> BankName = new List<Object>
            {
                new { Id = bankAcc.AccountNo1, Name = bankAcc.AccountNo1 },
                new { Id = bankAcc.AccountNo2, Name = bankAcc.AccountNo2 },
                new { Id = bankAcc.AccountNo3, Name = bankAcc.AccountNo3 },
            };
            var result = BankName; //Db.Function.ToList();

            return new JsonResult(result.ToList());
        }

        public JsonResult GetAllBankAccountForCancellation()
        {
            var bankAcc = Db.BankDetails.Where(x => x.isActive == true && x.Type == "Cancellation").FirstOrDefault();

            List<Object> BankName = new List<Object>
            {
                new { Id = bankAcc.AccountNo1, Name = bankAcc.AccountNo1 },
                new { Id = bankAcc.AccountNo2, Name = bankAcc.AccountNo2 },
                new { Id = bankAcc.AccountNo3, Name = bankAcc.AccountNo3 },
            };
            var result = BankName; //Db.Function.ToList();

            return new JsonResult(result.ToList());
        }

        public JsonResult GetAllBankAccountForLost()
        {
            var bankAcc = Db.BankDetails.Where(x => x.isActive == true && x.Type == "Lost").FirstOrDefault();

            List<Object> BankName = new List<Object>
            {
                new { Id = bankAcc.AccountNo1, Name = bankAcc.AccountNo1 },
                new { Id = bankAcc.AccountNo2, Name = bankAcc.AccountNo2 },
                new { Id = bankAcc.AccountNo3, Name = bankAcc.AccountNo3 },
            };
            var result = BankName; //Db.Function.ToList();

            return new JsonResult(result.ToList());
        }

        public static string NumberToWords(double doubleNumber)
        {
            var beforeFloatingPoint = (int)Math.Floor(doubleNumber);
            var beforeFloatingPointWord = $"{NumberToWords(beforeFloatingPoint)} sahaja";
            var afterFloatingPointWord =
                $"{SmallNumberToWord((int)((doubleNumber - beforeFloatingPoint) * 100), "")} sen";

            if(afterFloatingPointWord == " sen")
            {
                return $"{beforeFloatingPointWord}";
            }
            else
            {
                return $"{beforeFloatingPointWord} dan {afterFloatingPointWord}";
            }
            
        }

        private static string NumberToWords(int number)
        {
            if (number == 0)
                return "kosong";

            if (number < 0)
                return "negatif " + NumberToWords(Math.Abs(number));

            var words = "";

            if (number / 1000000000 > 0)
            {
                words += NumberToWords(number / 1000000000) + " bilion ";
                number %= 1000000000;
            }

            if (number / 1000000 > 0)
            {
                words += NumberToWords(number / 1000000) + " juta ";
                number %= 1000000;
            }

            if (number / 1000 > 0)
            {
                words += NumberToWords(number / 1000) + " ribu ";
                number %= 1000;
            }

            if (number / 100 > 0)
            {
                words += NumberToWords(number / 100) + " ratus ";
                number %= 100;
            }

            words = SmallNumberToWord(number, words);

            return words;
        }

        private static string SmallNumberToWord(int number, string words)
        {
            if (number <= 0) return words;
            if (words != "")
                words += " ";

            var unitsMap = new[] { "kosong", "satu", "dua", "tiga", "empat", "lima", "enam", "tujuh", "lapan", "sembilan", "sepuluh", "sebelas", "dua belas", "tiga belas", "empat belas", "lima belas", "enam belas", "tujuh belas", "lapan belas", "sembilan belas" };
            var tensMap = new[] { "kosong", "sepuluh", "dua puluh", "tiga puluh", "empat puluh", "lima puluh", "enam puluh", "tujuh puluh", "lapan puluh", "sembilan puluh" };

            if (number < 20)
                words += unitsMap[number];
            else
            {
                words += tensMap[number / 10];
                if ((number % 10) > 0)
                    words += " " + unitsMap[number % 10];
            }
            return words;
        }

        public JsonResult GetBankDetails(string type = null)
        {
            var result = Db.BankDetails.Where(x => x.isActive == true && x.Type == type).FirstOrDefault();

            return new JsonResult(result);
        }

        public JsonResult GetJumlah(string RefNo)
        {
            var wcwh = GetWCWHPenerima(null, RefNo);
            var jumlah = wcwh.Sum(x => x.Jumlah).Value.ToString();
            string word = NumberToWords(Double.Parse(jumlah));

            List<string> jumlahList = new List<string>() { (Convert.ToDecimal(jumlah)).ToString("#,##0.00"), word };

            return new JsonResult(jumlahList);
            //return new JsonResult((Convert.ToDecimal(jumlah)).ToString("#,##0.00"));
        }

        public JsonResult GetInstructionLetter(string refNo)
        {
            var result = Db.InstructionLetter.Where(x => x.LetterRefNo == refNo).FirstOrDefault();

            return new JsonResult(result);
        }


        public JsonResult GetAllWCReferenceNo()
        {
            var result = Db.BankDraft.Select(x=> new { Id = x.RefNo, Name = x.RefNo, Status = x.Status, InstructionLetterRefNo = x.InstructionLetterRefNo}).Where(x => x.Status == Data.Status.Accepted.ToString() && x.InstructionLetterRefNo == null).ToList(); //RefNo;

            return new JsonResult(result.ToList());
        }

        public JsonResult GetAllCancellationBDNo()
        {
            var result = Db.Cancellation.Select(x => new { Id = x.BDNo, Name = x.BDNo, Status = x.Status, InstructionLetterRefNo = x.InstructionLetterRefNo })
                .Where(x => x.Status == Data.Status.Accepted.ToString() && x.InstructionLetterRefNo == null).ToList(); //RefNo;

            return new JsonResult(result.ToList());
        }

        public JsonResult GetAllLostBDNo()
        {
            var result = Db.Lost.Select(x => new { Id = x.BDNo, Name = x.BDNo, Status = x.Status, InstructionLetterRefNo = x.InstructionLetterRefNo }).Where(x => x.Status == Data.Status.Accepted.ToString() && x.InstructionLetterRefNo == null).ToList(); //RefNo;

            return new JsonResult(result.ToList());
        }
    }
}