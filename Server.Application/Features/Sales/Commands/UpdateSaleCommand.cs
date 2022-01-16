using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Server.Application.Contracts;
using Server.Application.Responses;

namespace Server.Application.Features.Sales.Commands
{
    public class UpdateSaleCommand : IRequest<BaseResponse>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
    }

    public class UpdateSaleCommandHandler : IRequestHandler<UpdateSaleCommand, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateSaleCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _unitOfWork.Sales.GetByIdAsync(q => q.Id == request.Id);
            _mapper.Map(request, sale);
            await _unitOfWork.Sales.UpdateAsync(sale);
            await _unitOfWork.Save();
            return new BaseResponse()
            {
                data = sale
            };
        }
    }
}