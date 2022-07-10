using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VyaparInvoice.Models
{
    public class CreateProductUnitRateViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public List<Unit> Units { get; set; }
        public List<int> Rates { get; set; }
    }
}
