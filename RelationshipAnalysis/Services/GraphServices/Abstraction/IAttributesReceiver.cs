using System.Collections.Generic;
using System.Threading.Tasks;

namespace RelationshipAnalysis.Services.GraphServices.Abstraction;

public interface IAttributesReceiver
{
    Task<List<string>> GetAllAttributes(string name);
}