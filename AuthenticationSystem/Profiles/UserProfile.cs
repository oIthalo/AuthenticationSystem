using AuthenticationSystem.Data.DataRequests;
using AuthenticationSystem.Data.DataResponses;
using AuthenticationSystem.Models;
using AutoMapper;
namespace AuthenticationSystem.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserRequestRegister, User>().ReverseMap();
        CreateMap<UserResponse, User>().ReverseMap()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Name)); ;
        CreateMap<UserRequestRegister, UserResponse>().ReverseMap();
    }
}