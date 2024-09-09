
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Services.GraphServices;

namespace RelationshipAnalysis.Test.Services.GraphServices;

public class ClauseValidatorServiceTests
{
    private readonly ClauseValidatorService _sut;

    public ClauseValidatorServiceTests()
    {
        _sut = new ClauseValidatorService();
    }

    [Fact]
    public async Task AreClausesValid_ShouldReturnSuccess_WhenAllClausesAreValid()
    {
        // Arrange
        var searchGraphDto = new SearchGraphDto
        {
            SourceCategoryClauses = new Dictionary<string, string> { { "validSource", "value" } },
            TargetCategoryClauses = new Dictionary<string, string> { { "validTarget", "value" } },
            EdgeCategoryClauses = new Dictionary<string, string> { { "validEdge", "value" } }
        };

        var sourceAttributes = new List<string> { "validSource" };
        var targetAttributes = new List<string> { "validTarget" };
        var edgeAttributes = new List<string> { "validEdge" };

        // Act
        var result = await _sut.AreClausesValid(searchGraphDto, sourceAttributes, targetAttributes, edgeAttributes);

        // Assert
        Assert.Equal(StatusCodeType.Success, result.StatusCode);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task AreClausesValid_ShouldReturnNotFound_WhenSourceCategoryClausesAreInvalid()
    {
        // Arrange
        var searchGraphDto = new SearchGraphDto
        {
            SourceCategoryClauses = new Dictionary<string, string> { { "invalidSource", "value" } },
            TargetCategoryClauses = new Dictionary<string, string> { { "validTarget", "value" } },
            EdgeCategoryClauses = new Dictionary<string, string> { { "validEdge", "value" } }
        };

        var sourceAttributes = new List<string> { "validSource" };
        var targetAttributes = new List<string> { "validTarget" };
        var edgeAttributes = new List<string> { "validEdge" };

        // Act
        var result = await _sut.AreClausesValid(searchGraphDto, sourceAttributes, targetAttributes, edgeAttributes);

        // Assert
        Assert.Equal(StatusCodeType.NotFound, result.StatusCode);
        Assert.Equal(Resources.InvalidClauseInSourceCategory, result.Data.Message);
    }

    [Fact]
    public async Task AreClausesValid_ShouldReturnNotFound_WhenTargetCategoryClausesAreInvalid()
    {
        // Arrange
        var searchGraphDto = new SearchGraphDto
        {
            SourceCategoryClauses = new Dictionary<string, string> { { "validSource", "value" } },
            TargetCategoryClauses = new Dictionary<string, string> { { "invalidTarget", "value" } },
            EdgeCategoryClauses = new Dictionary<string, string> { { "validEdge", "value" } }
        };

        var sourceAttributes = new List<string> { "validSource" };
        var targetAttributes = new List<string> { "validTarget" };
        var edgeAttributes = new List<string> { "validEdge" };

        // Act
        var result = await _sut.AreClausesValid(searchGraphDto, sourceAttributes, targetAttributes, edgeAttributes);

        // Assert
        Assert.Equal(StatusCodeType.NotFound, result.StatusCode);
        Assert.Equal(Resources.InvalidClauseInDestinationCategory, result.Data.Message);
    }

    [Fact]
    public async Task AreClausesValid_ShouldReturnNotFound_WhenEdgeCategoryClausesAreInvalid()
    {
        // Arrange
        var searchGraphDto = new SearchGraphDto
        {
            SourceCategoryClauses = new Dictionary<string, string> { { "validSource", "value" } },
            TargetCategoryClauses = new Dictionary<string, string> { { "validTarget", "value" } },
            EdgeCategoryClauses = new Dictionary<string, string> { { "invalidEdge", "value" } }
        };

        var sourceAttributes = new List<string> { "validSource" };
        var targetAttributes = new List<string> { "validTarget" };
        var edgeAttributes = new List<string> { "validEdge" };

        // Act
        var result = await _sut.AreClausesValid(searchGraphDto, sourceAttributes, targetAttributes, edgeAttributes);

        // Assert
        Assert.Equal(StatusCodeType.NotFound, result.StatusCode);
        Assert.Equal(Resources.InvalidClauseInDestinationCategory, result.Data.Message);
    }
}
