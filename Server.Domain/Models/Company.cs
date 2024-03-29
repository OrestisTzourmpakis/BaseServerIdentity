using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Domain.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public string Website { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Facebook { get; set; }
        public double PointsToEuroRatio { get; set; }
        public double EuroToPointsRatio { get; set; }
        public string ApplicationUserId { get; set; }
        public int? CategoryId { get; set; }
        public Categories Category { get; set; }
        public ICollection<Store> Stores { get; set; }
        public ICollection<Sales> Sales { get; set; }
        public ICollection<Points> Points { get; set; }


        public ApplicationUser Owner { get; set; }
    }
}