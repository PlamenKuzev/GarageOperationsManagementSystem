using System.ComponentModel.DataAnnotations;

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
    }
}
