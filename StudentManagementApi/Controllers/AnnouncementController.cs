using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Data;
using StudentManagementApi.Domain;
using static StudentManagementApi.Dtos.AnnouncementDtos;
using System.Security.Claims;

namespace StudentManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AnnouncementController(AppDbContext db, UserManager<ApplicationUser> um) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<AnnouncementVm>>> Get()
        {
            return await db.Announcements
                .Include(a => a.Author)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new AnnouncementVm(
                    a.Id,
                    a.Title,
                    a.Content,
                    a.CreatedAt,
                    a.Author.FullName ?? "System"
                ))
                .ToListAsync();
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<AnnouncementVm>> GetById(int id)
        {
            var a = await db.Announcements
                .Include(x => x.Author)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (a is null) return NotFound();

            return new AnnouncementVm(
                a.Id,
                a.Title,
                a.Content,
                a.CreatedAt,
                a.Author.FullName ?? "System"
            );
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AnnouncementVm>> Create(AnnouncementCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var user = await um.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            var announcement = new Announcement
            {
                Title = dto.Title,
                Content = dto.Content,
                AuthorId = userId,
                CreatedAt = DateTime.UtcNow
            };

            db.Announcements.Add(announcement);
            await db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = announcement.Id }, 
                new AnnouncementVm(announcement.Id, announcement.Title, announcement.Content, announcement.CreatedAt, user.FullName ?? "Admin"));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, AnnouncementUpdateDto dto)
        {
            var a = await db.Announcements.FindAsync(id);
            if (a is null) return NotFound();

            a.Title = dto.Title;
            a.Content = dto.Content;

            await db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var a = await db.Announcements.FindAsync(id);
            if (a is null) return NotFound();

            db.Announcements.Remove(a);
            await db.SaveChangesAsync();

            return NoContent();
        }
    }
}
