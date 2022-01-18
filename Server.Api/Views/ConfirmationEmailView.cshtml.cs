using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Server.Api.Views
{
    public class ConfirmationEmailView : PageModel
    {
        private readonly ILogger<ConfirmationEmailView> _logger;

        public ConfirmationEmailView(ILogger<ConfirmationEmailView> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
