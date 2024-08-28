using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.Admin;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.AllUserService.Abstraction;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices.AllUserService;

public class AllUserService(
    IAllUserServiceValidator validator,
    IAllUserDtoCreator allUserDtoCreator)
    : IAllUserService
{
    public async Task<ActionResponse<GetAllUsersDto>> GetAllUser(List<User> users)
    {
        var validateResult = await validator.Validate(users);
        if (validateResult.StatusCode != StatusCodeType.Success) return validateResult;
        
        
        var userOutPuts = await allUserDtoCreator.Create(users);


        validateResult.Data = userOutPuts;
        return validateResult;
    }
}