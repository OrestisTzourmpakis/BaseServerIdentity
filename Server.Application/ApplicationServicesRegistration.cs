using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Server.Application
{
    public static class ApplicationServicesRegistration
    {

        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            // services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            // google authentication!!
            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = configuration["GoogleApi:ClientId"];
                    options.ClientSecret = configuration["GoogleApi:ClientSecret"];
                })
                .AddFacebook(
                    options =>
                    {
                        options.ClientId = configuration["FacebookApi:ClientId"];
                        options.ClientSecret = configuration["FacebookApi:ClientSecret"];
                    }
                )
                ;
            return services;
        }
    }
}