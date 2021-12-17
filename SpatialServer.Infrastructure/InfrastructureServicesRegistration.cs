using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpatialServer.Application.Contracts;
using SpatialServer.Infrastructure.Persistence;
using SpatialServer.Infrastructure.Repositories;
using SpatialServer.Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SpatialServer.Application.Models.Identity;
using System.Text;
using SpatialServer.Infrastructure.Authentication;

namespace SpatialServer.Infrastructure
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
                         q.SignIn.RequireConfirmedEmail = false;
                     }).AddRoles<IdentityRole>().AddEntityFrameworkStores<SpatialDbContext>().AddDefaultTokenProviders();
            // builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
            // builder.AddEntityFrameworkStores<SpatialDbContext>().AddDefaultTokenProviders();
            services.AddScoped<IUserAccount, UserAccount>();
            // var connectionString = "Server=localhost,1434;database=Spatial;User Id=sa;password=Orestis123!;";
            var connectionString = configuration.GetSection("ConnectionStrings:SpatialDatabase").Value;
            services.AddDbContext<SpatialDbContext>(
                options => options.UseSqlServer(connectionString)
            );

            // Authentication
            services.AddTransient<IAuthService, AuthService>();
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
                }
            );

            return services;

        }
    }
}