using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyEmployees.Entities.Models
{
    public class Issue
    {
        [Key]
        public int IssueId { get; set; }

        [ForeignKey(nameof(Employee))]
        public Guid Id { get; set; }
      //  public int UserId { get; set; }
        public string? Description { get; set; }
        public string CreatedDate { get; set; }
        public string? UserEmail { get; set; }
        public bool IsSolved { get; set; }= false;
    }
}
