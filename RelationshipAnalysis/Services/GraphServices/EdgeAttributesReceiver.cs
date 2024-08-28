using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices;

public class EdgeAttributesReceiver(IServiceProvider serviceProvider) : IAttributesReceiver
{
    public async Task<List<string>> GetAllAttributes(int id)
    {
        
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.EdgeAttributes
            .Where(ea => ea.EdgeValues.Any(v => v.Edge.EdgeCategoryId == id))
            .Select(ea => ea.EdgeAttributeName).ToListAsync();
    }
}