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

namespace Server.Application.Features.Companies.Queries
{
    public class GetCompanyByIdQuery : IRequest<BaseResponse>
    {
        public int Id { get; set; }
    }
    public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public async Task<BaseResponse> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(c => c.Id == request.Id);
            if (company == null)
            {
                var failure = new ValidationFailure("Id", $"Company with id: {request.Id} was not found!");
                throw new ValidationException(new List<ValidationFailure>() { failure });
            }
            return new BaseResponse() { data = company };
        }
    }
}