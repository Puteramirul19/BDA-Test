using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BDA.Web.Controllers
{
    [Authorize]
    public class LostMockupController : Controller
    {
        public IActionResult BankDraftList()
        {
            return View();
        }
        public IActionResult Create()
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
        public IActionResult Confirm()
        {
            return View();
        }
        public IActionResult Complete()
        {
            return View();
        }
        public IActionResult _CreateDetails()
        {
            return View();
        }
        public IActionResult _ApproveDetails()
        {
            return View();
        }
        public IActionResult _AcceptDetails()
        {
            return View();
        }
        public IActionResult _ProcessDetails()
        {
            return View();
        }
        public IActionResult _CompleteDetails()
        {
            return View();
        }
        public IActionResult _Document()
        {
            return View();
        }
        public IActionResult _StatusBar()
        {
            return View();
        }
        public IActionResult _ActionHistory()
        {
            return View();
        }
        public IActionResult _ActionButton()
        {
            return View();
        }
        public IActionResult _Comments()
        {
            return View();
        }
    }
}