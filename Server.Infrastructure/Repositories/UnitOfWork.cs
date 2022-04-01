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
        private readonly IHttpContextAccessorWrapper _httpContextAccessorWrapper;
        public IPointsRepository Points => _points ??= new PointsRepository(_context, _httpContextAccessorWrapper);

        public ICompanyRepository Companies => _companies ??= new CompanyRepository(_context, _httpContextAccessorWrapper);

        public IGenericRepository<Store> Stores => _stores ??= new GenericRepository<Store>(_context);
        public IGenericRepository<ApplicationUser> Users => _users ??= new GenericRepository<ApplicationUser>(_context);
        public IGenericRepository<PointsHistory> PointsHistory => _pointsHistory ??= new GenericRepository<PointsHistory>(_context);
        public IGenericRepository<ApplicationUserCompany> UserCompanies => _userCompanies ??= new GenericRepository<ApplicationUserCompany>(_context);
        public IGenericRepository<Sales> Sales => _sales ??= new GenericRepository<Sales>(_context);

        public IGenericRepository<Categories> Categories => _categories ??= new GenericRepository<Categories>(_context);
        private IGenericRepository<Categories> _categories;
        private PointsRepository _points;
        private ICompanyRepository _companies;
        private IGenericRepository<Store> _stores;
        private IGenericRepository<ApplicationUser> _users;
        private IGenericRepository<PointsHistory> _pointsHistory;

        private IGenericRepository<ApplicationUserCompany> _userCompanies;
        private IGenericRepository<Sales> _sales;
        public UnitOfWork(ServerDbContext context, IHttpContextAccessorWrapper httpContextAccessorWrapper)
        {
            _context = context;
            _httpContextAccessorWrapper = httpContextAccessorWrapper;
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