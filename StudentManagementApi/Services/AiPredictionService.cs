using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Data;
using StudentManagementApi.Dtos;

namespace StudentManagementApi.Services
{
    public class AiPredictionService
    {
        private readonly AppDbContext _context;

        public AiPredictionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PredictionResultDto> PredictStudentSuccessAsync(int studentId)
        {
            // 1. Gather Data
            var enrollments = await _context.Enrollments.Where(e => e.StudentId == studentId).ToListAsync();
            var attendanceRecords = await _context.AttendanceRecords
                .Include(ar => ar.Enrollment)
                .Where(ar => ar.Enrollment.StudentId == studentId)
                .ToListAsync();

            // 2. Feature Extraction
            double averageGrade = 0;
            if (enrollments.Any(e => e.Grade.HasValue))
            {
                averageGrade = (double)enrollments.Where(e => e.Grade.HasValue).Average(e => e.Grade!.Value);
            }

            double attendanceRate = 0;
            if (attendanceRecords.Any())
            {
                attendanceRate = (double)attendanceRecords.Count(a => a.Present) / attendanceRecords.Count * 100;
            }

            // 3. "AI" Prediction Logic (Simplified Linear Model)
            // Model: FinalGrade = (AvgGrade * 0.7) + (Attendance * 0.3) + Bias
            // This is a heuristic mock until we have training data.
            double predictedGrade = (averageGrade * 0.7) + (attendanceRate * 0.3);
            
            // Adjust for missing data penalties
            if (!enrollments.Any(e => e.Grade.HasValue)) predictedGrade = 50; // Neutral start if no grades

            double successProbability = predictedGrade; // Directly correlate for now

            // 4. Risk Assessment
            string riskLevel = "Low";
            var recommendations = new List<string>();

            if (predictedGrade < 50)
            {
                riskLevel = "High";
                recommendations.Add("Risk altında! Ders notlarını ve devamlılığı acilen artırmalısın.");
                if (attendanceRate < 70) recommendations.Add("Devamsızlık oranın çok yüksek, derslere katılmaya özen göster.");
                if (averageGrade < 50) recommendations.Add("Sınav ve ödev notlarını yükseltmek için ek çalışma yapmalısın.");
            }
            else if (predictedGrade < 70)
            {
                riskLevel = "Medium";
                recommendations.Add("Durumun orta seviyede. Daha iyi bir not için biraz daha gayret.");
                if (attendanceRate < 80) recommendations.Add("Devamlılığını artırmak notlarına olumlu yansıyacaktır.");
            }
            else
            {
                recommendations.Add("Harika gidiyorsun! Bu tempoyu koru.");
            }

            return new PredictionResultDto
            {
                StudentId = studentId,
                SuccessProbability = Math.Round(successProbability, 1),
                PredictedFinalGrade = Math.Round(predictedGrade, 1),
                RiskLevel = riskLevel,
                Recommendations = recommendations
            };
        }
    }
}
