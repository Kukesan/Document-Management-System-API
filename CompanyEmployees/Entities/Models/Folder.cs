using System.ComponentModel.DataAnnotations;

namespace CompanyEmployees.Entities.Models
{
    public class Folder
    {
        [Key]
        public int folderId { get; set; }
        public string name { get; set; }
        public string comment { get; set; }
        public string createdDate { get; set; }
        public string? userId { get; set; }
        public string? UserEmail { get; set; }
    }
}
