using System.Collections.Generic;
using System.Threading.Tasks;
using RelationshipAnalysis.Context;

namespace RelationshipAnalysis.Services.GraphServices.Node.Abstraction;

public interface ISingleNodeAdditionService
{
    Task AddSingleNode(ApplicationDbContext context, IDictionary<string, object> record, string uniqueHeaderName,
        int nodeCategoryId);
}