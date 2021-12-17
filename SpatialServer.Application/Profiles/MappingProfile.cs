using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SpatialServer.Application.DTO.Login;
using SpatialServer.Application.DTO.Register;
using SpatialServer.Application.Models.Identity;
using SpatialServer.Domain.Models;

namespace SpatialServer.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            #region UserAcctoun
            CreateMap<ApplicationUser, LoginDto>().ReverseMap();
            CreateMap<ApplicationUser, RegisterDto>().ReverseMap();
            CreateMap<LoginDto, AuthRequest>().ReverseMap();
            #endregion UserAccount
        }
    }
}