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
        IPointsRepository Points { get; }
        ICompanyRepository Companies { get; }
        IGenericRepository<Store> Stores { get; }
        IGenericRepository<ApplicationUser> Users { get; }
        IGenericRepository<PointsHistory> PointsHistory { get; }
        IGenericRepository<Sales> Sales { get; }
        IGenericRepository<Categories> Categories { get; }




        Task Save();
    }
}