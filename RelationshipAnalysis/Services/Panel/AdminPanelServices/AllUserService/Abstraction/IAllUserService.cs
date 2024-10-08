﻿using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Models.Auth;
using GetAllUsersDto = RelationshipAnalysis.Dto.Panel.Admin.GetAllUsersDto;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices.AllUserService.Abstraction;

public interface IAllUserService
{
    Task<ActionResponse<GetAllUsersDto>> GetAllUser(List<User> users);
}