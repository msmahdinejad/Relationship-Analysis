namespace RelationshipAnalysis.Services.GraphServices.Abstraction;

public interface ICsvProcessorService
{
    Task<List<dynamic>> ProcessCsvAsync(IFormFile file);
}