using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Server.Application.Exceptions;

namespace Server.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext, IWebHostEnvironment env)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex, env);
            }
        }
        private Task HandleExceptionAsync(HttpContext context, Exception exception, IWebHostEnvironment env)
        {
            context.Response.ContentType = "application/json";
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string result = JsonConvert.SerializeObject(new { errorMessage = exception.Message });
            switch (exception)
            {
                case BadRequestException badRequestException:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                case ValidationException validationException:
                    statusCode = HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(new { errorMessage = validationException.Errors });
                    break;
                case NotFoundException notFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    break;
                default:
                    if(exception.Message == "Sequence contains more than one element.")
                        result = JsonConvert.SerializeObject(new { errorMessage = "To email χρησιμοποιείται απο άλλον χρήστη" });                        
                        else if(exception.Message == "Παρακαλούμε όπως επικυρώσετε το email σας. ΠΛηροφορίες θα βρείτε στο email που σας έχει σταλεί.")
                        result = JsonConvert.SerializeObject(new { errorMessage = "Παρακαλούμε όπως επικυρώσετε το email σας. Πληροφορίες θα βρείτε στο email που σας έχει σταλεί." });
                         else if(exception.Message == "Incorrect Email or password.")
                        result = JsonConvert.SerializeObject(new { errorMessage = "Τα στοιχεία που δώσατε είναι λανθασμένα." });
                        
                    else result = JsonConvert.SerializeObject(new { errorMessage = "Κατι πήγε στραβά. Παρακαλώ προσπαθήστε αργότερα." });
                    break;
            }
            context.Response.StatusCode = (int)statusCode;
            return context.Response.WriteAsync(result);

        }
    }
}