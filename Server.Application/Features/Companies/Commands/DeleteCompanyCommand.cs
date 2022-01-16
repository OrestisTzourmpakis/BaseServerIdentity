using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;
using MediatR;
using Server.Application.Contracts;
using Server.Application.Exceptions;
using Server.Application.Responses;

namespace Server.Application.Features.Companies.Commands
{
    public class DeleteCompanyCommand : IRequest<BaseResponse>
    {
        public int Id { get; set; }
    }

    public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCompanyCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(c => c.Id == request.Id);
            if (company == null)
            {
                var failure = new ValidationFailure("Id", $"Company with id:{request.Id} was not found");
                throw new ValidationException(new List<ValidationFailure>() { failure });
            }
            // company found now delete it!
            await _unitOfWork.Companies.DeleteAsync(company);
            await _unitOfWork.Save();
            return new BaseResponse()
            {
                data = $"Company with id:{request.Id} deleted!!"
            };
        }
    }
}