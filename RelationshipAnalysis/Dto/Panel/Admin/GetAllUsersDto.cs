using System.Collections.Generic;
using RelationshipAnalysis.Dto.Panel.User;

namespace RelationshipAnalysis.Dto.Panel.Admin;

public class GetAllUsersDto
{
    public List<User.UserOutputInfoDto> Users { get; init; }
    public int AllUserCount { get; init; }
}