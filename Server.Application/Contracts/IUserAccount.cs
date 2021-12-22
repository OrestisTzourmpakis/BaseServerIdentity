using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Application.DTO.Login;
using Server.Application.DTO.Register;

namespace Server.Application.Contracts
{
    public interface IUserAccount
    {
        Task<bool> RegisterUser(RegisterDto user);
        Task<bool> LoginUser(LoginDto user);
    }
}