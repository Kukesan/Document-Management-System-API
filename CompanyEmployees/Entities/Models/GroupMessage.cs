using System.ComponentModel.DataAnnotations;

namespace CompanyEmployees.Entities.Models
{
    public class GroupMessage
    {
        [Key]
        public int Id { get; set; }
        public string Message { get; set; }
        public int GroupId { get; set; }
        public string CreatedDate { get; set; }
    }
}
