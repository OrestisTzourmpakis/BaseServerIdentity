using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Domain.Models
{
    public class Points
    {
        public double Total { get; set; }
        public DateTime UserJoined { get; set; }
        public string ApplicationUserId { get; set; }
        public int CompanyId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Company Company { get; set; }
    }
}
