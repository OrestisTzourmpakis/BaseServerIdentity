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

namespace Server.Application.Features.Categories.Queries
{
    public class GetCategoryByIdQuery : IRequest<BaseResponse>
    {
        public int Id { get; set; }
    }

    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, BaseResponse>
    {

        private readonly IUnitOfWork _unitOfWork;

        public GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(c => c.Id == request.Id);
            if (category == null)
            {
                var failure = new ValidationFailure("Id", $"Category with id:{request.Id} was not found.");
                // throw exception oti den yparxei to store
                throw new ValidationException(new List<ValidationFailure>() { failure });
            }
            return new BaseResponse
            {
                data = category
            };
        }
    }
}