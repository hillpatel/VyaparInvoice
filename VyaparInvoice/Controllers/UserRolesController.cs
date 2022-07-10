using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VyaparInvoice.Models;

namespace VyaparInvoice.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UserRolesController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserRolesController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index(string userId)
        {
            var viewModel = new List<UserRoleViewModel>();
            var user = await _userManager.FindByIdAsync(userId);
            foreach(var role in _roleManager.Roles)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    RoleName = role.Name
                };
                if(await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.Selected = true;
                }
                else
                {
                    userRoleViewModel.Selected = false;
                }
                viewModel.Add(userRoleViewModel);
            }
            var model = new ManageUserRoleViewModel()
            {
                UserId = userId,
                UserRoles = viewModel
            };
            return View(model);
        }

        public async Task<IActionResult> Update(string id, ManageUserRoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            result = await _userManager.AddToRolesAsync(user, model.UserRoles.Where(x => x.Selected).Select(x => x.RoleName));
            var currentUser = await _userManager.GetUserAsync(User);
            await _signInManager.RefreshSignInAsync(currentUser);
            await Seeds.DefaultUsers.SeedAdminAsync(_userManager, _roleManager);
            return RedirectToAction("Index", new { UserId = id });
        }
    }
}
