using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Server.Application.Contracts;
using Server.Application.Responses;
using Server.Domain.Models;

namespace Server.Application.Features.Stores.Commands
{
    public class AddStoreCommand : IRequest<BaseResponse>
    {
        public int CompanyId { get; set; }
        public string Address { get; set; }
    }

    public class AddStoreCommandHandler : IRequestHandler<AddStoreCommand, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AddStoreCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse> Handle(AddStoreCommand request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.Stores.AddAsync(_mapper.Map<Store>(request));
            await _unitOfWork.Save();
            return new BaseResponse()
            {
                data = result
            };
        }
    }
}