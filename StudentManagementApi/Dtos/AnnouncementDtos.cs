using System;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementApi.Dtos
{
    public class AnnouncementDtos
    {
        public record AnnouncementCreateDto(
            [Required] string Title,
            [Required] string Content
        );

        public record AnnouncementUpdateDto(
            [Required] string Title,
            [Required] string Content
        );

        public record AnnouncementVm(
            int Id,
            string Title,
            string Content,
            DateTime CreatedAt,
            string AuthorName
        );
    }
}
