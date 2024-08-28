namespace RelationshipAnalysis.Dto;

public class MessageDto
{
    public MessageDto(string message)
    {
        Message = message;
    }

    public string Message { get; set; }
}