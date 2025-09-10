using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Domain;

namespace StudentManagementApi.Data
{
	public class AppDbContext(DbContextOptions<AppDbContext> options): IdentityDbContext<ApplicationUser>(options)
	{
		public DbSet<Student> Students => Set<Student>();
		public DbSet<Teacher> Teachers => Set<Teacher>();
		public DbSet<Course> Courses => Set<Course>();
		public DbSet<Enrollment> Enrollments => Set<Enrollment>();
		public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();


		protected override void OnModelCreating(ModelBuilder b)
		{
			base.OnModelCreating(b);
			b.Entity<Course>().HasIndex(x => x.Code).IsUnique();
			b.Entity<Enrollment>()
			.HasIndex(e => new { e.StudentId, e.CourseId })
			.IsUnique();
		}
	}
}
