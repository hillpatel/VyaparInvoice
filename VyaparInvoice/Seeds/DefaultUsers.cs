using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VyaparInvoice.Constants;

namespace VyaparInvoice.Seeds
{
    public static class DefaultUsers
    {
        public static async Task SeedAdminAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var defaultUser = new IdentityUser
            {
                UserName = "test@gmail.com",
                Email = "test@gmail.com",
                EmailConfirmed = false,
                PhoneNumberConfirmed = false
            };
            if (userManager.Users.All(x => x.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if(user == null)
                {
                    await userManager.CreateAsync(defaultUser, "P@$$w0rd123");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.Client.ToString());
                }
                await roleManager.SeedClaimsForAdminAsync();
            }
        }

        public static async Task SeedClaimsForAdminAsync(this RoleManager<IdentityRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync("Admin");
            await roleManager.AddPermissionClaim(adminRole, "Products");
        }

        public static async Task AddPermissionClaim(this RoleManager<IdentityRole> roleManager, IdentityRole role, string module)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = Permissions.GeneratePermissionsForModule(module);
            foreach(var permission in allPermissions)
            {
                if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
                {
                    await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                }
            }
        }
    }
}
