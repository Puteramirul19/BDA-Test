using System;
using System.Linq;
using System.Threading.Tasks;
using BDA.Entities;
//using BDA.Services;
using BDA.Identity;
using BDA.ViewModel;
using BDA.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BDA.Controllers
{
    [Authorize]
    public class WangCagaranController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public WangCagaranController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        ////private BankDraftApplicationService service;
        //private readonly IHttpContextAccessor _contextAccessor;
        //public WangCagaranController( IFileStore fileStore, IHttpContextAccessor _contextAccessor) 
        //{
        //    //this.service = service;
        //    this._contextAccessor = _contextAccessor;
        //}

        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Executive/Engineer, Manager/Senior Engineer, Head of Zone (AD)/HOZA, Senior Manager/Lead")]
        public IActionResult Create()
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            ViewBag.Fullname = user.FullName;

            return View();
        }
        [HttpGet]
        public IActionResult Details(string id)
        {
            var model = Db.BankDraft
                            .Include("WangCagaran")
                            .Where(x=> x.Id == Guid.Parse(id))
                            .FirstOrDefault();
            return View(model);
            
        }
        public IActionResult Apply()
        {
            return View();
        }

        private async Task<BankDraft> TryFindBankDraft(Guid bankDraftId)
        {
            var bankDraft = await Db.BankDraft
                            .Include("WangCagaran")
                            .FirstOrDefaultAsync(x => x.Id == bankDraftId);
            if (bankDraft == null)
                throw new IdNotFoundException<BankDraft>(bankDraftId);
            return bankDraft;
        }


        public IActionResult Verify()
        {
            return View();
        }

        public IActionResult Submit()
        {
            return View();
        }
        public IActionResult Approve()
        {
            return View();
        }

        public IActionResult Accept()
        {
            return View();
        }
 
        public IActionResult Process()
        {
            return View();
        }


        public IActionResult Issue()
        {
            return View();
        }

         public IActionResult Complete()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Edit(string Id)
        {
            var model = new BankDraftViewModel();
            var item = Db.BankDraft.Where(x => x.Id == Guid.Parse(Id)).FirstOrDefault();
            if (item != null)
            {
                //Declare Bank Draft Details
                model.Id = item.Id.ToString();
                model.RefNo = item.RefNo;
                model.VerifierId = item.VerifierId;
                model.ApproverId = item.ApproverId;
                model.RequesterId = item.RequesterId;
                model.InstructionLetterRefNo = item.InstructionLetterRefNo;
                model.CoverMemoRefNo = item.CoverMemoRefNo;
                model.SendMethod = item.SendMethod;
                model.PostageNo = item.PostageNo;
                model.ReceiverContactNo = item.ReceiverContactNo;
                model.BankDrafNoIssued = item.BankDrafNoIssued;
                model.BankDraftDate = item.BankDraftDate;
                model.ReceiveBankDraftDate = item.ReceiveBankDraftDate;
                model.ReceiptNo = item.ReceiptNo;
                model.Status = item.Status;          

                //Declare Wang Cagaran Details
                var _wang = Db.WangCagaran.Where(x => x.BankDraftId == item.Id).FirstOrDefault();
                if(_wang != null)
                {
                    var wang = new WangCagaranViewModel();
                    wang.Id = _wang.Id.ToString();
                    wang.BankDraftId = _wang.BankDraftId;
                    wang.ErmsDocNo = _wang.ErmsDocNo == null ? "w": _wang.ErmsDocNo;
                    wang.PostingDate = _wang.PostingDate;
                    wang.Pemula = _wang.Pemula;
                    wang.Tarikh = DateTime.Parse(_wang.Tarikh.ToString());
                    wang.Alamat1 = _wang.Alamat1;
                    wang.Alamat2 = _wang.Alamat2;
                    wang.Bandar = _wang.Bandar;
                    wang.Poskod = _wang.Poskod;
                    wang.Negeri = _wang.Negeri;
                    wang.KeteranganKerja = _wang.KeteranganKerja;
                    wang.JKRInvolved = _wang.JKRInvolved ?? false;
                    wang.JKRType = _wang.JKRType;
                    wang.Jumlah = _wang.Jumlah;
                    wang.CajKod = _wang.CajKod;
                    wang.WBSProjekNo = _wang.WBSProjekNo;
                    wang.NamaPemegangCagaran = _wang.NamaPemegangCagaran;
                    wang.BusinessArea = _wang.BusinessArea;
                    wang.ErmsDocNo = _wang.ErmsDocNo;
                    wang.CoCode = _wang.CoCode;

                    model.WangCagaranViewModel = wang;
                }

                ViewBag.VerifierId = Db.Users.Find(model.VerifierId) != null ? Db.Users.Find(model.VerifierId).FullName : "";
                ViewBag.ApproverId = Db.Users.Find(model.ApproverId) != null ? Db.Users.Find(model.ApproverId).FullName : "";

                model.WCSuratKelulusanVM = new AttachmentViewModel();
                model.WCUMAPVM = new AttachmentViewModel();
                model.SignedLetterVM = new AttachmentViewModel();
                model.SignedMemoVM = new AttachmentViewModel();
                model.EvidenceVM = new AttachmentViewModel();

                var _wcsuratkelulusan = Db.Attachment.Where(x => x.ParentId == item.Id && x.FileType == Data.AttachmentType.BankDraft.ToString() && x.FileSubType == Data.BDAttachmentType.WCSuratKelulusan.ToString() ).FirstOrDefault();
                if(_wcsuratkelulusan != null)
                {
                    model.WCSuratKelulusanVM.Id = _wcsuratkelulusan.Id.ToString();
                    model.WCSuratKelulusanVM.FileName = _wcsuratkelulusan.FileName;
                }

                var _wcUMAP = Db.Attachment.Where(x => x.ParentId == item.Id && x.FileType == Data.AttachmentType.BankDraft.ToString() && x.FileSubType == Data.BDAttachmentType.WCUMAP.ToString()).FirstOrDefault();
                if (_wcUMAP != null)
                {
                    model.WCUMAPVM.Id = _wcUMAP.Id.ToString();
                    model.WCUMAPVM.FileName = _wcUMAP.FileName;
                }
                var _signedLetter = Db.Attachment.Where(x => x.ParentId == item.Id && x.FileType == Data.AttachmentType.InstructionLetter.ToString()).FirstOrDefault();
                if (_signedLetter != null)
                {
                    model.SignedLetterVM.Id = _signedLetter.Id.ToString();
                    model.SignedLetterVM.FileName = _signedLetter.FileName;
                }
                var _signedMemo = Db.Attachment.Where(x => x.ParentId == item.Id && x.FileType == Data.AttachmentType.Memo.ToString()).FirstOrDefault();
                if (_signedMemo != null)
                {
                    model.SignedMemoVM.Id = _signedMemo.Id.ToString();
                    model.SignedMemoVM.FileName = _signedMemo.FileName;
                }
                var _evidence = Db.Attachment.Where(x => x.ParentId == item.Id && x.FileType == Data.AttachmentType.Evidence.ToString()).FirstOrDefault();
                if (_evidence != null)
                {
                    model.EvidenceVM.Id = _evidence.Id.ToString();
                    model.EvidenceVM.FileName = _evidence.FileName;
                }


                return View(model);
            }
            return View();
        }
        [HttpGet]
        public JsonResult GetJKRType()
        {
            //var result = Enum.GetValues(typeof(JKRType))
            //               .Cast<JKRType>()
            //               .Select(t => new
            //               {
            //                   Id = ((int)t),
            //                   Name = t.ToString()
            //               }).ToList();

            var result = Db.JkrType.Select(s => new
            {
                Id = s.Id,
                Name = s.Type
            })
            .OrderBy(x => x.Name);

            return new JsonResult(result.ToList());
        }

    }
}