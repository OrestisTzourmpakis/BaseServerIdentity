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
                         q.SignIn.RequireConfirmedEmail = false;
                     }).AddRoles<IdentityRole>().AddEntityFrameworkStores<ServerDbContext>().AddDefaultTokenProviders();
            // builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
            // builder.AddEntityFrameworkStores<SpatialDbContext>().AddDefaultTokenProviders();
            services.AddScoped<IUserAccount, UserAccount>();
            // var connectionString = "Server=localhost,1434;database=Spatial;User Id=sa;password=Orestis123!;";
            var connectionString = configuration.GetSection("ConnectionStrings:ServerDatabase").Value;
            services.AddDbContext<ServerDbContext>(
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