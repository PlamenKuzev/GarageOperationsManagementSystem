using System.ComponentModel.DataAnnotations;

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

        public decimal? ReapairPrice { get; set; }

        public int CarId { get; set; }

        [Required]
        public Car Car { get; set; } = null!;

        public int GarageId { get; set; }

        [Required]
        public Garage Garage { get; set; } = null!;

    }
}
