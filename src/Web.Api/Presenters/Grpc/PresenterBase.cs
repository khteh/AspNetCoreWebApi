using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
using Web.Api.Serialization;

namespace Web.Api.Presenters.Grpc
{
    public abstract class PresenterBase<T> : IOutputPort<T> where T : UseCaseResponseMessage
    {
        protected readonly IMapper _mapper;
        protected Web.Api.Core.Grpc.Response BaseResponse {get; private set;}
        public PresenterBase(IMapper mapper)
        {
            _mapper = mapper;
            BaseResponse = new Web.Api.Core.Grpc.Response();
        }
        public virtual void Handle(T response)
        {
            BaseResponse.Success = response.Success;
            if (response.Errors != null && response.Errors.Any())
                BaseResponse.Errors.AddRange(_mapper.Map<List<Web.Api.Core.Grpc.Error>>(response.Errors));
        }
    }
}