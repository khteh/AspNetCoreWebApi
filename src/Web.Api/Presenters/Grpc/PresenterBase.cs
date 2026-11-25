using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using Web.Api.Core.Interfaces;
namespace Web.Api.Presenters.Grpc;
public abstract class PresenterBase<T> : IOutputPort<T> where T : UseCaseResponseMessage
{
    protected readonly IMapper _mapper;
    protected Identity.Response BaseResponse { get; init; }
    public PresenterBase(IMapper mapper)
    {
        _mapper = mapper;
        BaseResponse = new Identity.Response();
    }
    public virtual async Task Handle(T response)
    {
        BaseResponse.Success = response.Success;
        if (response.Errors != null && response.Errors.Any())
            BaseResponse.Errors.AddRange(_mapper.Map<List<Identity.Error>>(response.Errors));
    }
    /// <summary>proto3 string id: empty means "no id" (e.g. a failed request), not "00000000-0000-0000-0000-000000000000".</summary>
    protected static string ToProtoId(Guid id) => id == Guid.Empty ? string.Empty : id.ToString();
}