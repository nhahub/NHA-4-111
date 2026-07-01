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
    public class FitCoreDbContext : DbContext
    {
        //private readonly ICurrentService currentService;
        //private readonly IHttpContextAccessor httpContextAccessor;

        //private static readonly HashSet<string> SensitiveProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        //{
        //    "PasswordHash",
        //    "Password",
        //    "SecurityStamp",
        //    "ConcurrencyStamp",
        //    "TwoFactorSecret",
        //    "RefreshToken",
        //    "AccessToken",
        //    "NormalizedEmail",
        //    "NormalizedUserName"
        //};

        public FitCoreDbContext(DbContextOptions<FitCoreDbContext> options) : base(options)
        {
            //, ICurrentService _currentService, IHttpContextAccessor _httpContextAccessor
            //currentService = _currentService;
            //httpContextAccessor = _httpContextAccessor;
        }

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
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FitCoreDbContext).Assembly);
           
        }
    }
}
