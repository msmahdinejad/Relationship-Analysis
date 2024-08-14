using RelationshipAnalysis.Enums;

namespace RelationshipAnalysis.Controllers;

public class ActionResponse<T>
{
    public T Data { get; set; }
    
    public StatusCodeType StatusCode { get; set; }
    
    public ActionResponse()
    {
        StatusCode = StatusCodeType.Success;
    }
}