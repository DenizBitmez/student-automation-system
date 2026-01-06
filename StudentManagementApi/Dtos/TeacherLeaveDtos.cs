namespace StudentManagementApi.Dtos;

public record TeacherLeaveCreateDto(
    DateTime StartDate,
    DateTime EndDate,
    string Reason
);

public record TeacherLeaveViewDto(
    int Id,
    int TeacherId,
    string TeacherName,
    DateTime StartDate,
    DateTime EndDate,
    string Reason,
    string Status,
    DateTime CreatedAt
);

public record TeacherLeaveUpdateDto(
    string Status // Approved, Rejected
);
