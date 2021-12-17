using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpatialServer.Application.DTO.Login;
using SpatialServer.Application.DTO.Register;

namespace SpatialServer.Application.Contracts
{
    public interface IUserAccount
    {
        Task<bool> RegisterUser(RegisterDto user);
        Task<bool> LoginUser(LoginDto user);
    }
}