using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Services.GraphServices.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Edge.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Graph.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Node.Abstraction;
using ApplicationDbContext = RelationshipAnalysis.Context.ApplicationDbContext;

namespace RelationshipAnalysis.Services.GraphServices.Graph
{
    public class GraphSearcherService(
        IGraphDtoCreator graphDtoCreator,
        [FromKeyedServices("node")] IAttributesReceiver nodeCategoryReceiver,
        [FromKeyedServices("edge")] IAttributesReceiver edgeCategoryReceiver,
        ISearchNodeValidator nodeValidator,
        ISearchEdgeValidator edgeValidator,
        IClauseValidatorService clauseValidatorService) : IGraphSearcherService
    {

        public async Task<ActionResponse<GraphDto>> Search(SearchGraphDto searchGraphDto)
        {

            var sourceAttributes = await nodeCategoryReceiver.GetAllAttributes(searchGraphDto.SourceCategoryName);
            var targetAttributes = await nodeCategoryReceiver.GetAllAttributes(searchGraphDto.TargetCategoryName);
            var edgeAttributes = await edgeCategoryReceiver.GetAllAttributes(searchGraphDto.EdgeCategoryName);

            var validation = await clauseValidatorService.AreClausesValid(searchGraphDto, sourceAttributes, targetAttributes, edgeAttributes);
            if (validation.StatusCode != StatusCodeType.Success) return validation;

            var sourceNodes = await nodeValidator.GetValidNodes(searchGraphDto.SourceCategoryClauses, searchGraphDto.SourceCategoryName);
            var targetNodes = await nodeValidator.GetValidNodes(searchGraphDto.TargetCategoryClauses, searchGraphDto.TargetCategoryName);

            var edges = await edgeValidator.GetValidEdges(sourceNodes, targetNodes, searchGraphDto.EdgeCategoryName, searchGraphDto.EdgeCategoryClauses);
            
            validation.Data = graphDtoCreator.CreateResultGraphDto(sourceNodes.UnionBy(targetNodes, node => node.NodeId).ToList(), edges);
            return validation;
        }

    }
}
