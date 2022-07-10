using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VyaparInvoice.Data;
using VyaparInvoice.Models;

namespace VyaparInvoice.Controllers
{
    [Authorize(Roles = "Client")]
    public class GenerateInvoiceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _host;
        public GenerateInvoiceController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment host)
        {
            _context = context;
            _userManager = userManager;
            _host = host;
        }
        public async Task<IActionResult> Index()
        {
            var generateInvoiceViewModel = new GenerateInvoiceViewModel();
            generateInvoiceViewModel.Products = _context.Products.Where(x => x.CreatorUserId == _userManager.GetUserAsync(HttpContext.User).Result.Id).ToList();
            generateInvoiceViewModel.Units = _context.Units.Where(x => x.CreatorUserId == _userManager.GetUserAsync(HttpContext.User).Result.Id).OrderBy(x => x.Sequence).ToList();
            generateInvoiceViewModel.Rates = _context.Rates.Where(x => x.CreatorUserId == _userManager.GetUserAsync(HttpContext.User).Result.Id).ToList();
            var profile = _context.Profiles.Where(x => x.CreatorUserId == _userManager.GetUserAsync(HttpContext.User).Result.Id).FirstOrDefault();
            if (profile == null)
            {
                return RedirectToAction("Index","Profiles");
            }
            if (generateInvoiceViewModel.Units.Count == 0)
            {
                return RedirectToAction("Index", "Units");
            }
            if (generateInvoiceViewModel.Products.Count == 0)
            {
                return RedirectToAction("Index", "Products");
            }
            generateInvoiceViewModel.CentralTax = profile.CentralTax;
            generateInvoiceViewModel.StateTax = profile.StateTax;
            return View(generateInvoiceViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string[] productName, string[] unit, string[] rate, int[] quantity, string[] amount, string[] hsn, string clientName, string clientEmail, string clientPhone, string clientAddress, string clientGSTNumber, string differentiator, string creatorId)
        {
            creatorId = (creatorId != null) ? creatorId : _userManager.GetUserAsync(HttpContext.User).Result.Id;
            var profile = _context.Profiles.Where(x => x.CreatorUserId == creatorId).FirstOrDefault();
            HtmlToPdf pdf = new HtmlToPdf();
            string invoiceFile = string.Empty;
            var companyAddress = profile.Address.Replace(System.Environment.NewLine, "<br/>");
            var tableString = string.Empty;
            for (int i = 0; i < productName.Length; i++)
            {
                tableString += $"<tr><td align='center'>{i + 1}</td ><td>{productName[i]}</td><td>&#8377;{rate[i]}</td><td>{unit[i]}</td><td align='center'>{quantity[i]}</td><td class='bold'>&#8377;{amount[i]}</td><td>{hsn[i]}</td></tr>";
            }
            long taxableAmount = 0;
            foreach (var amt in amount) { taxableAmount += Convert.ToInt32(amt); }
            var jsonData = new ItemDetailsViewModel()
            {
                ProductName = productName,
                Unit = unit,
                Rate = rate,
                Quantity = quantity,
                Amount = amount,
                HSN = hsn
            };
            string printedFileName = "";
            //var creatorId = _userManager.GetUserAsync(HttpContext.User).Result.Id;
            if (differentiator == "invoice")
            {
                invoiceFile = System.IO.File.ReadAllText(_host.WebRootPath + @"\Invoice\InvoicePdf.html");
                var invoiceNumber = (_context.Invoice.Where(x => x.CreatorUserId == creatorId).Count() == 0) ? 1 : _context.Invoice.Where(x=>x.CreatorUserId==creatorId).Max(x => x.InvoiceNumber) + 1;
                var totalCgst = Convert.ToDecimal(profile.CentralTax) / 100 * taxableAmount;
                var totalSgst = Convert.ToDecimal(profile.StateTax) / 100 * taxableAmount;
                var totalGst = totalCgst + totalSgst;
                var finalAmount = taxableAmount + totalGst;
                invoiceFile = invoiceFile
                    .Replace("totalCgst", totalCgst.ToString())
                    .Replace("totalSgst", totalSgst.ToString())
                    .Replace("totalGst", totalGst.ToString())
                    .Replace("finalAmount", finalAmount.ToString())
                    .Replace("companyGSTNumber", profile.GSTINORUIN)
                    .Replace("clientGSTNumber", clientGSTNumber)
                    .Replace("cgst", profile.CentralTax.ToString())
                    .Replace("sgst", profile.StateTax.ToString())
                    .Replace("tgst", (profile.StateTax + profile.CentralTax).ToString())
                    .Replace("taxableAmount", taxableAmount.ToString())
                    .Replace("invoiceNo", invoiceNumber.ToString())
                    .Replace("amountInWords", NumberToWords.ConvertAmount(Convert.ToDouble(finalAmount)));

                InvoicesController invoicesController = new InvoicesController(_context, _userManager, _host);
                await invoicesController.Create(new Invoice()
                {
                    ClientName = clientName,
                    ClientEmail = clientEmail,
                    ClientPhoneNumber = clientPhone,
                    ClientAddress = clientAddress,
                    ClientGSTNumber = clientGSTNumber,
                    CGST = profile.CentralTax,
                    SGST = profile.StateTax,
                    TaxableAmount = taxableAmount.ToString(),
                    PayableAmount = finalAmount.ToString(),
                    ItemDetails = JsonConvert.SerializeObject(jsonData),
                    InvoiceNumber = invoiceNumber,
                    Date = DateTime.Today,
                    CreatorUserId = creatorId
                });

                printedFileName = invoiceNumber +"_"+ clientName + ".pdf";
            }
            else if(differentiator == "chalaan")
            {
                invoiceFile = System.IO.File.ReadAllText(_host.WebRootPath + @"\Invoice\ChalaanPdf.html");
                var chalaanNumber = (_context.Chalaan.Where(x => x.CreatorUserId == creatorId).Count() == 0) ? 1 : _context.Chalaan.Where(x=>x.CreatorUserId==creatorId).Max(x => x.ChalaanNumber) + 1;
                var amt = NumberToWords.ConvertAmount(taxableAmount);
                tableString += $"<tr><td colspan='5'><span>Amount chargeable in words : </span><span class='small total'>{amt}</span></td><td colspan='2' class='large total'>&#8377;{taxableAmount}</td></tr>";
                invoiceFile = invoiceFile
                    .Replace("chalaanNo", chalaanNumber.ToString());
                ChalaansController chalaansController = new ChalaansController(_context, _userManager, _host);
                await chalaansController.Create(new Chalaan()
                {
                    ClientName = clientName,
                    ClientEmail = clientEmail,
                    ClientPhoneNumber = clientPhone,
                    ClientAddress = clientAddress,
                    PayableAmount = taxableAmount.ToString(),
                    ItemDetails = JsonConvert.SerializeObject(jsonData),
                    ChalaanNumber = chalaanNumber,
                    Date = DateTime.Today,
                    CreatorUserId = creatorId,
                    ClientGSTNumber = clientGSTNumber
                });
                printedFileName = chalaanNumber + "_" + clientName+".pdf";
            }
            
            
            invoiceFile = invoiceFile
                .Replace("date",DateTime.Today.ToShortDateString())
                .Replace("imagePath", _host.WebRootPath + profile.Logo)
                .Replace("company_Name", profile.CompanyName)
                .Replace("company_Address",companyAddress)
                .Replace("company_city", profile.City)
                .Replace("company_state", profile.State)
                .Replace("companyEmail",profile.Email)
                .Replace("companyPhone",profile.PhoneNumber+", "+profile.AlternatePhoneNumber)
                .Replace("Client_Name",clientName)
                .Replace("client_Address", clientAddress!=null? clientAddress.Replace(System.Environment.NewLine, "<br/>"):String.Empty)
                .Replace("clientEmail", clientEmail)
                .Replace("clientPhone", clientPhone)
                .Replace("tableTag", tableString)
                .Replace("heartImage", _host.WebRootPath + "/image/heart.png")
                ;

            PdfDocument pdfDocument = pdf.ConvertHtmlString(invoiceFile);
            var bytes = pdfDocument.Save();
            return File(bytes,"application/pdf",  printedFileName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PrintExisting(string[] productName, string[] unit, string[] rate, int[] quantity, string[] amount, string[] hsn, string clientName, string clientEmail, string clientPhone, string clientAddress, string clientGSTNumber, string date, string differentiator, int num, string creatorId)
        {
            creatorId = (creatorId != null) ? creatorId : _userManager.GetUserAsync(HttpContext.User).Result.Id;
            var profile = _context.Profiles.Where(x => x.CreatorUserId == creatorId).FirstOrDefault();
            HtmlToPdf pdf = new HtmlToPdf();
            string invoiceFile = string.Empty;
            var companyAddress = profile.Address.Replace(System.Environment.NewLine, "<br/>");
            var tableString = string.Empty;
            for (int i = 0; i < productName.Length; i++)
            {
                tableString += $"<tr><td align='center'>{i + 1}</td ><td>{productName[i]}</td><td>&#8377;{rate[i]}</td><td>{unit[i]}</td><td align='center'>{quantity[i]}</td><td class='bold'>&#8377;{amount[i]}</td><td>{hsn[i]}</td></tr>";
            }
            long taxableAmount = 0;
            foreach (var amt in amount) { taxableAmount += Convert.ToInt32(amt); }
            var jsonData = new ItemDetailsViewModel()
            {
                ProductName = productName,
                Unit = unit,
                Rate = rate,
                Quantity = quantity,
                Amount = amount,
                HSN = hsn
            };
            string printedFileName = "";
            //var creatorId = _userManager.GetUserAsync(HttpContext.User).Result.Id;
            if (differentiator == "invoice")
            {
                invoiceFile = System.IO.File.ReadAllText(_host.WebRootPath + @"\Invoice\InvoicePdf.html");
                //var invoiceNumber = (_context.Invoice.Count() == 0) ? 1 : _context.Invoice.Where(x => x.CreatorUserId == creatorId).Max(x => x.InvoiceNumber) + 1;
                var totalCgst = Convert.ToDecimal(profile.CentralTax) / 100 * taxableAmount;
                var totalSgst = Convert.ToDecimal(profile.StateTax) / 100 * taxableAmount;
                var totalGst = totalCgst + totalSgst;
                var finalAmount = taxableAmount + totalGst;
                invoiceFile = invoiceFile
                    .Replace("totalCgst", totalCgst.ToString())
                    .Replace("totalSgst", totalSgst.ToString())
                    .Replace("totalGst", totalGst.ToString())
                    .Replace("finalAmount", finalAmount.ToString())
                    .Replace("companyGSTNumber", profile.GSTINORUIN)
                    .Replace("clientGSTNumber", clientGSTNumber)
                    .Replace("cgst", profile.CentralTax.ToString())
                    .Replace("sgst", profile.StateTax.ToString())
                    .Replace("tgst", (profile.StateTax + profile.CentralTax).ToString())
                    .Replace("taxableAmount", taxableAmount.ToString())
                    .Replace("invoiceNo", num.ToString())
                    .Replace("amountInWords", NumberToWords.ConvertAmount(Convert.ToDouble(finalAmount)));

                printedFileName = num + "_" + clientName + ".pdf";
            }
            else if (differentiator == "chalaan")
            {
                invoiceFile = System.IO.File.ReadAllText(_host.WebRootPath + @"\Invoice\ChalaanPdf.html");
                //var chalaanNumber = (_context.Chalaan.Count() == 0) ? 1 : _context.Chalaan.Where(x => x.CreatorUserId == creatorId).Max(x => x.ChalaanNumber) + 1;
                var amt = NumberToWords.ConvertAmount(taxableAmount);
                tableString += $"<tr><td colspan='5'><span>Amount chargeable in words : </span><span class='small total'>{amt}</span></td><td colspan='2' class='large total'>&#8377;{taxableAmount}</td></tr>";
                invoiceFile = invoiceFile
                    .Replace("chalaanNo", num.ToString());
                printedFileName = num + "_" + clientName + ".pdf";
            }


            invoiceFile = invoiceFile
                .Replace("date", date)
                .Replace("imagePath", _host.WebRootPath + profile.Logo)
                .Replace("company_Name", profile.CompanyName)
                .Replace("company_Address", companyAddress)
                .Replace("company_city", profile.City)
                .Replace("company_state", profile.State)
                .Replace("companyEmail", profile.Email)
                .Replace("companyPhone", profile.PhoneNumber + ", " + profile.AlternatePhoneNumber)
                .Replace("Client_Name", clientName)
                .Replace("client_Address", clientAddress != null ? clientAddress.Replace(System.Environment.NewLine, "<br/>") : String.Empty)
                .Replace("clientEmail", clientEmail)
                .Replace("clientPhone", clientPhone)
                .Replace("tableTag", tableString)
                .Replace("heartImage", _host.WebRootPath + "/image/heart.png")
                ;

            PdfDocument pdfDocument = pdf.ConvertHtmlString(invoiceFile);
            var bytes = pdfDocument.Save();
            return File(bytes, "application/pdf", printedFileName);
        }
    }

    public class NumberToWords
    {
        private static String[] units = { "Zero", "One", "Two", "Three",
    "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven",
    "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen",
    "Seventeen", "Eighteen", "Nineteen" };
        private static String[] tens = { "", "", "Twenty", "Thirty", "Forty",
    "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

        public static String ConvertAmount(double amount)
        {
            try
            {
                Int64 amount_int = (Int64)amount;
                Int64 amount_dec = (Int64)Math.Round((amount - (double)(amount_int)) * 100);
                if (amount_dec == 0)
                {
                    return Convert(amount_int) + " Only.";
                }
                else
                {
                    return Convert(amount_int) + " Point " + Convert(amount_dec) + " Only.";
                }
            }
            catch (Exception e)
            {
                // TODO: handle exception  
            }
            return "";
        }

        public static String Convert(Int64 i)
        {
            if (i < 20)
            {
                return units[i];
            }
            if (i < 100)
            {
                return tens[i / 10] + ((i % 10 > 0) ? " " + Convert(i % 10) : "");
            }
            if (i < 1000)
            {
                return units[i / 100] + " Hundred"
                        + ((i % 100 > 0) ? " And " + Convert(i % 100) : "");
            }
            if (i < 100000)
            {
                return Convert(i / 1000) + " Thousand "
                + ((i % 1000 > 0) ? " " + Convert(i % 1000) : "");
            }
            if (i < 10000000)
            {
                return Convert(i / 100000) + " Lakh "
                        + ((i % 100000 > 0) ? " " + Convert(i % 100000) : "");
            }
            if (i < 1000000000)
            {
                return Convert(i / 10000000) + " Crore "
                        + ((i % 10000000 > 0) ? " " + Convert(i % 10000000) : "");
            }
            return Convert(i / 1000000000) + " Arab "
                    + ((i % 1000000000 > 0) ? " " + Convert(i % 1000000000) : "");
        }
    }
}
