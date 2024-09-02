using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RelationshipAnalysis.Services.GraphServices.Abstraction;

public interface ICsvProcessorService
{
    Task<List<dynamic>> ProcessCsvAsync(IFormFile file);
}