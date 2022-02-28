using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Domain.Models
{
    public class Store
    {
        public int Id { get; set; }
        [Required]
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Telephone { get; set; }

        public DateTime Created { get; set; }
        [Required]
        public int CompanyId { get; set; }

        public Company Company { get; set; }

    }
}