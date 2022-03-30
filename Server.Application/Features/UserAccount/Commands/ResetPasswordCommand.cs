using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Server.Application.Extensions;
using Server.Application.Models.Identity;
using Server.Application.Utilities;
using Server.Domain.Models;

namespace Server.Application.Features.UserAccount.Commands
{
    public class ResetPasswordCommand : IRequest<ViewResult>
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
        public bool ModelIsValid { get; set; }
        public ViewDataDictionary ViewData { get; set; }
    }

    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ViewResult>
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public ResetPasswordCommandHandler(IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<ViewResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var model = _mapper.Map<ResetPasswordModel>(request);
            var htmlView = new HtmlView(request.ViewData);
            if (!request.ModelIsValid)
            {

                return htmlView.GetViewResult<ResetPasswordModel>(HtmlTemplates.ResetPasswordForm, model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {

                    return htmlView.GetViewResult<ResetPasswordModel>(HtmlTemplates.ResetPassSucceedView, model);
                }else{
                    request.ModelIsValid = false;
                    return htmlView.GetViewResult<ResetPasswordModel>(HtmlTemplates.ResetPasswordForm, model);
                }

            }
            return htmlView.GetViewResult<ResetPasswordModel>(HtmlTemplates.ResetPasswordForm, model);

        }
    }
}