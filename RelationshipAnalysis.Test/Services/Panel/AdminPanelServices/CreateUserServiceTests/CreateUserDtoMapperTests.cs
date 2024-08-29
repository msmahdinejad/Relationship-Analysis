using FluentAssertions;
using Moq;
using RelationshipAnalysis.Dto.Panel.Admin;
using RelationshipAnalysis.Services.AuthServices.Abstraction;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.CreateUserService;

namespace RelationshipAnalysis.Test.Services.Panel.AdminPanelServices.CreateUserServiceTests;

public class CreateUserDtoMapperTests
{
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly CreateUserDtoMapper _mapper;

    public CreateUserDtoMapperTests()
    {
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _mapper = new CreateUserDtoMapper(_passwordHasherMock.Object);
    }

    [Fact]
    public void Map_ShouldMapCreateUserDtoToUser()
    {
        // Arrange
        var createUserDto = new CreateUserDto
        {
            Username = "testuser",
            Password = "P@ssw0rd",
            Email = "testuser@example.com",
            FirstName = "John",
            LastName = "Doe",
            Roles = new List<string> { "Admin", "User" }
        };

        var hashedPassword = "hashedpassword";
        _passwordHasherMock.Setup(p => p.HashPassword(createUserDto.Password)).Returns(hashedPassword);

        // Act
        var user = _mapper.Map(createUserDto);

        // Assert
        user.Should().NotBeNull();
        user.Username.Should().Be(createUserDto.Username);
        user.PasswordHash.Should().Be(hashedPassword);
        user.Email.Should().Be(createUserDto.Email);
        user.FirstName.Should().Be(createUserDto.FirstName);
        user.LastName.Should().Be(createUserDto.LastName);
    }
}