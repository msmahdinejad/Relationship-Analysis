using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;

namespace RelationshipAnalysis.Services.Abstraction;

public interface IMessageResponseCreator
{
    ActionResponse<MessageDto> Create(StatusCodeType statusCodeType, string message);
}