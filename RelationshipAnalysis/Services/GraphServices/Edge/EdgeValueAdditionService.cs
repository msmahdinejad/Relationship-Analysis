using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models.Graph.Edge;
using RelationshipAnalysis.Services.GraphServices.Edge.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices.Edge;

public class EdgeValueAdditionService : IEdgeValueAdditionService
{
    public async Task AddKvpToValues(ApplicationDbContext context, KeyValuePair<string, object> kvp, Models.Graph.Edge.Edge newEdge)
    {
        
        var newEdgeAttribute = await context.EdgeAttributes.SingleOrDefaultAsync(na =>
            na.EdgeAttributeName == kvp.Key);
        if (newEdgeAttribute == null)
        {
            newEdgeAttribute = new EdgeAttribute
            {
                EdgeAttributeName = kvp.Key
            };
            await context.AddAsync(newEdgeAttribute);
            await context.SaveChangesAsync();
        }

        var value = await context.EdgeValues.SingleOrDefaultAsync(nv =>
            nv.EdgeAttributeId == newEdgeAttribute.EdgeAttributeId &&
            nv.EdgeId == newEdge.EdgeId);

        if (value != null) throw new Exception(Resources.FailedAddRecordsMessage);

        var newEdgeValue = new EdgeValue
        {
            EdgeAttributeId = newEdgeAttribute.EdgeAttributeId,
            ValueData = kvp.Value.ToString(),
            EdgeId = newEdge.EdgeId
        };

        await context.AddAsync(newEdgeValue);
        await context.SaveChangesAsync();
    }
}