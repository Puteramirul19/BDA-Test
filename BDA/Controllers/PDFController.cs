using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using SelectPdf;

namespace BDA.Controllers
{
    public class PDFController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult InstructionLetter_IronPDF(int id = 0)
        {
            var html = System.IO.File.ReadAllText("wwwroot/template/instructionletter.html");
            var htmlToPdf = new IronPdf.HtmlToPdf();
            var pdf = htmlToPdf.RenderHtmlAsPdf(html);

            return File(pdf.BinaryData, "application/pdf");
        }

        public IActionResult SLetter()
        {
            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();

            // create a new pdf document converting an url
            PdfDocument doc = converter.ConvertUrl("wwwroot/template/instructionletter.html");

            doc.Save("sample.pdf");

            return null;
        }
        public IActionResult RLetter()
        {
            var report = new ViewAsPdf("RLetter")
            {
                PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 },
            };
            return report;
        }

        public IActionResult ILetter()
        {
            var html = System.IO.File.ReadAllText("wwwroot/template/instructionletter.html");
            var htmlToPdf = new IronPdf.HtmlToPdf();
            var pdf = htmlToPdf.RenderHtmlAsPdf(html);
            
            Stream stream = pdf.Stream;

            var file = new FileStreamResult(stream, "application/pdf")
                        {
                            FileDownloadName = "InstructionLetter.pdf"
                        };
            return file;
        }
       

        public IActionResult InstructionLetter_SelectPDF()
        {
            // create a new pdf document
            PdfDocument doc = new PdfDocument();

            // add a new page to the document
            PdfPage page = doc.AddPage(PdfCustomPageSize.A4,
                new PdfMargins(50f), PdfPageOrientation.Portrait);

            // define a rendering result object
            PdfRenderingResult result;

            // create a new pdf font
            PdfFont font = doc.AddFont(PdfStandardFont.TimesRoman);
            font.Size = 11;

            PdfFont bold = doc.AddFont(PdfStandardFont.TimesBold);
            bold.Size = 11;

            PdfFont boldI = doc.AddFont(PdfStandardFont.TimesBoldItalic);
            boldI.Size = 11;

            PdfFont title = doc.AddFont(PdfStandardFont.TimesBold);
            title.Size = 11;
            title.IsUnderline = true;

            // create text element 
            PdfTextElement text1 = new PdfTextElement(0, 0,
                "Rujukan Kami: TNB/KEW (UP) B 14/8/Deraf", font);
            result = page.Add(text1);

            text1 = new PdfTextElement(400, 0,
                 "12 Mac 2019", font);
            result = page.Add(text1);
            
            text1 = new PdfTextElement(0, 0,
                "\n\nPengurus Maybank," +
                "\nLevel 1, Tower A, Dataran Maybank" +
                "\nNo. 1, Jalan Maarof" +
                "\nKuala Lumpur", font);
            result = page.Add(text1);

            text1 = new PdfTextElement(0, 100,
               "U/Perhatian : Pn. Wan Surainee", title);
            result = page.Add(text1);

            text1 = new PdfTextElement(0, 100,
                "\n\n\nPuan,", font);
            result = page.Add(text1);

            text1 = new PdfTextElement(0, 170,
                "DERAF BANK PADA 13.03.2019 (RABU)", title);
            result = page.Add(text1);

            text1 = new PdfTextElement(0, 190,
                "Sila uruskan deraf bank dalam matawang Ringgit Malaysia seperti berikut :", font);
            result = page.Add(text1);

            text1 = new PdfTextElement(50, 220,
                "Nama Penerima", title);
            result = page.Add(text1);

            text1 = new PdfTextElement(280, 220,
                "Tempat", title);
            result = page.Add(text1);

            text1 = new PdfTextElement(400, 220,
                "Jumlah", title);
            result = page.Add(text1);

            text1 = new PdfTextElement(0, 220,
                "\n\n1. PENGARAH JKR NEGERI PAHANG" +
                "\n\n2. PENGARAH JKR NEGERI PAHANG" +
                "\n\n3. MAJLIS PERBANDARAN SUBANG JAYA" +
                "\n\n", bold);
            result = page.Add(text1);

            text1 = new PdfTextElement(260, 220,
                "\n\nPAHANG" +
                "\n\nPAHANG" +
                "\n\nSELANGOR" +
                "\n\nJUMLAH", bold);
            result = page.Add(text1);

            text1 = new PdfTextElement(380, 220,
                "\n\nRM 232,659.00" +
                "\n\nRM 391,440.00" +
                "\n\nRM 26,730.00" +
                "\n\nRM 650,829.00", bold);
            result = page.Add(text1);

            int undertable = 220 + (4 * 30) + 10;
            text1 = new PdfTextElement(0, undertable,
                "Ringgit: Enam ratus lima puluh ribu lapan ratus dua puluh sembilan sahaja.", font);
            result = page.Add(text1);

            text1 = new PdfTextElement(0, undertable + 20,
                    "Sila debit Akaun TNB berikut :" +
                    "\n\t\t\t\t\t- 5 - 14253 - 33333 - 4 bagi jumlah diatas" +
                    "\n\t\t\t\t\t- 5 - 14253 - 50002 - 0 untuk caj bank.", font);
            result = page.Add(text1);

            text1 = new PdfTextElement(0, undertable + 70,
                    "Ref: 1)PJKRNP 2) MPSJ", bold);
            result = page.Add(text1);

            text1 = new PdfTextElement(0, undertable + 90,
                    "Sekian, terima kasih.", font);
            result = page.Add(text1);
            
            int tagline = 600;
            text1 = new PdfTextElement(0, tagline,
                    "BETTER. BRIGHTER." +
                    "\nTNB MENGAMALKAN DASAR TIADA HADIAH", boldI);
            text1.HorizontalAlign = PdfTextHorizontalAlign.Center;
            result = page.Add(text1);

            text1 = new PdfTextElement(0, tagline + 30,
                    "Yang Benar,", font);
            result = page.Add(text1);

            text1 = new PdfTextElement(0, tagline + 75,
                    "_______________________________", font);
            result = page.Add(text1);
            text1 = new PdfTextElement(0, tagline + 90,
                    "Penandatangan Yang Diberikuasa", font);
            result = page.Add(text1);

            text1 = new PdfTextElement(300, tagline + 75,
                    "_______________________________", font);
            result = page.Add(text1);

            text1 = new PdfTextElement(300, tagline + 90,
                    "Penandatangan Yang Diberikuasa", font);
            result = page.Add(text1);

            // save pdf document
            byte[] pdf = doc.Save();

            // close pdf document
            doc.Close();

            return File(pdf, "application/pdf");
        }
    }
}
