using System.Collections.Generic;
using System.Threading.Tasks;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Models.Graph.Node;

namespace RelationshipAnalysis.Services.GraphServices.Node.Abstraction;

public interface IContextNodesAdditionService
{
    Task<ActionResponse<MessageDto>> AddToContext(string uniqueKeyHeaderName, ApplicationDbContext context,
        List<dynamic> objects, NodeCategory nodeCategory);
}