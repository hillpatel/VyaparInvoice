using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VyaparInvoice.Models
{
    [Table("Profile")]
    public class Profile
    {
        [Key]
        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternatePhoneNumber { get; set; }
        public string GSTINORUIN { get; set; }
        public int CentralTax { get; set; }
        public int StateTax { get; set; }
        public string Logo { get; set; }
        public string CreatorUserId { get; set; }
        [NotMapped]
        [DisplayName("Upload Logo")]
        public IFormFile ImageFile { get; set; }
    }
}
