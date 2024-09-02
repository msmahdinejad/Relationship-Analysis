using System.Collections.Generic;
using System.Threading.Tasks;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph.Edge;
using RelationshipAnalysis.Models.Graph.Edge;
using RelationshipAnalysis.Models.Graph.Node;

namespace RelationshipAnalysis.Services.GraphServices.Edge.Abstraction;

public interface IContextEdgesAdditionService
{
    Task<ActionResponse<MessageDto>> AddToContext(ApplicationDbContext context, EdgeCategory edgeCategory,
        NodeCategory sourceCategory,
        NodeCategory targetCategory, List<dynamic> objects, UploadEdgeDto uploadEdgeDto);
}