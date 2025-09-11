using Microsoft.AspNetCore.Identity;

namespace StudentManagementApi.Domain
{
	public class ApplicationUser:IdentityUser
	{
		public string FullName { get; set; } = string.Empty;
	}
}
