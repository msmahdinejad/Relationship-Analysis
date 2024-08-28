namespace RelationshipAnalysis.Services.GraphServices.Abstraction;

public interface IAttributesReceiver
{
    Task<List<string>> GetAllAttributes(int id);
}