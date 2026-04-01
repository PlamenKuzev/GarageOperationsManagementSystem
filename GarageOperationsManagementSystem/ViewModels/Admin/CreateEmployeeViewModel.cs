using System.ComponentModel.DataAnnotations;

namespace GarageOperationsManagementSystem.ViewModels.Admin
{
    public class CreateEmployeeViewModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Position")]
        public string Position { get; set; } = string.Empty;

        [Required]
        [Range(0, 1_000_000, ErrorMessage = "Salary must be between 0 and 1,000,000.")]
        [Display(Name = "Monthly Salary")]
        public decimal Salary { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Working Since")]
        public DateTime WorkingSince { get; set; } = DateTime.Today;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a garage.")]
        [Display(Name = "Garage")]
        public int GarageId { get; set; }

        [Display(Name = "Trusted Employee")]
        public bool IsTrusted { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Login Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
