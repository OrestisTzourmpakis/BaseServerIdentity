using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Server.Application.Contracts;
using Server.Application.Responses;

namespace Server.Application.Features.Stores.Commands
{
    public class UpdateStoreCommand : IRequest<BaseResponse>
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Telephone { get; set; }
    }
    public class UpdateStoreCommandHandler : IRequestHandler<UpdateStoreCommand, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateStoreCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse> Handle(UpdateStoreCommand request, CancellationToken cancellationToken)
        {
            var store = await _unitOfWork.Stores.GetByIdAsync(c => c.Id == request.Id);
            _mapper.Map(request, store);
            _unitOfWork.Stores.UpdateAsync(store);
            await _unitOfWork.Save();
            return new BaseResponse()
            {
                data = store
            };
        }
    }
}