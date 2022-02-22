using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Application.Responses;
using Server.Domain.Models;

namespace Server.Application.Contracts
{
    public interface ICompanyRepository : IGenericRepository<Company>
    {
        Task<List<CompaniesWithCountResponse>> GetComapniesWithCount();
    }
}