using FluentAssertions;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Services;

namespace RelationshipAnalysis.Test.Services;

public class MessageResponseCreatorTests
{
    private readonly MessageResponseCreator _sut;

    public MessageResponseCreatorTests()
    {
        _sut = new MessageResponseCreator();
    }

    [Fact]
    public void Create_ShouldReturnActionResponse_WithCorrectStatusCodeAndMessage()
    {
        // Arrange
        var statusCodeType = StatusCodeType.Success;
        var message = "Operation completed successfully";

        // Act
        var result = _sut.Create(statusCodeType, message);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(statusCodeType);
        result.Data.Should().NotBeNull();
        result.Data.Message.Should().Be(message);
    }

    [Fact]
    public void Create_ShouldReturnActionResponse_WithMessageDto()
    {
        // Arrange
        var statusCodeType = StatusCodeType.BadRequest;
        var message = "An error occurred";

        // Act
        var result = _sut.Create(statusCodeType, message);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeOfType<MessageDto>();
    }
}