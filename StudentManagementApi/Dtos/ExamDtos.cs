using StudentManagementApi.Domain;

namespace StudentManagementApi.Dtos
{
    public class ExamDtos
    {
        public record ExamCreateDto(
            string Title, 
            string Description, 
            int CourseId, 
            int DurationMinutes, 
            DateTime StartTime, 
            DateTime EndTime,
            List<QuestionCreateDto> Questions
        );

        public record QuestionCreateDto(
            string Text, 
            int Points, 
            QuestionType Type, 
            List<QuestionOptionCreateDto> Options
        );

        public record QuestionOptionCreateDto(string Text, bool IsCorrect);

        public record ExamListItemDto(
            int Id, 
            string Title, 
            string CourseName, 
            DateTime StartTime, 
            DateTime EndTime,
            int DurationMinutes,
            bool CanTake,
            int? Score = null
        );

        public record ExamTakeDto(
            int Id, 
            string Title, 
            int DurationMinutes, 
            List<QuestionTakeDto> Questions
        );

        public record QuestionTakeDto(
            int Id, 
            string Text, 
            int Points, 
            QuestionType Type, 
            List<QuestionOptionTakeDto> Options
        );

        public record QuestionOptionTakeDto(int Id, string Text);

        public record ExamSubmitDto(int ExamId, List<StudentAnswerDto> Answers);

        public record StudentAnswerDto(int QuestionId, int? SelectedOptionId, string? TextAnswer);

        public record ExamResultDto(int Id, string StudentName, int Score, DateTime SubmittedAt);
    }
}
