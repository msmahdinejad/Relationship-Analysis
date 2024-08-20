using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Category;
using RelationshipAnalysis.Services.CategoryServices.EdgeCategory.Abstraction;

namespace RelationshipAnalysis.Services.CategoryServices.EdgeCategory;

public class EdgeCategoryReceiver(IServiceProvider serviceProvider) : IEdgeCategoryReceiver
{
    public async Task<List<string>> GetAllEdgeCategories()
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await context.EdgeCategories.Select(e => e.EdgeCategoryName).ToListAsync();
    }
}