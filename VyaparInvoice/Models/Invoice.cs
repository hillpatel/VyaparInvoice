using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VyaparInvoice.Models
{
    [Table("Invoice")]
    public class Invoice
    {
        [Key]
        public Guid Id { get; set; }
        public int InvoiceNumber { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public string ClientPhoneNumber { get; set; }
        public string ClientAddress { get; set; }
        public string ClientGSTNumber { get; set; }
        public string TaxableAmount { get; set; }
        public int CGST { get; set; }
        public int SGST { get; set; }
        public string PayableAmount { get; set; }
        public string ItemDetails { get; set; }
        public DateTime Date { get; set; }
        public string CreatorUserId { get; set; }
    }
}
