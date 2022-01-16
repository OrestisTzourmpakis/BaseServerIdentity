using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation.Results;
using MediatR;
using Server.Application.Contracts;
using Server.Application.Exceptions;
using Server.Application.Responses;

namespace Server.Application.Features.Stores.Commands
{
    public class DeleteStoreCommand : IRequest<BaseResponse>
    {
        public int Id { get; set; }
    }

    public class DeleteStoreCommandHandler : IRequestHandler<DeleteStoreCommand, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeleteStoreCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse> Handle(DeleteStoreCommand request, CancellationToken cancellationToken)
        {
            var store = await _unitOfWork.Stores.GetByIdAsync(c => c.Id == request.Id);
            if (store == null)
            {
                var failure = new ValidationFailure("Id", $"Store with id:{request.Id} was not found.");
                // throw exception oti den yparxei to store
                throw new ValidationException(new List<ValidationFailure>() { failure });
            }
            _unitOfWork.Stores.DeleteAsync(store);
            await _unitOfWork.Save();
            return new BaseResponse()
            {
                data = "Store deleted!"
            };
        }
    }
}