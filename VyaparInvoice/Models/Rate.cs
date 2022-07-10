using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VyaparInvoice.Models
{
    [Table("Rate")]
    public class Rate
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid UnitId { get; set; }
        public int Price { get; set; }
        public string CreatorUserId { get; set; }
    }
}
