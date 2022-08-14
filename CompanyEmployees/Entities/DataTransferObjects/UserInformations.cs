using System.ComponentModel.DataAnnotations;

namespace CompanyEmployees.Entities.DataTransferObjects
{
    public class UserInformations
    {
        [Key]
        public int Id { get; set; }
        public string? EmpId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? JobPosition { get; set; }
        public string? TelephoneNo { get; set; }
        public bool? UserAccepted { get; set; }
        public string? UserEmail { get; set; }
        public int groupId { get; set; } = -1;
    }
}
