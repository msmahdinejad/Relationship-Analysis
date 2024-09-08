using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices.Node;

public class NodeCategoryNameValidator(IServiceProvider serviceProvider) : ICategoryNameValidator
{
    public async Task<bool> Validate(string categoryName)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var categoryNames = await context.NodeCategories.Select(nc => nc.NodeCategoryName).ToListAsync();
        if (categoryNames.Contains(categoryName)) return true;
        return false;
    }
}