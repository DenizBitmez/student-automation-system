using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StudentManagementApi.Domain;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentManagementApi.Services
{
	public class JwtTokenService(IOptions<JwtOptions> options, UserManager<ApplicationUser> userManager)
	{
		private readonly JwtOptions _opt = options.Value;
		private readonly UserManager<ApplicationUser> _userManager = userManager;

		public async Task<string> CreateToken(ApplicationUser user)
		{
			var roles = await _userManager.GetRolesAsync(user);
			var claims = new List<Claim>
{
new(JwtRegisteredClaimNames.Sub, user.Id),
new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
new(ClaimTypes.Name, user.UserName ?? ""),
};
			claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));


			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Key));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


			var token = new JwtSecurityToken(
			issuer: _opt.Issuer,
			audience: _opt.Audience,
			claims: claims,
			expires: DateTime.UtcNow.AddHours(8),
			signingCredentials: creds);


			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}

	public class JwtOptions
	{
		public string Issuer { get; set; } = default!;
		public string Audience { get; set; } = default!;
		public string Key { get; set; } = default!;
	}
}
