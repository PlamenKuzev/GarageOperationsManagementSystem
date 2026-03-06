using System.ComponentModel.DataAnnotations;

namespace GarageOperationsManagementSystem.Models
{
    public class RepairOrder
    {
        public int Id { get; set; }

        [Required]
        public string IssueCode { get; set; } = null!;

        public string IssueDescription { get; set; }

        public DateTime ArrivalDate { get; set; }

        public DateTime? CompletionDate { get; set; }

        public bool IsCompleted { get; set; }

        public decimal? ReapairPrice { get; set; }

        public int CarId { get; set; }

        public Car Car { get; set; }

        public int GarageId { get; set; }

        public Garage Garage { get; set; }

    }
}
