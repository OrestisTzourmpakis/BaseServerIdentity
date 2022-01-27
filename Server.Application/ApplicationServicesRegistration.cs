using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Server.Application
{
    public static class ApplicationServicesRegistration
    {

        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            // services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            // google authentication!!
            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = "715533597070-a5oamiocsjaheqnvirc55g1j14avef47.apps.googleusercontent.com";
                    options.ClientSecret = "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";
                });
            return services;
        }
    }
}