using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Application.Contracts;
using Server.Domain.Models;
using Server.Infrastructure.Persistence;

namespace Server.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ServerDbContext _context;
        public IGenericRepository<Points> Points => _points ??= new GenericRepository<Points>(_context);

        public IGenericRepository<Company> Companies => _companies ??= new GenericRepository<Company>(_context);

        public IGenericRepository<Store> Stores => _stores ??= new GenericRepository<Store>(_context);
        public IGenericRepository<ApplicationUser> Users => _users ??= new GenericRepository<ApplicationUser>(_context);
        public IGenericRepository<PointsHistory> PointsHistory => _pointsHistory ??= new GenericRepository<PointsHistory>(_context);
        public IGenericRepository<ApplicationUserCompany> UserCompanies => _userCompanies ??= new GenericRepository<ApplicationUserCompany>(_context);
        public IGenericRepository<Sales> Sales => _sales ??= new GenericRepository<Sales>(_context);

        private IGenericRepository<Points> _points;
        private IGenericRepository<Company> _companies;
        private IGenericRepository<Store> _stores;
        private IGenericRepository<ApplicationUser> _users;
        private IGenericRepository<PointsHistory> _pointsHistory;

        private IGenericRepository<ApplicationUserCompany> _userCompanies;
        private IGenericRepository<Sales> _sales;
        public UnitOfWork(ServerDbContext context)
        {
            _context = context;
        }


        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}