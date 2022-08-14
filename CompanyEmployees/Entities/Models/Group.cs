using System.ComponentModel.DataAnnotations;

namespace CompanyEmployees.Entities.Models
{
    public class Group
    {
        [Key]
        public int groupId { get; set; }
        public string name { get; set; }
        public string comment { get; set; }
        public string createdDate { get; set; }
    }
}
