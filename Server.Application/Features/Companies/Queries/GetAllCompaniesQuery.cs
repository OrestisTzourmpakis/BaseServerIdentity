using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Server.Application.Contracts;
using Server.Application.Models.CompanyModel;
using Server.Application.Responses;
using Server.Domain.Models;

namespace Server.Application.Features.Companies.Queries
{
    public class GetAllCompaniesQuery : IRequest<BaseResponse>
    {

    }

    public class GetAllCompaniesQueryHandler : IRequestHandler<GetAllCompaniesQuery, BaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllCompaniesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse> Handle(GetAllCompaniesQuery request, CancellationToken cancellationToken)
        {
            var includeList = new List<Expression<Func<Company, object>>>() { c => c.Owner };
            // get all companies
            var companies = await _unitOfWork.Companies.GetAsync(includes: includeList);
            var mappedCompanies = _mapper.Map<List<GetAllCompaniesModel>>(companies);
            Type type = companies.GetType();
            Type listType = typeof(List<>).MakeGenericType(new[] { type });
            IList list = (IList)Activator.CreateInstance(listType);
            return new BaseResponse()
            {
                data = mappedCompanies
            };
        }
    }
}