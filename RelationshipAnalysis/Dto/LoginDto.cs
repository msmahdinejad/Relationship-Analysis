using System.ComponentModel.DataAnnotations;

namespace RelationshipAnalysis.Dto;

public class LoginDto
{
    [Required] public string Username { get; set; }

    [Required] public string? Password { get; set; }
}