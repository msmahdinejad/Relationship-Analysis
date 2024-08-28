using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices.Node;

public class NodeAttributesReceiver(IServiceProvider serviceProvider) : IAttributesReceiver
{
    public async Task<List<string>> GetAllAttributes(string name)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.NodeAttributes
            .Where(na => na.Values.Any(v => v.Node.NodeCategory.NodeCategoryName == name))
            .Select(na => na.NodeAttributeName).ToListAsync();
    }
}