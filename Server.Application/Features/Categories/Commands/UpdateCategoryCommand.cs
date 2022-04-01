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
    public class UpdateCategoryCommand : IRequest<BaseResponse>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(c => c.Id == request.Id);
            _mapper.Map(request, category);
            _unitOfWork.Categories.UpdateAsync(category);
            await _unitOfWork.Save();
            return new BaseResponse()
            {
                data = category
            };
        }
    }
}