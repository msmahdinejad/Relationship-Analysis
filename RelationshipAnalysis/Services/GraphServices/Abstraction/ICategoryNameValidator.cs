namespace RelationshipAnalysis.Services.GraphServices.Abstraction;

public interface ICategoryNameValidator
{
    public Task<bool> Validate(string categoryName);
}