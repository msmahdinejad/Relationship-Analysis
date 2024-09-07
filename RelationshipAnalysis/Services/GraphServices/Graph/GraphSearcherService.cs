using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Services.GraphServices.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Graph.Abstraction;
using ApplicationDbContext = RelationshipAnalysis.Context.ApplicationDbContext;

namespace RelationshipAnalysis.Services.GraphServices.Graph;

public class GraphSearcherService(
    IGraphDtoCreator graphDtoCreator,
    IServiceProvider serviceProvider,
    [FromKeyedServices("node")] IAttributesReceiver nodeCategoryReceiver,
    [FromKeyedServices("edge")] IAttributesReceiver edgeCategoryReceiver) : IGraphSearcherService
{
    public async Task<ActionResponse<GraphDto>> Search(SearchGraphDto searchGraphDto)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();


        var sourceAttributes = await nodeCategoryReceiver.GetAllAttributes(searchGraphDto.SourceCategoryName);
        var targetAttributes = await nodeCategoryReceiver.GetAllAttributes(searchGraphDto.TargetCategoryName);
        var edgeAttributes = await edgeCategoryReceiver.GetAllAttributes(searchGraphDto.EdgeCategoryName);

        var validation = await AreClausesValid(searchGraphDto, sourceAttributes, targetAttributes, edgeAttributes);
        if (validation.StatusCode != StatusCodeType.Success) return validation;

        var sourceNodes = await context.Nodes
            .Where(n => searchGraphDto.SourceCategoryName == n.NodeCategory.NodeCategoryName).ToListAsync();
        var targetNodes = await context.Nodes
            .Where(n => searchGraphDto.TargetCategoryName == n.NodeCategory.NodeCategoryName).ToListAsync();

        sourceNodes = sourceNodes.Where(sn => IsNodeValid(sn, searchGraphDto.SourceCategoryClauses)).ToList();
        targetNodes = targetNodes.Where(tn => IsNodeValid(tn, searchGraphDto.TargetCategoryClauses)).ToList();

        var edges = await GetValidEdges(sourceNodes, targetNodes, searchGraphDto.SourceCategoryName,
            searchGraphDto.TargetCategoryName, searchGraphDto.EdgeCategoryName);

        edges = edges.Where(e => IsEdgeValid(e, searchGraphDto.EdgeCategoryClauses)).ToList();

        validation.Data = graphDtoCreator.CreateResultGraphDto(sourceNodes.Union(targetNodes).ToList(), edges);
        return validation;
    }


    private bool IsNodeValid(Models.Graph.Node.Node node, Dictionary<string, string> clauses)
    {
        var attributeValues = new Dictionary<string, string>();
        node.Values.ToList().ForEach(nv => attributeValues.Add(nv.NodeAttribute.NodeAttributeName, nv.ValueData));

        foreach (var kvp in clauses)
        {
            var actualValue = attributeValues[kvp.Key];
            if (!actualValue.StartsWith(kvp.Value)) return false;
        }

        return true;
    }

    private bool IsEdgeValid(Models.Graph.Edge.Edge edge, Dictionary<string, string> clauses)
    {
        var attributeValues = new Dictionary<string, string>();
        edge.EdgeValues.ToList().ForEach(ev => attributeValues.Add(ev.EdgeAttribute.EdgeAttributeName, ev.ValueData));

        foreach (var kvp in clauses)
        {
            var actualValue = attributeValues[kvp.Key];
            if (!actualValue.StartsWith(kvp.Value)) return false;
        }

        return true;
    }

    private async Task<List<Models.Graph.Edge.Edge>> GetValidEdges(List<Models.Graph.Node.Node> sourceNodes,
        List<Models.Graph.Node.Node> targetNodes, string sourceCategory, string targetCategory,
        string edgeCategory)
    {
        var sourceNodeIds = sourceNodes.Select(n => n.NodeId).ToList();
        var targetNodeIds = targetNodes.Select(n => n.NodeId).ToList();

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var edges = await context.Edges.Include(e => e.EdgeValues)
            .ThenInclude(ev => ev.EdgeAttribute)
            .Where(e => edgeCategory == e.EdgeCategory.EdgeCategoryName &&
                        sourceNodeIds.Contains(e.EdgeSourceNodeId) &&
                        targetNodeIds.Contains(e.EdgeDestinationNodeId))
            .ToListAsync();
        return edges;
    }

    private async Task<ActionResponse<GraphDto>> AreClausesValid(SearchGraphDto searchGraphDto,
        List<string> sourceAttributes, List<string> targetAttributes,
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