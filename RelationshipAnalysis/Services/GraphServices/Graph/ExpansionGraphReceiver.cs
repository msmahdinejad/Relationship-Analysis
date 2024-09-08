using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Services.GraphServices.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Graph.Abstraction;


namespace RelationshipAnalysis.Services.GraphServices.Graph
{
    public class ExpansionGraphReceiver(IServiceProvider serviceProvider, IGraphDtoCreator graphDtoCreator,
        IExpansionCategoriesValidator expansionCategoriesValidator) : IExpansionGraphReceiver
    {

        public async Task<GraphDto> GetExpansionGraph(int nodeId, string sourceCategoryName, string targetCategoryName, string edgeCategoryName)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();


            var (isValid, graphDto) = await expansionCategoriesValidator.ValidateCategories(sourceCategoryName, targetCategoryName, edgeCategoryName);
            if (!isValid)
            {
                return graphDto;
            }

            var allEdges = await context.Edges.ToListAsync(); 
            var validEdges = GetValidEdges(nodeId, allEdges, edgeCategoryName, sourceCategoryName, targetCategoryName);
            var validNodes = GetValidNodes(validEdges);

            return graphDtoCreator.CreateResultGraphDto(validNodes, validEdges);
        }

        private List<Models.Graph.Node.Node> GetValidNodes(List<Models.Graph.Edge.Edge> validEdges)
        {
            var validNodes = new HashSet<Models.Graph.Node.Node>();
            validEdges.ForEach(e => validNodes.Add(e.NodeSource));
            validEdges.ForEach(e => validNodes.Add(e.NodeDestination));
            return validNodes.ToList();
        }

        private List<Models.Graph.Edge.Edge> GetValidEdges(int nodeId, List<Models.Graph.Edge.Edge> edges,
            string edgeCategoryName, string sourceCategory, string targetCategory)
        {
            var categoryEdges = edges.FindAll(e => e.EdgeCategory.EdgeCategoryName == edgeCategoryName);
            var inEdges = categoryEdges.FindAll(e =>
                e.EdgeDestinationNodeId == nodeId && e.NodeSource.NodeCategory.NodeCategoryName == sourceCategory);
            var outEdges = categoryEdges.FindAll(e =>
                e.EdgeSourceNodeId == nodeId &&
                e.NodeDestination.NodeCategory.NodeCategoryName == targetCategory);
            var resultEdges = inEdges.Union(outEdges).ToList();
            return resultEdges;
        }
    }
}
