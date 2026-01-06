using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentManagementApi.Data;
using StudentManagementApi.Domain;
using StudentManagementApi.Services;
using System.Text;

namespace StudentManagementApi.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration cfg)
		{
			if (!services.Any(x => x.ServiceType == typeof(DbContextOptions<AppDbContext>)))
			{
				services.AddDbContext<AppDbContext>(opt =>
				{
					if (cfg["DbProvider"] == "InMemory")
					{
						opt.UseInMemoryDatabase("TestDb");
					}
					else
					{
						opt.UseNpgsql(cfg.GetConnectionString("Default"))
						.UseSnakeCaseNamingConvention();
					}
				});
			}


			services.AddIdentityCore<ApplicationUser>(opt =>
			{
				opt.Password.RequireNonAlphanumeric = false;
				opt.Password.RequireUppercase = false;
			})
			.AddRoles<IdentityRole>()
			.AddEntityFrameworkStores<AppDbContext>()
			.AddSignInManager();


			services.Configure<JwtOptions>(cfg.GetSection("Jwt"));
			services.AddScoped<IJwtTokenService, JwtTokenService>();
			services.AddScoped<IEmailService, EmailService>();
			services.AddScoped<IPdfExportService, PdfExportService>();
			services.AddScoped<INotificationService, NotificationService>();
			services.AddSignalR();


			var jwt = cfg.GetSection("Jwt").Get<JwtOptions>()!;
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(opt =>
			{
				opt.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = jwt.Issuer,
					ValidAudience = jwt.Audience,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key))
				};
			});


			services.AddAuthorization();
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen();
			services.AddCors(opt =>
			{
				opt.AddPolicy("client", p =>
				p.WithOrigins("http://localhost:5173", "http://localhost:5004", "http://localhost:5186", "https://localhost:7187")
				.AllowAnyHeader().AllowAnyMethod());
			});


			return services;
		}
	}
}
