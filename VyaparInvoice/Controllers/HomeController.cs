using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using VyaparInvoice.Data;
using VyaparInvoice.Models;

namespace VyaparInvoice.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            TempData["productsCount"] = _context.Products.Where(x => x.CreatorUserId == _userManager.GetUserAsync(HttpContext.User).Result.Id).Count();
            TempData["chalaansCount"] = _context.Chalaan.Where(x => x.CreatorUserId == _userManager.GetUserAsync(HttpContext.User).Result.Id).Count();
            TempData["invoicesCount"] = _context.Invoice.Where(x => x.CreatorUserId == _userManager.GetUserAsync(HttpContext.User).Result.Id).Count();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
