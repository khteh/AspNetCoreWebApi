using System;
using System.Linq;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
using Web.Api.Core.DTO;
using AutoMapper;
namespace Web.Api.Presenters.Grpc
{
    public class LockUserPresenter : PresenterBase<UseCaseResponseMessage>
    {
        public Web.Api.Core.Grpc.Response Response {get => BaseResponse; }
        public LockUserPresenter(IMapper mapper) : base(mapper) {}
    }
}