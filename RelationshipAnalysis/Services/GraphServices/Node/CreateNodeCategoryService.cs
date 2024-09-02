using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph.Node;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Graph.Node;
using RelationshipAnalysis.Services.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Node.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices.Node;

public class CreateNodeCategoryService(IServiceProvider serviceProvider, IMessageResponseCreator responseCreator) : ICreateNodeCategoryService
{
    public async Task<ActionResponse<MessageDto>> CreateNodeCategory(CreateNodeCategoryDto createNodeCategoryDto)
    {
        if (createNodeCategoryDto is null)
        {
            return responseCreator.Create(StatusCodeType.BadRequest, Resources.NullDtoErrorMessage);
        }
        if (IsNotUniqueCategoryName(createNodeCategoryDto))
        {
            return responseCreator.Create(StatusCodeType.BadRequest, Resources.NotUniqueCategoryNameErrorMessage);
        }
        
        await AddCategory(createNodeCategoryDto);
        return responseCreator.Create(StatusCodeType.Success, Resources.SuccessfulCreateCategory);
    }
    private async Task AddCategory(CreateNodeCategoryDto createNodeCategoryDto)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.NodeCategories.AddAsync(new NodeCategory
        {
            NodeCategoryName = createNodeCategoryDto.NodeCategoryName
        });
        await context.SaveChangesAsync();
    }

    private bool IsNotUniqueCategoryName(CreateNodeCategoryDto dto)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return context.NodeCategories.Any(c => c.NodeCategoryName == dto.NodeCategoryName);
    }
}