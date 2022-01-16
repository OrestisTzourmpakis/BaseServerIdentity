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

namespace Server.Application.Features.Companies.Commands
{
    public class UpdateCompanyCommand : IRequest<BaseResponse>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public string Website { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Facebook { get; set; }
        public double EuroToPointsRatio { get; set; }
        public double PointsToEuroRatio { get; set; }
        public string ApplicationUserId { get; set; }
    }
    public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateCompanyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
        {
            // find the company first
            var company = await _unitOfWork.Companies.GetByIdAsync(c => c.Id == request.Id);
            if (company == null)
            {
                var failure = new ValidationFailure("Id", $"Company with id: {request.Id} does not exist!");
                throw new ValidationException(new List<ValidationFailure>() { failure });
            }
            // exists update it
            _mapper.Map(request, company);
            await _unitOfWork.Companies.UpdateAsync(company);
            await _unitOfWork.Save();
            return new BaseResponse()
            {
                data = company
            };
        }
    }
}