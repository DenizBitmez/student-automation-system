namespace StudentManagementFrontend.Models
{
    public class DocumentVm
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
    }
}
