using System.ComponentModel.DataAnnotations;

namespace GarageOperationsManagementSystem.Models
{
    public class Owner
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = null!;

        [Phone]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public ICollection<Car> Cars { get; set; } = new List<Car>();
    }
}
