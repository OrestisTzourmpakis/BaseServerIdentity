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

namespace Server.Application.Features.Categories.Commands
{
    public class DeleteCategoryCommand : IRequest<BaseResponse>
    {
        public int Id { get; set; }
    }
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(c => c.Id == request.Id);
            if (category == null)
            {
                var failure = new ValidationFailure("Id", $"Store with id:{request.Id} was not found.");
                // throw exception oti den yparxei to store
                throw new ValidationException(new List<ValidationFailure>() { failure });
            }
            _unitOfWork.Categories.DeleteAsync(category);
            await _unitOfWork.Save();
            return new BaseResponse()
            {
                data = "Category deleted!"
            };
        }
    }
}