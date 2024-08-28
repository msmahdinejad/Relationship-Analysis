using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph.Edge;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Graph.Edge;
using RelationshipAnalysis.Models.Graph.Node;
using RelationshipAnalysis.Services.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Edge.Abstraction;
using IEdgesAdditionService = RelationshipAnalysis.Services.GraphServices.Edge.Abstraction.IEdgesAdditionService;
using ISingleEdgeAdditionService = RelationshipAnalysis.Services.GraphServices.Edge.Abstraction.ISingleEdgeAdditionService;

namespace RelationshipAnalysis.Services.GraphServices.Edge;

public class EdgesAdditionService(
    IServiceProvider serviceProvider,
    ICsvValidatorService csvValidatorService,
    ICsvProcessorService csvProcessorService,
    IContextEdgesAdditionService contextEdgesAdditionService,
    IMessageResponseCreator responseCreator) : IEdgesAdditionService
{
    public async Task<ActionResponse<MessageDto>> AddEdges(UploadEdgeDto uploadEdgeDto)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var targetCategory = await GetTargetCategory(uploadEdgeDto, context);
        var sourceCategory = await GetSourceCategory(uploadEdgeDto, context);
        var edgeCategory = await GetEdgeCategory(uploadEdgeDto, context);
        
        var nullCheckResponse = CheckForNullValues(edgeCategory, sourceCategory, targetCategory);
        if (nullCheckResponse.StatusCode == StatusCodeType.BadRequest)
        {
            return nullCheckResponse;
        }

        var validationResult = csvValidatorService.Validate(uploadEdgeDto.File, uploadEdgeDto.UniqueKeyHeaderName, uploadEdgeDto.SourceNodeHeaderName, uploadEdgeDto.TargetNodeHeaderName);
        if (validationResult.StatusCode == StatusCodeType.BadRequest)
        {
            return validationResult;
        }

        var objects = await csvProcessorService.ProcessCsvAsync(uploadEdgeDto.File);

        return await contextEdgesAdditionService.AddToContext(context, edgeCategory, sourceCategory, targetCategory, objects,
            uploadEdgeDto);
    }

    private ActionResponse<MessageDto> CheckForNullValues(EdgeCategory? edgeCategory, NodeCategory? sourceCategory, NodeCategory? targetCategory)
    {
        if (edgeCategory == null)
        {
            return responseCreator.Create(StatusCodeType.BadRequest, Resources.InvalidEdgeCategory);
        }

        if (sourceCategory == null)
        {
            return responseCreator.Create(StatusCodeType.BadRequest, Resources.InvalidSourceNodeCategory);
        }

        if (targetCategory == null)
        {
            return responseCreator.Create(StatusCodeType.BadRequest, Resources.InvalidTargetNodeCategory);
        }

        return responseCreator.Create(StatusCodeType.Success, string.Empty);
    }

    private async Task<NodeCategory?> GetTargetCategory(UploadEdgeDto uploadEdgeDto, ApplicationDbContext context)
    {
        var targetNodeCategory = await context.NodeCategories.SingleOrDefaultAsync(nc =>
            nc.NodeCategoryName == uploadEdgeDto.TargetNodeCategoryName);
        return targetNodeCategory;
    }

    private async Task<NodeCategory?> GetSourceCategory(UploadEdgeDto uploadEdgeDto, ApplicationDbContext context)
    {
        var sourceNodeCategory = await context.NodeCategories.SingleOrDefaultAsync(nc =>
            nc.NodeCategoryName == uploadEdgeDto.SourceNodeCategoryName);
        return sourceNodeCategory;
    }

    private async Task<EdgeCategory?> GetEdgeCategory(UploadEdgeDto uploadEdgeDto, ApplicationDbContext context)
    {
        var edgeCategory = await context.EdgeCategories.SingleOrDefaultAsync(ec =>
            ec.EdgeCategoryName == uploadEdgeDto.EdgeCategoryName);
        return edgeCategory;
    }
}