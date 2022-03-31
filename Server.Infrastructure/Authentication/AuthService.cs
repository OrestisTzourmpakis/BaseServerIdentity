using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Server.Application.Contracts;
using Server.Application.Models.Identity;
using Server.Application.Options;
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

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DomainsOptions _domainOptions;
        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IUnitOfWork unitOfWork, IOptions<JwtSettings> jwtSettings, IHttpContextAccessor httpContextAccessor, IOptionsSnapshot<DomainsOptions> domainsOptions)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _domainOptions = domainsOptions.Value;
        }

        public async Task<bool> CheckEmailConfirmation(AuthRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (!user.EmailConfirmed)
                throw new Exception("Παρακαλούμε όπως επικυρώσετε το email σας. ΠΛηροφορίες θα βρείτε στο email που σας έχει σταλεί.");
            return true;
        }
        public async Task<AuthResponse> Login(AuthRequest request, bool fromAdmin = false)
        {
            var cookies = _httpContextAccessor.HttpContext.Request.Cookies.ToList();
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new Exception($"Incorrect Email or password.");
            if (!user.EmailConfirmed)
                throw new Exception("Παρακαλούμε όπως επικυρώσετε το email σας. ΠΛηροφορίες θα βρείτε στο email που σας έχει σταλεί.");
            SignInResult result = default(SignInResult);
            if (!string.IsNullOrEmpty(request.ProviderKey) && !string.IsNullOrWhiteSpace(request.LoginProvider))
                result = await _signInManager.ExternalLoginSignInAsync(request.LoginProvider, request.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            else
                result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);
            if (!result.Succeeded)
                throw new Exception($"Incorrect Email or password.");
            var roles = await _userManager.GetRolesAsync(user);
            int? companyId = null;
            if (roles.Contains(UserRoles.CompanyOwner.ToString()))
            {
                // get the company id 
                var company = await _unitOfWork.Companies.GetByIdAsync(c => c.ApplicationUserId == user.Id);
                if (company != null) companyId = company.Id;
            }
            JwtSecurityToken jwtSecurityToken = await GenerateToken(user, roles);
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            AuthResponse response = new AuthResponse
            {
                Id = user.Id,
                Token = token,
                Email = user.Email,
                UserName = user.UserName,
                Roles = roles,
                CompanyId = companyId

            };
            if (fromAdmin)
            {
                // check the roles if he is company owner or admin
                if (roles.Contains(UserRoles.CompanyOwner.ToString()) || roles.Contains(UserRoles.Administrator.ToString()))
                {
                    _httpContextAccessor.HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.None,
                        Secure = true,
                        Domain = _domainOptions.AdminDomain

                    });
                }
            }
            else
            {
                // he is from the webapp
                if (!roles.Contains(UserRoles.CompanyOwner.ToString()) && !roles.Contains(UserRoles.Administrator.ToString()))
                {
                    _httpContextAccessor.HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.None,
                        Secure = true,
                        Domain = _domainOptions.WebDomain
                    });
                }

            }
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
                new Claim("uid", user.Id),
            }
            .Union(userClaims)
            .Union(rolesClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMonths(2),
                signingCredentials: signingCredentials

            );
            return jwtSecurityToken;
        }
    }
}