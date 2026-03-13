using System.ComponentModel.DataAnnotations;

namespace GarageOperationsManagementSystem.Models
{
    public class Owner
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = null!;

        [Phone]
        [Required]
        public string PhoneNumber { get; set; } = null!;

        [EmailAddress]
        [Required]
        public string Email { get; set; } = null!;

        public ICollection<Car> Cars { get; set; } = new List<Car>();
    }
}
