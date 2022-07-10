using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VyaparInvoice.Data;
using VyaparInvoice.Models;

namespace VyaparInvoice.Controllers
{
    [Authorize(Roles = "Client")]
    public class ProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;

        public ProfilesController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
        }

        // GET: Profiles
        public async Task<IActionResult> Index()
        {
            return View(await _context.Profiles.Where(x => x.CreatorUserId == _userManager.GetUserAsync(HttpContext.User).Result.Id).ToListAsync());
        }

        // GET: Profiles/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profile = await _context.Profiles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (profile == null)
            {
                return NotFound();
            }

            return View(profile);
        }

        // GET: Profiles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Profiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CompanyName,State,City,Address,Email,PhoneNumber,AlternatePhoneNumber,GSTINORUIN,CentralTax,StateTax,Logo,CreatorUserId,ImageFile")] Profile profile)
        {
            if (ModelState.IsValid)
            {
                profile.Id = Guid.NewGuid();
                profile.CreatorUserId = _userManager.GetUserAsync(HttpContext.User).Result.Id;
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(profile.ImageFile.FileName);
                string extension = Path.GetExtension(profile.ImageFile.FileName);
                fileName = fileName + profile.Id + extension;
                string path = Path.Combine(wwwRootPath + "/LogoImages/", fileName);
                profile.Logo = "/LogoImages/" + fileName;
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await profile.ImageFile.CopyToAsync(fileStream);
                }
                _context.Add(profile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(profile);
        }

        // GET: Profiles/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profile = await _context.Profiles.FindAsync(id);
            if (profile == null)
            {
                return NotFound();
            }
            return View(profile);
        }

        // POST: Profiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,CompanyName,State,City,Address,Email,PhoneNumber,AlternatePhoneNumber,GSTINORUIN,CentralTax,StateTax,Logo,CreatorUserId,ImageFile")] Profile profile)
        {
            if (id != profile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if(profile.ImageFile != null)
                    {
                        var imagePath = _hostEnvironment.WebRootPath + profile.Logo;
                        if (System.IO.File.Exists(imagePath))
                            System.IO.File.Delete(imagePath);
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(profile.ImageFile.FileName);
                        string extension = Path.GetExtension(profile.ImageFile.FileName);
                        fileName = fileName + profile.Id + extension;
                        string path = Path.Combine(wwwRootPath + "/LogoImages/", fileName);
                        profile.Logo = "/LogoImages/" + fileName;
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await profile.ImageFile.CopyToAsync(fileStream);
                        }
                    }
                    else
                    {
                        var pro = await _context.Profiles.FindAsync(id);
                        profile.Logo = pro.Logo;
                        var local = _context.Set<Profile>().Local.FirstOrDefault(x => x.Id == id);
                        if (!local.Equals(null))
                        {
                            _context.Entry(local).State = EntityState.Detached;
                        }
                        _context.Entry(profile).State = EntityState.Modified;
                    }
                    
                    _context.Update(profile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfileExists(profile.Id))
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
            return View(profile);
        }

        // GET: Profiles/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profile = await _context.Profiles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (profile == null)
            {
                return NotFound();
            }

            return View(profile);
        }

        // POST: Profiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var profile = await _context.Profiles.FindAsync(id);

            //delete image from wwwroot/image
            var imagePath = _hostEnvironment.WebRootPath + profile.Logo;
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);

            _context.Profiles.Remove(profile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProfileExists(Guid id)
        {
            return _context.Profiles.Any(e => e.Id == id);
        }
    }
}
