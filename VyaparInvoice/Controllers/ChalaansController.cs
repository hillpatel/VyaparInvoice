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
    public class ChalaansController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _host;

        public ChalaansController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment host)
        {
            _context = context;
            _userManager = userManager;
            _host = host;
        }

        // GET: Chalaans
        public async Task<IActionResult> Index()
        {
            return View(await _context.Chalaan.Where(x => x.CreatorUserId == _userManager.GetUserAsync(HttpContext.User).Result.Id).ToListAsync());
            //return View(await _context.Chalaan.ToListAsync());
        }

        // GET: Chalaans/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chalaan = await _context.Chalaan
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chalaan == null)
            {
                return NotFound();
            }

            return View(chalaan);
        }

        // GET: Chalaans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Chalaans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ChalaanNumber,ClientName,ClientEmail,ClientPhoneNumber,ClientAddress,PayableAmount,ItemDetails,Date,CreatorUserId")] Chalaan chalaan)
        {
            if (ModelState.IsValid)
            {
                chalaan.Id = Guid.NewGuid();
                //chalaan.CreatorUserId = _userManager.GetUserAsync(HttpContext.User).Result.Id;
                _context.Add(chalaan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(chalaan);
        }

        // GET: Chalaans/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chalaan = await _context.Chalaan.FindAsync(id);
            if (chalaan == null)
            {
                return NotFound();
            }
            return View(chalaan);
        }

        // POST: Chalaans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ChalaanNumber,ClientName,ClientEmail,ClientPhoneNumber,ClientAddress,PayableAmount,ItemDetails,Date,CreatorUserId")] Chalaan chalaan)
        {
            if (id != chalaan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    chalaan.ChalaanNumber = _context.Chalaan.FindAsync(id).Result.ChalaanNumber;
                    _context.Update(chalaan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChalaanExists(chalaan.Id))
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
            return View(chalaan);
        }

        // GET: Chalaans/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chalaan = await _context.Chalaan
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chalaan == null)
            {
                return NotFound();
            }

            return View(chalaan);
        }

        // POST: Chalaans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var chalaan = await _context.Chalaan.FindAsync(id);
            _context.Chalaan.Remove(chalaan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChalaanExists(Guid id)
        {
            return _context.Chalaan.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateInvoice(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chalaan = await _context.Chalaan
                .FirstOrDefaultAsync(m => m.Id.ToString() == id);
            if (chalaan == null)
            {
                return NotFound();
            }

            var jsonData = JsonConvert.DeserializeObject<ItemDetailsViewModel>(chalaan.ItemDetails);
            GenerateInvoiceController generateInvoice = new GenerateInvoiceController(_context, _userManager, _host);
            var r = await generateInvoice.Create(jsonData.ProductName, jsonData.Unit, jsonData.Rate, jsonData.Quantity, jsonData.Amount, jsonData.HSN, chalaan.ClientName, chalaan.ClientEmail, chalaan.ClientPhoneNumber, chalaan.ClientAddress, chalaan.ClientGSTNumber, "invoice", _userManager.GetUserAsync(HttpContext.User).Result.Id);
            return r;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PrintChalaan(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chalaan = await _context.Chalaan
                .FirstOrDefaultAsync(m => m.Id.ToString() == id);
            if (chalaan == null)
            {
                return NotFound();
            }

            var jsonData = JsonConvert.DeserializeObject<ItemDetailsViewModel>(chalaan.ItemDetails);
            GenerateInvoiceController generateInvoice = new GenerateInvoiceController(_context, _userManager, _host);
            var r = await generateInvoice.PrintExisting(jsonData.ProductName, jsonData.Unit, jsonData.Rate, jsonData.Quantity, jsonData.Amount, jsonData.HSN, chalaan.ClientName, chalaan.ClientEmail, chalaan.ClientPhoneNumber, chalaan.ClientAddress, chalaan.ClientGSTNumber, chalaan.Date.ToShortDateString(), "chalaan", chalaan.ChalaanNumber, _userManager.GetUserAsync(HttpContext.User).Result.Id);
            return r;
        }
    }
    
}
