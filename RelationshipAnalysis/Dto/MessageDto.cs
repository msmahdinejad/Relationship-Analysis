namespace RelationshipAnalysis.DTO;

public class MessageDto
{
    public string Message { get; set; }

    public MessageDto(string message)
    {
        Message = message;
    }
}