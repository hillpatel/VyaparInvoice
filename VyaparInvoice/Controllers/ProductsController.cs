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
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        
        public ProductsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.Where(x => x.CreatorUserId == _userManager.GetUserAsync(HttpContext.User).Result.Id).ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            var createProductUnitRateViewModel = new CreateProductUnitRateViewModel();
            createProductUnitRateViewModel.Units = await _context.Units.Where(x => x.CreatorUserId == _userManager.GetUserAsync(HttpContext.User).Result.Id).OrderBy(x=>x.Sequence).ToListAsync();
            var rates = new List<int>();
            createProductUnitRateViewModel.Units.ForEach(y=> {
                var r = _context.Rates.Where(x => x.ProductId == id && x.UnitId == y.Id && x.CreatorUserId == _userManager.GetUserAsync(HttpContext.User).Result.Id).First().Price;
                rates.Add(r);
            });
            createProductUnitRateViewModel.Rates = rates;
            createProductUnitRateViewModel.Name = product.Name;
            createProductUnitRateViewModel.Code = product.Code;
            return View(createProductUnitRateViewModel);

            //return View(product);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            var createProductUnitRateViewModel = new CreateProductUnitRateViewModel();
            createProductUnitRateViewModel.Units = await _context.Units.Where(x => x.CreatorUserId == _userManager.GetUserAsync(HttpContext.User).Result.Id).OrderBy(x=>x.Sequence).ToListAsync();
            var rates = new List<int>();
            createProductUnitRateViewModel.Units.ForEach(x => rates.Add(0));
            createProductUnitRateViewModel.Rates = rates;
            return View(createProductUnitRateViewModel);
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(/*[Bind("Name,Code,LastModificationTime,LastModifierUserId,CreationTime,CreatorUserId,Id")]*/ CreateProductUnitRateViewModel createProductUnitRateViewModel)
        {
            if (ModelState.IsValid)
            {
                var product = new Product() { 
                        Name = createProductUnitRateViewModel.Name,
                        Code = createProductUnitRateViewModel.Code
                    };
                product.Id = Guid.NewGuid();
                product.CreatorUserId = _userManager.GetUserAsync(HttpContext.User).Result.Id;
                createProductUnitRateViewModel.Units = await _context.Units.Where(x => x.CreatorUserId == _userManager.GetUserAsync(HttpContext.User).Result.Id).OrderBy(x => x.Sequence).ToListAsync();
                for (var i=0; i< createProductUnitRateViewModel.Units.Count; i++)
                {
                    RatesController ratesController = new RatesController(_context, _userManager);
                    await ratesController.Create(new Rate() {
                        ProductId = product.Id,
                        UnitId = createProductUnitRateViewModel.Units[i].Id,
                        Price = createProductUnitRateViewModel.Rates[i]
                    }, product.CreatorUserId);
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(createProductUnitRateViewModel);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var createProductUnitRateViewModel = new CreateProductUnitRateViewModel();
            createProductUnitRateViewModel.Units = await _context.Units.Where(x => x.CreatorUserId == _userManager.GetUserAsync(HttpContext.User).Result.Id).OrderBy(x => x.Sequence).ToListAsync();
            var rates = new List<int>();
            createProductUnitRateViewModel.Units.ForEach(y => {
                var r = _context.Rates.Where(x => x.ProductId == id && x.UnitId == y.Id && x.CreatorUserId == _userManager.GetUserAsync(HttpContext.User).Result.Id).First().Price;
                rates.Add(r);
            });
            createProductUnitRateViewModel.Rates = rates;
            createProductUnitRateViewModel.Name = product.Name;
            createProductUnitRateViewModel.Code = product.Code;
            return View(createProductUnitRateViewModel);
            //return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, /*[Bind("Name,Code,LastModificationTime,LastModifierUserId,CreationTime,CreatorUserId,Id")]*/ CreateProductUnitRateViewModel createProductUnitRateViewModel)
        {
            if (id != createProductUnitRateViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                product.Name = createProductUnitRateViewModel.Name;
                product.Code = createProductUnitRateViewModel.Code;
                createProductUnitRateViewModel.Units = await _context.Units.Where(x => x.CreatorUserId == _userManager.GetUserAsync(HttpContext.User).Result.Id).OrderBy(x => x.Sequence).ToListAsync();
                for (var i = 0; i < createProductUnitRateViewModel.Units.Count; i++)
                {
                    var rateId = _context.Rates.Where(x => x.ProductId == id && x.UnitId == createProductUnitRateViewModel.Units[i].Id && x.CreatorUserId == _userManager.GetUserAsync(HttpContext.User).Result.Id).First().Id;
                    await EditRate(rateId, product, createProductUnitRateViewModel.Units[i].Id, createProductUnitRateViewModel.Rates[i]);
                }
                _context.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(createProductUnitRateViewModel);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.Rates.Where(x => x.ProductId == id).ForEachAsync(x=>_context.Rates.Remove(x));
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(Guid id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        private async Task<IActionResult> EditRate(Guid rateId, Product product, Guid unitId, int newRate) {
            RatesController ratesController = new RatesController(_context, _userManager);
            var editedData = await ratesController.Edit(rateId, new Rate()
            {
                Id = rateId,
                ProductId = product.Id,
                UnitId = unitId,
                Price = newRate,
                CreatorUserId = product.CreatorUserId
            });
            return editedData;
        }
    }
}
