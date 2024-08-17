using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Controllers;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.DTO;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models;
using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Services;

public class UserCreateService(ApplicationDbContext context, IPasswordHasher passwordHasher) : IUserCreateService
{
    public async Task<ActionResponse<MessageDto>> CreateUser(CreateUserDto createUserDto)
    {
        var isUserExist = context.Users.Select(x => x.Username).ToList().Contains(createUserDto.Username);
        if (isUserExist)
        {
            return BadRequestResult(Resources.UsernameExistsMessage);
        }

        var isEmailExist = context.Users.Select(x => x.Email).ToList().Contains(createUserDto.Email);
        if (isEmailExist)
        {
            return BadRequestResult(Resources.EmailExistsMessage);
        }

        if (createUserDto.Roles.IsNullOrEmpty())
        {
            return BadRequestResult(Resources.EmptyRolesMessage);
        }

        var invalidRoles = createUserDto.Roles.FindAll(r => !context.Roles.Select(R => R.Name)
            .Contains(r));
        if (invalidRoles.Any())
        {
            return BadRequestResult(Resources.InvalidRolesListMessage);
        }

        var user = await AddUser(createUserDto);
        await AddUserRoles(createUserDto, user);
        

        return SuccessResult();
    }

    private ActionResponse<MessageDto> SuccessResult()
    {
        return new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.SucceddfulCreateUser),
            StatusCode = StatusCodeType.Success
        };
    }

    private async Task AddUserRoles(CreateUserDto createUserDto, User user)
    {
        var roles = createUserDto.Roles.Select(r => context.Roles
            .SingleOrDefault(R => R.Name == r));
        roles.ToList().ForEach(r => context.UserRoles.Add(new UserRole()
            { RoleId = r.Id, UserId = user.Id }));
        await context.SaveChangesAsync();
    }

    private async Task<User> AddUser(CreateUserDto createUserDto)
    {
        var user = new User()
        {
            Username = createUserDto.Username,
            PasswordHash = passwordHasher.HashPassword(createUserDto.Password),
            Email = createUserDto.Email,
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
        };
        context.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    private ActionResponse<MessageDto> BadRequestResult(string message)
    {
        return new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(message),
            StatusCode = StatusCodeType.BadRequest
        };
    }
}