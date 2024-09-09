using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices
{
    public class ClauseValidatorService : IClauseValidatorService
    {
        public async Task<ActionResponse<GraphDto>> AreClausesValid(
            SearchGraphDto searchGraphDto,
            List<string> sourceAttributes,
            List<string> targetAttributes,
            List<string> edgeAttributes)
        {
            if (!searchGraphDto.SourceCategoryClauses.Keys.All(item => sourceAttributes.Contains(item)))
                return NotFoundResult(Resources.InvalidClauseInSourceCategory);

            if (!searchGraphDto.TargetCategoryClauses.Keys.All(item => targetAttributes.Contains(item)))
                return NotFoundResult(Resources.InvalidClauseInDestinationCategory);

            if (!searchGraphDto.EdgeCategoryClauses.Keys.All(item => edgeAttributes.Contains(item)))
                return NotFoundResult(Resources.InvalidClauseInDestinationCategory);

            return SuccessResult();
        }

        private ActionResponse<GraphDto> SuccessResult()
        {
            return new ActionResponse<GraphDto>
            {
                StatusCode = StatusCodeType.Success,
                Data = null
            };
        }

        private ActionResponse<GraphDto> NotFoundResult(string message)
        {
            return new ActionResponse<GraphDto>
            {
                StatusCode = StatusCodeType.NotFound,
                Data = new GraphDto
                {
                    Message = message
                }
            };
        }
    }
}