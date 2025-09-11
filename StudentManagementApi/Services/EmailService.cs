namespace StudentManagementApi.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            // Email gönderme implementasyonu buraya eklenecek
            await Task.CompletedTask;
        }
    }
}