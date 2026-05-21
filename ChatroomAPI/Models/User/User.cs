using System.ComponentModel.DataAnnotations;
public class User
{
    public Guid Id {get; set;}
    [MaxLength(14, ErrorMessage = "Username cannot exceed 14 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Username must be one word with no special characters.")]
    public required string Username {get; set;}
    public string? DisplayName {get; set;}
    public required string PasswordHash {get; set;}
}