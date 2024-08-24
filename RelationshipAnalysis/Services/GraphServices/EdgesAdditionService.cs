using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices;

public class EdgesAdditionService(
    IServiceProvider serviceProvider,
    ICsvValidatorService csvValidatorService,
    ICsvProcessorService csvProcessorService,
    ISingleEdgeAdditionService singleEdgeAdditionService) : IEdgesAdditionService
{
    public async Task<ActionResponse<MessageDto>> AddEdges(UploadEdgeDto uploadEdgeDto)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var edgeCategory = await context.EdgeCategories.SingleOrDefaultAsync(ec =>
            ec.EdgeCategoryName == uploadEdgeDto.EdgeCategoryName);
        
        
        var sourceNodeCategory = await context.NodeCategories.SingleOrDefaultAsync(nc =>
            nc.NodeCategoryName == uploadEdgeDto.SourceNodeCategoryName);
        var targetNodeCategory = await context.NodeCategories.SingleOrDefaultAsync(nc =>
            nc.NodeCategoryName == uploadEdgeDto.TargetNodeCategoryName);
        
        
        var file = uploadEdgeDto.File;
        var uniqueHeader = uploadEdgeDto.UniqueKeyHeaderName;
        var uniqueSourceHeader = uploadEdgeDto.SourceNodeHeaderName;
        var uniqueTargetHeader = uploadEdgeDto.TargetNodeHeaderName;
        

        if (edgeCategory == null)
            return BadRequestResult(Resources.InvalidEdgeCategory);
        
        if (sourceNodeCategory == null)
            return BadRequestResult(Resources.InvalidSourceNodeCategory);
        if (targetNodeCategory == null)
            return BadRequestResult(Resources.InvalidTargetNodeCategory);

        var validationResult = csvValidatorService.Validate(file, uniqueHeader, uniqueSourceHeader, uniqueTargetHeader);
        if (validationResult.StatusCode == StatusCodeType.BadRequest)
            return validationResult;

        var objects = await csvProcessorService.ProcessCsvAsync(file);

        await using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            { 
                foreach (var obj in objects)
                {
                    await singleEdgeAdditionService.AddSingleEdge(context, (IDictionary<string, object>)obj,
                        uniqueHeader, uniqueSourceHeader, uniqueTargetHeader,
                        edgeCategory.EdgeCategoryId, sourceNodeCategory.NodeCategoryId,
                        targetNodeCategory.NodeCategoryId);
                }
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return BadRequestResult(e.Message);
            }
        }

        return SuccessResult();
    }


    private ActionResponse<MessageDto> BadRequestResult(string message)
    {
        return new ActionResponse<MessageDto>
        {
            Data = new MessageDto(message),
            StatusCode = StatusCodeType.BadRequest
        };
    }

    private ActionResponse<MessageDto> SuccessResult()
    {
        return new ActionResponse<MessageDto>
        {
            Data = new MessageDto(Resources.SuccessfulEdgeAdditionMessage),
            StatusCode = StatusCodeType.Success
        };
    }
}