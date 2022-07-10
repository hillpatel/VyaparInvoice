using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VyaparInvoice.Constants
{
    public class Permissions
    {
        public static List<string> GeneratePermissionsForModule(string module)
        {
            return new List<string>()
            {
                $"Permissions.{module}.Default"
            };
        }
        public static class Products {
            public const string Default = "Permissions.Products.Default";
        }
    }
}
