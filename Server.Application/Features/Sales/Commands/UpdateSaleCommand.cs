using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
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
        public IFormFile ImageFile { get; set; }
    }

    public class UpdateSaleCommandHandler : IRequestHandler<UpdateSaleCommand, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public UpdateSaleCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<BaseResponse> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _unitOfWork.Sales.GetByIdAsync(q => q.Id == request.Id);
            // ama to imagefile einai null tote
            // tote kane request.image = sale.image!!!
            // ama omws to imagefile dn einai null shmainei oti exw valei kapoia eikona!!
            // ara ta gnwsta save....
            if (request.ImageFile == null)
                request.Image = sale.Image;
            else
                request.Image = await _fileService.SaveImage(request.ImageFile, $"SalesImage-{sale.CompanyId}-");
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