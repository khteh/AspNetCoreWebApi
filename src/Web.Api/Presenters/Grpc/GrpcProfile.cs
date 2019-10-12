using System;
using AutoMapper;
using Web.Api.Core.DTO;
using Web.Api.Core.Domain.Entities;
using Google.Protobuf.WellKnownTypes;
namespace Web.Api.Presenters.Grpc
{
    public class GrpcProfile : Profile
    {
        public GrpcProfile()
        {
            CreateMap<Error, Web.Api.Core.Grpc.Error>().ConstructUsing(i => new Web.Api.Core.Grpc.Error() {Code = i.Code, Description = i.Description});
            CreateMap<RefreshToken, Web.Api.Core.Grpc.RefreshToken>().ConstructUsing(i => new Web.Api.Core.Grpc.RefreshToken() {
                Token = i.Token, 
                UserId = i.UserId, 
                RemoteIpAddress = i.RemoteIpAddress
            }).ForMember(i => i.Expires, o => o.MapFrom(src => Timestamp.FromDateTimeOffset(src.Expires)));
            CreateMap<User, Web.Api.Core.Grpc.User>()
                .ForMember(i => i.Id, o => o.MapFrom(src => src.Id))
                .ForMember(i => i.IdentityId, o => o.MapFrom(src => !string.IsNullOrEmpty(src.IdentityId) ? src.IdentityId : string.Empty))
                .ForMember(i => i.FirstName, o => o.MapFrom(src => !string.IsNullOrEmpty(src.FirstName) ? src.FirstName : string.Empty))
                .ForMember(i => i.LastName, o => o.MapFrom(src => !string.IsNullOrEmpty(src.LastName) ? src.LastName : string.Empty))
                .ForMember(i => i.UserName, o => o.MapFrom(src => !string.IsNullOrEmpty(src.UserName) ? src.UserName : string.Empty))
                .ForMember(i => i.Email, o => o.MapFrom(src => !string.IsNullOrEmpty(src.Email) ? src.Email : string.Empty))
                .ForMember(i => i.RefreshTokens, o => o.MapFrom(src => src.RefreshTokens));
        }
    }
}