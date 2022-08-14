using System.ComponentModel.DataAnnotations;

namespace CompanyEmployees.Entities.Models
{
    public class FileUpload
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Address { get; set; }
        public string? ImgPath { get; set; }
        public bool Status { get; set; } = true;
        public int? folderId { get; set; } = -1;
        public string CreatedDate { get; set; }
        public string? userId { get; set; }
        public string? UserEmail { get; set; }
    }
}
