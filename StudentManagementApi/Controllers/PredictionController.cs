using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Data;
using StudentManagementApi.Dtos;
using StudentManagementApi.Services;
using System.Security.Claims;

namespace StudentManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PredictionController : ControllerBase
    {
        private readonly AiPredictionService _predictionService;
        private readonly AppDbContext _context;

        public PredictionController(AiPredictionService predictionService, AppDbContext context)
        {
            _predictionService = predictionService;
            _context = context;
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<PredictionResultDto>> GetPrediction(int studentId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
             // If ID is 0, resolve current student
            if (studentId == 0 && User.IsInRole("Student"))
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == currentUserId);
                if (student == null) return NotFound("Student profile not found.");
                studentId = student.Id;
            }

            // Security Check
            if (User.IsInRole("Student"))
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == studentId);
                if (student == null || student.UserId != currentUserId) return Forbid();
            }

            var result = await _predictionService.PredictStudentSuccessAsync(studentId);
            return Ok(result);
        }
    }
}
