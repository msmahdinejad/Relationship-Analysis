using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph;

namespace RelationshipAnalysis.Services.GraphServices.Abstraction;

public interface IClauseValidatorService
{
    Task<ActionResponse<GraphDto>> AreClausesValid(
        SearchGraphDto searchGraphDto,
        List<string> sourceAttributes,
        List<string> targetAttributes,
        List<string> edgeAttributes);
}