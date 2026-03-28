using System.ComponentModel.DataAnnotations;

namespace GarageOperationsManagementSystem.ViewModels
{
    public class AdminCarFormViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Brand { get; set; } = string.Empty;

        [Required]
        public string Model { get; set; } = string.Empty;

        public int? Year { get; set; }

        public int? Mileage { get; set; }

        [Required(ErrorMessage = "Select an owner from the list.")]
        [Range(1, int.MaxValue, ErrorMessage = "Select an owner from the list.")]
        [Display(Name = "Owner")]
        public int? OwnerId { get; set; }
    }
}
