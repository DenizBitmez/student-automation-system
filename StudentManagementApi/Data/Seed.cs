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

			if (!ctx.Complaints.Any())
			{
				var student = await ctx.Students.FirstAsync();
				ctx.Complaints.Add(new Complaint 
				{ 
					StudentId = student.Id, 
					Title = "Yemekhane Menüsü", 
					Description = "Yemekhane menüsünün daha sağlıklı seçenekler içermesini rica ediyorum.",
					CreatedAt = DateTime.UtcNow.AddDays(-5),
					AdminResponse = "Öneriniz için teşekkürler, diyetisyenimiz ile görüşeceğiz.",
					IsResolved = true,
					ResolvedAt = DateTime.UtcNow.AddDays(-2)
				});
				ctx.Complaints.Add(new Complaint 
				{ 
					StudentId = student.Id, 
					Title = "Kütüphane Çalışma Saatleri", 
					Description = "Sınav haftalarında kütüphanenin 7/24 açık olmasını talep ediyoruz.",
					CreatedAt = DateTime.UtcNow.AddDays(-1)
				});
			}

			if (!ctx.SocialActivities.Any())
			{
				var student = await ctx.Students.FirstAsync();
				ctx.SocialActivities.Add(new SocialActivity 
				{ 
					StudentId = student.Id, 
					Title = "Nutuk - Mustafa Kemal Atatürk", 
					Type = "Kitap", 
					Description = "Nutuk kitabını bitirdim.",
					ActivityDate = DateTime.UtcNow.AddDays(-10)
				});
				ctx.SocialActivities.Add(new SocialActivity 
				{ 
					StudentId = student.Id, 
					Title = "Okul Satranç Turnuvası", 
					Type = "Spor", 
					Description = "Okul içi satranç turnuvasına katıldım ve 3. oldum.",
					ActivityDate = DateTime.UtcNow.AddDays(-3)
				});
			}

			if (!ctx.StudentDocuments.Any())
			{
				var student = await ctx.Students.FirstAsync();
				ctx.StudentDocuments.Add(new StudentDocument 
				{ 
					StudentId = student.Id, 
					Title = "2024-2025 Güz Dönemi Karnesi", 
					Type = "Karne", 
					FileUrl = "#", 
					IssueDate = DateTime.UtcNow.AddMonths(-1)
				});
				ctx.StudentDocuments.Add(new StudentDocument 
				{ 
					StudentId = student.Id, 
					Title = "Üstün Başarı Sertifikası", 
					Type = "Sertifika", 
					FileUrl = "#", 
					IssueDate = DateTime.UtcNow.AddMonths(-2)
				});
			}
			await ctx.SaveChangesAsync();
		}
	}
}
