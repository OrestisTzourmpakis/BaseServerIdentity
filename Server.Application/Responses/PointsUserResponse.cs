using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Application.Responses
{
    public class PointsUserResponse
    {
        public double Total { get; set; }
        public DateTime DateJoined { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }
}