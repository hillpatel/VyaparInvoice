using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using VyaparInvoice.Models;

namespace VyaparInvoice.Helpers
{
    public static class ClaimsHelper
    {
        public static void GetPermissions(this List<RoleClaimsViewModel> allPermissions, Type policy, string roleId)
        {
            FieldInfo[] fields = policy.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (FieldInfo fi in fields)
            {
                allPermissions.Add(new RoleClaimsViewModel { Value=fi.GetValue(null).ToString(), Type="Permissions"});
            }
        }
        public async static Task AddPermissionClaim(this RoleManager<IdentityRole> roleManager, IdentityRole role, string permission)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            if (!allClaims.Any(x => x.Type == "Permission" && x.Value == permission))
            {
                await roleManager.AddClaimAsync(role, new Claim("Permission",permission));
            }
        }
    }
}
