using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpatialServer.Application.Models.Identity;

namespace SpatialServer.Application.Contracts
{
    public interface IAuthService
    {
        Task<AuthResponse> Login(AuthRequest request);
        Task<RegistrationResponse> Register(RegistrationRequest request);
    }
}