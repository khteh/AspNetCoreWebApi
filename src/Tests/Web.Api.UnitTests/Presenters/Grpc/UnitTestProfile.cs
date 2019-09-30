using System;
using AutoMapper;
using Web.Api.Core.DTO;
namespace Web.Api.UnitTests.Presenters.Grpc
{
    public class UnitTestProfile : Profile
    {
        public UnitTestProfile()
        {
            CreateMap<Error, Web.Api.Core.Grpc.Error>().ConstructUsing(i => new Web.Api.Core.Grpc.Error() {Code = i.Code, Description = i.Description});
        }
    }
}