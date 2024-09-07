using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices.Edge;

public class EdgeAttributesReceiver(IServiceProvider serviceProvider) : IAttributesReceiver
{
    public async Task<List<string>> GetAllAttributes(string name)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.EdgeAttributes
            .Where(ea => ea.EdgeValues.Any(v => v.Edge.EdgeCategory.EdgeCategoryName == name))
            .Select(ea => ea.EdgeAttributeName).ToListAsync();
    }
}