using Microsoft.EntityFrameworkCore;
using AutoPartsPOS.Models;
using AutoPartsPOS.Models.Maintenances;

namespace AutoPartsPOS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // ================= AUTH =================
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        // ================= CORE =================
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<SalesInvoice> SalesInvoices { get; set; }
        public DbSet<SalesInvoiceItem> SalesInvoiceItems { get; set; }

        public DbSet<Return> Returns { get; set; }
        public DbSet<ReturnItem> ReturnItems { get; set; }

        public DbSet<CashTransaction> CashTransactions { get; set; }

        // ================= MAINTENANCE =================
        public DbSet<Maintenance> Maintenances { get; set; }
        public DbSet<MaintenanceItem> MaintenanceItems { get; set; }
        public DbSet<MaintenanceHoldItem> MaintenanceHoldItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ================= AUTH =================
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

            // ================= SALES =================
            modelBuilder.Entity<SalesInvoice>()
                .HasOne(si => si.Customer)
                .WithMany(c => c.SalesInvoices)
                .HasForeignKey(si => si.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SalesInvoice>()
                .HasOne(si => si.User)
                .WithMany(u => u.SalesInvoices)
                .HasForeignKey(si => si.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SalesInvoiceItem>()
                .HasOne(sii => sii.SalesInvoice)
                .WithMany(si => si.SalesInvoiceItems)
                .HasForeignKey(sii => sii.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SalesInvoiceItem>()
                .HasOne(sii => sii.Product)
                .WithMany(p => p.SalesInvoiceItems)
                .HasForeignKey(sii => sii.ProductId)
                .IsRequired(false) // ضيف السطر ده فوراً
                .OnDelete(DeleteBehavior.Restrict);

            // ================= RETURNS =================
            modelBuilder.Entity<Return>()
                .HasOne(r => r.SalesInvoice)
                .WithMany(si => si.Returns)
                .HasForeignKey(r => r.SalesInvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // ================= MAINTENANCE =================
            modelBuilder.Entity<Maintenance>()
                .HasOne(m => m.Customer)
                .WithMany(c => c.Maintenances) // كل العمليات المرتبطة بالعميل
                .HasForeignKey(m => m.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Maintenance>()
                .HasOne(m => m.Technician)
                .WithMany(u => u.AssignedMaintenances) // كل العمليات المكلف بها الفني
                .HasForeignKey(m => m.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Maintenance>()
                .HasOne(m => m.SalesInvoice)
                .WithOne(si => si.Maintenance)
                .HasForeignKey<Maintenance>(m => m.SalesInvoiceId)
                .IsRequired(false) // أضف السطر ده عشان نأكد إنه مش إجباري
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<MaintenanceItem>()
                .HasOne(mi => mi.Maintenance)
                .WithMany(m => m.Items)
                .HasForeignKey(mi => mi.MaintenanceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MaintenanceItem>()
                .HasOne(mi => mi.Product)
                .WithMany(p => p.MaintenanceItems)
                .HasForeignKey(mi => mi.ProductId)
                .IsRequired(false) // دي الضمان الأكيد
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MaintenanceHoldItem>()
                .HasOne(h => h.Maintenance)
                .WithMany(m => m.HoldItems)
                .HasForeignKey(h => h.MaintenanceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
