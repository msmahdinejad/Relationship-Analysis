using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph.Edge;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Graph.Edge;
using RelationshipAnalysis.Models.Graph.Node;
using RelationshipAnalysis.Services.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Edge.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices.Edge;

public class ContextEdgesAdditionService(
    IMessageResponseCreator responseCreator,
    ISingleEdgeAdditionService singleEdgeAdditionService) : IContextEdgesAdditionService
{
    public async Task<ActionResponse<MessageDto>> AddToContext(ApplicationDbContext context, EdgeCategory edgeCategory,
        NodeCategory sourceCategory,
        NodeCategory targetCategory, List<dynamic> objects, UploadEdgeDto uploadEdgeDto)
    {
        await using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                foreach (var obj in objects)
                {
                    var dictObject = (IDictionary<string, object>)obj;
                    await singleEdgeAdditionService.AddSingleEdge(context, dictObject,
                        uploadEdgeDto.UniqueKeyHeaderName,
                        uploadEdgeDto.SourceNodeHeaderName, uploadEdgeDto.TargetNodeHeaderName,
                        edgeCategory.EdgeCategoryId,
                        sourceCategory.NodeCategoryId, targetCategory.NodeCategoryId);
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return responseCreator.Create(StatusCodeType.BadRequest, e.Message + e.InnerException);
            }
        }

        return responseCreator.Create(StatusCodeType.Success, Resources.SuccessfulEdgeAdditionMessage);
    }
}