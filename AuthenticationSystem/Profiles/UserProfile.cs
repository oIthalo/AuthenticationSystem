using AuthenticationSystem.Data.DataRequests;
using AuthenticationSystem.Data.DataResponses;
using AuthenticationSystem.Models;
using AutoMapper;
namespace AuthenticationSystem.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<RequestRegister, User>().ReverseMap();
        CreateMap<ResponseUser, User>().ReverseMap()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Name)); ;
        CreateMap<RequestRegister, ResponseUser>().ReverseMap();
    }
}