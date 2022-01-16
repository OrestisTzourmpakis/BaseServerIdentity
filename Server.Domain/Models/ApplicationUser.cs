using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Server.Domain.Models
{
    public class ApplicationUser : IdentityUser
    {

        public ICollection<Points> Points { get; set; }
        public Company Company { get; set; }

    }
}