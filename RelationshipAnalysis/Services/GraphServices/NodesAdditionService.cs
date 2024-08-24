using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices;

public class NodesAdditionService(
    IServiceProvider serviceProvider,
    ICsvValidatorService csvValidatorService,
    ICsvProcessorService csvProcessorService,
    ISingleNodeAdditionService singleNodeAdditionService) : INodesAdditionService
{
    public async Task<ActionResponse<MessageDto>> AddNodes(UploadNodeDto uploadNodeDto)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var nodeCategory = await context.NodeCategories.SingleOrDefaultAsync(nc =>
            nc.NodeCategoryName == uploadNodeDto.NodeCategoryName);
        var file = uploadNodeDto.File;
        var uniqueHeader = uploadNodeDto.UniqueKeyHeaderName;

        if (nodeCategory == null)
            return BadRequestResult(Resources.InvalidNodeCategory);

        var validationResult = csvValidatorService.Validate(file, uniqueHeader);
        if (validationResult.StatusCode == StatusCodeType.BadRequest)
            return validationResult;

        var objects = await csvProcessorService.ProcessCsvAsync(file);

        using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                objects.ForEach(ob =>
                    singleNodeAdditionService.AddSingleNode((IDictionary<string, object>)ob, uniqueHeader,
                        nodeCategory.NodeCategoryId));
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
            Data = new MessageDto(Resources.SuccessfulNodeAdditionMessage),
            StatusCode = StatusCodeType.Success
        };
    }
}