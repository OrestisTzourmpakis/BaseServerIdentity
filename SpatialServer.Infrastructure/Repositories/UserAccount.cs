using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SpatialServer.Application.Contracts;
using SpatialServer.Application.DTO.Login;
using SpatialServer.Application.DTO.Register;
using SpatialServer.Application.Exceptions;
using SpatialServer.Domain.Models;

namespace SpatialServer.Infrastructure.Repositories
{
    public class UserAccount : IUserAccount
    {
        private readonly IMapper _mapper;
        private SignInManager<ApplicationUser> _signInManager;
        private UserManager<ApplicationUser> _userManager;

        public UserAccount(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<bool> LoginUser(LoginDto user)
        {
            await _signInManager.SignInAsync(_mapper.Map<ApplicationUser>(user), true);
            return true;
        }
        public async Task<bool> RegisterUser(RegisterDto user)
        {
            var saveUser = await _userManager.CreateAsync(_mapper.Map<ApplicationUser>(user), user.Password);
            if (!saveUser.Succeeded)
                throw new ValidationException(saveUser.Errors);
            return true;
        }
    }
}