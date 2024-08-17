using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Controllers;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.DTO;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models;
using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Services;

public class UserUpdateInfoService(ApplicationDbContext context, IMapper mapper) : IUserUpdateInfoService
{
    public async Task<ActionResponse<MessageDto>> UpdateUserAsync(User user, UserUpdateInfoDto userUpdateInfoDto, HttpResponse response)
    {
        if (user is null)
        {
            return NotFoundResult();
        }

        if (!IsUsernameUnique(user.Username, userUpdateInfoDto.Username))
        {
            return BadRequestResult(Resources.UsernameExistsMessage);
        }

        if (!IsEmailUnique(user.Email, userUpdateInfoDto.Email))
        {
            return BadRequestResult(Resources.EmailExistsMessage);
        }
        mapper.Map(userUpdateInfoDto, user);
        context.Update(user);
        await context.SaveChangesAsync();
        return SuccessResult();
    }

    private bool IsUsernameUnique(string currentValue, string newValue)
    {
        if (currentValue == newValue) return true;
        return !context.Users.Select(u => u.Username).Contains(newValue);
    }
    private bool IsEmailUnique(string currentValue, string newValue)
    {
        if (currentValue == newValue) return true;
        return !context.Users.Select(u => u.Email).Contains(newValue);
    }
    private ActionResponse<MessageDto> BadRequestResult(string message)
    {
        return new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(message),
            StatusCode = StatusCodeType.BadRequest
        };
    }
    private ActionResponse<MessageDto> NotFoundResult()
    {
        return new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.UserNotFoundMessage),
            StatusCode = StatusCodeType.NotFound
        };
    }

    private ActionResponse<MessageDto> SuccessResult()
    {
        return new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.SuccessfulUpdateUserMessage),
            StatusCode = StatusCodeType.Success
        };
    }
}