using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VyaparInvoice.Data;
using VyaparInvoice.Models;

namespace VyaparInvoice.Controllers
{
    [Authorize(Roles = "Client")]
    public class RatesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public RatesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Rates
        public async Task<IActionResult> Index()
        {
            return View(await _context.Rates.ToListAsync());
        }

        // GET: Rates/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rate = await _context.Rates
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rate == null)
            {
                return NotFound();
            }

            return View(rate);
        }

        // GET: Rates/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Rates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,UnitId,Price,CreatorUserId")] Rate rate, string creatorId)
        {
            if (ModelState.IsValid)
            {
                rate.Id = Guid.NewGuid();
                rate.CreatorUserId = creatorId;
                _context.Add(rate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(rate);
        }

        // GET: Rates/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rate = await _context.Rates.FindAsync(id);
            if (rate == null)
            {
                return NotFound();
            }
            return View(rate);
        }

        // POST: Rates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ProductId,UnitId,Price,CreatorUserId")] Rate rate)
        {
            if (id != rate.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var local = _context.Set<Rate>().Local.FirstOrDefault(x => x.Id == id);
                    if (!local.Equals(null))
                    {
                        _context.Entry(local).State = EntityState.Detached;
                    }
                    _context.Entry(rate).State = EntityState.Modified;
                    //_context.Update(rate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RateExists(rate.Id))
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
            return View(rate);
        }

        // GET: Rates/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rate = await _context.Rates
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rate == null)
            {
                return NotFound();
            }

            return View(rate);
        }

        // POST: Rates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var rate = await _context.Rates.FindAsync(id);
            _context.Rates.Remove(rate);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RateExists(Guid id)
        {
            return _context.Rates.Any(e => e.Id == id);
        }
    }
}
