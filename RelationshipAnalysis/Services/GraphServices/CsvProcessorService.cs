using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices;

public class CsvProcessorService : ICsvProcessorService
{
    public async Task<List<dynamic>> ProcessCsvAsync(IFormFile file)
    {
        var records = new List<dynamic>();

        using var stream = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(stream, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "," });
        await csv.ReadAsync();
        csv.ReadHeader();
        var headers = csv.HeaderRecord;

        while (await csv.ReadAsync())
        {
            var record = new ExpandoObject() as IDictionary<string, object>;
            foreach (var header in headers) record[header] = csv.GetField(header);
            records.Add(record);
        }

        return records;
    }
}