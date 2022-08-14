using System.ComponentModel.DataAnnotations;

namespace CompanyEmployees.Entities.Models
{
    public class UserActivity
    {
        [Key]
        public int Id { get; set; }
        public string UserEmail { get; set; }
        public string LoginTime { get; set; }
        public string LogoutTime { get; set; }
    }
}
