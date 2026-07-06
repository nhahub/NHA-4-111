//using FitCore.DAL.Data.Models;
//using Microsoft.EntityFrameworkCore.Storage;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace FitCore.DAL.Interfaces
//{
//    public interface IUnitOfWork : IDisposable
//    {
//        IGenericRepository<T> GetRepository<T>() where T : class;

//        Task<int> SaveChangesAsync();

//        Task<IDbContextTransaction> BeginTransactionAsync();
//    }
//}