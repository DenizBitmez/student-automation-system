using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentManagementApi.Domain;
using static StudentManagementApi.Dtos.AuthDtos;
using StudentManagementApi.Services;

namespace StudentManagementApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController(UserManager<ApplicationUser> userMgr, RoleManager<IdentityRole> roleMgr, JwtTokenService jwt) : ControllerBase
	{
		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterDto dto)
		{
			if (!await roleMgr.RoleExistsAsync(dto.Role)) return BadRequest("Invalid role");
			var user = new ApplicationUser { UserName = dto.Username, Email = dto.Email, FullName = dto.FullName, EmailConfirmed = true };
			var res = await userMgr.CreateAsync(user, dto.Password);
			if (!res.Succeeded) return BadRequest(res.Errors);
			await userMgr.AddToRoleAsync(user, dto.Role);
			return Ok();
		}


		[HttpPost("login")]
		public async Task<ActionResult<AuthResponse>> Login(LoginDto dto)
		{
			var user = await userMgr.FindByNameAsync(dto.Username);
            if (user is null)
            {
                user = await userMgr.FindByEmailAsync(dto.Username);
            }

			if (user is null) return Unauthorized();
			if (!await userMgr.CheckPasswordAsync(user, dto.Password)) return Unauthorized();
			var token = await jwt.CreateToken(user);
			var roles = await userMgr.GetRolesAsync(user);
			return new AuthResponse(token, user.Id, user.Email!, user.FullName ?? "", roles.ToArray());
		}
	}
}
