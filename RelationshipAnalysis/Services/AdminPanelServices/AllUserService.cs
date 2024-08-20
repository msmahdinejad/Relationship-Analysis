using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.AdminPanelServices.Abstraction;

namespace RelationshipAnalysis.Services.AdminPanelServices;

public class AllUserService(IServiceProvider serviceProvider, IMapper mapper, IRoleReceiver rolesReceiver) : IAllUserService
{
    public async Task<int> ReceiveAllUserCount()
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var users = await context.Users.ToListAsync();
        return users.Count;
    }
    public async Task<ActionResponse<GetAllUsersDto>> GetAllUser(List<User> users)
    {
        if (users.IsNullOrEmpty())
        {
            return NotFoundResult();
        }

        var usersList = await GetAllUsersList(users);
        var result = await GetAllUsersOutPut(usersList);

        return SuccessResult(result);
    }

    private async Task<GetAllUsersDto> GetAllUsersOutPut(List<UserOutputInfoDto> usersList)
    {
        return new GetAllUsersDto()
        {
            Users = usersList,
            AllUserCount = await ReceiveAllUserCount()
        };
    }

    private async Task<List<UserOutputInfoDto>> GetAllUsersList(List<User> users)
    {
        var userOutputs = new List<UserOutputInfoDto>();
        foreach (var user in users)
        {
            var data = new UserOutputInfoDto();
            mapper.Map(user, data);
            data.Roles = await rolesReceiver.ReceiveRoles(user.Id);
            
            userOutputs.Add(data);
        }

        return userOutputs;
    }

    private ActionResponse<GetAllUsersDto> SuccessResult(GetAllUsersDto outPut)
    {
        return new ActionResponse<GetAllUsersDto>()
        {
            Data = outPut,
            StatusCode = StatusCodeType.Success
        };
    }
    
    private ActionResponse<GetAllUsersDto> NotFoundResult()
    {
        return new ActionResponse<GetAllUsersDto>()
        {
            StatusCode = StatusCodeType.NotFound
        };
    }
}