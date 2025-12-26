namespace StudentManagementApi.Dtos
{
	public class AuthDtos
	{
		public record RegisterDto(string UserName, string Email, string Password, string FullName, string Role);
		public record LoginDto(string Username, string Password);
		public record AuthResponse(string Token, string Email, string FullName, string[] Roles);
	}
}
