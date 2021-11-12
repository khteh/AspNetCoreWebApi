using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using Web.Api.Core.Interfaces;
namespace Web.Api.Presenters.Grpc;
public abstract class PresenterBase<T> : IOutputPort<T> where T : UseCaseResponseMessage
{
    protected readonly IMapper _mapper;
    protected Web.Api.Identity.Response BaseResponse {get; init;}
    public PresenterBase(IMapper mapper)
    {
        _mapper = mapper;
        BaseResponse = new Web.Api.Identity.Response();
    }
    public virtual void Handle(T response)
    {
        BaseResponse.Success = response.Success;
        if (response.Errors != null && response.Errors.Any())
            BaseResponse.Errors.AddRange(_mapper.Map<List<Web.Api.Identity.Error>>(response.Errors));
    }
}