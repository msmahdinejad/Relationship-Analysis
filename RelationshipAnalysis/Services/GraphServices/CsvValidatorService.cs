using System.Globalization;
using AngleSharp.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.IdentityModel.Tokens;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Services.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices;

public class CsvValidatorService(IMessageResponseCreator responseCreator) : ICsvValidatorService
{
    public ActionResponse<MessageDto> Validate(IFormFile file, params string[] uniqueHeaderNames)
    {
        using (var stream = new StreamReader(file.OpenReadStream()))
        using (var csv = new CsvReader(stream, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "," }))
        {
            csv.Read();
            csv.ReadHeader();
            var headers = csv.HeaderRecord;
            if (headers.SingleOrDefault(h => h == string.Empty) != null || headers.IsNullOrEmpty())
                return responseCreator.Create(StatusCodeType.BadRequest, Resources.InvalidHeaderAttribute);

            if (headers.Distinct().Count() != headers.Length)
                return responseCreator.Create(StatusCodeType.BadRequest, Resources.TwoSameHeadersMessage);

            if (uniqueHeaderNames.Count(h => headers.Contains(h)) != uniqueHeaderNames.Length)
                return responseCreator.Create(StatusCodeType.BadRequest, Resources.InvalidHeaderAttribute);
        }

        return responseCreator.Create(StatusCodeType.Success, Resources.ValidFileMessage);
    }

    
}