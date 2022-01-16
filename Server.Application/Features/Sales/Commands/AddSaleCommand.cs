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
    public class AddSaleCommand : IRequest<BaseResponse>
    {

        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public int CompanyId { get; set; }
    }
    public class AddSaleCommandHandler : IRequestHandler<AddSaleCommand, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AddSaleCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse> Handle(AddSaleCommand request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.Sales.AddAsync(_mapper.Map<Domain.Models.Sales>(request));
            await _unitOfWork.Save();
            return new BaseResponse()
            {
                data = result
            };
        }
    }
}