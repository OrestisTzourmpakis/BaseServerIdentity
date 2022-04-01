using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Server.Application.Contracts;
using Server.Application.Responses;

namespace Server.Application.Features.Categories.Commands
{
    public class AddCategoryCommand : IRequest<BaseResponse>
    {
        public string Name { get; set; }
    }

    public class AddCategoryCommandHandler : IRequestHandler<AddCategoryCommand, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AddCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.Categories.AddAsync(_mapper.Map<Domain.Models.Categories>(request));
            await _unitOfWork.Save();
            return new BaseResponse
            {
                data = result
            };
        }
    }
}