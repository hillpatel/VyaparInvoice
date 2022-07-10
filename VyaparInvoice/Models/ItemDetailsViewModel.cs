using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VyaparInvoice.Models
{
    public class ItemDetailsViewModel
    {
        public string[] ProductName { get; set; }
        public string[] Unit { get; set; }
        public string[] Rate { get; set; }
        public int[] Quantity { get; set; }
        public string[] Amount { get; set; }
        public string[] HSN { get; set; }
    }
}
