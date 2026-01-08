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
		public DbSet<Complaint> Complaints => Set<Complaint>();
		public DbSet<SocialActivity> SocialActivities => Set<SocialActivity>();
		public DbSet<StudentDocument> StudentDocuments => Set<StudentDocument>();
		public DbSet<TeacherLeave> TeacherLeaves => Set<TeacherLeave>();
		public DbSet<Announcement> Announcements => Set<Announcement>();
		public DbSet<Assignment> Assignments => Set<Assignment>();
		public DbSet<Submission> Submissions => Set<Submission>();
		public DbSet<ScheduleItem> ScheduleItems => Set<ScheduleItem>();
		public DbSet<Message> Messages => Set<Message>();
		public DbSet<Parent> Parents => Set<Parent>();
		public DbSet<Exam> Exams => Set<Exam>();
		public DbSet<Question> Questions => Set<Question>();
		public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
		public DbSet<ExamResult> ExamResults => Set<ExamResult>();
		public DbSet<StudentAnswer> StudentAnswers => Set<StudentAnswer>();


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
