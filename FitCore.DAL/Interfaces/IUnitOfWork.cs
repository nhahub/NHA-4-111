using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitCore.DAL.Data.Models;

namespace FitCore.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<MemberProfile> MemberProfiles { get; }
        IGenericRepository<Trainer> Trainers { get; }
        IGenericRepository<SubscriptionPlan> SubscriptionPlans { get; }
        IGenericRepository<Membership> Memberships { get; }
        IGenericRepository<Class> Classes { get; }
        IGenericRepository<Booking> Bookings { get; }
        IGenericRepository<Wallet> Wallets { get; }
        IGenericRepository<Invoice> Invoices { get; }
        IGenericRepository<Product> Products { get; }

        Task<int> CompleteAsync();
    }
}