using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Domain.Models;

namespace Server.Application.Contracts
{
    public interface IUnitOfWork : IDisposable
    {

        // points , pointsHistory, stores,
        IGenericRepository<Points> Points { get; }
        ICompanyRepository Companies { get; }
        IGenericRepository<Store> Stores { get; }
        IGenericRepository<ApplicationUser> Users { get; }
        IGenericRepository<PointsHistory> PointsHistory { get; }
        IGenericRepository<Sales> Sales { get; }



        Task Save();
    }
}