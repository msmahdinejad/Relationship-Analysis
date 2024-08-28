using Microsoft.AspNetCore.Http;
using NSubstitute;
using RelationshipAnalysis.Services.GraphServices;

namespace RelationshipAnalysis.Test.Services.GraphServices;

public class CsvProcessorServiceTests
{
    private readonly CsvProcessorService _sut;

    public CsvProcessorServiceTests()
    {
        _sut = new CsvProcessorService();
    }

    [Fact]
    public async Task ProcessCsvAsync_ShouldReturnValidList_WhenFileIsValidated()
    {
        // Arrange
        var csvContent = @"""AccountID"",""CardID"",""IBAN""
""6534454617"",""6104335000000190"",""IR120778801496000000198""
""4000000028"",""6037699000000020"",""IR033880987114000000028""
";
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

        var expected = new List<dynamic>
        {
            new Dictionary<string, object>
            {
                { "AccountID", "6534454617" },
                { "CardID", "6104335000000190" },
                { "IBAN", "IR120778801496000000198" }
            },
            new Dictionary<string, object>
            {
                { "AccountID", "4000000028" },
                { "CardID", "6037699000000020" },
                { "IBAN", "IR033880987114000000028" }
            }
        };

        // Act
        var result = await _sut.ProcessCsvAsync(fileMock);

        // Assert
        Assert.Equivalent(expected, result);
    }
}