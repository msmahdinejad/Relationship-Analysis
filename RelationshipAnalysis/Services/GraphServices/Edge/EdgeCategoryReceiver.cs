using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.GraphServices.Edge.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices.Edge;

public class EdgeCategoryReceiver(IServiceProvider serviceProvider) : IEdgeCategoryReceiver
{
    public async Task<List<string>> GetAllEdgeCategories()
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await context.EdgeCategories.Select(e => e.EdgeCategoryName).ToListAsync();
    }
}