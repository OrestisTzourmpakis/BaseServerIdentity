using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Server.Application.DTO.Login;
using Server.Application.DTO.Register;
using Server.Application.Features.UserAccount.Commands.Login;
using Server.Application.Features.UserAccount.Commands.Register;
using Server.Application.Models.Identity;
using Server.Domain.Models;

namespace Server.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            #region UserAccount
            CreateMap<ApplicationUser, LoginDto>().ReverseMap();
            CreateMap<ApplicationUser, RegisterDto>().ReverseMap();
            CreateMap<LoginDto, AuthRequest>().ReverseMap();
            CreateMap<LoginCommand, ApplicationUser>().ReverseMap();
            CreateMap<RegisterCommand, ApplicationUser>().ReverseMap();
            #endregion UserAccount
        }
    }
}