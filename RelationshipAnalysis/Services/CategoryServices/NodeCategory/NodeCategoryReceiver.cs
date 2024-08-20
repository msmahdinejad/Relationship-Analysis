using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.CategoryServices.NodeCategory.Abstraction;

namespace RelationshipAnalysis.Services.CategoryServices.NodeCategory;

public class NodeCategoryReceiver(IServiceProvider serviceProvider) : INodeCategoryReceiver
{
    public async Task<List<string>> GetAllNodeCategories()
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await context.NodeCategories.Select(e => e.NodeCategoryName).ToListAsync();
    }
}