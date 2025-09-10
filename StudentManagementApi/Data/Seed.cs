using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Domain;

namespace StudentManagementApi.Data
{
	public static class Seed
	{
		public static async Task InitializeAsync(IServiceProvider sp)
		{
			using var scope = sp.CreateScope();
			var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
			await ctx.Database.MigrateAsync();


			var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
			var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();


			foreach (var role in new[] { "Admin", "Teacher", "Student" })
				if (!await roleMgr.RoleExistsAsync(role))
					await roleMgr.CreateAsync(new IdentityRole(role));


			async Task<ApplicationUser> EnsureUser(string email, string role)
			{
				var u = await userMgr.FindByEmailAsync(email);
				if (u is null)
				{
					u = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true, FullName = email.Split('@')[0] };
					await userMgr.CreateAsync(u, "Passw0rd!");
					await userMgr.AddToRoleAsync(u, role);
				}
				return u;
			}


			var admin = await EnsureUser("admin@test.com", "Admin");
			var tUser = await EnsureUser("teacher@test.com", "Teacher");
			var sUser = await EnsureUser("student@test.com", "Student");


			if (!ctx.Teachers.Any())
			{
				ctx.Teachers.Add(new Teacher { UserId = tUser.Id, Department = "CS" });
			}
			if (!ctx.Students.Any())
			{
				ctx.Students.Add(new Student { UserId = sUser.Id });
			}
			await ctx.SaveChangesAsync();


			if (!ctx.Courses.Any())
			{
				var teacher = await ctx.Teachers.FirstAsync();
				ctx.Courses.Add(new Course { Code = "CS101", Name = "Intro to CS", TeacherId = teacher.Id });
				await ctx.SaveChangesAsync();
			}
		}
	}
}
