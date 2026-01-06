using Microsoft.EntityFrameworkCore;
using AutoPartsPOS.Models;

namespace AutoPartsPOS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }



        // جداول النظام الأساسية للDashboard
          // جداول النظام
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<SalesInvoice> SalesInvoices { get; set; }
        public DbSet<SalesInvoiceItem> SalesInvoiceItems { get; set; }
        public DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }
        public DbSet<PurchaseInvoiceItem> PurchaseInvoiceItems { get; set; }
        public DbSet<Return> Returns { get; set; }
        public DbSet<ReturnItem> ReturnItems { get; set; }

        public DbSet<CashTransaction> CashTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany()
                .HasForeignKey(rp => rp.PermissionId);



            // علاقات النظام
            modelBuilder.Entity<SalesInvoice>()
                .HasOne(si => si.Customer)
                .WithMany(c => c.SalesInvoices)
                .HasForeignKey(si => si.CustomerId);

            modelBuilder.Entity<SalesInvoice>()
                .HasOne(si => si.User)
                .WithMany(u => u.SalesInvoices)
                .HasForeignKey(si => si.UserId);

            modelBuilder.Entity<SalesInvoiceItem>()
                .HasOne(sii => sii.SalesInvoice)
                .WithMany(si => si.SalesInvoiceItems)
                .HasForeignKey(sii => sii.InvoiceId);

            modelBuilder.Entity<SalesInvoiceItem>()
                .HasOne(sii => sii.Product)
                .WithMany(p => p.SalesInvoiceItems)
                .HasForeignKey(sii => sii.ProductId);

            modelBuilder.Entity<PurchaseInvoice>()
                .HasOne(pi => pi.Supplier)
                .WithMany(s => s.PurchaseInvoices)
                .HasForeignKey(pi => pi.SupplierId);

            modelBuilder.Entity<PurchaseInvoiceItem>()
                .HasOne(pii => pii.PurchaseInvoice)
                .WithMany(pi => pi.PurchaseInvoiceItems)
                .HasForeignKey(pii => pii.InvoiceId);

            modelBuilder.Entity<PurchaseInvoiceItem>()
                .HasOne(pii => pii.Product)
                .WithMany(p => p.PurchaseInvoiceItems)
                .HasForeignKey(pii => pii.ProductId);
// ================= RETURNS =================

// Sales Return
modelBuilder.Entity<Return>()
    .HasOne(r => r.SalesInvoice)
    .WithMany(si => si.Returns)
    .HasForeignKey(r => r.SalesInvoiceId)
    .OnDelete(DeleteBehavior.Restrict);

// Purchase Return
modelBuilder.Entity<Return>()
    .HasOne(r => r.PurchaseInvoice)
    .WithMany(pi => pi.Returns)
    .HasForeignKey(r => r.PurchaseInvoiceId)
    .OnDelete(DeleteBehavior.Restrict);



modelBuilder.Entity<Return>()
    .HasCheckConstraint(
        "CK_Return_InvoiceType",
        @"(ReturnType = 1 AND SalesInvoiceId IS NOT NULL AND PurchaseInvoiceId IS NULL)
       OR (ReturnType = 2 AND PurchaseInvoiceId IS NOT NULL AND SalesInvoiceId IS NULL)"
    );


        }
    }
}
