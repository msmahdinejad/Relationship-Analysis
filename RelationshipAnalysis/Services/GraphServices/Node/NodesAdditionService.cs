using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph.Node;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Services.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Node.Abstraction;
using INodesAdditionService = RelationshipAnalysis.Services.GraphServices.Node.Abstraction.INodesAdditionService;
using ISingleNodeAdditionService = RelationshipAnalysis.Services.GraphServices.Node.Abstraction.ISingleNodeAdditionService;

namespace RelationshipAnalysis.Services.GraphServices.Node;

public class NodesAdditionService(
    IServiceProvider serviceProvider,
    ICsvValidatorService csvValidatorService,
    ICsvProcessorService csvProcessorService,
    IMessageResponseCreator responseCreator,
    IContextNodesAdditionService contextAdditionService) : INodesAdditionService
{
    public async Task<ActionResponse<MessageDto>> AddNodes(UploadNodeDto uploadNodeDto)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var nodeCategory = await context.NodeCategories.SingleOrDefaultAsync(nc =>
            nc.NodeCategoryName == uploadNodeDto.NodeCategoryName);

        if (nodeCategory == null)
        {
            return responseCreator.Create(StatusCodeType.BadRequest, Resources.InvalidNodeCategory);
        }

        var validationResult = csvValidatorService.Validate(uploadNodeDto.File, uploadNodeDto.UniqueKeyHeaderName);
        
        if (validationResult.StatusCode == StatusCodeType.BadRequest)
        {
            return validationResult;
        }

        var objects = await csvProcessorService.ProcessCsvAsync(uploadNodeDto.File);

        return await contextAdditionService.AddToContext(uploadNodeDto.UniqueKeyHeaderName, context, objects, nodeCategory);
    }

}