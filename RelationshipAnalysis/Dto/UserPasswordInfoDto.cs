using System.ComponentModel.DataAnnotations;

namespace RelationshipAnalysis.Dto;

public class UserPasswordInfoDto
{
    [Required(ErrorMessageResourceName = "OldPasswordRequired", ErrorMessageResourceType = typeof(Resources))]
    public string OldPassword { get; set; }

    [Required(ErrorMessageResourceName = "NewPasswordRequired", ErrorMessageResourceType = typeof(Resources))]
    [RegularExpression("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*\\W)(?!.* ).{8,}$",
        ErrorMessageResourceName = "InvalidPasswordMessage", ErrorMessageResourceType = typeof(Resources))]
    public string NewPassword { get; set; }
}