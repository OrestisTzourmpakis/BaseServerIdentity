using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Application.DTO.Login;
using Server.Application.DTO.Register;
using Server.Application.Features.UserAccount.Commands.Login;
using Server.Application.Features.UserAccount.Commands.Register;

namespace Server.Application.Contracts
{
    public interface IUserAccount
    {
        Task<bool> RegisterUser(RegisterCommand user);
        Task<bool> LoginUser(LoginCommand user);
    }
}