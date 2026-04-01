using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarageOperationsManagementSystem.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Position { get; set; } = null!;

        public decimal Salary { get; set; }

        public DateTime WorkingSince { get; set; }

        public int GarageId { get; set; }

        [Required]
        public Garage Garage { get; set; } = null!;

        public string? ApplicationUserId { get; set; }

        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser? ApplicationUser { get; set; }

        public bool IsTrusted { get; set; }
    }
}
