using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices.Edge;

public class EdgeCategoryNameValidator(IServiceProvider serviceProvider) : ICategoryNameValidator
{
    public async Task<bool> Validate(string categoryName)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var categoryNames = await context.EdgeCategories.Select(ec => ec.EdgeCategoryName).ToListAsync();
        if (categoryNames.Contains(categoryName)) return true;
        return false;
    }
}