using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Infrastructure.Options
{
    public class MailOptions
    {
        public const string Mail = "Mail";
        public string Server { get; set; }
        public int Port { get; set; }
        public string FromEmailAddress { get; set; }
        public string DisplayName { get; set; }
        public string FromPassword { get; set; }
    }
}