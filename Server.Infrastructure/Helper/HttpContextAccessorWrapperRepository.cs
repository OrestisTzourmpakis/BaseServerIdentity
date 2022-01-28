using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Server.Application.Contracts;

namespace Server.Infrastructure.Helper
{
    public class HttpContextAccessorWrapperRepository : IHttpContextAccessorWrapper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string baseUrl;

        public HttpContextAccessorWrapperRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            baseUrl = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host.Value + "/";

        }

        public string ConstructUrl(string controllerAction, Dictionary<string, string> dict)
        {
            var hostController = baseUrl + controllerAction;
            // set the query parameters
            string queryString = "?";
            int dictLength = dict.Count();
            int increment = 1;
            foreach (var kvp in dict)
            {
                queryString += $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}";
                if (increment != dictLength)
                    queryString += "&";
                increment++;
                // add the query parameters ?email=dgfgg &token=sdffsg ....
            }
            var finalUrl = hostController + queryString;
            return finalUrl;
        }

        public string GetUrl()
        {
            return baseUrl;
        }
    }
}