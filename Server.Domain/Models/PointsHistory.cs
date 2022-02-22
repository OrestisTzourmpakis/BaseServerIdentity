using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Domain.Models
{
    public class PointsHistory
    {
        public int Id { get; set; }
        [Required]
        public string ApplicationUserId { get; set; }
        public DateTime TransactionDate { get; set; }
        public int CompanyId { get; set; }
        public double Transaction { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Company Company { get; set; }

    }
}