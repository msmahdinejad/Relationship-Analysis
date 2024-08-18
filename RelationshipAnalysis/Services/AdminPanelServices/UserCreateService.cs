using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.AdminPanelServices.Abstraction;
using RelationshipAnalysis.Services.UserPanelServices.Abstraction.AuthServices.Abstraction;

namespace RelationshipAnalysis.Services.AdminPanelServices;

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
        var roles = new List<Role>();
        foreach (var roleName in createUserDto.Roles)
        {
            var role = await context.Roles
                .SingleOrDefaultAsync(r => r.Name == roleName);
        
            if (role != null)
            {
                roles.Add(role);
            }
        }

        var userRoles = roles.Select(role => new UserRole
        {
            RoleId = role.Id,
            UserId = user.Id
        }).ToList();

        await context.UserRoles.AddRangeAsync(userRoles);
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
        await context.AddAsync(user);
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