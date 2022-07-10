using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VyaparInvoice.Models
{
    public class UserRoleViewModel
    {
        public string RoleName { get; set; }
        public bool Selected { get; set; }
    }
    public class ManageUserRoleViewModel
    {
        public string UserId { get; set; }
        public IList<UserRoleViewModel> UserRoles { get; set; }
    }
}
