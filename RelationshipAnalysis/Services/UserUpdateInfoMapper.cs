using AutoMapper;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Models;

namespace RelationshipAnalysis.Services;

public class UserUpdateInfoMapper : Profile
{
    public UserUpdateInfoMapper()
    {
        CreateMap<UserUpdateInfoDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) 
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
        CreateMap<User, UserOutputInfoDto>()
            .ForMember(dest => dest.Roles, opt => opt.Ignore());
    }
}