using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph;

namespace RelationshipAnalysis.Services.GraphServices.Abstraction;

public interface ISingleNodeAdditionService
{
    Task AddSingleNode(ApplicationDbContext context, IDictionary<string, object> record, string uniqueHeaderName, int nodeCategoryId);
}