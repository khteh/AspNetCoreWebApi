using AutoMapper;
using Web.Api.Core.Domain.Entities;
using Web.Api.Infrastructure.Identity;

namespace Web.Api.Infrastructure.Data.Mapping
{
    public class DataProfile : Profile
    {
        public DataProfile()
        {
            CreateMap<User, AppUser>().ConstructUsing(u => new AppUser(u.UserName, u.Email, u.FirstName, u.LastName))
                                        .ForMember(au => au.Id, opt => opt.Ignore())
                                        .ForAllOtherMembers(o => o.Ignore());
            CreateMap<AppUser, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IdentityId, o => o.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FirstName, o => o.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, o => o.MapFrom(src => src.LastName))
                .ForAllOtherMembers(opt => opt.Ignore());
        }
    }
}