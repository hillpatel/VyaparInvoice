using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VyaparInvoice.Data;
using VyaparInvoice.Models;

namespace VyaparInvoice.Controllers
{
    [Authorize(Roles = "Client")]
    public class InvoicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _host;

        public InvoicesController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment host)
        {
            _context = context;
            _userManager = userManager;
            _host = host;
        }

        // GET: Invoices
        public async Task<IActionResult> Index()
        {
            return View(await _context.Invoice.Where(x => x.CreatorUserId == _userManager.GetUserAsync(HttpContext.User).Result.Id).ToListAsync());
            //return View(await _context.Invoice.ToListAsync());
        }

        // GET: Invoices/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoice
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // GET: Invoices/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Invoices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,InvoiceNumber,ClientName,ClientEmail,ClientPhoneNumber,ClientAddress,ClientGSTNumber,TaxableAmount,CGST,SGST,PayableAmount,ItemDetails,Date,CreatorUserId")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                invoice.Id = Guid.NewGuid();
                //invoice.CreatorUserId = _userManager.GetUserAsync(HttpContext.User).Result.Id;
                _context.Add(invoice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(invoice);
        }

        // GET: Invoices/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoice.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            return View(invoice);
        }

        // POST: Invoices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,InvoiceNumber,ClientName,ClientEmail,ClientPhoneNumber,ClientAddress,ClientGSTNumber,TaxableAmount,CGST,SGST,PayableAmount,ItemDetails,Date,CreatorUserId")] Invoice invoice)
        {
            if (id != invoice.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    invoice.InvoiceNumber = _context.Invoice.FindAsync(id).Result.InvoiceNumber;
                    _context.Update(invoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvoiceExists(invoice.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(invoice);
        }

        // GET: Invoices/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoice
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var invoice = await _context.Invoice.FindAsync(id);
            _context.Invoice.Remove(invoice);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InvoiceExists(Guid id)
        {
            return _context.Invoice.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PrintInvoice(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoice
                .FirstOrDefaultAsync(m => m.Id.ToString() == id);
            if (invoice == null)
            {
                return NotFound();
            }

            var jsonData = JsonConvert.DeserializeObject<ItemDetailsViewModel>(invoice.ItemDetails);
            GenerateInvoiceController generateInvoice = new GenerateInvoiceController(_context, _userManager, _host);
            var r = await generateInvoice.PrintExisting(jsonData.ProductName, jsonData.Unit, jsonData.Rate, jsonData.Quantity, jsonData.Amount, jsonData.HSN, invoice.ClientName, invoice.ClientEmail, invoice.ClientPhoneNumber, invoice.ClientAddress, invoice.ClientGSTNumber, invoice.Date.ToShortDateString(), "invoice", invoice.InvoiceNumber, _userManager.GetUserAsync(HttpContext.User).Result.Id);
            return r;
        }
    }
}
