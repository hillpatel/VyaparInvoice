using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VyaparInvoice.Models
{
    public class GenerateInvoiceViewModel
    {
        public List<Product> Products { get; set; }
        public List<Unit> Units { get; set; }
        public List<Rate> Rates { get; set; }
        public int CentralTax { get; set; }
        public int StateTax { get; set; }
    }
}
