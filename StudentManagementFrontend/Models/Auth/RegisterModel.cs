using System.ComponentModel.DataAnnotations;

namespace StudentManagementFrontend.Models.Auth;

public class RegisterModel
{
    [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
    [StringLength(50, ErrorMessage = "Kullanıcı adı en fazla 50 karakter olabilir")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta zorunludur")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur")]
    [StringLength(100, ErrorMessage = "Şifre en az {2} ve en fazla {1} karakter uzunluğunda olmalıdır.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ad zorunludur")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad zorunludur")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Rol seçimi zorunludur")]
    public string Role { get; set; } = string.Empty;
}
