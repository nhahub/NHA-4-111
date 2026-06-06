using FitCore.DAL.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCore.DAL.Data.Contexts
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Tables :
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<MemberProfile> MemberProfiles { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletTransaction> WalletTransactions { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. UserRole (Composite Key & Many-to-Many Configuration)
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.RoleID, ur.UserID });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserID);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleID);

            // 2. User Settings
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.FullName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
                entity.Property(u => u.PhoneNumber).IsRequired().HasMaxLength(20);
                entity.Property(u => u.PasswordHash).IsRequired();
            });

            // 3. Role Settings
            modelBuilder.Entity<Role>()
                .Property(r => r.RoleName).IsRequired().HasMaxLength(50);

            // 4. One-to-One: User & Trainer
            modelBuilder.Entity<Trainer>()
                .HasOne(t => t.User)
                .WithOne(u => u.Trainer)
                .HasForeignKey<Trainer>(t => t.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Trainer>(entity =>
            {
                entity.HasKey(t => t.TrainerID); 
                entity.Property(t => t.Specialization).HasMaxLength(100);
                entity.Property(t => t.Bio).HasMaxLength(500);
                entity.Property(t => t.WorkingHours).HasMaxLength(100);
            });

            // 5. One-to-One: User & MemberProfile
            modelBuilder.Entity<MemberProfile>()
                .HasOne(m => m.User)
                .WithOne(u => u.MemberProfile)
                .HasForeignKey<MemberProfile>(m => m.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MemberProfile>(entity =>
            {
                entity.HasKey(m => m.MemberID);
            });

            // 6. SubscriptionPlan Settings
            modelBuilder.Entity<SubscriptionPlan>(entity =>
            {
                entity.HasKey(p => p.PlanID);
                entity.Property(p => p.PlanName).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Description).HasMaxLength(500);
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            });

            // 7. Membership Settings
            modelBuilder.Entity<Membership>(entity =>
            {
                entity.HasKey(m => m.MembershipID);
                entity.Property(m => m.Status).IsRequired().HasMaxLength(20);
            });

            modelBuilder.Entity<Membership>()
                .HasOne(m => m.MemberProfile)
                .WithMany(mp => mp.Memberships)
                .HasForeignKey(m => m.MemberID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Membership>()
                .HasOne(m => m.SubscriptionPlan)
                .WithMany(p => p.Memberships)
                .HasForeignKey(m => m.PlanID)
                .OnDelete(DeleteBehavior.Restrict);

            // 8. Class Settings
            modelBuilder.Entity<Class>(entity =>
            {
                entity.HasKey(c => c.ClassID); 
                entity.Property(c => c.ClassName).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Status).HasMaxLength(20);
            });

            modelBuilder.Entity<Class>()
                .HasOne(c => c.Trainer)
                .WithMany(t => t.Classes)
                .HasForeignKey(c => c.TrainerID)
                .OnDelete(DeleteBehavior.Restrict);

            // 9. Booking Settings
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.BookingID);
                entity.Property(b => b.Status).HasMaxLength(20);
            });

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.MemberProfile)
                .WithMany(mp => mp.Bookings)
                .HasForeignKey(b => b.MemberID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Class)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.ClassID)
                .OnDelete(DeleteBehavior.Restrict);

            // 10. Attendance Settings
            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.HasKey(a => a.AttendanceID);
                entity.Property(a => a.Status).HasMaxLength(20);
                entity.Property(a => a.Type).HasMaxLength(20);
            });

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.MemberProfile)
                .WithMany(mp => mp.Attendances)
                .HasForeignKey(a => a.MemberID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Class)
                .WithMany(c => c.Attendances)
                .HasForeignKey(a => a.ClassID)
                .OnDelete(DeleteBehavior.Restrict);

            // 11. One-to-One: MemberProfile & Wallet
            modelBuilder.Entity<Wallet>()
                .HasOne(w => w.MemberProfile)
                .WithOne(mp => mp.Wallet)
                .HasForeignKey<Wallet>(w => w.MemberID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Wallet>()
                .Property(w => w.Balance).HasColumnType("decimal(18,2)");

            // 12. WalletTransaction Settings
            modelBuilder.Entity<WalletTransaction>()
                .HasOne(wt => wt.Wallet)
                .WithMany(w => w.WalletTransactions)
                .HasForeignKey(wt => wt.WalletID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WalletTransaction>(entity =>
            {
                entity.HasKey(wt => wt.TransactionID);
                entity.Property(wt => wt.Amount).HasColumnType("decimal(18,2)");
                entity.Property(wt => wt.TransactionType).IsRequired().HasMaxLength(20);
            });

            // 13. Invoice Settings
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(i => i.InvoiceID);
                entity.Property(i => i.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(i => i.InvoiceStatus).IsRequired().HasMaxLength(20);
            });

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.MemberProfile)
                .WithMany(mp => mp.Invoices)
                .HasForeignKey(i => i.MemberID)
                .OnDelete(DeleteBehavior.Restrict);

            // 14. InvoiceItem Settings
            modelBuilder.Entity<InvoiceItem>(entity =>
            {
                entity.HasKey(ii => ii.InvoiceItemID);
                entity.Property(ii => ii.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(ii => ii.LineTotal).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<InvoiceItem>()
                .HasOne(ii => ii.Invoice)
                .WithMany(i => i.InvoiceItems)
                .HasForeignKey(ii => ii.InvoiceID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InvoiceItem>()
                .HasOne(ii => ii.Product)
                .WithMany(p => p.InvoiceItems)
                .HasForeignKey(ii => ii.ProductID)
                .OnDelete(DeleteBehavior.Restrict);

            // 15. Product Settings
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Category).HasMaxLength(50);
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            });

            // 16. Supplier Settings
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.Property(s => s.CompanyName).IsRequired().HasMaxLength(100);
                entity.Property(s => s.SupplierPhone).HasMaxLength(20);
            });

            // 17. InventoryTransaction Settings
            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(it => it.Product)
                .WithMany(p => p.InventoryTransactions)
                .HasForeignKey(it => it.ProductID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InventoryTransaction>(entity =>
            {
                entity.HasKey(it => it.TransactionID);
                entity.Property(it => it.TransactionType).IsRequired().HasMaxLength(20);
            });

            // 18. Payment Settings
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(p => p.PaymentID);
                entity.Property(p => p.AmountPaid).HasColumnType("decimal(18,2)");
                entity.Property(p => p.PaymentMethod).IsRequired().HasMaxLength(30);
                entity.Property(p => p.TransactionReference).HasMaxLength(100);
            });

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Invoice)
                .WithMany(i => i.Payments)
                .HasForeignKey(p => p.InvoiceID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.MemberProfile)
                .WithMany(mp => mp.Payments)
                .HasForeignKey(p => p.MemberID)
                .OnDelete(DeleteBehavior.Restrict);

            // 19. Notification Settings
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .Property(n => n.Content).IsRequired().HasMaxLength(500);

            // 20. AuditLog Settings
            modelBuilder.Entity<AuditLog>()
                .HasOne(al => al.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(al => al.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(al => al.LogID);
                entity.Property(al => al.Action).IsRequired().HasMaxLength(50);
                entity.Property(al => al.TableName).IsRequired().HasMaxLength(100);
            });
        }
    }
}
