using Abp.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VyaparInvoice.Constants;

namespace VyaparInvoice.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        public UsersController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsersExceptCurrentUser = await _userManager.Users.Where(a => a.Id != currentUser.Id).ToListAsync();
            return View(allUsersExceptCurrentUser);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(string Email, string Password)
        {
            if (ModelState.IsValid)
            {
                var newUser = new IdentityUser
                {
                    UserName = Email,
                    Email = Email,
                    EmailConfirmed = false,
                    PhoneNumberConfirmed = false
                };
                if (_userManager.Users.All(x => x.Id != newUser.Id))
                {
                    var user = await _userManager.FindByEmailAsync(newUser.Email);
                    if (user == null)
                    {
                        await _userManager.CreateAsync(newUser, Password);
                        await _userManager.AddToRoleAsync(newUser, Roles.Client.ToString());
                    }
                }
            }
            return RedirectToAction("Index","Users");
            //return View(emp);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ChangePassword(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string id, string currentPassword, string newPassword)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user==null)
            {
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
            {
                return NoContent();
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}
