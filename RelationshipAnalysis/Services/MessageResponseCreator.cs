using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Services.Abstraction;

namespace RelationshipAnalysis.Services;

public class MessageResponseCreator : IMessageResponseCreator
{
    public ActionResponse<MessageDto> Create(StatusCodeType statusCodeType, string message)
    {
        return new ActionResponse<MessageDto>
        {
            StatusCode = statusCodeType,
            Data = new MessageDto(message)
        };
    }
}