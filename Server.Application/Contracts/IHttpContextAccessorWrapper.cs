using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Application.Contracts
{
    public interface IHttpContextAccessorWrapper
    {
        string ConstructUrl(string controllerAction, Dictionary<string, string> dict);
        string GetUrl();
    }
}