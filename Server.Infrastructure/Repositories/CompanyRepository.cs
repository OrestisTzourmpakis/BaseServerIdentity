using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Server.Application.Contracts;
using Server.Application.Responses;
using Server.Domain.Models;
using Server.Infrastructure.Persistence;

namespace Server.Infrastructure.Repositories
{
    public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
    {
        private readonly ServerDbContext _context;
        private readonly IHttpContextAccessorWrapper _httpContextAccessorWrapper;

        public CompanyRepository(ServerDbContext context, IHttpContextAccessorWrapper httpContextAccessorWrapper) : base(context)
        {
            _context = context;
            _httpContextAccessorWrapper = httpContextAccessorWrapper;
        }

        public async Task<List<CompaniesWithCountResponse>> GetComapniesWithCount()
        {
            var result = await _context.Companies.Include(a => a.Stores).Include(a => a.Points).Include(a => a.Sales).Select(a =>
            new CompaniesWithCountResponse
            {
                Name = a.Name,
                ApplicationUserId = a.ApplicationUserId,
                Logo = _httpContextAccessorWrapper.GetUrl() + "Images/" + a.Logo,
                Website = a.Website,
                Twitter = a.Twitter,
                Facebook = a.Facebook,
                Instagram = a.Instagram,
                EuroToPointsRatio = a.EuroToPointsRatio,
                PointsToEuroRatio = a.PointsToEuroRatio,
                CompanySalesCount = a.Sales.Count,
                CompanyStoresCount = a.Stores.Count,
                CompanyUsersCount = a.Points.Count
            }
            ).ToListAsync();
            return result;
        }

    }
}