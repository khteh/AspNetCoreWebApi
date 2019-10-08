using System;
using System.Linq;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
using Web.Api.Core.DTO;
using AutoMapper;
namespace Web.Api.Presenters.Grpc
{
    public class ResetPasswordPresenter : PresenterBase<UseCaseResponseMessage>
    {
        public Web.Api.Core.Grpc.Response Response {get => base.BaseResponse; }
        public ResetPasswordPresenter(IMapper mapper) : base(mapper) {}
    }
}