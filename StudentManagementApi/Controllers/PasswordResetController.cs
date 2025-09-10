using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentManagementApi.Domain;
using StudentManagementApi.Services;
using System.Security.Cryptography;
using System.Text;

namespace StudentManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PasswordResetController(UserManager<ApplicationUser> userManager, IEmailService emailService) : ControllerBase
{
    private readonly Dictionary<string, PasswordResetInfo> _resetTokens = new();

    [HttpPost("request-reset")]
    public async Task<IActionResult> RequestPasswordReset(PasswordResetRequestDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            // Don't reveal that user doesn't exist
            return Ok(new { message = "If the email exists, a reset link has been sent." });
        }

        // Generate reset token
        var token = GenerateResetToken();
        var resetInfo = new PasswordResetInfo
        {
            UserId = user.Id,
            Email = user.Email!,
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        _resetTokens[token] = resetInfo;

        // Send email (simulated)
        await emailService.SendPasswordResetEmailAsync(user.Email!, token, user.FullName ?? user.UserName!);

        return Ok(new { message = "If the email exists, a reset link has been sent." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(PasswordResetDto dto)
    {
        if (!_resetTokens.TryGetValue(dto.Token, out var resetInfo))
        {
            return BadRequest(new { message = "Invalid or expired reset token." });
        }

        if (DateTime.UtcNow > resetInfo.ExpiresAt)
        {
            _resetTokens.Remove(dto.Token);
            return BadRequest(new { message = "Reset token has expired." });
        }

        var user = await userManager.FindByIdAsync(resetInfo.UserId);
        if (user == null)
        {
            return BadRequest(new { message = "User not found." });
        }

        // Reset password
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, dto.NewPassword);

        if (!result.Succeeded)
        {
            return BadRequest(new { message = "Failed to reset password.", errors = result.Errors });
        }

        // Remove used token
        _resetTokens.Remove(dto.Token);

        return Ok(new { message = "Password has been reset successfully." });
    }

    [HttpPost("verify-token")]
    public IActionResult VerifyToken(string token)
    {
        if (!_resetTokens.TryGetValue(token, out var resetInfo))
        {
            return BadRequest(new { message = "Invalid reset token." });
        }

        if (DateTime.UtcNow > resetInfo.ExpiresAt)
        {
            _resetTokens.Remove(token);
            return BadRequest(new { message = "Reset token has expired." });
        }

        return Ok(new { 
            valid = true, 
            email = resetInfo.Email,
            expiresAt = resetInfo.ExpiresAt 
        });
    }

    private string GenerateResetToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[32];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }
}

public record PasswordResetRequestDto(string Email);
public record PasswordResetDto(string Token, string NewPassword);

public class PasswordResetInfo
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string email, string resetToken, string userName);
}

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendPasswordResetEmailAsync(string email, string resetToken, string userName)
    {
        // Email simulation - In production, integrate with SendGrid, AWS SES, etc.
        var resetLink = $"https://localhost:5001/reset-password?token={resetToken}";
        
        var emailContent = $@"
            Merhaba {userName},

            Şifre sıfırlama talebiniz alındı. Şifrenizi sıfırlamak için aşağıdaki linke tıklayın:

            {resetLink}

            Bu link 1 saat geçerlidir.

            Eğer bu talebi siz yapmadıysanız, bu e-postayı dikkate almayın.

            İyi günler,
            Öğrenci Otomasyon Sistemi
        ";

        _logger.LogInformation("Password reset email simulated for {Email}: {Content}", email, emailContent);
        
        // Simulate email sending delay
        await Task.Delay(100);
    }
}