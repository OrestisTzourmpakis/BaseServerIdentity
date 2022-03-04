using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Server.Application.Contracts;
using Server.Application.Responses;
using Server.Domain.Models;
using Server.Infrastructure.Persistence;

namespace Server.Infrastructure.Repositories
{
    public class PointsRepository : GenericRepository<Points>, IPointsRepository
    {
        private readonly ServerDbContext _context;
        private readonly IHttpContextAccessorWrapper _httpContextAccessorWrapper;

        public PointsRepository(ServerDbContext context, IHttpContextAccessorWrapper httpContextAccessorWrapper) : base(context)
        {
            _context = context;
            _httpContextAccessorWrapper = httpContextAccessorWrapper;
        }

        public async Task<List<PointsUserResponse>> GetTotalUsersPoints()
        {
            var result = await _context.Points.Include(b => b.ApplicationUser).GroupBy(c => new
            {
                c.ApplicationUserId,
                c.ApplicationUser.UserName,
                c.ApplicationUser.Email,
                c.ApplicationUser.DateJoined
            }).
                    Select(g => new PointsUserResponse
                    {
                        Username = g.Key.UserName,
                        Email = g.Key.Email,
                        Total = g.Sum(s => s.Total),
                        DateJoined = g.Key.DateJoined
                    }).OrderByDescending(s => s.Total).Take(30).ToListAsync();
            //var users = await result.Include(c => c.ApplicationUser).ToListAsync();
            return result;
        }
    }
}