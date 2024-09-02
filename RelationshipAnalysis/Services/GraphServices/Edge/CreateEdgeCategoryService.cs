using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph.Edge;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Graph.Edge;
using RelationshipAnalysis.Services.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Edge.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices.Edge;

public class CreateEdgeCategoryService(IServiceProvider serviceProvider, IMessageResponseCreator responseCreator) : ICreateEdgeCategoryService
{
    public async Task<ActionResponse<MessageDto>> CreateEdgeCategory(CreateEdgeCategoryDto createEdgeCategoryDto)
    {
        if (createEdgeCategoryDto is null) return responseCreator.Create(StatusCodeType.BadRequest, Resources.NullDtoErrorMessage);
        if (IsNotUniqueCategoryName(createEdgeCategoryDto))
            return responseCreator.Create(StatusCodeType.BadRequest,Resources.NotUniqueCategoryNameErrorMessage);
        await AddCategory(createEdgeCategoryDto);
        return responseCreator.Create(StatusCodeType.Success, Resources.SuccessfulCreateCategory);
    }


    private async Task AddCategory(CreateEdgeCategoryDto createEdgeCategoryDto)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.EdgeCategories.AddAsync(new EdgeCategory
        {
            EdgeCategoryName = createEdgeCategoryDto.EdgeCategoryName
        });
        await context.SaveChangesAsync();
    }

    private bool IsNotUniqueCategoryName(CreateEdgeCategoryDto dto)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return context.EdgeCategories.Any(c => c.EdgeCategoryName == dto.EdgeCategoryName);
    }

}