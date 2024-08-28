using Microsoft.AspNetCore.Http;
using NSubstitute;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Services;
using RelationshipAnalysis.Services.GraphServices;

namespace RelationshipAnalysis.Test.Services.GraphServices;

public class CsvValidatorServiceTests
{
    private readonly CsvValidatorService _sut;

    public CsvValidatorServiceTests()
    {
        _sut = new CsvValidatorService(new MessageResponseCreator());
    }

    [Fact]
    public void Validate_ShouldReturnSuccess_WhenFileIsValid()
    {
        // Arrange
        var csvContent = @"""AccountID"",""CardID"",""IBAN""
""6534454617"",""6104335000000190"",""IR120778801496000000198""
""4000000028"",""6037699000000020"",""IR033880987114000000028""
";
        var fileMock = CreateFileMock(csvContent);

        var expected = new ActionResponse<MessageDto>
        {
            Data = new MessageDto(Resources.ValidFileMessage),
            StatusCode = StatusCodeType.Success
        };

        // Act
        var result = _sut.Validate(fileMock, "AccountID");

        // Assert
        Assert.Equivalent(expected, result);
    }

    [Fact]
    public void Validate_ShouldReturnFailed_WhenFileUniqueHeaderIsInvalid()
    {
        // Arrange
        var csvContent = @"""SomeOtheKey"",""CardID"",""IBAN""
""6534454617"",""6104335000000190"",""IR120778801496000000198""
""4000000028"",""6037699000000020"",""IR033880987114000000028""
";

        var fileMock = CreateFileMock(csvContent);
        var expected = new ActionResponse<MessageDto>
        {
            Data = new MessageDto(Resources.InvalidHeaderAttribute),
            StatusCode = StatusCodeType.BadRequest
        };

        // Act
        var result = _sut.Validate(fileMock, "AccountID");

        // Assert
        Assert.Equivalent(expected, result);
    }


    [Fact]
    public void Validate_ShouldReturnFailed_WhenAHeaderIsEmpty()
    {
        // Arrange
        var csvContent = @"""AccountID"","""",""IBAN""
""6534454617"",""6104335000000190"",""IR120778801496000000198""
""4000000028"",""6037699000000020"",""IR033880987114000000028""
";

        var fileMock = CreateFileMock(csvContent);
        var expected = new ActionResponse<MessageDto>
        {
            Data = new MessageDto(Resources.InvalidHeaderAttribute),
            StatusCode = StatusCodeType.BadRequest
        };

        // Act
        var result = _sut.Validate(fileMock, "AccountID");

        // Assert
        Assert.Equivalent(expected, result);
    }


    [Fact]
    public void Validate_ShouldReturnFailed_WhenTwoHeadersAreSame()
    {
        // Arrange
        var csvContent = @"""AccountID"",""IBAN"",""IBAN""
""6534454617"",""6104335000000190"",""IR120778801496000000198""
""4000000028"",""6037699000000020"",""IR033880987114000000028""
";

        var fileMock = CreateFileMock(csvContent);
        var expected = new ActionResponse<MessageDto>
        {
            Data = new MessageDto(Resources.TwoSameHeadersMessage),
            StatusCode = StatusCodeType.BadRequest
        };

        // Act
        var result = _sut.Validate(fileMock, "AccountID");

        // Assert
        Assert.Equivalent(expected, result);
    }


    private IFormFile CreateFileMock(string csvContent)
    {
        var csvFileName = "test.csv";
        var fileMock = Substitute.For<IFormFile>();
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(csvContent);
        writer.Flush();
        stream.Position = 0;

        fileMock.OpenReadStream().Returns(stream);
        fileMock.FileName.Returns(csvFileName);
        fileMock.Length.Returns(stream.Length);
        return fileMock;
    }
}