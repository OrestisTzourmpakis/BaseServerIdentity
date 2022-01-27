using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Server.Application.Contracts;
using Server.Application.Exceptions;
using Server.Application.Responses;
using Server.Domain.Models;

namespace Server.Application.Features.Sales.Commands
{
    public class AddSaleCommand : IRequest<BaseResponse>
    {

        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string Email { get; set; }
        public int CompanyId { get; set; }
        public IFormFile ImageFile { get; set; }
    }
    public class AddSaleCommandHandler : IRequestHandler<AddSaleCommand, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AddSaleCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<BaseResponse> Handle(AddSaleCommand request, CancellationToken cancellationToken)
        {
            // find the company id of the user
            var imageName = await SaveImage(request.ImageFile);
            var user = await _userManager.FindByEmailAsync(request.Email);
            var userCompany = await _unitOfWork.Companies.GetByIdAsync(c => c.ApplicationUserId == user.Id);
            if (userCompany == null)
            {
                var failure = new ValidationFailure("Id", $"User with email: {request.Email} does not have a registered company.");
                throw new ValidationException(new List<ValidationFailure>() { failure });
            }
            request.CompanyId = userCompany.Id;
            var result = await _unitOfWork.Sales.AddAsync(_mapper.Map<Domain.Models.Sales>(request));
            await _unitOfWork.Save();
            return new BaseResponse()
            {
                data = result
            };
        }

        private async Task<string> SaveImage(IFormFile imageFile)
        {
            string imageName = Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray().ToString().Replace(' ', '-');
            imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(imageFile.FileName);
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images", imageName);
            // save the image
            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            return imageName;
        }
    }
}