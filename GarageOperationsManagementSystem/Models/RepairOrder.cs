using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GarageOperationsManagementSystem.Models
{
    public class RepairOrder
    {
        public int Id { get; set; }

        [Required]
        public string IssueCode { get; set; } = null!;

        [Required]
        public string IssueDescription { get; set; } = null!;

        public DateTime ArrivalDate { get; set; }

        public DateTime? CompletionDate { get; set; }

        public bool IsCompleted { get; set; }

        public decimal? RepairPrice { get; set; }

        public int CarId { get; set; }

        [ValidateNever]
        public Car Car { get; set; } = null!;

        public int GarageId { get; set; }

        [ValidateNever]
        public Garage Garage { get; set; } = null!;

    }
}
