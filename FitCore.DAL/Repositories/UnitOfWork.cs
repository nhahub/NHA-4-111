using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitCore.DAL.Data.Contexts;
using FitCore.DAL.Data.Models;
using FitCore.DAL.Interfaces;

namespace FitCore.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IGenericRepository<User> Users { get; private set; }
        public IGenericRepository<MemberProfile> MemberProfiles { get; private set; }
        public IGenericRepository<Trainer> Trainers { get; private set; }
        public IGenericRepository<SubscriptionPlan> SubscriptionPlans { get; private set; }
        public IGenericRepository<Membership> Memberships { get; private set; }
        public IGenericRepository<Class> Classes { get; private set; }
        public IGenericRepository<Booking> Bookings { get; private set; }
        public IGenericRepository<Wallet> Wallets { get; private set; }
        public IGenericRepository<Invoice> Invoices { get; private set; }
        public IGenericRepository<Product> Products { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            Users = new GenericRepository<User>(_context);
            MemberProfiles = new GenericRepository<MemberProfile>(_context);
            Trainers = new GenericRepository<Trainer>(_context);
            SubscriptionPlans = new GenericRepository<SubscriptionPlan>(_context);
            Memberships = new GenericRepository<Membership>(_context);
            Classes = new GenericRepository<Class>(_context);
            Bookings = new GenericRepository<Booking>(_context);
            Wallets = new GenericRepository<Wallet>(_context);
            Invoices = new GenericRepository<Invoice>(_context);
            Products = new GenericRepository<Product>(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}