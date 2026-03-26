using System;
using System.ComponentModel.DataAnnotations;

namespace GarageOperationsManagementSystem.Models
{
    public class CreateRepairOrderViewModel
    {
        [Required]
        [Display(Name = "Car")]
        public int CarId { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Issue Code")]
        public string IssueCode { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Arrival Date")]
        public DateTime ArrivalDate { get; set; }
    }
}

