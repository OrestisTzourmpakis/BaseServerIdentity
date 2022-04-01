using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Domain.Models
{
    public class Categories
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Company> Companies { get; set; }
    }
}