using System.ComponentModel.DataAnnotations;

namespace CompanyEmployees.Entities.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        public string message { get; set; }
        public string CreatedDate { get; set; }
    }
}
