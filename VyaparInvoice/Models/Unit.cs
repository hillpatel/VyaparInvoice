using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VyaparInvoice.Models
{
    [Table("Unit")]
    public class Unit
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Sequence { get; set; }
        public string CreatorUserId { get; set; }
    }
}
