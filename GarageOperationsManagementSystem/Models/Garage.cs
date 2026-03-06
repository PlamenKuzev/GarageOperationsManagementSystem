using System.ComponentModel.DataAnnotations;

namespace GarageOperationsManagementSystem.Models
{
    public class Garage
    {
        public int Id { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Address { get; set; }

        public int Capacity { get; set; }

        public string WorkSchedule { get; set; }

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();

        public ICollection<RepairOrder> RepairOrders { get; set; } = new List<RepairOrder>();

    }
}
