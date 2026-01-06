namespace StudentManagementApi.Dtos
{
    public class StudentActionDtos
    {
        // Complaint DTOs
        public record ComplaintCreateDto(string Title, string Description);
        public record ComplaintResponseDto(int Id, string AdminResponse);
        public record ComplaintViewDto(int Id, string Title, string Description, DateTime CreatedAt, string? AdminResponse, bool IsResolved, DateTime? ResolvedAt);

        // Activity DTOs
        public record ActivityCreateDto(string Title, string Type, string? Description, DateTime ActivityDate);
        public record ActivityViewDto(int Id, string Title, string Type, string? Description, DateTime ActivityDate);

        // Document DTOs
        public record DocumentViewDto(int Id, string Title, string Type, string FileUrl, DateTime IssueDate);
    }
}
