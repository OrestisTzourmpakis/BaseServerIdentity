using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Application.Models.Identity;

namespace Server.Application.Contracts
{
    public interface IAuthService
    {
        Task<AuthResponse> Login(AuthRequest request, bool fromAdmin = false);
        Task<RegistrationResponse> Register(RegistrationRequest request);
    }
}