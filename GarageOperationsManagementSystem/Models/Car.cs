using System.ComponentModel.DataAnnotations;

namespace GarageOperationsManagementSystem.Models
{
    public class Car
    {
        public int Id { get; set; }

        public string Brand { get; set; }

        [Required]
        public string Model { get; set; } = null!;

        public int? Year { get; set; }

        public int? Mileage { get; set; }

        public int OwnerId { get; set; }

        public Owner Owner { get; set; }

        //public ICollection<RepairOrder> RepairOrders { get; set; } = new List<RepairOrder>();
    }
}
