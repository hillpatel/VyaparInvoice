using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VyaparInvoice.Models
{
    [Table("Chalaan")]
    public class Chalaan
    {
        [Key]
        public Guid Id { get; set; }
        public int ChalaanNumber { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public string ClientPhoneNumber { get; set; }
        public string ClientAddress { get; set; }
        public string ClientGSTNumber { get; set; }
        public string PayableAmount { get; set; }
        public string ItemDetails { get; set; }
        public DateTime Date { get; set; }
        public string CreatorUserId { get; set; }
    }
}
