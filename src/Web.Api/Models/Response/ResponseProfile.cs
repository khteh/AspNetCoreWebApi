using AutoMapper;
using System.Net;
using Web.Api.Presenters;
using Web.Api.Serialization;
namespace Web.Api.Models.Response;
public class ResponseProfile : Profile
{
    public ResponseProfile()
    {
        CreateMap<RegisterUserResponse, JsonContentResult>()
                .ForMember(dest => dest.StatusCode, o => o.MapFrom(src => (int)(src.Success ? HttpStatusCode.Created : HttpStatusCode.BadRequest)))
                .ForMember(dest => dest.Content, o => o.MapFrom(src => JsonSerializer.SerializeObject(src)));
        CreateMap<LoginResponse, JsonContentResult>()
                .ForMember(dest => dest.StatusCode, o => o.MapFrom(src => (int)(src.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized)))
                .ForMember(dest => dest.Content, o => o.MapFrom(src => JsonSerializer.SerializeObject(src)));
        CreateMap<DeleteUserResponse, JsonContentResult>()
                .ForMember(dest => dest.StatusCode, o => o.MapFrom(src => (int)(src.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest)))
                .ForMember(dest => dest.Content, o => o.MapFrom(src => JsonSerializer.SerializeObject(src)));
        CreateMap<ExchangeRefreshTokenResponse, JsonContentResult>()
                .ForMember(dest => dest.StatusCode, o => o.MapFrom(src => (int)(src.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest)))
                .ForMember(dest => dest.Content, o => o.MapFrom(src => JsonSerializer.SerializeObject(src)));
        CreateMap<ChangePasswordResponse, JsonContentResult>()
                .ForMember(dest => dest.StatusCode, o => o.MapFrom(src => (int)(src.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest)))
                .ForMember(dest => dest.Content, o => o.MapFrom(src => JsonSerializer.SerializeObject(src)));
        CreateMap<ResetPasswordResponse, JsonContentResult>()
                .ForMember(dest => dest.StatusCode, o => o.MapFrom(src => (int)(src.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest)))
                .ForMember(dest => dest.Content, o => o.MapFrom(src => JsonSerializer.SerializeObject(src)));
        CreateMap<LockUserResponse, JsonContentResult>()
                .ForMember(dest => dest.StatusCode, o => o.MapFrom(src => (int)(src.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest)))
                .ForMember(dest => dest.Content, o => o.MapFrom(src => JsonSerializer.SerializeObject(src)));
    }
}