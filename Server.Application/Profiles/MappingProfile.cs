using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Server.Application.DTO.Login;
using Server.Application.DTO.Register;
using Server.Application.Features.Categories.Commands;
using Server.Application.Features.Companies.Commands;
using Server.Application.Features.Sales.Commands;
using Server.Application.Features.Stores.Commands;
using Server.Application.Features.UserAccount.Commands;
using Server.Application.Features.UserAccount.Commands.Login;
using Server.Application.Features.UserAccount.Commands.Register;
using Server.Application.Features.UserAccount.Queries;
using Server.Application.Models.CompanyModel;
using Server.Application.Models.Identity;
using Server.Application.Responses;
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
            CreateMap<LoginCommand, AuthRequest>().ReverseMap();
            CreateMap<RegisterCommand, ApplicationUser>().ReverseMap();
            CreateMap<RegisterFromAdminCommand, ApplicationUser>().ReverseMap();
            CreateMap<ResetPasswordCommand, ResetPasswordModel>().ReverseMap();
            CreateMap<ResetPasswordQuery, ResetPasswordModel>().ReverseMap();
            #endregion UserAccount
            #region Companies
            CreateMap<AddCompanyCommand, Company>().ReverseMap();
            CreateMap<UpdateCompanyCommand, Company>().ReverseMap();
            CreateMap<GetAllCompaniesModel, Company>()
                .ForPath(dest => dest.Owner.Email,
                a => a.MapFrom(src => src.OwnerEmail)
                )
                .ForPath(dest => dest.Category.Name, a => a.MapFrom(src => src.Category))
                .ReverseMap();
            CreateMap<Company, CompaniesWithCountResponse>().ReverseMap();
            #endregion Companies

            #region Stores
            CreateMap<AddStoreCommand, Store>().ReverseMap();
            CreateMap<UpdateStoreCommand, Store>().ReverseMap();
            #endregion Stores

            #region  Sales
            CreateMap<AddSaleCommand, Sales>().ReverseMap();
            CreateMap<UpdateSaleCommand, Sales>().ReverseMap();
            #endregion Sales
            #region Points
            CreateMap<PointsUserResponse, Points>()
                .ForPath(dest => dest.ApplicationUser.UserName,
                a => a.MapFrom(src => src.Username))
                .ForPath(dest => dest.ApplicationUser.Email,
                a => a.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserJoined, opt => opt.MapFrom(src => src.DateJoined)).ReverseMap();

            #endregion Points
            #region Categories
            CreateMap<Categories, AddCategoryCommand>().ReverseMap();
            CreateMap<Categories, UpdateCategoryCommand>().ReverseMap();
            #endregion
        }
    }
}