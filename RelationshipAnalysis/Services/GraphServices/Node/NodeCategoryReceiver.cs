using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.GraphServices.Node.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices.Node;

public class NodeCategoryReceiver(IServiceProvider serviceProvider) : INodeCategoryReceiver
{
    public async Task<List<string>> GetAllNodeCategories()
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await context.NodeCategories.Select(e => e.NodeCategoryName).ToListAsync();
    }
}