using AutoMapper;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdateInfoService;

namespace RelationshipAnalysis.Test.Services.Panel.UserPanelServices.UserUpdateInfoService;

public class UserUpdateInfoMapperTests
{
    private readonly IMapper _mapper;

    public UserUpdateInfoMapperTests()
    {
        var config = new MapperConfiguration(cfg => { cfg.AddProfile<UserUpdateInfoMapper>(); });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Mapping_UserUpdateInfoDto_To_User_Should_MapCorrectly()
    {
        // Arrange
        var dto = new UserUpdateInfoDto
        {
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User"
        };

        // Act
        var user = _mapper.Map<User>(dto);

        // Assert
        Assert.Null(user.PasswordHash);
        Assert.Equal(dto.Username, user.Username);
        Assert.Equal(dto.Email, user.Email);
        Assert.Equal(dto.FirstName, user.FirstName);
        Assert.Equal(dto.LastName, user.LastName);
    }

    [Fact]
    public void Mapping_User_To_UserOutputInfoDto_Should_MapCorrectly()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            PasswordHash = "hashedpassword"
        };

        // Act
        var dto = _mapper.Map<UserOutputInfoDto>(user);

        // Assert
        Assert.Equal(user.Username, dto.Username);
        Assert.Equal(user.Email, dto.Email);
        Assert.Equal(user.FirstName, dto.FirstName);
        Assert.Equal(user.LastName, dto.LastName);
        Assert.Null(dto.Roles);
    }
}