using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Domain.Models
{
    public class Points
    {
        public int Id { get; set; }
        public string CustomUserId { get; set; }
        public ApplicationUser CustomUser { get; set; }

        public int StoreId { get; set; }
        public Store Store { get; set; }

        public double Total { get; set; }
    }
}