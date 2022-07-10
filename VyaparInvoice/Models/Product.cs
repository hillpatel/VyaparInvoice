using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VyaparInvoice.Models
{
    [Table("Product")]
    public class Product
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string CreatorUserId { get; set; }

    }
}
