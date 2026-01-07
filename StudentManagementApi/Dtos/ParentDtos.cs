using System;
using System.Collections.Generic;

namespace StudentManagementApi.Dtos
{
    public class ParentDtos
    {
        public record ChildSummaryDto(int StudentId, string FullName, string GradeLevel, int ActiveCourses);
        
        public record ChildOverviewDto(
            int StudentId, 
            string FullName, 
            double GPA, 
            double AttendanceRate, 
            int CompletedAssignments, 
            List<GradeDistributionDto> RecentGrades
        );
    }
}
