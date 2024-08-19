namespace RelationshipAnalysis.Dto;

public class GetAllUsersDto
{
    public List<UserOutputInfoDto> Users { get; init; }
    public int AllUserCount { get; init; }
}