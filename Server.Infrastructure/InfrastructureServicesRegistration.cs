using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Server.Application.Contracts;
using Server.Infrastructure.Persistence;
using Server.Infrastructure.Repositories;
using Server.Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Server.Application.Models.Identity;
using System.Text;
using Server.Infrastructure.Authentication;
using Microsoft.Extensions.Logging;
using Server.Infrastructure.Mail;
using Server.Infrastructure.Options;
using Server.Infrastructure.Helper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Server.Infrastructure
{
    public static class InfrastructureServicesRegistration
    {

        public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddDefaultIdentity<ApplicationUser>(q =>
                     {
                         q.Password.RequiredLength = 8;
                         q.Password.RequiredUniqueChars = 3;
                         q.SignIn.RequireConfirmedEmail = true;
                         q.User.RequireUniqueEmail = true;
                     }).AddRoles<IdentityRole>().AddEntityFrameworkStores<ServerDbContext>().AddDefaultTokenProviders();
            // builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
            // builder.AddEntityFrameworkStores<SpatialDbContext>().AddDefaultTokenProviders();
            services.AddScoped<IUserAccount, UserAccount>();
            // var connectionString = "Server=localhost,1434;database=Spatial;User Id=sa;password=Orestis123!;";
            var connectionString = configuration.GetSection("ConnectionStrings:ServerDatabase").Value;
            services.AddDbContext<ServerDbContext>(
                options => options.UseSqlServer(connectionString).LogTo(Console.WriteLine, LogLevel.Information)
            );

            // Email
            services.Configure<MailOptions>(configuration.GetSection(MailOptions.Mail));
            services.AddTransient<IEmailSender, EmailSender>();

            // Authentication
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            // FileService
            services.AddScoped<IFileService, FileService>();

            // HttpContextAccessor
            services.AddTransient<IHttpContextAccessorWrapper, HttpContextAccessorWrapperRepository>();

            // we tell the authentication scheme to use jwt scheme
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(
                o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidIssuer = configuration["JwtSettings:Issuer"],
                        ValidAudience = configuration["JwtSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]))
                    };
                    o.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var url = context.Request.GetEncodedUrl();
                            context.Request.Headers.Remove("Authorization");
                            context.Token = context.Request.Cookies["jwt"];
                            return Task.CompletedTask;
                        }
                    };
                }
            );

            return services;

        }
    }
}