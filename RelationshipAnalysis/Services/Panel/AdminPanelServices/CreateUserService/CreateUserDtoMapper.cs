using RelationshipAnalysis.Dto.Panel.Admin;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.AuthServices.Abstraction;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.CreateUserService.Abstraction;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices.CreateUserService;

public class CreateUserDtoMapper(IPasswordHasher passwordHasher) : ICreateUserDtoMapper
{
    public User Map(CreateUserDto createUserDto)
    {
        return new User
        {
            Username = createUserDto.Username,
            PasswordHash = passwordHasher.HashPassword(createUserDto.Password),
            Email = createUserDto.Email,
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName
        };
    }
}