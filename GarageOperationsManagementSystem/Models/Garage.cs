using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace GarageOperationsManagementSystem.Models
{
    public class Garage
    {
        public int Id { get; set; }

        [Required]
        public string City { get; set; } = null!;

        [Required]
        public string Address { get; set; } = null!;

        public int Capacity { get; set; }

        [Required]
        public string WorkSchedule { get; set; } = null!;

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();

        public ICollection<RepairOrder> RepairOrders { get; set; } = new List<RepairOrder>();

    }
}
