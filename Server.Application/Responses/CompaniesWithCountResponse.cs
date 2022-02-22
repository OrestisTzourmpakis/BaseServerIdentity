using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Domain.Models;

namespace Server.Application.Responses
{
    public class CompaniesWithCountResponse : Company
    {
        public int CompanyUsersCount { get; set; }
        public int CompanySalesCount { get; set; }
        public int CompanyStoresCount { get; set; }
    }
}