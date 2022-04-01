using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Domain.Models;

namespace Server.Application.Models.CompanyModel
{
    public class GetAllCompaniesModel
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
        public int? CategoryId { get; set; }
        public string Category { get; set; }
        public string OwnerEmail { get; set; }
    }
}