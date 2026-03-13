using System.ComponentModel.DataAnnotations;

namespace GarageOperationsManagementSystem.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required]
        public required string Brand { get; set; } = null!;

        [Required]
        public string Model { get; set; } = null!;

        public int? Year { get; set; }

        public int? Mileage { get; set; }

        public int OwnerId { get; set; }

        [Required]
        public Owner Owner { get; set; } = null!;


        public ICollection<RepairOrder> RepairOrders { get; set; } = new List<RepairOrder>();
    }
}
