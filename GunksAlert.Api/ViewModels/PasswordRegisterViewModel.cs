using System.ComponentModel.DataAnnotations;

namespace GunksAlert.Api.ViewModels;

public class PasswordRegisterViewModel {
    [Required]
    [MaxLength(255, ErrorMessage = "Username must be less than 256 characters")]
    [MinLength(2, ErrorMessage = "Username must be at least 2 characters")]
    public required string Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Password and confirmation password do not match")]
    public required string ConfirmPassword { get; set; }
}
