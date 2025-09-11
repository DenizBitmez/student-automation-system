using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Data;
using StudentManagementApi.Extensions;
using StudentManagementApi.Domain;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddAppServices(builder.Configuration);


var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.UseCors("client");
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.MapHub<StudentManagementApi.Hubs.NotificationHub>("/notificationHub");

// Ensure database is created and migrated
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
}

await Seed.InitializeAsync(app.Services);

app.Run();