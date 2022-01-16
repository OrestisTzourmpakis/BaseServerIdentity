using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Server.Application.Contracts;
using Server.Application.Responses;

namespace Server.Application.Features.Sales.Commands
{
    public class DeleteSaleCommand : IRequest<BaseResponse>
    {
        public int Id { get; set; }
    }

    public class DeleteSaleCommandHandler : IRequestHandler<DeleteSaleCommand, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSaleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _unitOfWork.Sales.GetByIdAsync(c => c.Id == request.Id);
            await _unitOfWork.Sales.DeleteAsync(sale);
            await _unitOfWork.Save();
            return new BaseResponse()
            {
                data = sale
            };
        }
    }
}