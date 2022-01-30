using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Server.Application.Contracts;
using Server.Application.Models.Identity;
using Server.Application.Utilities;
using Server.Domain.Models;
using Server.Infrastructure.Persistence;

namespace Server.Infrastructure.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IUnitOfWork unitOfWork, IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthResponse> Login(AuthRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new Exception($"User with {request.Email} not found.");
            if (!user.EmailConfirmed)
                throw new Exception("Please validate your email first.");
            SignInResult result = default(SignInResult);
            if (!string.IsNullOrEmpty(request.ProviderKey) && !string.IsNullOrWhiteSpace(request.LoginProvider))
                result = await _signInManager.ExternalLoginSignInAsync(request.LoginProvider, request.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            else
                result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);
            if (!result.Succeeded)
                throw new Exception($"Credentials for '{request.Email} aren't valid'.");
            var roles = await _userManager.GetRolesAsync(user);
            int? companyId = null;
            if (roles.Contains(UserRoles.CompanyOwner.ToString()))
            {
                // get the company id 
                var company = await _unitOfWork.Companies.GetByIdAsync(c => c.ApplicationUserId == user.Id);
                if (company != null) companyId = company.Id;
            }
            JwtSecurityToken jwtSecurityToken = await GenerateToken(user, roles);
            AuthResponse response = new AuthResponse
            {
                Id = user.Id,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Email = user.Email,
                UserName = user.UserName,
                Roles = roles,
                CompanyId = companyId

            };
            return response;


        }

        public Task<RegistrationResponse> Register(RegistrationRequest request)
        {
            throw new NotImplementedException();
        }

        private async Task<JwtSecurityToken> GenerateToken(ApplicationUser user, IList<string> roles)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var rolesClaims = new List<Claim>();
            for (int i = 0; i < roles.Count; i++)
                rolesClaims.Add(new Claim(ClaimTypes.Role, roles[i]));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.UserName),
            }
            .Union(userClaims)
            .Union(rolesClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials

            );
            return jwtSecurityToken;
        }
    }
}