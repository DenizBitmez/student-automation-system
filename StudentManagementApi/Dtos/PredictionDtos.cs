namespace StudentManagementApi.Dtos
{
    public class PredictionResultDto
    {
        public int StudentId { get; set; }
        public double SuccessProbability { get; set; } // 0-100
        public string RiskLevel { get; set; } = string.Empty; // "Low", "Medium", "High"
        public double PredictedFinalGrade { get; set; }
        public List<string> Recommendations { get; set; } = new();
    }
}
