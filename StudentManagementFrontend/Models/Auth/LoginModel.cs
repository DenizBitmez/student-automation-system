using System.ComponentModel.DataAnnotations;

namespace StudentManagementFrontend.Models.Auth;

public class LoginModel
{
    [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}
