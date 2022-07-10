using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VyaparInvoice.Models;

namespace VyaparInvoice.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Rate> Rates { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Chalaan> Chalaan { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
    }
}
